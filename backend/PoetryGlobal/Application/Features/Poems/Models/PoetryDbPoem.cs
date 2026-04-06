


namespace PoetryGlobal.Features.Poems
{
    public class PoetryDbPoem
    {
        public required string Title { get; set; } = null!;
        public required string Author { get; set; } = null!;
        public required List<string> Lines { get; set; } = null!;
    }
}