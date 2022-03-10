using BusinessLayer.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinance.Services
{
    public interface IAuthService
    {
        Task<UserDTO> ValidateUser(string login, string password);

        Task<UserDTO> GetLoggedInUser(ClaimsPrincipal principal);
    }
}
