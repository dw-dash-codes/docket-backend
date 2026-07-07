using System.ComponentModel.DataAnnotations;

namespace RagSaaS.Backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

    public class AuthRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Name { get; set; }
    }
}
