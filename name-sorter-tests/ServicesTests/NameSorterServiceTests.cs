using Xunit;
using name_sorter.Services;
using name_sorter.Interfaces;
using Moq;

namespace name_sorter.tests.ServicesTests
{
    public class NameSorterServiceTests
    {
        private readonly Mock<INameParser> _mockNameParser;
        private readonly NameSorterService _nameSorterService;

        public NameSorterServiceTests()
        {
            _mockNameParser = new Mock<INameParser>();
            _nameSorterService = new NameSorterService(_mockNameParser.Object);
        }

        [Fact]
        public void SortNames_WithValidNames_ShouldReturnSortedNames()
        {
            // Arrange
            var names = new List<string> { "John Doe", "Jane Smith", "Alice Johnson" };

            _mockNameParser.Setup(x => x.ParseName("John Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John"]));
            _mockNameParser.Setup(x => x.ParseName("Jane Smith"))
                .Returns(new name_sorter.Models.NameObject("Smith", ["Jane"]));
            _mockNameParser.Setup(x => x.ParseName("Alice Johnson"))
                .Returns(new name_sorter.Models.NameObject("Johnson", ["Alice"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Equal(3, result.Count);
            Assert.Equal("John Doe", result[0]);
            Assert.Equal("Alice Johnson", result[1]);
            Assert.Equal("Jane Smith", result[2]);
        }

        [Fact]
        public void SortNames_WithSameLastName_ShouldSortByFirstName()
        {
            // Arrange
            var names = new List<string> { "John Smith", "Jane Smith", "Alice Smith" };

            _mockNameParser.Setup(x => x.ParseName("John Smith"))
                .Returns(new name_sorter.Models.NameObject("Smith", ["John"]));
            _mockNameParser.Setup(x => x.ParseName("Jane Smith"))
                .Returns(new name_sorter.Models.NameObject("Smith", ["Jane"]));
            _mockNameParser.Setup(x => x.ParseName("Alice Smith"))
                .Returns(new name_sorter.Models.NameObject("Smith", ["Alice"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Equal("Alice Smith", result[0]);
            Assert.Equal("Jane Smith", result[1]);
            Assert.Equal("John Smith", result[2]);
        }

        [Fact]
        public void SortNames_WithMiddleNames_ShouldSortCorrectly()
        {
            // Arrange
            var names = new List<string> { "John Michael Doe", "John Smith Doe", "Jane Doe" };

            _mockNameParser.Setup(x => x.ParseName("John Michael Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John", "Michael"]));
            _mockNameParser.Setup(x => x.ParseName("John Smith Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John", "Smith"]));
            _mockNameParser.Setup(x => x.ParseName("Jane Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["Jane"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Equal("Jane Doe", result[0]);
            Assert.Equal("John Michael Doe", result[1]);
            Assert.Equal("John Smith Doe", result[2]);
        }

        [Fact]
        public void SortNames_WithFourNames_ShouldSortCorrectly()
        {
            // Arrange
            var names = new List<string> { "John Michael Smith Doe", "John Michael Doe", "John Doe" };

            _mockNameParser.Setup(x => x.ParseName("John Michael Smith Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John", "Michael", "Smith"]));
            _mockNameParser.Setup(x => x.ParseName("John Michael Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John", "Michael"]));
            _mockNameParser.Setup(x => x.ParseName("John Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Equal("John Doe", result[0]);
            Assert.Equal("John Michael Doe", result[1]);
            Assert.Equal("John Michael Smith Doe", result[2]);
        }

        [Fact]
        public void SortNames_WithEmptyList_ShouldReturnEmptyList()
        {
            // Arrange
            var names = new List<string>();

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void SortNames_WithSingleName_ShouldReturnSingleName()
        {
            // Arrange
            var names = new List<string> { "John Doe" };

            _mockNameParser.Setup(x => x.ParseName("John Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Single(result);
            Assert.Equal("John Doe", result[0]);
        }

        [Fact]
        public void SortNames_WithNullMiddleNames_ShouldSortCorrectly()
        {
            // Arrange
            var names = new List<string> { "John Doe", "John Michael Doe" };

            _mockNameParser.Setup(x => x.ParseName("John Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John"]));
            _mockNameParser.Setup(x => x.ParseName("John Michael Doe"))
                .Returns(new name_sorter.Models.NameObject("Doe", ["John", "Michael"]));

            // Act
            var result = _nameSorterService.SortNames(names).ToList();

            // Assert
            Assert.Equal("John Doe", result[0]);
            Assert.Equal("John Michael Doe", result[1]);
        }
    }
}