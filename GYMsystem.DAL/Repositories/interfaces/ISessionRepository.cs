using GYMsystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.interfaces
{
    public interface ISessionRepository:IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct);
        Task<int> GetCountOfBookedSoltsAsync(int SessionId, CancellationToken ct);
        Task<Session?> GetSessionWithTrainerAndCategoryByIdAsync(int id, CancellationToken ct);
    }
}
