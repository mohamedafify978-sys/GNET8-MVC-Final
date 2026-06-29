using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.ViewModels.TrainerViewModel
{
    public class TrainerViewModel
    {
        public int Id { get; set; }
        public string? photo { get; set; } 
        public string Name { get; set; }= default!;
        public string Email { get; set; }= default!;
        public string Phone { get; set; } = default!;
        public string  DateOfBirth { get; set; } = default!;
        public string specialization { get; set; } = default!;

        public string gender { get; set; } = default!;
        public string address { get; set; } = default!;

    }
}