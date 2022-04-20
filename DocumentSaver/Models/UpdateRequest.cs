using DocumentSaver.Data.Entities;

namespace DocumentSaver.Models
{
    public class UpdateRequest
    {
        //public string? Username { get; set; }
        public string? Password { get; set; }
        public Role? Role { get; set; }
        public bool? IsBlocked { get; set; }
    }
}
