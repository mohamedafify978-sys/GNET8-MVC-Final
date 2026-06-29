using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models
{
    public class Booking : BaseEntity

    {
        public int SessionId { get; set; }
        public Session Session { get; set; } = null!;
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;


        //public DateTime BookingDate { get; set; }
        public bool IsAttended { get; set; } = false;
    }
}
