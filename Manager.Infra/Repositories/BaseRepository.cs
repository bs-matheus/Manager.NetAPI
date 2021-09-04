using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Manager.Infra.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : Base
    {
        private readonly ManagerContext _context;

        public BaseRepository(ManagerContext context)
        {
            _context = context;
        }

        public virtual async Task<T> CreateAsync(T obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<T> UpdateAsync(T obj)
        {
            _context.Entry(obj).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return obj;
        }

        public virtual async Task<bool> RemoveAsync(ulong id)
        {
            var obj = await GetByIdAsync(id);

            if (obj != null)
            {
                _context.Remove(obj);
                await _context.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public virtual async Task<T> GetByIdAsync(ulong id)
        {
            var obj = await _context.Set<T>().AsNoTracking().Where(x => x.Id == id).ToListAsync();

            return obj.FirstOrDefault();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().AsNoTracking().ToListAsync();
        }
    }
}
