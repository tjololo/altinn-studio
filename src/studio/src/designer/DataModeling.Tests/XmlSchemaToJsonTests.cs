using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Json.Keywords;
using Altinn.Studio.DataModeling.Visitor;
using Altinn.Studio.DataModeling.Visitor.Xml;
using DataModeling.Tests.Assertions;
using Json.Schema;
using Xunit;

namespace DataModeling.Tests
{
    public class XmlSchemaToJsonTests
    {
        [Fact]
        public async Task SimpleAll()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleAll.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleAll.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task AltinnAnnotation()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/AltinnAnnotation.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/AltinnAnnotation.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Any()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Any.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Any.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Attributes()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaConverter converter = new XmlSchemaToJsonSchemaConverter();

            //XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Attributes.json");
            XmlSchema  xsd      = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Attributes.xsd");

            // Act
            // xsd.Accept(visitor);
            // JsonSchema actual = visitor.Schema;
            JsonSchema actual = converter.ConvertSchemaNode(xsd);
            string json = JsonSerializer.Serialize(actual, new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = true });

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task BuiltinTypes()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/BuiltinTypes.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/BuiltinTypes.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleChoice.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleChoice.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedChoice.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedChoice.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithOptionalChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithOptionalChoice.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithOptionalChoice.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithArrayChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithArrayChoice.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithArrayChoice.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexContentExtension()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexContentExtension.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexContentExtension.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexContentRestriction()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexContentRestriction.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexContentRestriction.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexSchema()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexSchema.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexSchema.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Definitions()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Definitions.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Definitions.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            string json = visitor.GetSchemaString();

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ElementAnnotation()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ElementAnnotation.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ElementAnnotation.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleTypeRestrictions()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleTypeRestrictions.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleTypeRestrictions.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleSequence.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleSequence.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedSequence.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedSequence.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedSequences()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedSequences.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedSequences.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task InterleavedNestedSequences()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/InterleavedNestedSequences.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/InterleavedNestedSequences.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithOptionalSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithOptionalSequence.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithOptionalSequence.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithArraySequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithArraySequence.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithArraySequence.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleContentExtension()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleContentExtension.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleContentExtension.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleContentRestriction()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleContentRestriction.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleContentRestriction.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleTypeList()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleTypeList.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleTypeList.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SequenceWithGroupRef()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            XmlSchemaToJsonSchemaVisitor visitor = new XmlSchemaToJsonSchemaVisitor();

            JsonSchema expected = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleTypeList.json");
            XmlSchema xsd = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SequenceWithGroupRef.xsd");

            // Act
            xsd.Accept(visitor);
            JsonSchema actual = visitor.Schema;

            var json = visitor.GetSchemaString();

            // Assert
            JsonSchemaAssertions.IsEquivalentTo(expected, actual);
        }
    }
}
