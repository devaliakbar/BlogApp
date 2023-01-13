
using System.ComponentModel.DataAnnotations;

namespace BlogApp.DTOs
{
    public class SignUpDTO
    {
        [Required, MinLength(3)]
        public string UserName { get; set; }
        [Required, MinLength(8)]
        public string Password { get; set; }
    }
}