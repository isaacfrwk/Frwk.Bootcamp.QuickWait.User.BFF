using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FrwkQuickWait.Domain.Entity
{
    public class UserProfile
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public DateTime Birth { get; set; }
        public string? Phone { get; set; }
        [ValidateNever]
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? CPF { get; set; }
        public string? CNPJ { get; set; }
        public Address? Address { get; set; }
    }
}
