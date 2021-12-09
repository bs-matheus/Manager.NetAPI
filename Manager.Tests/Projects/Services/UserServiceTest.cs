using AutoMapper;
using Bogus;
using Bogus.DataSets;
using EscNet.Cryptography.Interfaces;
using FluentAssertions;
using Manager.Core.Exceptions;
using Manager.Domain.Entities;
using Manager.Infra.Interfaces;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using Manager.Tests.Configuration;
using Manager.Tests.Fixtures;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Manager.Tests.Projects.Services
{
    public class UserServiceTest
    {
        // Subject Under Test
        private readonly IUserService _sut;

        // Mocks
        private readonly IMapper _mapper;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRijndaelCryptography> _rijndaelCryptographyMock;

        public UserServiceTest()
        {
            _mapper = AutoMapperConfiguration.GetConfiguration();
            _userRepositoryMock = new Mock<IUserRepository>();
            _rijndaelCryptographyMock = new Mock<IRijndaelCryptography>();

            _sut = new UserService(_mapper, _userRepositoryMock.Object, _rijndaelCryptographyMock.Object);
        }

        #region Create

        [Fact(DisplayName = "Create Valid User")]
        [Trait("Category", "Services")]
        public async Task CreateAsync_WhenUserIsValid_ReturnsUserDTO()
        {
            // Arrange
            var userToCreate = UserFixture.CreateValidUserDTO();

            var encryptedPassword = new Randomizer().AlphaNumeric(30);
            var userCreated = _mapper.Map<User>(userToCreate);

            userCreated.ChangePassword(encryptedPassword);

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            _rijndaelCryptographyMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userCreated);

            // Act
            var result = await _sut.CreateAsync(userToCreate);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userCreated));
        }

        [Fact(DisplayName = "Create When User Exists")]
        [Trait("Category", "Services")]
        public void CreateAsync_WhenUserExists_ThrowsNewDomainException()
        {
            // Arrange
            var userToCreate = UserFixture.CreateValidUserDTO();
            var userExists = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => userExists);

            // Act
            Func<Task<UserDTO>> act = async () =>
            {
                return await _sut.CreateAsync(userToCreate);
            };

            // Assert
            act.Should().ThrowAsync<DomainException>()
                .WithMessage("Email informado já cadastrado!");
        }

        [Fact(DisplayName = "Create When User Is Invalid")]
        [Trait("Category", "Services")]
        public void CreateAsync_WhenUserIsInvalid_ThrowsNewDomainException()
        {
            // Arrange
            var userToCreate = UserFixture.CreateInvalidUserDTO();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            // Act
            Func<Task<UserDTO>> act = async () =>
            {
                return await _sut.CreateAsync(userToCreate);
            };

            // Assert
            act.Should().ThrowAsync<DomainException>();
        }

        #endregion

        #region Update

        [Fact(DisplayName = "Update Valid User")]
        [Trait("Category", "Services")]
        public async Task UpdateAsync_WhenUserIsValid_ReturnsUserDTO()
        {
            // Arrange
            var oldUser = UserFixture.CreateValidUser();
            var userToUpdate = UserFixture.CreateValidUserDTO();
            var userUpdated = _mapper.Map<User>(userToUpdate);

            var encryptedPassword = new Randomizer().AlphaNumeric(30);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<ulong>()))
                .ReturnsAsync(() => oldUser);

            _rijndaelCryptographyMock.Setup(x => x.Encrypt(It.IsAny<string>()))
                .Returns(encryptedPassword);

            _userRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(() => userUpdated);

            // Act
            var result = await _sut.UpdateAsync(userToUpdate);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userUpdated));
        }

        [Fact(DisplayName = "Update When User Does Not Exist")]
        [Trait("Category", "Services")]
        public void UpdateAsync_WhenUserDoesNotExist_ThrowsNewDomainException()
        {
            // Arrange
            var userToUpdate = UserFixture.CreateValidUserDTO();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<ulong>()))
                .ReturnsAsync(() => null);

            // Act
            Func<Task<UserDTO>> act = async () =>
            {
                return await _sut.UpdateAsync(userToUpdate);
            };

            // Assert
            act.Should().ThrowAsync<DomainException>()
                .WithMessage("Nenhum usuário encontrado com o ID informado!");
        }

        [Fact(DisplayName = "Update When User Is Invalid")]
        [Trait("Category", "Services")]
        public void UpdateAsync_WhenUserIsInvalid_ThrowsNewDomainException()
        {
            // Arrange
            var oldUser = UserFixture.CreateValidUser();
            var userToUpdate = UserFixture.CreateInvalidUserDTO();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<ulong>()))
                .ReturnsAsync(() => oldUser);

            // Act
            Func<Task<UserDTO>> act = async () =>
            {
                return await _sut.UpdateAsync(userToUpdate);
            };

            // Assert
            act.Should().ThrowAsync<DomainException>();
        }

        #endregion

        #region Remove

        [Fact(DisplayName = "Remove User")]
        [Trait("Category", "Services")]
        public async Task RemoveAsync_WhenUserExists_RemoveUser()
        {
            // Arrange
            var userId = new Randomizer().UInt(0, 1000);

            _userRepositoryMock.Setup(x => x.RemoveAsync(It.IsAny<uint>()))
                .Verifiable();

            // Act
            await _sut.RemoveAsync(userId);

            // Assert
            _userRepositoryMock.Verify(x => x.RemoveAsync(userId), Times.Once);
        }

        #endregion

        #region Get

        [Fact(DisplayName = "Get By Id")]
        [Trait("Category", "Services")]
        public async Task GetByIdAsync_WhenUserExists_ReturnsUserDTO()
        {
            // Arrange
            var userId = new Randomizer().UInt(0, 1000);
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(() => userFound);

            // Act
            var result = await _sut.GetByIdAsync(userId);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userFound));
        }

        [Fact(DisplayName = "Get By Id When User Not Exists")]
        [Trait("Category", "Services")]
        public async Task GetByIdAsync_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            var userId = new Randomizer().UInt(0, 1000);

            _userRepositoryMock.Setup(x => x.GetByIdAsync(userId))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetByIdAsync(userId);

            // Assert
            result.Should().Be(null);
        }

        [Fact(DisplayName = "Get By Email")]
        [Trait("Category", "Services")]
        public async Task GetByEmailAsync_WhenUserExists_ReturnsUserDTO()
        {
            // Arrange
            var userEmail = new Internet().Email();
            var userFound = UserFixture.CreateValidUser();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail))
                .ReturnsAsync(() => userFound);

            // Act
            var result = await _sut.GetByEmailAsync(userEmail);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<UserDTO>(userFound));
        }

        [Fact(DisplayName = "Get By Email When User Not Exists")]
        [Trait("Category", "Services")]
        public async Task GetByEmailAsync_WhenUserNotExists_ReturnsNull()
        {
            // Arrange
            var userEmail = new Internet().Email();

            _userRepositoryMock.Setup(x => x.GetByEmailAsync(userEmail))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetByEmailAsync(userEmail);

            // Assert
            result.Should().Be(null);
        }

        [Fact(DisplayName = "Get All Users")]
        [Trait("Category", "Services")]
        public async Task GetAllUsersAsync_WhenUsersExists_ReturnsAListOfUserDTO()
        {
            // Arrange
            var usersFound = UserFixture.CreateValidUserList();

            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
        }

        [Fact(DisplayName = "Get All Users When None User Found")]
        [Trait("Category", "Services")]
        public async Task GetAllUsersAsync_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            _userRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.GetAllAsync();

            // Assert
            result.Should().BeEmpty();
        }

        #endregion

        #region Search

        [Fact(DisplayName = "Search By Name")]
        [Trait("Category", "Services")]
        public async Task SearchByNameAsync_WhenAnyUserFound_ReturnsAListOfUserDTO()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();
            var usersFound = UserFixture.CreateValidUserList();

            _userRepositoryMock.Setup(x => x.SearchByNameAsync(nameToSearch))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
        }

        [Fact(DisplayName = "Search By Name When None User Found")]
        [Trait("Category", "Services")]
        public async Task SearchByNameAsync_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var nameToSearch = new Name().FirstName();

            _userRepositoryMock.Setup(x => x.SearchByNameAsync(nameToSearch))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.SearchByNameAsync(nameToSearch);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact(DisplayName = "Search By Email")]
        [Trait("Category", "Services")]
        public async Task SearchByEmailAsync_WhenAnyUserFound_ReturnsAListOfUserDTO()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();
            var usersFound = UserFixture.CreateValidUserList();

            _userRepositoryMock.Setup(x => x.SearchByEmailAsync(emailSoSearch))
                .ReturnsAsync(() => usersFound);

            // Act
            var result = await _sut.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should().BeEquivalentTo(_mapper.Map<List<UserDTO>>(usersFound));
        }

        [Fact(DisplayName = "Search By Email When None User Found")]
        [Trait("Category", "Services")]
        public async Task SearchByEmailAsync_WhenNoneUserFound_ReturnsEmptyList()
        {
            // Arrange
            var emailSoSearch = new Internet().Email();

            _userRepositoryMock.Setup(x => x.SearchByEmailAsync(emailSoSearch))
                .ReturnsAsync(() => null);

            // Act
            var result = await _sut.SearchByEmailAsync(emailSoSearch);

            // Assert
            result.Should().BeEmpty();
        }

        #endregion
    }
}
