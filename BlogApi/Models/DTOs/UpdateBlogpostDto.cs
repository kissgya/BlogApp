namespace BlogApi.Models.DTOs
{
    public class UpdateBlogpostDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int blogId { get; set; }
    }
}
