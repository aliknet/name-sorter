using name_sorter.Interfaces;
using name_sorter.Models;

namespace name_sorter.Services
{
    public class NameSorterService(INameParser nameParser) : INameSorter
    {
        private readonly INameParser _nameParser = nameParser ?? throw new ArgumentNullException(nameof(nameParser));

        public IEnumerable<string> SortNames(IEnumerable<string> names)
        {
            ArgumentNullException.ThrowIfNull(names);

            var nameObjects = names.Select(name => _nameParser.ParseName(name)).ToList();

            //Sorting in the following order : LasName > Given Name 1 > Given Name 2 > Given Name 3
            return nameObjects
                .OrderBy(nObj => nObj.LastName)
                .ThenBy(nObj => nObj.GivenNames.First())
                .ThenBy(nObj => nObj.GivenNames.ElementAtOrDefault(1))
                .ThenBy(nObj => nObj.GivenNames.ElementAtOrDefault(2))
                .Select(nObj => nObj.FullName);
        }
    }

    public class NameSortProcessor(IFileService fileService, INameSorter nameSorter)
    {
        private readonly IFileService
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));

        private readonly INameSorter _nameSorter = nameSorter ?? throw new ArgumentNullException(nameof(nameSorter));

        public async Task SortNamesAsync(string inputFilePath, string outputFilePath,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(inputFilePath))
                throw new ArgumentException("Input file path cannot be null or empty", nameof(inputFilePath));

            if (string.IsNullOrWhiteSpace(outputFilePath))
                throw new ArgumentException("Output file path cannot be null or empty", nameof(outputFilePath));

            try
            {
                // Read names from input file
                var names = await _fileService.ReadTextFileLinesAsync(inputFilePath, cancellationToken);

                // Sort names
                var sortedNames = _nameSorter.SortNames(names).ToList();

                // Write sorted names to output file
                await _fileService.WriteTextFileLinesAsync(outputFilePath, sortedNames, cancellationToken);

                Console.WriteLine(
                    $"Successfully processed {names.Count} names. Sorted names written to: {outputFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing names: {ex.Message}");
                throw;
            }
        }
    }
}