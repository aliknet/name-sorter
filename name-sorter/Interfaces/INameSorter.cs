namespace name_sorter.Interfaces
{
    public interface INameSorter
    {
        IEnumerable<string> SortNames(IEnumerable<string> names);
    }
}