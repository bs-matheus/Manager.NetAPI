using Manager.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Manager.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> CreateAsync(UserDTO userDTO);

        Task<UserDTO> UpdateAsync(UserDTO userDTO);

        Task<bool> RemoveAsync(ulong id);

        Task<UserDTO> GetByIdAsync(ulong id);

        Task<List<UserDTO>> GetAllAsync();

        Task<UserDTO> GetByEmailAsync(string email);

        Task<List<UserDTO>> SearchByNameAsync(string name);

        Task<List<UserDTO>> SearchByEmailAsync(string email);
    }
}
