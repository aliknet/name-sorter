using name_sorter.Interfaces;
using name_sorter.Models;

namespace name_sorter.Services
{
    public class NameParser : INameParser
    {
        public NameObject ParseName(string fullName)
        {
            //Validating input
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be null or empty", nameof(fullName));
            //Array of Names - index 0 is LastName, index 1-3 are given name(s)
            string[] nameParts = fullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            //Validating Last Name and Given Names - At Least 1 Last Name and 1 Given name should be provided
            if (string.IsNullOrWhiteSpace(nameParts[0]))
                throw new ArgumentException("Last name is required");
            if (nameParts.Length <= 1)
                throw new ArgumentException("Given name(s) not provided, A name must have between 1 and 3 given names",
                    fullName);
            if (nameParts.Length > 4)
                throw new ArgumentException($"Given Names more than 3 for '{fullName}'", fullName);
            //Returning Name Objet
            return new NameObject(nameParts.Last(), nameParts.Except([nameParts.Last()]));
        }
    }
}