using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.AnalytaicsViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        public async Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default)
        {
            var now = DateTime.Now;
            var upcomingSessions = await unitOfWork.GetRepository<Session>().countAsync(s => s.StartDate > now);
            var ongoingSessions = await unitOfWork.GetRepository<Session>().countAsync(s => s.StartDate <= now && s.EndDate >= now);
            var completedSessions = await unitOfWork.GetRepository<Session>().countAsync(s => s.EndDate < now);
             var totalMembers = await unitOfWork.GetRepository<Member>().countAsync(ct:ct);
            var activeMembers = await unitOfWork.GetRepository<Membership>().countAsync(m => m.EndDate > now , ct);
            var totalTrainers =await unitOfWork.GetRepository<Trainer>().countAsync(ct:ct);

            return new AnalyticsViewModel
            {
                TotalMembers = totalMembers,
                ActiveMembers = activeMembers,
                TotalTrainers = totalTrainers,
                UpcomingSessions = upcomingSessions,
                OngoingSessions = ongoingSessions,
                CompletedSessions = completedSessions
            };

        }
    }
}
