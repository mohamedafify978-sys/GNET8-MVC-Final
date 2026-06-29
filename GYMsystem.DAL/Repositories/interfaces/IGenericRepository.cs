
using GYMsystem.DAL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity , new()
    {
        Task<IEnumerable<TEntity>> GetAllAsync(bool istracked = false, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);
        void Add(TEntity item);
        void  Update(TEntity item);
        void Delete(TEntity item);
        //Task<int> completeAsync();

        Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate,CancellationToken ct = default);
        Task<TEntity> firstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool istracked = false, CancellationToken ct = default);

    }
}
