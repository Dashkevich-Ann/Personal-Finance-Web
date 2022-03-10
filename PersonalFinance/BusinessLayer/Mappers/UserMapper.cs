using BusinessLayer.Models;
using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Mappers
{
    public static class UserMapper
    {
        public static UserDTO MapToDTO(this User user)
        {
            var userDTO = new UserDTO
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Login = user.Login,
                Password = user.Password,
                DateOfBirth = user.DateOfBirth,
                UserRoles = user.UserRoles.Select(x => x.Role)   
            };
            return userDTO;
        }

        public static User MapToUser(this UserDTO dto)
        {
            var user = new User
            {
                UserId = dto.UserId,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Login = dto.Login,
                Password = dto.Password,
                DateOfBirth = dto.DateOfBirth,
                UserRoles = dto.UserRoles.Select(x => new UserRole { RoleId = x.RoleId }).ToList()
            };
            return user;
        }
    }
}
