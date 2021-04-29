using System.Xml.Schema;
using Json.Schema;

namespace Altinn.Studio.DataModeling.Visitor
{
    /// <summary>
    /// Converter interface for an XmlSchema
    /// </summary>
    public interface IXmlSchemaConverter<T>
    {
        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="schema">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaNode(XmlSchema schema);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAnnotation(XmlSchemaAnnotation item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAttribute(XmlSchemaAttribute item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAttributeGroup(XmlSchemaAttributeGroup item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAttributeGroupRef(XmlSchemaAttributeGroupRef item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaElement(XmlSchemaElement item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaChoice(XmlSchemaChoice item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAll(XmlSchemaAll item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSequence(XmlSchemaSequence item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaGroup(XmlSchemaGroup item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaGroupRef(XmlSchemaGroupRef item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleType(XmlSchemaSimpleType item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleTypeList(XmlSchemaSimpleTypeList item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleTypeRestriction(XmlSchemaSimpleTypeRestriction item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleTypeUnion(XmlSchemaSimpleTypeUnion item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaComplexType(XmlSchemaComplexType item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleContent(XmlSchemaSimpleContent item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaComplexContent(XmlSchemaComplexContent item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleContentExtension(XmlSchemaSimpleContentExtension item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaSimpleContentRestriction(XmlSchemaSimpleContentRestriction item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaComplexContentExtension(XmlSchemaComplexContentExtension item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaComplexContentRestriction(XmlSchemaComplexContentRestriction item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAny(XmlSchemaAny item);

        /// <summary>
        /// Visit the schema object
        /// </summary>
        /// <param name="item">The object to visit</param>
        JsonSchemaBuilder ConvertSchemaAnyAttribute(XmlSchemaAnyAttribute item);
    }
}
