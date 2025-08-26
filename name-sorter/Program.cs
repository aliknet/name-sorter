using name_sorter.Services;
using name_sorter.Interfaces;

namespace name_sorter
{
    internal class Program
    {
        private static async Task Main(string[] args)
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

                // Dependency injections
                IFileService fileService = new FileService();
                INameParser nameParser = new NameParser();
                INameSorter nameSorter = new NameSorterService(nameParser);
                var processor = new NameSortProcessor(fileService, nameSorter);

                // Sort the names
                await processor.SortNamesAsync(inputFilePath, outputFilePath, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Application error: {ex.Message}");
                Environment.Exit(1);
            }
        }
    }
}