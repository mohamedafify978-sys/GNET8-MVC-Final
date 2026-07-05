using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.ViewModels.MemberShipViewModel
{
    public class CreateMemberShipViewModel
    {
        public int PlanId { get; set; }
        public int MemberId { get; set; }
        public DateTime? StartDate { get; set; }

    }
}
