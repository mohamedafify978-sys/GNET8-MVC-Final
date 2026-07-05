using GYMsystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.interfaces
{
    public interface IUnitOfWork
    {
        
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new();

        Task<int> SaveChangesAsync(CancellationToken ct);
        public ISessionRepository SessionRepository { get; }
        public IMembershipRepository MembershipRepository { get; }
      
        public IBookingRepository BookingRepository { get; }
    }
}
