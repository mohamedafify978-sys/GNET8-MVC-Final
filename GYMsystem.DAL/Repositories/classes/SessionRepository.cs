using GYMsystem.DAL.Context;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GYMDBContext dbcontext;

        public SessionRepository(GYMDBContext dbcontext) : base(dbcontext)
        {
            this.dbcontext = dbcontext;
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(CancellationToken ct)
        {
           
            var seasons = dbcontext.Sessions.AsNoTracking().Include(s=>s.Trainer).Include(s=>s.Category);
            return await seasons.ToListAsync();
        }

        public async Task<int> GetCountOfBookedSoltsAsync(int SessionId, CancellationToken ct)
        {
            return await dbcontext.Bookings.AsNoTracking().CountAsync(s => s.Id == SessionId);
        }
    }
}
