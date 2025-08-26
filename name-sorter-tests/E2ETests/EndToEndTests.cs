using name_sorter.Services;

namespace name_sorter.tests.E2ETests
{
    public class EndToEndTests
    {
        private readonly string _testDirectory;
        private readonly FileService _fileService;
        private readonly NameParser _nameParser;
        private readonly NameSorterService _nameSorterService;
        private readonly NameSortProcessor _nameSortProcessor;

        public EndToEndTests()
        {
            _testDirectory = Path.Combine(Path.GetTempPath(), "name-sorter-e2e-tests");
            if (Directory.Exists(_testDirectory))
            {
                Directory.Delete(_testDirectory, true);
            }

            Directory.CreateDirectory(_testDirectory);

            _fileService = new FileService();
            _nameParser = new NameParser();
            _nameSorterService = new NameSorterService(_nameParser);
            _nameSortProcessor = new NameSortProcessor(_fileService, _nameSorterService);
        }

        [Fact]
        public async Task EndToEnd_WithSampleData_ShouldSortCorrectly()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "output.txt");

            var inputNames = new List<string>
            {
                "Janet Parsons",
                "Vaughn Lewis",
                "Adonis Julius Archer",
                "Shelby Nathan Yoder",
                "Marin Alvarez",
                "London Lindsey",
                "Beau Tristan Bentley",
                "Leo Gardner",
                "Hunter Uriah Mathew Clarke",
                "Mikayla Lopez",
                "Frankie Conner Ritter"
            };

            await File.WriteAllLinesAsync(inputFilePath, inputNames);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath);

            Assert.Equal(11, sortedNames.Length);

            // Verify the names are sorted correctly (by last name, then by other names)
            var expectedOrder = new List<string>
            {
                "Marin Alvarez",
                "Adonis Julius Archer",
                "Beau Tristan Bentley",
                "Hunter Uriah Mathew Clarke",
                "Leo Gardner",
                "Vaughn Lewis",
                "London Lindsey",
                "Mikayla Lopez",
                "Janet Parsons",
                "Frankie Conner Ritter",
                "Shelby Nathan Yoder"
            };

            for (int i = 0; i < expectedOrder.Count; i++)
            {
                Assert.Equal(expectedOrder[i], sortedNames[i]);
            }
        }

        [Fact]
        public async Task EndToEnd_WithEmptyFile_ShouldCreateEmptyOutput()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "empty-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "empty-output.txt");

            await File.WriteAllTextAsync(inputFilePath, "");

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath);
            Assert.Empty(sortedNames);
        }

        [Fact]
        public async Task EndToEnd_WithWhitespaceOnly_ShouldCreateEmptyOutput()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "whitespace-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "whitespace-output.txt");

            await File.WriteAllTextAsync(inputFilePath, "   \n  \n  ");

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath);
            Assert.Empty(sortedNames);
        }

        [Fact]
        public async Task EndToEnd_WithMixedNameFormats_ShouldSortCorrectly()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "mixed-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "mixed-output.txt");

            var inputNames = new List<string>
            {
                "John Doe",
                "Jane Michael Smith",
                "Alice Johnson",
                "Bob A B C",
                "Charlie D E F"
            };

            await File.WriteAllLinesAsync(inputFilePath, inputNames);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath);

            Assert.Equal(5, sortedNames.Length);

            // Verify sorting order
            Assert.Equal("Bob A B C", sortedNames[0]);
            Assert.Equal("John Doe", sortedNames[1]);
            Assert.Equal("Charlie D E F", sortedNames[2]);
            Assert.Equal("Alice Johnson", sortedNames[3]);
            Assert.Equal("Jane Michael Smith", sortedNames[4]);
        }

        [Fact]
        public async Task EndToEnd_WithSpecialCharacters_ShouldSortCorrectly()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "special-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "special-output.txt");

            var inputNames = new List<string>
            {
                "José María García",
                "François Dupont",
                "Björk Guðmundsdóttir",
                "Jean-Pierre O'Connor"
            };

            await File.WriteAllLinesAsync(inputFilePath, inputNames, System.Text.Encoding.UTF8);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath, System.Text.Encoding.UTF8);

            Assert.Equal(4, sortedNames.Length);

            // Verify sorting order (case-sensitive)
            Assert.Equal("François Dupont", sortedNames[0]);
            Assert.Equal("José María García", sortedNames[1]);
            Assert.Equal("Björk Guðmundsdóttir", sortedNames[2]);
            Assert.Equal("Jean-Pierre O'Connor", sortedNames[3]);
        }

        [Fact]
        public async Task EndToEnd_WithLargeDataset_ShouldSortCorrectly()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "large-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "large-output.txt");

            var inputNames = Enumerable.Range(1, 1000)
                .Select(i => $"Person{i} LastName{i}")
                .ToList();

            await File.WriteAllLinesAsync(inputFilePath, inputNames);

            // Act
            await _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);

            // Assert
            Assert.True(File.Exists(outputFilePath));
            var sortedNames = await File.ReadAllLinesAsync(outputFilePath);

            Assert.Equal(1000, sortedNames.Length);

            // Verify first and last entries are correct
            Assert.Equal("Person1 LastName1", sortedNames[0]);
            Assert.Equal("Person998 LastName998", sortedNames[998]);
            Assert.Equal("Person999 LastName999", sortedNames[999]);
        }

        [Fact]
        public async Task EndToEnd_WithInvalidNameFormat_ShouldThrowException()
        {
            // Arrange
            var inputFilePath = Path.Combine(_testDirectory, "invalid-input.txt");
            var outputFilePath = Path.Combine(_testDirectory, "invalid-output.txt");

            var inputNames = new List<string>
            {
                "John Doe",
                "Invalid", // Single name - should cause exception
                "Jane Smith"
            };

            await File.WriteAllLinesAsync(inputFilePath, inputNames);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                _nameSortProcessor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None));

            Assert.Contains("Given name(s) not provided", exception.Message);
        }
    }
}