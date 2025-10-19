using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinsFashion.Data.Models
{
    public class Color
    {
        [Key]
        public Guid Id { get; set; }

        public required string Name { get; set; }
    }
}
