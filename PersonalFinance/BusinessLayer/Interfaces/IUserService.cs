using BusinessLayer.Models;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Interfaces
{
    public interface IUserService
    {
        Task<UserDTO> GetUser(int userId);
        Task<UserDTO> GetUser(string login, string password);
        Task<UserDTO> GetUser(string login);
        Task<IEnumerable<UserDTO>> GetUserDTOList();
        Task<ServiceResult<UserDTO>> CreateUser(UserDTO userDTO);
        Task<ServiceResult> UpdateUserProfile(UserDTO userDTO);
        Task<ServiceResult> UpdateUserRole(UserDTO userDTO);
        Task DeleteUser(int userId);
        Task<IEnumerable<Role>> GetRoles();
        Task<ServiceResult> SendRestorePasswordEmail(string userEmail);
    }
}
