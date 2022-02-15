using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwkQuickWait.Domain.Entity
{
    public class Address
    {
        public Guid UserId { get; set; }
        public string? Street { get; set; }
        public string? Neighborhood { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Number { get; set; }
        public string? ZipeCode { get; set; }
    }
}
