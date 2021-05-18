using System;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Json.Keywords;
using Altinn.Studio.DataModeling.Utils;
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

            return xsd;
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
            throw new NotImplementedException();
        }
    }
}
