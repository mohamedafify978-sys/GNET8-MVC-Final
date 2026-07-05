using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models
{
    public class Member : GymUser
    {
        public string photo { get; set; } 
      

        public HealthRecord? HealthRecord { get; set; } 
        public ICollection<Membership> Memberships { get; set; } = new HashSet<Membership>();
   
        public ICollection< Booking> Booking { get; set; } = new HashSet<Booking>();
    }
}
