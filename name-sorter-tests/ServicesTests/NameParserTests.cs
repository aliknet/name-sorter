using Xunit;
using name_sorter.Services;
using name_sorter.Models;

namespace name_sorter.tests.ServicesTests
{
    public class NameParserTests
    {
        private readonly NameParser _nameParser = new();

        [Fact]
        public void ParseName_WithTwoNames_ShouldReturnCorrectNameObject()
        {
            // Arrange
            string fullName = "John Doe";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("John", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("Doe", result.LastName);
            Assert.Null(result.GivenNames.ElementAtOrDefault(1));
            Assert.Null(result.GivenNames.ElementAtOrDefault(2));
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public void ParseName_WithThreeNames_ShouldReturnCorrectNameObject()
        {
            // Arrange
            string fullName = "John Michael Doe";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("John", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("Michael", result.GivenNames.ElementAtOrDefault(1));
            Assert.Null(result.GivenNames.ElementAtOrDefault(2));
            Assert.Equal("John Michael Doe", result.FullName);
        }

        [Fact]
        public void ParseName_WithFourNames_ShouldReturnCorrectNameObject()
        {
            // Arrange
            string fullName = "John Michael Smith Doe";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("John", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("Michael", result.GivenNames.ElementAtOrDefault(1));
            Assert.Equal("Smith", result.GivenNames.ElementAtOrDefault(2));
            Assert.Equal("John Michael Smith Doe", result.FullName);
        }

        [Fact]
        public void ParseName_WithExtraSpaces_ShouldTrimAndParseCorrectly()
        {
            // Arrange
            string fullName = "  John   Doe  ";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("John", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("John Doe", result.FullName);
        }

        [Fact]
        public void ParseName_WithSingleName_ShouldThrowArgumentException()
        {
            // Arrange
            string fullName = "John";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _nameParser.ParseName(fullName));
            Assert.Contains("Given name(s) not provided", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ParseName_WithNullOrEmpty_ShouldThrowArgumentException(string fullName)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _nameParser.ParseName(fullName));
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        [Fact]
        public void ParseName_WithMoreThanFourNames_ShouldThrowArgumentException()
        {
            // Arrange
            string fullName = "John Michael Smith Doe Jr";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _nameParser.ParseName(fullName));
            Assert.Contains("Given Names more than 3", exception.Message);
            Assert.Contains(fullName, exception.Message);
        }

        [Fact]
        public void ParseName_WithZeroNames_ShouldThrowArgumentException()
        {
            // Arrange
            string fullName = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => _nameParser.ParseName(fullName));
            Assert.Contains("cannot be null or empty", exception.Message);
        }

        [Fact]
        public void ParseName_WithSpecialCharacters_ShouldParseCorrectly()
        {
            // Arrange
            string fullName = "Jean-Pierre O'Connor";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("Jean-Pierre", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("O'Connor", result.LastName);
            Assert.Equal("Jean-Pierre O'Connor", result.FullName);
        }

        [Fact]
        public void ParseName_WithNumbers_ShouldParseCorrectly()
        {
            // Arrange
            string fullName = "John 2nd Doe";

            // Act
            var result = _nameParser.ParseName(fullName);

            // Assert
            Assert.Equal("John", result.GivenNames.ElementAtOrDefault(0));
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("2nd", result.GivenNames.ElementAtOrDefault(1));
            Assert.Equal("John 2nd Doe", result.FullName);
        }
    }
}