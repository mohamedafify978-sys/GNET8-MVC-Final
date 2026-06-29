using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models
{
    public class Plan : BaseEntity
    {
        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;
        [Required, MaxLength(200)]

        public string Description { get; set; } = null!;
        public int DurationDays { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    }
}
