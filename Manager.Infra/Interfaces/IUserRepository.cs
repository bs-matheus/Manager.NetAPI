using Manager.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Infra.Interfaces
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User> GetByEmailAsync(string email);

        Task<List<User>> SearchByNameAsync(string name);

        Task<List<User>> SearchByEmailAsync(string email);
    }
}
