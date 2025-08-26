using name_sorter.Interfaces;

namespace name_sorter
{
    /// Application orchestrator class
    public class NameSorter(IFileService fileService, INameSorter sorter)
    {
        public async Task Run(string[] args)
        {
            try

            {
                //Checking Argument input - input file Path is received as an argument
                if (args.Length <= 0)
                {
                    Console.WriteLine("ERROR: no Argument(Input File Path) Provided");
                    Console.WriteLine("Usage: name-sorter <input file path>");
                    return;
                }

                var inputFilePath = args[0];
                //Default Output Path
                const string outputFilePath = "sorted-names-list.txt";

                // Sort the names
                await SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
                Environment.Exit(1);
            }
        }

        //The main Name Sort Orchestrator function: reading input file > sort names > write output file
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
                var names = await fileService.ReadTextFileLinesAsync(inputFilePath, cancellationToken);

                // Sort names
                var sortedNames = sorter.SortNames(names).ToList();

                // Write sorted names to output file
                await fileService.WriteTextFileLinesAsync(outputFilePath, sortedNames, cancellationToken);

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