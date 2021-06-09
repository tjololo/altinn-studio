using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
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

        private readonly Dictionary<string, string> _namespaces;
        private readonly XmlDocument   _xmlFactoryDocument;

        /// <summary>
        /// Placeholder
        /// </summary>
        public JsonSchemaToXmlSchemaConverter()
        {
            _xmlFactoryDocument = new XmlDocument();
            _namespaces = new Dictionary<string, string>();
        }

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

            HandleDefinition(xsd, keywords, false);
            HandleDefinitions(xsd, keywords.Pull<DefinitionsKeyword>());
            HandleDefs(xsd, keywords.Pull<DefsKeyword>());

            List<IJsonSchemaKeyword> unhandled = keywords.EnumerateUnhandledItems().ToList();
            if (unhandled.Count > 0)
            {
                // throw new Exception($"Not handled keywords{Environment.NewLine}{string.Join(Environment.NewLine, unhandled.Select(kw => kw.Keyword()))}");
            }

            return xsd;
        }

        private void HandleDefinition(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords, bool required)
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
                HandleType(item, type, keywords, required);
            }
            else
            {
                HandleObjectDefinition(item, keywords, required);
            }

            ConstKeyword constKeyword = keywords.Pull<ConstKeyword>();
            DefaultKeyword defaultKeyword = keywords.Pull<DefaultKeyword>();

            if (item is XmlSchemaElement element)
            {
                element.FixedValue = constKeyword?.Value.GetString();
                element.DefaultValue = defaultKeyword?.Value.GetString();
            }
            else if (item is XmlSchemaAttribute attribute)
            {
                attribute.FixedValue = constKeyword?.Value.GetString();
                attribute.DefaultValue = defaultKeyword?.Value.GetString();
            }

            AddUnhandledAttributes(item, keywords);
        }

        private void AddUnhandledAttributes(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords)
        {
            XsdUnhandledAttributesKeyword unhandledAttributesKeyword = keywords.Pull<XsdUnhandledAttributesKeyword>();
            if (unhandledAttributesKeyword != null)
            {
                XmlSchemaAnnotated annotatedItem = item as XmlSchemaAnnotated;
                if (annotatedItem == null)
                {
                    throw new Exception("Unhandled attributes must be added to an annotated xml schema object");
                }

                List<XmlAttribute> unhandledAttributes = new List<XmlAttribute>();

                foreach (var (name, value) in unhandledAttributesKeyword.Properties)
                {
                    string prefix = null, localName, ns = null;
                    string[] nameParts = name.Split(':', 2);
                    if (nameParts.Length == 2)
                    {
                        prefix = nameParts[0];
                        localName = nameParts[1];
                        ns = _namespaces[prefix];
                    }
                    else
                    {
                        localName = name;
                    }

                    XmlAttribute attribute = _xmlFactoryDocument.CreateAttribute(prefix, localName, ns);
                    attribute.Value = value;
                    unhandledAttributes.Add(attribute);
                }

                annotatedItem.UnhandledAttributes = unhandledAttributes.ToArray();
            }
        }

        private void HandleType(XmlSchemaObject item, TypeKeyword type, WorkList<IJsonSchemaKeyword> keywords, bool required)
        {
            switch (type.Type)
            {
                case SchemaValueType.Null:
                case SchemaValueType.Object:
                    HandleObjectDefinition(item, keywords, required);
                    break;
                case SchemaValueType.Array:
                    HandleArrayType(item, keywords, required);
                    break;
                case SchemaValueType.Boolean:
                case SchemaValueType.String:
                case SchemaValueType.Number:
                case SchemaValueType.Integer:
                    SetType(item, type.Type, keywords.Pull<FormatKeyword>()?.Value, keywords.Pull<XsdTypeKeyword>()?.Value);
                    SetRequired(item, required);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }

        private static void SetRequired(XmlSchemaObject item, bool required)
        {
            switch (item)
            {
                case XmlSchemaElement element:
                    element.MinOccursString = required ? null : "0";
                    break;
                case XmlSchemaAttribute attribute:
                    attribute.Use = required ? XmlSchemaUse.Required : XmlSchemaUse.None;
                    break;
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

            switch (item)
            {
                case XmlSchemaAttribute x:
                    x.SchemaTypeName = new XmlQualifiedName(xsdType, XmlSchemaNamespace);
                    break;
                case XmlSchemaElement x:
                    x.SchemaTypeName = new XmlQualifiedName(xsdType, XmlSchemaNamespace);
                    break;
                default:
                    throw new IndexOutOfRangeException("Item does not have a type");
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

        private void HandleArrayType(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords, bool required)
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
            HandleType(item, itemsKeywords.Pull<TypeKeyword>(), itemsKeywords, false);
        }

        private void HandleObjectDefinition(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords, bool b)
        {
            List<XmlSchemaObject> items = new List<XmlSchemaObject>();

            if (keywords.TryPull(out PropertiesKeyword properties))
            {
                ISet<string> required = keywords.Pull<RequiredKeyword>()?.Properties.ToHashSet() ?? new HashSet<string>();

                foreach (var (name, value) in properties.Properties)
                {
                    XmlSchemaObject propertyValue = HandleProperty(name, value, required.Contains(name));
                    items.Add(propertyValue);
                }
            }

            XmlSchemaObject elementContainer = null;
            if (keywords.TryPull(out XsdStructureKeyword structure))
            {
                switch (structure.Value.ToUpperInvariant())
                {
                    case "ALL":
                        elementContainer = new XmlSchemaAll();
                        break;
                    case "SEQUENCE":
                        elementContainer = new XmlSchemaSequence();
                        break;
                    case "CHOICE":
                        elementContainer = new XmlSchemaChoice();
                        break;
                    default:
                        throw new Exception("Unknown structure type");
                }
            }

            XmlSchemaObject typeDefinition;

            switch (item)
            {
                case XmlSchema:
                    typeDefinition = item;
                    elementContainer = item;
                    break;
                case XmlSchemaComplexType:
                    typeDefinition = item;
                    elementContainer ??= new XmlSchemaAll();
                    elementContainer.Parent = typeDefinition;
                    break;
                case XmlSchemaElement x:
                    elementContainer ??= new XmlSchemaAll();
                    typeDefinition = new XmlSchemaComplexType
                    {
                        Parent = item,
                        Particle = (XmlSchemaParticle)elementContainer
                    };
                    elementContainer.Parent = typeDefinition;
                    x.SchemaType = (XmlSchemaType) typeDefinition;
                    break;
                default:
                    throw new Exception("Unknown type definition for element");
            }

            foreach (XmlSchemaObject property in items)
            {
                AddElementOrAttribute(property is XmlSchemaElement ? elementContainer : typeDefinition, property);
            }

            if (elementContainer != item && !HasItems(elementContainer))
            {
                if (elementContainer.Parent != null)
                {
                    ((XmlSchemaComplexType)elementContainer.Parent).Particle = null;
                }

                elementContainer.Parent = null;
            }
            
            //item = PrepareItemForContent(item, keywords);

            //if (keywords.TryPull(out PropertiesKeyword props))
            //{
            //    foreach (var (name, value) in props.Properties)
            //    {
            //        WorkList<IJsonSchemaKeyword> valueKeywords = value.AsWorkList();
            //        bool isAttribute = valueKeywords.Pull<XsdAttributeKeyword>()?.Value == true;

            //        XmlSchemaObject valueItem = isAttribute ?
            //            new XmlSchemaAttribute { Name = name } :
            //            new XmlSchemaElement { Name = name };

            //        HandleDefinition(valueItem, valueKeywords);
            //        AddElementOrAttribute(item, valueItem);

            //        var unhandledKeywords = valueKeywords.EnumerateUnhandledItems().Aggregate(string.Empty, (input, kw) => $"{input}, {kw.Keyword()}").TrimStart(',', ' ');

            //        if (!string.IsNullOrEmpty(unhandledKeywords))
            //        {
            //            //throw new Exception("Unhandled keywords in schema"); // TODO: This is supposed to be silently ignored, remove when the converter is stable
            //        }
            //    }
            //}
        }

        private static bool HasItems(XmlSchemaObject item)
        {
            switch (item)
            {
                case XmlSchema x:
                    return x.Items.Count > 0;
                case XmlSchemaGroupBase x:
                    return x.Items.Count > 0;
                default:
                    throw new ArgumentException($"Schema object of type {item.GetType().Name} cannot have child items");
            }
        }

        private XmlSchemaObject HandleProperty(string name, JsonSchema value, bool required)
        {
            WorkList<IJsonSchemaKeyword> keywords = value.AsWorkList();
            bool isAttribute = keywords.Pull<XsdAttributeKeyword>()?.Value == true;

            XmlSchemaObject valueItem = isAttribute ?
                new XmlSchemaAttribute { Name = name } :
                new XmlSchemaElement { Name = name };

            HandleDefinition(valueItem, keywords, required);

            return valueItem;
        }

        private static void AddElementOrAttribute(XmlSchemaObject item, XmlSchemaObject valueItem)
        {
            if (valueItem is XmlSchemaElement element)
            {
                switch (item)
                {
                    case XmlSchema x:
                        x.Items.Add(element);
                        valueItem.Parent = x;
                        break;
                    case XmlSchemaSequence x:
                        x.Items.Add(element);
                        valueItem.Parent = x;
                        break;
                    case XmlSchemaAll x:
                        x.Items.Add(element);
                        valueItem.Parent = x;
                        break;
                    case XmlSchemaChoice x:
                        x.Items.Add(element);
                        valueItem.Parent = x;
                        break;
                    default:
                        throw new ArgumentException($"Invalid container for element '{item.GetType().Name}'", nameof(item));
                }
            }
            else if (valueItem is XmlSchemaAttribute attribute)
            {
                switch (item)
                {
                    case XmlSchema x:
                        x.Items.Add(attribute);
                        valueItem.Parent = x;
                        break;
                    case XmlSchemaComplexType x:
                        x.Attributes.Add(attribute);
                        valueItem.Parent = x;
                        break;

                    //case XmlSchemaSimpleTypeContent x:
                    //    x.Attributes.Add(attribute);
                    //    valueItem.Parent = x;
                    //    break;
                    default:
                        throw new ArgumentException($"Invalid container for attribute '{item.GetType().Name}'", nameof(item));
                }
            }
        }

        //private static XmlSchemaObject PrepareItemForContent(XmlSchemaObject item, WorkList<IJsonSchemaKeyword> keywords)
        //{
        //    if (item is XmlSchema)
        //    {
        //        return item;
        //    }

        //    if (item is XmlSchemaElement elm)
        //    {
        //        XmlSchemaComplexType content = new XmlSchemaComplexType
        //        {
        //            Parent = elm
        //        };
        //        elm.SchemaType = content;

        //        XmlSchemaSequence sequence = new XmlSchemaSequence
        //        {
        //            Parent = content
        //        };
        //        content.Particle = sequence;

        //        return sequence;
        //    }

        //    throw new Exception();
        //}

        private void HandleDefinitions(XmlSchema xsd, DefinitionsKeyword definitions)
        {
            // throw new NotImplementedException();
        }

        private void HandleDefs(XmlSchema xsd, DefsKeyword defs)
        {
            // throw new NotImplementedException();
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

        private void HandleNamespaces(XmlSchemaObject item, XsdNamespacesKeyword namespaces, bool ensureDefaultXmlNamespaces)
        {
            if (namespaces != null)
            {
                foreach ((string prefix, string ns) in namespaces.Namespaces)
                {
                    item.Namespaces.Add(prefix, ns);
                    _namespaces.Add(prefix, ns);
                }
            }

            if (ensureDefaultXmlNamespaces)
            {
                XmlQualifiedName[] ns = item.Namespaces.ToArray();
                if (ns.All(n => n.Namespace != XmlSchemaNamespace))
                {
                    item.Namespaces.Add("xsd", XmlSchemaNamespace);
                    _namespaces.Add("xsd", XmlSchemaNamespace);
                }

                if (ns.All(n => n.Namespace != XmlSchemaInstanceNamespace))
                {
                    item.Namespaces.Add("xsi", XmlSchemaInstanceNamespace);
                    _namespaces.Add("xsi", XmlSchemaInstanceNamespace);
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
