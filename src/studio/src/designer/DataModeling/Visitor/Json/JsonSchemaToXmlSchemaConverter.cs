using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Json.Keywords;
using Altinn.Studio.DataModeling.Utils;
using Json.Pointer;
using Json.Schema;

namespace Altinn.Studio.DataModeling.Visitor.Json
{
    /// <summary>
    /// placeholder
    /// </summary>
    public class JsonSchemaToXmlSchemaConverter
    {
        private const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        private const string XmlSchemaInstanceNamespace = "http://www.w3.org/2001/XMLSchema-instance";

        /// <summary>
        /// placeholder
        /// </summary>
        /// <param name="schema">The Json Schema to be converted</param>
        /// <returns>An xml schema</returns>
        public XmlSchema Convert(JsonSchema schema)
        {
            XmlSchema xsd = new XmlSchema();

            WorkList<IJsonSchemaKeyword> keywords = schema.AsWorkList();

            HandleNamespaces(xsd, keywords.Pull<XsdNamespacesKeyword>(), true);
            HandleSchemaAttributes(xsd, keywords.Pull<XsdSchemaAttributesKeyword>());
            HandleInfo(xsd, keywords.Pull<InfoKeyword>());

            HandleDefinition(xsd, keywords);
            HandleDefinitions(xsd, keywords.Pull<DefinitionsKeyword>());
            HandleDefs(xsd, keywords.Pull<DefsKeyword>());

            List<IJsonSchemaKeyword> unhandled = keywords.EnumerateUnhandledItems().ToList();
            if (unhandled.Count > 0)
            {
                throw new Exception($"Not handled keywords{Environment.NewLine}{string.Join(Environment.NewLine, unhandled.Select(kw => kw.Keyword()))}");
            }

            return xsd;
        }

        private static void HandleDefinition(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords)
        {
            if (keywords.TryPull(out RefKeyword reference))
            {
                HandleRef(item, reference, keywords);
            }
            else if (keywords.TryPull(out OneOfKeyword oneOf))
            {
                throw new NotImplementedException();
            }
            else if (keywords.TryPull(out AnyOfKeyword anyOf))
            {
                throw new NotImplementedException();
            }
            else if (keywords.TryPull(out AllOfKeyword allOf))
            {
                throw new NotImplementedException();
            }
            else if (keywords.TryPull(out TypeKeyword type))
            {
                HandleType(item, type, keywords);
            }
            else
            {
                HandleObjectDefinition(item, keywords);
            }
        }

        private static void HandleType(XmlSchemaObject item, TypeKeyword type, WorkList<IJsonSchemaKeyword> keywords)
        {
            switch (type.Type)
            {
                case SchemaValueType.Null:
                case SchemaValueType.Object:
                    HandleObjectDefinition(item, keywords);
                    break;
                case SchemaValueType.Array:
                    HandleArrayType(item, keywords);
                    break;
                case SchemaValueType.Boolean:
                case SchemaValueType.String:
                case SchemaValueType.Number:
                case SchemaValueType.Integer:
                    SetType(item, type.Type, keywords.Pull<FormatKeyword>()?.Value, keywords.Pull<XsdTypeKeyword>()?.Value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void SetType(XmlSchemaObject item, SchemaValueType type, Format format, string xsdType)
        {
            if (string.IsNullOrWhiteSpace(xsdType))
            {
                switch (type)
                {
                    case SchemaValueType.Boolean:
                        xsdType = "boolean";
                        break;
                    case SchemaValueType.String:
                        xsdType = GetStringTypeFromFormat(format);
                        break;
                    case SchemaValueType.Number:
                        xsdType = "double";
                        break;
                    case SchemaValueType.Integer:
                        xsdType = "long";
                        break;
                    default:
                        xsdType = "string"; // Fallback to open string value
                        break;
                }
            }
        }

        private static string GetStringTypeFromFormat(Format format)
        {
            switch (format.Key)
            {
                case "date-time":
                    return "dateTime";
                case "date":
                    return "date";
                case "time":
                    return "time";
                case "uri":
                    return "anyURI";
            }

            return "string"; // Fallback to open string value
        }

        private static void HandleArrayType(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords)
        {
            ItemsKeyword items = keywords.Pull<ItemsKeyword>();
            if (items == null)
            {
                throw new InvalidOperationException("Schema definition with type 'array' requires an 'items' keyword");
            }

            if (items.ArraySchemas != null && items.ArraySchemas.Count != 1)
            {
                throw new InvalidOperationException("Altinn studio does not support tuple validation of arrays");
            }

            JsonSchema itemsSchema = items.SingleSchema ?? items.ArraySchemas?[0];
            if (itemsSchema == null)
            {
                throw new InvalidOperationException("'items' keyword is missing a definition");
            }

            WorkList<IJsonSchemaKeyword> itemsKeywords = itemsSchema.AsWorkList();
            HandleType(item, itemsKeywords.Pull<TypeKeyword>(), itemsKeywords);

        }

        private static void HandleObjectDefinition(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords)
        {
            List<(string name, XmlSchemaObject)> properties = new List<(string name, XmlSchemaObject)>();

        }

        private void HandleDefinitions(XmlSchema xsd, DefinitionsKeyword definitions)
        {
            throw new NotImplementedException();
        }

        private void HandleDefs(XmlSchema xsd, DefsKeyword defs)
        {
            throw new NotImplementedException();
        }

        private static void HandleRef(XmlSchemaObject item, RefKeyword reference, WorkList<IJsonSchemaKeyword> keywords)
        {
            if (item is XmlSchema schema)
            {
                XmlSchemaElement element = new XmlSchemaElement
                {
                    Parent = schema,
                    Name = "melding",
                    SchemaTypeName = GetTypeFromReference(reference.Reference)
                };

                schema.Items.Add(element);
            }
            else
            {
                switch (item)
                {
                    case XmlSchemaElement x:
                        x.SchemaTypeName = GetTypeFromReference(reference.Reference);
                        break;
                    case XmlSchemaAttribute x:
                        x.SchemaTypeName = GetTypeFromReference(reference.Reference);
                        break;
                    default:
                        throw new ArgumentException($"reference not supported on xml schema object of type {item.GetType().Name}");
                }
            }
        }

        private static XmlQualifiedName GetTypeFromReference(Uri reference)
        {
            JsonPointer pointer = JsonPointer.Parse(reference.OriginalString);
            return new XmlQualifiedName(pointer.Segments.Last().Value);
        }

        private static void HandleNamespaces(XmlSchemaObject item, XsdNamespacesKeyword namespaces, bool ensureDefaultXmlNamespaces)
        {
            if (namespaces != null)
            {
                foreach ((string prefix, string ns) in namespaces.Namespaces)
                {
                    item.Namespaces.Add(prefix, ns);
                }
            }

            if (ensureDefaultXmlNamespaces)
            {
                XmlQualifiedName[] ns = item.Namespaces.ToArray();
                if (ns.All(n => n.Namespace != XmlSchemaNamespace))
                {
                    item.Namespaces.Add("xsd", XmlSchemaNamespace);
                }

                if (ns.All(n => n.Namespace != XmlSchemaInstanceNamespace))
                {
                    item.Namespaces.Add("xsi", XmlSchemaInstanceNamespace);
                }
            }
        }

        private static void HandleSchemaAttributes(XmlSchema schema, XsdSchemaAttributesKeyword attributes)
        {
            if (attributes == null)
            {
                return;
            }

            foreach ((string name, string value) in attributes.Properties)
            {
                switch (name)
                {
                    case nameof(XmlSchema.AttributeFormDefault):
                        schema.AttributeFormDefault = Enum.Parse<XmlSchemaForm>(value);
                        break;
                    case nameof(XmlSchema.ElementFormDefault):
                        schema.ElementFormDefault = Enum.Parse<XmlSchemaForm>(value);
                        break;
                    case nameof(XmlSchema.BlockDefault):
                        schema.BlockDefault = Enum.Parse<XmlSchemaDerivationMethod>(value);
                        break;
                    case nameof(XmlSchema.FinalDefault):
                        schema.FinalDefault = Enum.Parse<XmlSchemaDerivationMethod>(value);
                        break;
                }
            }
        }

        private static void HandleInfo(XmlSchema schema, InfoKeyword info)
        {
            if (info == null || info.Value.ValueKind != JsonValueKind.Object)
            {
                return;
            }

            XmlSchemaAnnotation annotation = new XmlSchemaAnnotation { Parent = schema };
            XmlSchemaDocumentation doc = new XmlSchemaDocumentation();

            annotation.Items.Add(doc);
            doc.Parent = annotation;

            XmlDocument xmlDoc = new XmlDocument();
            List<XmlNode> markup = new List<XmlNode>();

            string nsPrefix = schema.Namespaces.ToArray().Single(ns => ns.Namespace == XmlSchemaNamespace).Name;

            foreach (JsonProperty property in info.Value.EnumerateObject())
            {
                XmlElement attribute = xmlDoc.CreateElement(nsPrefix, "attribute", XmlSchemaNamespace);
                attribute.SetAttribute("name", property.Name);
                attribute.SetAttribute("fixed", property.Value.GetString());
                markup.Add(attribute);
            }

            doc.Markup = markup.ToArray();
            schema.Items.Add(annotation);
        }
    }
}
