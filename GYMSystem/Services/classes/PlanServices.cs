using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
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

        public PlanServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlanAsync(CancellationToken ct = default)
        {
            var plans = await unitOfWork.GetRepository<Plan>().GetAllAsync( ct:ct);
          return plans.Select(p => new PlanViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                durationDays = p.DurationDays,
                price = p.Price,
                IsActive = p.IsActive
            });
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id, CancellationToken ct = default)
        {
           var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return null;
            else
               return new PlanViewModel
                {
                    Id = plan.Id,
                    Name = plan.Name,
                    Description = plan.Description,
                    durationDays = plan.DurationDays,
                    price = plan.Price,
                    IsActive = plan.IsActive
                };
            
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int id, CancellationToken ct = default)
        {

            var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null || !plan.IsActive) return null;
            if( await HasActivaMemberShipsAsync(id, ct))
                return null;
            else return new UpdatePlanViewModel
            {
                PlanName = plan.Name,
                Description = plan.Description,
                durationDays = plan.DurationDays,
                price = plan.Price
            };

        }

        public async Task<bool> ToggleActivatioAsync(int id, CancellationToken ct = default)
        {
          
            var plan =await unitOfWork.GetRepository<Plan>().GetByIdAsync(id, ct);
            if (plan is null) return false;
            if (plan.IsActive && await HasActivaMemberShipsAsync(id, ct)) return false;
            

            plan.IsActive = !plan.IsActive;
            plan.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0;





        }

        public async Task<bool> UpdatePlanAsync(int id, UpdatePlanViewModel model, CancellationToken ct = default)
        {
           var plan = await unitOfWork.GetRepository<Plan>() .GetByIdAsync(id, ct);
            if (plan is null) return false;
            if (await HasActivaMemberShipsAsync(id, ct)) return false;


            plan.UpdatedAt = DateTime.Now;
            plan.Description = model.Description;
            plan.DurationDays = model.durationDays;
            plan.Price = model.price;
            unitOfWork.GetRepository<Plan>().Update(plan);
            var result = await unitOfWork.SaveChangesAsync(ct); 
            return result > 0;
        }
        private async Task<bool> HasActivaMemberShipsAsync(int planId, CancellationToken ct = default)
        {

            return await unitOfWork.GetRepository<Membership>().AnyAsync(m => m.PlanId == planId && m.EndDate > DateTime.Now, ct);
        }
    }
}
