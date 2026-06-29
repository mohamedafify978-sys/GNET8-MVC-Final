using GYMsystem.DAL.Models.Enums;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models
{
    public class Trainer : GymUser
    {
        public Specialties Specialty { get; set; }
       
        public ICollection<Session> Sessions { get; set; } = new HashSet<Session>();


    }
}
