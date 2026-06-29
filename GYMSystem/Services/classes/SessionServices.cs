using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.SessionViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;

        public SessionServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (sessions == null || !sessions.Any()) return null;

            var mapped = sessions.Select(t => new SessionViewModel()
            {
            
            
                Id = t.Id,  
                Capacity = t.Capacity,
                CategoryName=t.Category.CategoryName,
                TrainerName=t.Trainer.Name,
                Description=t.Description,
                EndDate=t.EndDate,
                StartDate=t.StartDate,
                

            
            
            
            });

            foreach (var session in mapped) {
            
              session.AvailableSlots =  session.Capacity -await  unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(session.Id,ct);
            
            }
            return mapped;


        }


    }
}
