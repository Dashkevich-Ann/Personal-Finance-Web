using BusinessLayer.Interfaces;
using BusinessLayer.Mappers;
using BusinessLayer.Models;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly DbContext _dbContext;
        private readonly IEmailMessageService _emailMessageService;

        public UserService(IRepository<User> userRepository, IRepository<Role> roleRepository, DbContext dbContext,
            IEmailMessageService emailMessageService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _dbContext = dbContext;
            _emailMessageService = emailMessageService;
        }

        public async Task<IEnumerable<UserDTO>> GetUserDTOList()
        {
            return await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Select(x => UserMapper.MapToDTO(x)).ToListAsync();
        }

        public async Task<UserDTO> GetUser(int userId)
        {
            return await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.UserId == userId)
                .Select(x => UserMapper.MapToDTO(x)).FirstOrDefaultAsync();
        }

        public async Task<UserDTO> GetUser(string login, string password)
        {
            return await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.Login == login && x.Password == password)
                .Select(x => UserMapper.MapToDTO(x)).FirstOrDefaultAsync();
        }

        public async Task<UserDTO> GetUser(string login)
        {
            return await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.Login == login)
                .Select(x => UserMapper.MapToDTO(x)).FirstOrDefaultAsync();
        }

        public async Task<ServiceResult<UserDTO>> CreateUser(UserDTO userDTO)
        {
            var role = _roleRepository.Query().FirstOrDefault(r => r.Name == "user");
            userDTO.UserRoles = new List<Role> { role };

            var user = userDTO.MapToUser();
            _userRepository.Create(user);
            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success(user.MapToDTO());
        }

        public async Task<ServiceResult> UpdateUserProfile(UserDTO userDTO)
        {
            var user = await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.UserId == userDTO.UserId)
                .FirstOrDefaultAsync();

            if(user == null)
            {
                return ServiceResult.Error($"User with id {userDTO.UserId} is not found");
            }

            if(await _userRepository.Query().AnyAsync(x => x.UserId != userDTO.UserId && x.Email.Equals(userDTO.Email)))
            {
                return ServiceResult.Error("User with same email exists");
            }

            user.FirstName = userDTO.FirstName;
            user.LastName = userDTO.LastName;
            user.DateOfBirth = userDTO.DateOfBirth;
            user.Email = userDTO.Email;
            user.Login = userDTO.Login;
            user.Password = userDTO.Password;

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateUserRole(UserDTO userDTO)
        {
            var user = await _userRepository.Query()
                .Include(x => x.UserRoles)
                .ThenInclude(x => x.Role)
                .Where(x => x.UserId == userDTO.UserId)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return ServiceResult.Error($"User with id {userDTO.UserId} is not found");
            }

            user.UserRoles = null;
            user.UserRoles = userDTO.UserRoles.Select(x => new UserRole { RoleId = x.RoleId }).ToList();

            await _dbContext.SaveChangesAsync();

            return ServiceResult.Success();
        }

        public async Task DeleteUser(int userId)
        {
            var user = await _userRepository.Query()
                .Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();

            _userRepository.Delete(user);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Role>> GetRoles()
        {
            return await _roleRepository.Query().ToListAsync();
        }

        public async Task<ServiceResult> SendRestorePasswordEmail(string userEmail)
        {
            try
            {
                var user = await _userRepository.Query()
                    .Include(x => x.UserRoles)
                    .ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(x => x.Email == userEmail);

                if (user == null)
                    return ServiceResult.Error($"User with email {userEmail} was not found");

                var userDto = user.MapToDTO();
                var newPassword = NewPassword();

                await _emailMessageService.SendTempPasswordEmail(userDto, newPassword);

                user.Password = newPassword;
                await _dbContext.SaveChangesAsync();

                return ServiceResult.Success();
            }
            catch(Exception e)
            {
                return ServiceResult.Error("Unexpected error has occurred.\nPlease connect with administrator.");
            }

            string NewPassword()
            {
                var newPassword = Guid.NewGuid().ToString("N").ToLower()
                    .Replace("1", "").Replace("o", "").Replace("0", "")
                    .Substring(0, 10);
                return newPassword;
            }
        }
    }
}
