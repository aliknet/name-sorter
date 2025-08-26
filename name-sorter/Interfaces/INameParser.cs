using name_sorter.Models;

namespace name_sorter.Interfaces
{
    public interface INameParser
    {
        NameObject ParseName(string fullName);
    }
}