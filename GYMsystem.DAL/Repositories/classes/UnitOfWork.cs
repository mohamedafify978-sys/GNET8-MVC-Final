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
    public class UnitOfWork : IUnitOfWork

    {
        private readonly GYMDBContext dbContext;
        private readonly Dictionary<string, object> _repositories = [];

        public UnitOfWork(GYMDBContext dBContext, ISessionRepository sessionRepository)
        {
            dbContext = dBContext;
            this.SessionRepository = sessionRepository;
        }
        

        public ISessionRepository SessionRepository { get; }

        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            var TyoeName = typeof(TEntity).Name;
            if(_repositories.TryGetValue(TyoeName,out object? value))
                return (IGenericRepository<TEntity>)value;
            else
            {
                var repo = new GenericRepository<TEntity>(dbContext);
                _repositories[TyoeName] = repo; 
                return repo;

            }
           
        }

        public async Task<int> SaveChangesAsync(CancellationToken ct) => await dbContext.SaveChangesAsync(ct);
    }
}
