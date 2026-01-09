namespace BookService.Models
{
    public class CreateBook
    {
        public string Title { get; set; } = string.Empty;  // non-null
        public string Author { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
    }
}
