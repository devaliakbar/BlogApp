
namespace BlogApp.DTOs
{
    public class BlogDto : CreateBlogDto
    {
        public string Id { get; set; }
        public UserDTO Owner { get; set; }
    }
}