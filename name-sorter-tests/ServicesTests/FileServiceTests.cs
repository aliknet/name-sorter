using Xunit;
using name_sorter.Services;
using System.Text;

namespace name_sorter.tests.ServicesTests
{
    public class FileServiceTests : IDisposable
    {
        private readonly FileService _fileService;
        private readonly string _testDirectory;
        private readonly string _testFilePath;
        private readonly string _outputFilePath;

        public FileServiceTests()
        {
            _fileService = new FileService();
            _testDirectory = Path.Combine(Path.GetTempPath(), "name-sorter-tests");
            _testFilePath = Path.Combine(_testDirectory, "test-names.txt");
            _outputFilePath = Path.Combine(_testDirectory, "output-names.txt");

            Directory.CreateDirectory(_testDirectory);
        }

        public void Dispose()
        {
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }
        }

        [Fact]
        public async Task WriteTextFileLinesAsync_WithValidData_ShouldWriteFile()
        {
            // Arrange
            var lines = new List<string> { "John Doe", "Jane Smith", "Alice Johnson" };

            // Act
            await _fileService.WriteTextFileLinesAsync(_outputFilePath, lines, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(_outputFilePath));
            var writtenLines = await File.ReadAllLinesAsync(_outputFilePath);
            Assert.Equal(lines.Count, writtenLines.Length);
            Assert.Equal(lines[0], writtenLines[0]);
            Assert.Equal(lines[1], writtenLines[1]);
            Assert.Equal(lines[2], writtenLines[2]);
        }

        [Fact]
        public async Task WriteTextFileLinesAsync_WithNullLines_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _fileService.WriteTextFileLinesAsync(_outputFilePath, null, CancellationToken.None));

            Assert.Equal("lines", exception.ParamName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task WriteTextFileLinesAsync_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            // Arrange
            var lines = new List<string> { "John Doe" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileService.WriteTextFileLinesAsync(path, lines, CancellationToken.None));

            Assert.Contains("cannot be null or empty", exception.Message);
        }

        [Fact]
        public async Task ReadTextFileLinesAsync_WithValidFile_ShouldReturnAllLines()
        {
            // Arrange
            var expectedLines = new List<string> { "John Doe", "Jane Smith", "Alice Johnson" };
            await File.WriteAllLinesAsync(_testFilePath, expectedLines);

            // Act
            var result = await _fileService.ReadTextFileLinesAsync(_testFilePath, CancellationToken.None);

            // Assert
            Assert.Equal(expectedLines.Count, result.Count);
            Assert.Equal(expectedLines[0], result[0]);
            Assert.Equal(expectedLines[1], result[1]);
            Assert.Equal(expectedLines[2], result[2]);
        }


        [Fact]
        public async Task ReadTextFileLinesAsync_WithNonExistentFile_ShouldThrowFileNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<FileNotFoundException>(() =>
                _fileService.ReadTextFileLinesAsync("non-existent-file.txt", CancellationToken.None));

            Assert.Contains("non-existent-file.txt", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task ReadTextFileLinesAsync_WithInvalidPath_ShouldThrowArgumentException(string path)
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _fileService.ReadTextFileLinesAsync(path, CancellationToken.None));

            Assert.Contains("cannot be null or empty", exception.Message);
        }
    }
}