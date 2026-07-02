using AutoMapper;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Common;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.PlanViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class PlanServices : IPlanServices
    {

         private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        

        public PlanServices(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }


        public async Task<IEnumerable<PlanViewModel>> GetAllPlanAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAllAsync( ct:ct);
          return mapper.Map<IEnumerable< PlanViewModel>>(plans);
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
           var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return null;
            else
               return mapper.Map<PlanViewModel>(plan);
            
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {

            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return null;
            if( await HasActivaMemberShipsAsync(id, ct))
                return null;
            else return mapper.Map<UpdatePlanViewModel>(plan);

        }

        public async Task<Result> ToggleActivatioAsync(int id, CancellationToken ct = default)
        {
          
            var plan =await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return Result.NotFound("Plan not found");
            if (plan.IsActive && await HasActivaMemberShipsAsync(id, ct)) return Result.Fail("Cannot deactivate plan with active memberships", ResultKind.Conflict);


            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");





        }

        public async Task<Result> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
           var plan = await unitOfWork.GetRepository<Plan>() .GetByIdAsync(id, ct);
            if (plan is null) return Result.NotFound("Plan not found");
            if (await HasActivaMemberShipsAsync(id, ct)) return Result.Fail("Cannot update plan with active memberships", ResultKind.Conflict);


            mapper.Map(model, plan);

            plan.UpdatedAt = DateTime.Now;
           
            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.SaveChangesAsync(ct); 
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");
        }
        private async Task<bool> HasActivaMemberShipsAsync(int planId, CancellationToken ct = default)
        {

            return await unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
        }
    }
}
