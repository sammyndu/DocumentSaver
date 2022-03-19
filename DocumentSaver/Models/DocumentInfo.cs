using System.ComponentModel.DataAnnotations;

namespace DocumentSaver.Models
{
    public class DocumentInfo
    {
        [Key]
        public long Id { get; set; }

        public string DocumentId { get; set; }

        public string DocumentName { get; set; }

        public string DocumentContent { get; set; }

        public DateTime DateSubmitted  { get; set; }

        public DateTime DateModified { get; set; }
    }
}
