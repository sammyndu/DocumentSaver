using DocumentSaver.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace DocumentSaver.Models
{
    public class RegisterRequest
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public Role Role { get; set; }

    }
}
