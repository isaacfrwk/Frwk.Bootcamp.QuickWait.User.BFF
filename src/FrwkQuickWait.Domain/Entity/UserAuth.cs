using System.ComponentModel.DataAnnotations;

namespace FrwkQuickWait.Domain.Entity
{
    public class UserAuth
    {
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
