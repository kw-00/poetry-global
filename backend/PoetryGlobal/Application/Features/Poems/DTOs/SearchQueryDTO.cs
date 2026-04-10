namespace PoetryGlobal.Features.Poems
{
    /// <summary>
    /// Represents a search query for poems. Importantly, 
    /// it contains its own Equals/GetHashCode implementation,
    /// so equality checks check for class and data equality, 
    /// not object identity.
    /// </summary>
    public class SearchQueryDTO(string? title, string? author)
    {
        public string Title { get; set; } = title ?? "";
        public string Author { get; set; } = author ?? "";

        public override bool Equals(object? obj)
        {
            if (obj is SearchQueryDTO other) return Title == other.Title && Author == other.Author;
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Title, Author);
    }
}