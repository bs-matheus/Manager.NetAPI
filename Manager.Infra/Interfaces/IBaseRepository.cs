using Manager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Infra.Interfaces
{
    public interface IBaseRepository<T> where T : Base
    {
        Task<T> CreateAsync(T obj);
        
        Task<T> UpdateAsync(T obj);

        Task<bool> RemoveAsync(ulong id);

        Task<T> GetByIdAsync(ulong id);
        
        Task<List<T>> GetAllAsync();
    }
}
