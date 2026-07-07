namespace RagSaaS.Backend.Models
{
    public class ChatRequest
    {
        public string Question { get; set; }
        public int UserId { get; set; }
        public int? SessionId { get; set; }
        public string? ActiveFile { get; set; }
    }
}
