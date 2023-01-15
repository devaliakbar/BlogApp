
using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class CreateBlogDto
    {
        [Required]
        public string BlogTitle { get; set; }
        [Required]
        public string BlogContent { get; set; }
        [Required]
        public bool IsPublished { get; set; }
    }
}