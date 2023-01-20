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

        public string Case { get; set; }

        public DateTime DateSubmitted  { get; set; }

        public DateTime DateModified { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? FormDate { get; set; }

        public bool New { get; set; }
    }
}
