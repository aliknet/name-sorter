using name_sorter.Interfaces;
using System.Text;

namespace name_sorter.Services
{
    public class FileService : IFileService
    {
        public async Task<List<string>> ReadTextFileLinesAsync(string path, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));

            var trimmedPath = path.Trim();

            if (!File.Exists(trimmedPath))
                throw new FileNotFoundException($"File does not exist: {trimmedPath}");

            var fileLines = await File.ReadAllLinesAsync(trimmedPath, cancellationToken);

            return fileLines.Select(line => line.Trim()).Where(trimmedLine => !string.IsNullOrEmpty(trimmedLine))
                .ToList();
        }

        public async Task WriteTextFileLinesAsync(string path, List<string> lines, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Path cannot be null or empty", nameof(path));
            await File.WriteAllLinesAsync(path, lines, Encoding.UTF8, cancellationToken);
        }
    }
}