using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Models { 

    public class Category :BaseEntity
    {
        public string CategoryName { get; set; } = null!;

        public ICollection<Session> Sessions { get; set; } = new List<Session>();

    }
}
