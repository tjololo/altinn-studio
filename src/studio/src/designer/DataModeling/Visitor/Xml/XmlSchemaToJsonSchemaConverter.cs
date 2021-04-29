using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Utils;
using Json.More;
using Json.Schema;

namespace Altinn.Studio.DataModeling.Visitor.Xml
{
    /// <summary>
    /// Visitor class for converting XML schema to Json Schema, this will produce a Json Schema with custom keywords to preserve XML schema information
    /// </summary>
    public class XmlSchemaToJsonSchemaConverter : IXmlSchemaConverter<JsonSchemaBuilder>
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaNode(XmlSchema schema)
        {
            JsonSchemaBuilder builder = new JsonSchemaBuilder()
               .Schema(MetaSchemas.Draft201909Id)
               .Id("schema.json")
               .Type(SchemaValueType.Object)
               .XsdNamespaces(
                    schema.Namespaces
                       .ToArray()
                       .Select(ns => (ns.Name, ns.Namespace)))
               .XsdSchemaAttributes(
                    (nameof(XmlSchema.AttributeFormDefault), schema.AttributeFormDefault.ToString()),
                    (nameof(XmlSchema.ElementFormDefault), schema.ElementFormDefault.ToString()),
                    (nameof(XmlSchema.BlockDefault), schema.BlockDefault.ToString()),
                    (nameof(XmlSchema.FinalDefault), schema.FinalDefault.ToString()));

            List<(string name, JsonSchemaBuilder schema)> items = new List<(string name, JsonSchemaBuilder schema)>();

            foreach (XmlSchemaObject item in schema.Items.Cast<XmlSchemaObject>())
            {
                switch (item)
                {
                    case XmlSchemaImport:
                        throw new NotImplementedException("Schema imports are not supported by Altinn Studio");
                    case XmlSchemaRedefine:
                        throw new NotImplementedException("Redefine is not supported by Altinn Studio");
                    case XmlSchemaAnnotation x:
                        AddAnnotation(x, builder);
                        break;
                    case XmlSchemaSimpleType x:
                        items.Add((x.Name, ConvertSchemaSimpleType(x)));
                        break;
                    case XmlSchemaComplexType x:
                        items.Add((x.Name, ConvertSchemaComplexType(x)));
                        break;
                    case XmlSchemaGroup x:
                        items.Add((x.Name, ConvertSchemaGroup(x)));
                        break;
                    case XmlSchemaElement x:
                        items.Add((x.Name, ConvertSchemaElement(x)));
                        break;
                    case XmlSchemaAttribute x:
                        items.Add((x.Name, ConvertSchemaAttribute(x)));
                        break;
                    case XmlSchemaAttributeGroup x:
                        items.Add((x.Name, ConvertSchemaAttributeGroup(x)));
                        break;
                    default:
                        throw new XmlSchemaException("Unsupported global element in xml schema", null, item.LineNumber, item.LinePosition);
                }
            }

            if (items.Count > 0)
            {
                builder.Properties(items.Take(1).Select(def => (def.name, def.schema.Build())).ToArray());
            }

            if (items.Count > 1)
            {
                builder.Definitions(items.Skip(1).Select(def => (def.name, def.schema.Build())).ToArray());
            }

            return builder;
        }

        private void AddAnnotation(XmlSchemaAnnotation annotation, JsonSchemaBuilder builder)
        {
            if (annotation.Parent is XmlSchema)
            {
                AddSchemaRootAnnotation(annotation, builder);
                return;
            }

            XmlSchemaAppInfo appInfo = annotation.Items.Cast<XmlSchemaObject>().FirstOrDefault(o => o is XmlSchemaAppInfo) as XmlSchemaAppInfo;
            XmlSchemaDocumentation doc = annotation.Items.Cast<XmlSchemaObject>().FirstOrDefault(o => o is XmlSchemaDocumentation) as XmlSchemaDocumentation;

            if (appInfo?.Markup != null && appInfo.Markup.Length > 0)
            {
                XmlDocument xml = new XmlDocument();
                XmlElement root = xml.CreateElement("root");
                xml.AppendChild(root);
                foreach (XmlNode node in appInfo.Markup)
                {
                    root.AppendChild(xml.ImportNode(node!, true));
                }

                builder.Comment(root.InnerXml);
            }

            if (doc?.Markup != null && doc.Markup.Length > 0)
            {
                XmlDocument xml = new XmlDocument();
                XmlElement root = xml.CreateElement("root");
                xml.AppendChild(root);
                foreach (XmlNode node in doc.Markup)
                {
                    root.AppendChild(xml.ImportNode(node!, true));
                }

                builder.Description(root.InnerXml);
            }
        }

        /// <summary>
        /// Specifically for handling SERES schema annotation
        /// </summary>
        private void AddSchemaRootAnnotation(XmlSchemaAnnotation annotation, JsonSchemaBuilder builder)
        {
            XmlSchemaDocumentation documentation = (XmlSchemaDocumentation)annotation.Items
               .OfType<XmlSchemaObject>()
               .FirstOrDefault(obj => obj is XmlSchemaDocumentation);

            if (documentation == null)
            {
                return;
            }

            ArrayBufferWriter<byte> buffer = new ArrayBufferWriter<byte>();

            using (Utf8JsonWriter writer = new Utf8JsonWriter(buffer))
            {
                if (documentation.Markup != null)
                {
                    writer.WriteStartObject();
                    foreach (XmlNode node in documentation.Markup)
                    {
                        if (node == null)
                        {
                            continue;
                        }

                        if (XmlSchemaNamespace.Equals(node.NamespaceURI) && "attribute".Equals(node.LocalName))
                        {
                            string name = node.Attributes?["name"]?.Value;
                            string value = node.Attributes?["fixed"]?.Value;

                            if (string.IsNullOrWhiteSpace(name))
                            {
                                continue;
                            }

                            writer.WriteString(name, value);
                        }
                    }

                    writer.WriteEndObject();
                }
            }

            Utf8JsonReader reader = new Utf8JsonReader(buffer.WrittenSpan);
            JsonDocument info = JsonDocument.ParseValue(ref reader);

            builder.Info(info.RootElement);
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAnnotation(XmlSchemaAnnotation item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAttribute(XmlSchemaAttribute item)
        {
            JsonSchemaBuilder builder = new JsonSchemaBuilder();

            if (item.Annotation != null)
            {
                AddAnnotation(item.Annotation, builder);
            }

            if (!item.RefName.IsEmpty)
            {
                builder.Ref(GetReferenceFromTypename(item.RefName));
                builder.XsdType("#ref");
            }
            else if (!item.SchemaTypeName.IsEmpty)
            {
                int minOccurs = item.Use == XmlSchemaUse.Optional ? 0 : 1;
                HandleType(item.SchemaTypeName, minOccurs, 1, builder);
            }
            else if (item.SchemaType != null)
            {
                HandleSimpleType(item.SchemaType, builder);
            }

            if (item.DefaultValue != null)
            {
                builder.Default(item.DefaultValue.AsJsonElement());
            }

            if (item.FixedValue != null)
            {
                builder.Const(item.FixedValue.AsJsonElement());
            }

            AddUnhandledAttributes(item, builder);

            return builder;
        }

        private void HandleSimpleType(XmlSchemaSimpleType schemaType, JsonSchemaBuilder builder)
        {
            switch (schemaType.Content)
            {
                case XmlSchemaSimpleTypeRestriction x:
                    HandleSimpleTypeRestriction(x, builder);
                    break;
                case XmlSchemaSimpleTypeList x:
                    HandleSimpleTypeList(x, builder);
                    break;
                case XmlSchemaSimpleTypeUnion x:
                    throw new XmlSchemaException("Altinn studio does not support xsd unions", null, x.LineNumber, x.LinePosition);
            }
        }

        private void HandleSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction item, JsonSchemaBuilder builder)
        {
            JsonSchemaBuilder baseTypeSchema = null;

            if (item.BaseType != null)
            {
                baseTypeSchema = ConvertSchemaSimpleType(item.BaseType);
            }
            else if (!item.BaseTypeName.IsEmpty)
            {
                baseTypeSchema = new JsonSchemaBuilder();
                HandleType(item.BaseTypeName, 1, 1, baseTypeSchema);
            }

            JsonSchemaBuilder restrictionSchemaBuilder = baseTypeSchema == null ? builder : new JsonSchemaBuilder();

            List<string> enumValues = new List<string>();
            List<string> xsdRestrictions = new List<string>();

            foreach (XmlSchemaFacet facet in item.Facets.Cast<XmlSchemaFacet>())
            {
                HandleRestrictionFacet(facet, restrictionSchemaBuilder, ref enumValues, ref xsdRestrictions);
            }

            if (enumValues.Count > 0)
            {
                restrictionSchemaBuilder.Enum(enumValues.Select(val => val.AsJsonElement()));
            }

            if (baseTypeSchema != null)
            {
                builder.AllOf(baseTypeSchema, restrictionSchemaBuilder);
            }
        }

        private void HandleRestrictionFacet(XmlSchemaFacet facet, JsonSchemaBuilder builder, ref List<string> enumValues, ref List<string> xsdRestrictions)
        {
            decimal dLength;
            uint uiLength;

            string xsdRestriction = facet.GetType().Name;
            xsdRestriction = xsdRestriction[9..^5];
            xsdRestriction = char.ToLowerInvariant(xsdRestriction[0]) + xsdRestriction[1..];
            xsdRestrictions.Add($"xsdRestriction:{xsdRestriction}:{facet.Value}");

            switch (facet)
            {
                case XmlSchemaEnumerationFacet:
                    enumValues.Add(facet.Value);
                    break;
                case XmlSchemaFractionDigitsFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && uint.TryParse(facet.Value, out uiLength))
                    {
                        builder.MultipleOf(1m / (decimal)Math.Pow(10, uiLength));
                    }

                    break;
                case XmlSchemaLengthFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && uint.TryParse(facet.Value, out uiLength))
                    {
                        builder.MaxLength(uiLength);
                        builder.MinLength(uiLength);
                    }

                    break;
                case XmlSchemaMaxExclusiveFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && decimal.TryParse(facet.Value, out dLength))
                    {
                        builder.ExclusiveMaximum(dLength);
                    }

                    break;
                case XmlSchemaMaxInclusiveFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && decimal.TryParse(facet.Value, out dLength))
                    {
                        builder.Maximum(dLength);
                    }

                    break;
                case XmlSchemaMaxLengthFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && uint.TryParse(facet.Value, out uiLength))
                    {
                        builder.MaxLength(uiLength);
                    }

                    break;
                case XmlSchemaMinExclusiveFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && decimal.TryParse(facet.Value, out dLength))
                    {
                        builder.ExclusiveMinimum(dLength);
                    }

                    break;
                case XmlSchemaMinInclusiveFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && decimal.TryParse(facet.Value, out dLength))
                    {
                        builder.Minimum(dLength);
                    }

                    break;
                case XmlSchemaMinLengthFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && uint.TryParse(facet.Value, out uiLength))
                    {
                        builder.MinLength(uiLength);
                    }

                    break;
                case XmlSchemaTotalDigitsFacet:
                    if (!string.IsNullOrWhiteSpace(facet.Value) && uint.TryParse(facet.Value, out uiLength))
                    {
                        builder.MaxLength(uiLength);
                    }

                    break;
                case XmlSchemaPatternFacet:
                    string pattern = facet.Value;
                    builder.Pattern(pattern ?? throw new NullReferenceException("value of the pattern facet cannot be null"));
                    break;
                case XmlSchemaWhiteSpaceFacet:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(facet.GetType().Name);
            }
        }

        private void HandleSimpleTypeList(XmlSchemaSimpleTypeList item, JsonSchemaBuilder builder)
        {
            builder.Type(SchemaValueType.Array);

            JsonSchemaBuilder itemTypeSchema;
            if (item.ItemType != null)
            {
                itemTypeSchema = ConvertSchemaSimpleType(item.ItemType);
            }
            else if (!item.ItemTypeName.IsEmpty)
            {
                itemTypeSchema = new JsonSchemaBuilder();
                HandleType(item.ItemTypeName, 1, 1, itemTypeSchema);
            }
            else
            {
                throw new XmlSchemaException("Invalid list definition, must include \"itemType\" or \"simpleType\"", null, item.LineNumber, item.LinePosition);
            }

            builder.Items(itemTypeSchema);
        }

        private void AddUnhandledAttributes(XmlSchemaAnnotated item, JsonSchemaBuilder builder)
        {
            if (item.UnhandledAttributes != null && item.UnhandledAttributes.Length > 0)
            {
                IEnumerable<(string Name, string Value)> unhandledAttributes = item.UnhandledAttributes.Select(attr => (attr.Name, attr.Value));
                builder.XsdUnhandledAttributes(unhandledAttributes);
            }
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAttributeGroup(XmlSchemaAttributeGroup item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAttributeGroupRef(XmlSchemaAttributeGroupRef item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaElement(XmlSchemaElement item)
        {
            JsonSchemaBuilder builder = new JsonSchemaBuilder();

            if (item.Annotation != null)
            {
                AddAnnotation(item.Annotation, builder);
            }

            switch (item.SchemaType)
            {
                case XmlSchemaSimpleType x:
                    break;
                case XmlSchemaComplexType x:
                    break;
            }

            AddUnhandledAttributes(item, builder);

            return builder;
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaChoice(XmlSchemaChoice item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAll(XmlSchemaAll item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSequence(XmlSchemaSequence item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaGroup(XmlSchemaGroup item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaGroupRef(XmlSchemaGroupRef item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleType(XmlSchemaSimpleType item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleTypeList(XmlSchemaSimpleTypeList item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleTypeUnion(XmlSchemaSimpleTypeUnion item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaComplexType(XmlSchemaComplexType item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleContent(XmlSchemaSimpleContent item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaComplexContent(XmlSchemaComplexContent item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleContentExtension(XmlSchemaSimpleContentExtension item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaSimpleContentRestriction(XmlSchemaSimpleContentRestriction item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaComplexContentExtension(XmlSchemaComplexContentExtension item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaComplexContentRestriction(XmlSchemaComplexContentRestriction item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAny(XmlSchemaAny item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public JsonSchemaBuilder ConvertSchemaAnyAttribute(XmlSchemaAnyAttribute item)
        {
            throw new NotImplementedException();
        }

        private static string GetReferenceFromTypename(XmlQualifiedName typeName)
        {
            if (typeName.IsEmpty)
            {
                throw new InvalidOperationException("Cannot create reference to empty type name");
            }

            return GetReferenceFromName(typeName.Name);
        }

        private static string GetReferenceFromName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new InvalidOperationException("Cannot create reference to empty name");
            }

            return $"#/definitions/{name}";
        }

        private void HandleType(XmlQualifiedName typeName, decimal minOccurs, decimal maxOccurs, JsonSchemaBuilder builder)
        {
            //if (ArrayScope && maxOccurs <= 1)
            //{
            //    maxOccurs = decimal.MaxValue;
            //    if (OptionalScope)
            //    {
            //        minOccurs = 0;
            //    }
            //}

            JsonSchemaBuilder typeBuilder = builder;

            if (GetTypeAndFormat(typeName, out SchemaValueType? type, out Format format, out string xsdType))
            {
                if (maxOccurs > 1)
                {
                    typeBuilder = new JsonSchemaBuilder();
                }

                if (type != null)
                {
                    typeBuilder.Type(type.Value);
                }
                else
                {
                    typeBuilder.Ref(GetReferenceFromTypename(typeName));
                }

                if (format != null)
                {
                    typeBuilder.Format(format);
                }

                if (maxOccurs > 1)
                {
                    if (minOccurs > 0)
                    {
                        typeBuilder.MinItems((uint)minOccurs);
                    }

                    if (maxOccurs < decimal.MaxValue)
                    {
                        typeBuilder.MaxItems((uint)maxOccurs);
                    }

                    JsonSchema itemsSchema = typeBuilder;
                    typeBuilder = builder;
                    builder.Type(SchemaValueType.Array);
                    builder.Items(itemsSchema);
                }

                if (xsdType != null)
                {
                    typeBuilder.XsdType(xsdType);
                }
            }
        }

        private static bool GetTypeAndFormat(XmlQualifiedName typename, out SchemaValueType? type, out Format format, out string xsdType)
        {
            if (typename.IsEmpty)
            {
                type = null;
                format = null;
                xsdType = null;
                return false;
            }

            if (XmlSchemaNamespace.Equals(typename.Namespace))
            {
                xsdType = typename.Name;

                switch (typename.Name)
                {
                    case "boolean":
                        type = SchemaValueType.Boolean;
                        format = null;
                        return true;

                    case "integer":
                    case "nonPositiveInteger":
                    case "negativeInteger":
                    case "nonNegativeInteger":
                    case "positiveInteger":
                    case "long":
                    case "int":
                    case "short":
                    case "byte":
                    case "unsignedLong":
                    case "unsignedInt":
                    case "unsignedShort":
                    case "unsignedByte":
                        type = SchemaValueType.Integer;
                        format = null;
                        return true;

                    case "anyAtomicType":
                    case "anySimpleType":
                    case "string":
                    case "gYearMonth":
                    case "gYear":
                    case "gMonthDay":
                    case "gDay":
                    case "gMonth":
                    case "hexBinary":
                    case "base64Binary":
                    case "QName":
                    case "NOTATION":
                    case "normalizedString":
                    case "token":
                    case "language":
                    case "NMTOKEN":
                    case "Name":
                    case "NCName":
                    case "ID":
                    case "IDREF":
                    case "ENTITY":
                    case "yearMonthDuration":
                    case "dayTimeDuration":
                        type = SchemaValueType.String;
                        format = null;
                        return true;

                    case "dateTime":
                        type = SchemaValueType.String;
                        format = Formats.DateTime;
                        return true;
                    case "time":
                        type = SchemaValueType.String;
                        format = Formats.Time;
                        return true;
                    case "date":
                        type = SchemaValueType.String;
                        format = Formats.Date;
                        return true;
                    case "duration":
                        type = SchemaValueType.String;
                        format = Formats.Duration;
                        return true;
                    case "anyURI":
                        type = SchemaValueType.String;
                        format = Formats.Uri;
                        return true;

                    case "decimal":
                    case "float":
                    case "double":
                        type = SchemaValueType.Number;
                        format = null;
                        return true;

                    default:
                        throw new IndexOutOfRangeException($"Unknown in-build type '{typename}'");
                }
            }

            type = null;
            format = null;
            xsdType = null;
            return true;
        }
    }
}
