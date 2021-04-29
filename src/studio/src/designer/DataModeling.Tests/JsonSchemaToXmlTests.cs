using System.Threading.Tasks;
using System.Xml.Schema;
using Altinn.Studio.DataModeling.Json.Keywords;
using Altinn.Studio.DataModeling.Visitor;
using Altinn.Studio.DataModeling.Visitor.Json;
using DataModeling.Tests.Assertions;
using Json.Schema;
using Xunit;

namespace DataModeling.Tests
{
    public class JsonSchemaToXmlTests
    {
        [Fact]
        public async Task SimpleAll()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleAll.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleAll.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task AltinnAnnotation()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/AltinnAnnotation.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/AltinnAnnotation.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Any()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Any.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Any.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Attributes()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Attributes.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Attributes.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task BuiltinTypes()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/BuiltinTypes.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/BuiltinTypes.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleChoice.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleChoice.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedChoice.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedChoice.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithOptionalChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithOptionalChoice.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithOptionalChoice.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithArrayChoice()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithArrayChoice.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithArrayChoice.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexContentExtension()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexContentExtension.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexContentExtension.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexContentRestriction()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexContentRestriction.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexContentRestriction.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ComplexSchema()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ComplexSchema.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ComplexSchema.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task Definitions()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/Definitions.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/Definitions.xsd");

            // Act
            jsonSchema.Accept(null, visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task ElementAnnotation()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/ElementAnnotation.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/ElementAnnotation.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleTypeRestrictions()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleTypeRestrictions.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleTypeRestrictions.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleSequence.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleSequence.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedSequence.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedSequence.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedSequences()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedSequences.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedSequences.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithOptionalSequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithOptionalSequence.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithOptionalSequence.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task NestedWithArraySequence()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/NestedWithArraySequence.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/NestedWithArraySequence.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleContentExtension()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleContentExtension.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleContentExtension.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleContentRestriction()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleContentRestriction.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleContentRestriction.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }

        [Fact]
        public async Task SimpleTypeList()
        {
            // Arrange
            JsonSchemaKeywords.RegisterXsdKeywords();
            JsonSchemaToXmlSchemaVisitor visitor = new JsonSchemaToXmlSchemaVisitor();

            JsonSchema jsonSchema = await ResourceHelpers.LoadJsonSchemaTestData("Model/JsonSchema/SimpleTypeList.json");
            XmlSchema expected = ResourceHelpers.LoadXmlSchemaTestData("Model/XmlSchema/SimpleTypeList.xsd");

            // Act
            jsonSchema.Accept(visitor);
            XmlSchema actual = visitor.Schema;

            // Assert
            XmlSchemaAssertions.IsEquivalentTo(expected, actual);
        }
    }
}
