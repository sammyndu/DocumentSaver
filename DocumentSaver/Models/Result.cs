namespace DocumentSaver.Models
{
    public class Result<T>
    {
        public T Content { get; set; }
        public Error Error { get; set; }
        public string RequestId { get; set; } = "";
        public bool IsSuccess => Error == null;
        public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
    }

}
