using Moq;
using name_sorter.Interfaces;
using name_sorter.Services;

namespace name_sorter.tests.ServicesTests
{
    public class NameSortProcessorTests
    {
        private readonly Mock<IFileService> _mockFileService;
        private readonly Mock<INameSorter> _mockNameSorter;
        private readonly NameSortProcessor _nameSortProcessor;

        public NameSortProcessorTests()
        {
            _mockFileService = new Mock<IFileService>();
            _mockNameSorter = new Mock<INameSorter>();
            _nameSortProcessor = new NameSortProcessor(_mockFileService.Object, _mockNameSorter.Object);
            var testDirectory = Path.Combine(Path.GetTempPath(), "name-sorter-processor-tests");
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }

            Directory.CreateDirectory(testDirectory);
        }

        [Fact]
        public async Task SortNamesAsync_WithValidInput_ShouldProcessSuccessfully()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = new List<string> { "John Doe", "Jane Smith", "Alice Johnson" };
            var sortedNames = new List<string> { "Alice Johnson", "John Doe", "Jane Smith" };

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Returns(sortedNames);
            _mockFileService.Setup(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            _mockFileService.Verify(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None), Times.Once);
            _mockNameSorter.Verify(x => x.SortNames(inputNames), Times.Once);
            _mockFileService.Verify(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task SortNamesAsync_WithEmptyInputFile_ShouldProcessSuccessfully()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = new List<string>();
            var sortedNames = new List<string>();

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Returns(sortedNames);
            _mockFileService.Setup(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            _mockFileService.Verify(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None), Times.Once);
            _mockNameSorter.Verify(x => x.SortNames(inputNames), Times.Once);
            _mockFileService.Verify(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None),
                Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SortNamesAsync_WithInvalidInputPath_ShouldThrowArgumentException(string inputFilePath)
        {
            // Arrange
            const string outputFilePath = "output.txt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Contains("Input file path cannot be null or empty", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SortNamesAsync_WithInvalidOutputPath_ShouldThrowArgumentException(string outputFilePath)
        {
            // Arrange
            const string inputFilePath = "input.txt";

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Contains("Output file path cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task SortNamesAsync_WhenFileServiceThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var expectedException = new FileNotFoundException("File not found");

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task SortNamesAsync_WhenNameSorterThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = new List<string> { "John Doe" };
            var expectedException = new ArgumentException("Invalid name format");

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Throws(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task SortNamesAsync_WhenWriteFileThrowsException_ShouldPropagateException()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = new List<string> { "John Doe" };
            var sortedNames = new List<string> { "John Doe" };
            var expectedException = new UnauthorizedAccessException("Access denied");

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Returns(sortedNames);
            _mockFileService.Setup(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task SortNamesAsync_WithLargeDataset_ShouldProcessSuccessfully()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = Enumerable.Range(1, 1000)
                .Select(i => $"Person{i} LastName{i}")
                .ToList();
            var sortedNames = inputNames.OrderBy(x => x).ToList();

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Returns(sortedNames);
            _mockFileService.Setup(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            _mockFileService.Verify(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None), Times.Once);
            _mockNameSorter.Verify(x => x.SortNames(inputNames), Times.Once);
            _mockFileService.Verify(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None),
                Times.Once);
        }

        [Fact]
        public async Task SortNamesAsync_WithSpecialCharacters_ShouldProcessSuccessfully()
        {
            // Arrange
            const string inputFilePath = "input.txt";
            const string outputFilePath = "output.txt";
            var inputNames = new List<string> { "José María García", "François Dupont", "Björk Guðmundsdóttir" };
            var sortedNames = new List<string> { "Björk Guðmundsdóttir", "François Dupont", "José María García" };

            _mockFileService.Setup(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None))
                .ReturnsAsync(inputNames);
            _mockNameSorter.Setup(x => x.SortNames(inputNames))
                .Returns(sortedNames);
            _mockFileService.Setup(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None))
                .Returns(Task.CompletedTask);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            _mockFileService.Verify(x => x.ReadTextFileLinesAsync(inputFilePath, CancellationToken.None), Times.Once);
            _mockNameSorter.Verify(x => x.SortNames(inputNames), Times.Once);
            _mockFileService.Verify(x => x.WriteTextFileLinesAsync(outputFilePath, sortedNames, CancellationToken.None),
                Times.Once);
        }
    }
}