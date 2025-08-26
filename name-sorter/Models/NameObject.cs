namespace name_sorter.Models
{
    //Having an Array of GivenNames for easy expansion of the input
    public sealed record NameObject(string LastName, IEnumerable<string> GivenNames)
    {
        //FullName
        public string FullName => string.Join(' ', GivenNames.Append(LastName));

        public override string ToString() => FullName;
    }
}
