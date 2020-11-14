using Silent.Tool.Hexagonal.Cli.Models;
using System;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Models
{
    public class TokenTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Ctor_WithInvalidName_ShouldThrowException(string name)
        {
            // Act, Assert
            Assert.Throws<ArgumentException>(() => new Token(name));
        }

        [Theory]
        [InlineData("service")]
        [InlineData("serVICE")]
        [InlineData("SERVICE")]
        public void Ctor_WithValidName_ShouldCreateInstance(string name)
        {
            // Act
            var token = new Token(name);

            // Assert
            Assert.Equal("{SERVICE}", token.Name);
        }

        [Theory]
        [InlineData("service")]
        [InlineData("serVICE")]
        [InlineData("SERVICE")]
        public void ToString_WithValidName_ShouldCreateInstance(string name)
        {
            // Act
            var token = new Token(name);

            // Assert
            Assert.Equal("{SERVICE}", token.ToString());
        }
    }
}
