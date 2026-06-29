using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models
{
    public class Membership: BaseEntity
    {
       
        public DateTime EndDate { get; set; }
        [NotMapped]
        public string status => DateTime.Now < EndDate ? "Active" : "Expired";

        public int MemberId { get; set; }   
        public Member Member { get; set; } = null!;
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = null!;
    }
}
