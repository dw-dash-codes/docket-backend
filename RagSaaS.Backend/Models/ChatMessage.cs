using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RagSaaS.Backend.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Sender { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public int SessionId { get; set; }

        [ForeignKey("SessionId")]
        public ChatSession? Session { get; set; }
    }
}
