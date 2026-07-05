using GYMsystem.DAL.Context;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GYMDBContext _dbContext;

        public BookingRepository(GYMDBContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking().Include(b => b.Member).Where(b => b.SessionId == sessionId).ToListAsync(ct);


    }
}
