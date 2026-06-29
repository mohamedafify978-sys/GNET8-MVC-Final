using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.ViewModels.MemberViewModel
{
    public class MemberViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? photo { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }

        public string? DateofBirth { get; set; }
        public string? Address { get; set; }
        public string? PlanName { get; set; }
        public string? membershipStartDate { get; set; }
        public string? membershipEndDate { get; set; }
    }
}
