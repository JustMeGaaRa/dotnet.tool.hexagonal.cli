using Silent.Tool.Hexagonal.Cli.Models;
using System;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Models
{
    public class TemplateTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void FromFormat_WithNull_ShouldThrowException(string format)
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() => Template.FromFormat(format));
        }

        [Fact]
        public void FromFormat_WithNotEmptyString_ShouldCreateInstance()
        {
            // Act
            var template = Template.FromFormat("Some kind of string");

            // Assert
            Assert.NotNull(template);
            Assert.Empty(template.AllTokens);
            Assert.Empty(template.UnresolvedTokens);
            Assert.True(template.IsResolved);
        }

        [Fact]
        public void FromFormat_ValidTokens_ShouldCreateNonDefaultInstance()
        {
            // Arrange
            var format = "{COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE}";

            // Act
            var template = Template.FromFormat(format);

            // Assert
            Assert.NotNull(template);
            Assert.NotEmpty(template.AllTokens);
            Assert.NotEmpty(template.UnresolvedTokens);
            Assert.False(template.IsResolved);
        }

        [Fact]
        public void ResolveToken_ResolvingOneToken_ShouldKeepUnresolved()
        {
            // Arrange
            var companyToken = new Token("COMPANY_NAME");
            var serviceToken = new Token("SERVICE_NAME");
            var projectTypeToken = new Token("PROJECT_TYPE");
            var format = "{COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE}";

            // Act
            var template = Template
                .FromFormat(format)
                .ResolveToken(companyToken, "Mozilla");

            // Assert
            Assert.NotNull(template);
            Assert.NotEmpty(template.AllTokens);
            Assert.NotEmpty(template.UnresolvedTokens);
            Assert.False(template.IsResolved);
        }

        [Fact]
        public void ResolveToken_ResolvingAllTokens_ShouldBeResolved()
        {
            // Arrange
            var companyToken = new Token("COMPANY_NAME");
            var serviceToken = new Token("SERVICE_NAME");
            var projectTypeToken = new Token("PROJECT_TYPE");
            var format = "{COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE}";

            // Act
            var template = Template
                .FromFormat(format)
                .ResolveToken(companyToken, "Mozilla")
                .ResolveToken(serviceToken, "Firefox")
                .ResolveToken(projectTypeToken, "Web");

            // Assert
            Assert.NotNull(template);
            Assert.NotEmpty(template.AllTokens);
            Assert.Empty(template.UnresolvedTokens);
            Assert.True(template.IsResolved);
        }
    }
}
