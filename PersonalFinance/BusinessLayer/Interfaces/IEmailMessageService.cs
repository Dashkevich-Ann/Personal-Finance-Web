using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Interfaces
{
    public interface IEmailMessageService
    {
        Task SendTempPasswordEmail(UserDTO userDto, string password);
    }
}