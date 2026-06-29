using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.ViewModels.PlanViewModel
{
    public class PlanViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } =default!;
        public string Description { get; set; } = default!;
        public int durationDays  { get; set; }
        public decimal price { get; set; }
        public bool IsActive { get; set; }

    }
}
