using GYMSystem.BLL.Common;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using GYMSystem.BLL.ViewModels.PlanViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.Interface
{
    public interface IPlanServices
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlanAsync(CancellationToken ct = default);
        Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct = default);
     
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default);
        Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default);
        Task<Result> ToggleActivatioAsync(int id, CancellationToken ct = default);

    }
}
