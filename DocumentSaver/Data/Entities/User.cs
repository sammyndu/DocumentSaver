using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DocumentSaver.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }

        public Role Role { get; set; }

        [Column(TypeName = "BIT")]
        public bool IsBlocked { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }
    }
}
