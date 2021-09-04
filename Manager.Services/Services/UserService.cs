using AutoMapper;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Infra.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using EscNet.Cryptography.Interfaces;

namespace Manager.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IMapper _mapper;

        private readonly IUserRepository _userRepository;

        private readonly IRijndaelCryptography _rijndaelCryptography;

        public UserService(IMapper mapper, IUserRepository userRepository, IRijndaelCryptography rijndaelCryptography)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _rijndaelCryptography = rijndaelCryptography;
        }

        public async Task<UserDTO> CreateAsync(UserDTO userDTO)
        {
            var emailExists = await _userRepository.GetByEmailAsync(userDTO.Email);

            if (emailExists != null)
                throw new DomainException("Email informado já cadastrado!");

            var user = _mapper.Map<User>(userDTO);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));

            var userCreated = await _userRepository.CreateAsync(user);

            return _mapper.Map<UserDTO>(userCreated);
        }

        public async Task<UserDTO> UpdateAsync(UserDTO userDTO)
        {
            var userExists = await _userRepository.GetByIdAsync(userDTO.Id);

            if (userExists == null)
                throw new DomainException("Nenhum usuário encontrado com o ID informado!");

            var emailExists = await _userRepository.GetByEmailAsync(userDTO.Email);

            if (emailExists != null && emailExists.Id != userDTO.Id)
                throw new DomainException("Email informado já cadastrado!");

            var user = _mapper.Map<User>(userDTO);
            user.Validate();
            user.ChangePassword(_rijndaelCryptography.Encrypt(user.Password));

            var userUpdated = await _userRepository.UpdateAsync(user);

            return _mapper.Map<UserDTO>(userUpdated);
        }

        public async Task<bool> RemoveAsync(ulong id)
        {
            return await _userRepository.RemoveAsync(id);
        }

        public async Task<UserDTO> GetByIdAsync(ulong id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> GetAllAsync()
        {
            var allUsers = await _userRepository.GetAllAsync();

            return _mapper.Map<List<UserDTO>>(allUsers);
        }

        public async Task<UserDTO> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);

            return _mapper.Map<UserDTO>(user);
        }

        public async Task<List<UserDTO>> SearchByNameAsync(string name)
        {
            var users = await _userRepository.SearchByNameAsync(name);

            return _mapper.Map<List<UserDTO>>(users);
        }

        public async Task<List<UserDTO>> SearchByEmailAsync(string email)
        {
            var users = await _userRepository.SearchByEmailAsync(email);

            return _mapper.Map<List<UserDTO>>(users);
        }
    }
}
