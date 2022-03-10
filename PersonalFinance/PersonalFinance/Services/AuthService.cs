using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataLayer.Data;
using DataLayer.Entities;
using DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinance.Services
{
    public class AuthService : IAuthService
    {
        public readonly IUserService _userService;

        public AuthService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<UserDTO> GetLoggedInUser(ClaimsPrincipal principal)
        {
           if(principal.Identity.IsAuthenticated)
            {
                var login = principal.FindFirstValue(ClaimTypes.NameIdentifier);

                return await _userService.GetUser(login);
            }

            return null;
        }

        public async Task<UserDTO> ValidateUser(string login, string password)
        {
            return await _userService.GetUser(login, password);
        }
    }
}
