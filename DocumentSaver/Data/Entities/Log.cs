namespace DocumentSaver.Data.Entities
{
    public class Log
    {
        public long Id { get; set; }

        public string Username {  get; set; }
        
        public string Action { get; set; }

        public DateTime DateCreated { get; set; }
    }
}
