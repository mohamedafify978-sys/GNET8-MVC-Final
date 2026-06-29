using GYMsystem.DAL.Context;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.Repositories.classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        public readonly GYMDBContext dbcontext;
        private readonly DbSet<TEntity> Set;
        public GenericRepository(GYMDBContext _dbcontext) {
         dbcontext = _dbcontext;
            Set = dbcontext.Set<TEntity>();
        }
        public void Add(TEntity item)
        {
            Set.Add(item);
            //return await dbcontext.SaveChangesAsync();
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
          return dbcontext.Set<TEntity>().AsNoTracking().AnyAsync(predicate, ct);
        }

        //public async Task<int> completeAsync()
        //{
        //    return await dbcontext.SaveChangesAsync();
        //}

        public void Delete(TEntity item)
        {
            //var item = dbcontext.Set<TEntity>().FirstOrDefault(x => x.Id == id);
            //    if (item is not null) {


                Set.Remove(item);
            //return await dbcontext.SaveChangesAsync();
        }

       

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool istracked, CancellationToken ct = default)
        {
            IQueryable<TEntity> item = istracked ? Set : Set.AsNoTracking();
            return await item.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await Set.FirstOrDefaultAsync(x=>x.Id == id,ct);
        }

        public void  Update (TEntity item)
        {
            Set.Update(item);
            //return await dbcontext.SaveChangesAsync();
        }

        public async Task<TEntity?> firstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool istracked = false, CancellationToken ct = default)
        {
            IQueryable<TEntity> query =  istracked ? Set : Set.AsNoTracking();
            return await query.FirstOrDefaultAsync(predicate);
        }
    }
}
