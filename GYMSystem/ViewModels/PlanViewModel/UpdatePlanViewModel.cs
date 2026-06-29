using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.ViewModels.PlanViewModel
{
    public class UpdatePlanViewModel
    {
        public string PlanName { get; set; } = default!;
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200,MinimumLength =5 ,ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = default!;
        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days.")]
        public int durationDays { get; set; }
        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01,10000, ErrorMessage = "Price must be greater than 0.")]
        public decimal price { get; set; }
    }
}
