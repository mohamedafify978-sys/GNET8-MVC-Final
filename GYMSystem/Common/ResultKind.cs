using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Common
{
    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,          
        ValidationFailed,   
        Forbidden,
    }
}
