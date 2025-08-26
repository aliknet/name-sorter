namespace name_sorter.Interfaces
{
    public interface IFileService
    {
        Task<List<string>> ReadTextFileLinesAsync(string path, CancellationToken cancellationToken);
        Task WriteTextFileLinesAsync(string path, List<string> lines, CancellationToken cancellationToken);
    }
}
