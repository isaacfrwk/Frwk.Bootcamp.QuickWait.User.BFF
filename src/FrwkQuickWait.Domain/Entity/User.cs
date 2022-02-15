using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace FrwkQuickWait.Domain.Entity
{
    public class User
    {
        [ValidateNever]
        public string? Username { get; set; }
        public string? Password { get; set; }
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
