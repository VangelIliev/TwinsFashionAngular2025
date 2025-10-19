using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TwinsFashion.Data.Models
{
    public class Order
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;
        public DateTime Date { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}
