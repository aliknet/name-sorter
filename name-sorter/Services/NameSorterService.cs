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
}