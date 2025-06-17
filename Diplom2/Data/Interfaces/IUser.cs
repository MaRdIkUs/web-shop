using Diplom2.Data.Models;
using System.Net;
using System.Security.Claims;

namespace Diplom2.Data.Interfaces
{
    public interface IUser
    {
        Task<List<User>> GetAll();
        Task<User?> Get(string UUID);
        Task<User?> Get(ClaimsPrincipal claim, IPAddress address);
        Task<User?> Create(User user);
        Task<int> Update(string UUID, User user);
        Task<int> Delete(string UUID);
        Task<int> CreateUserTag(UserTag usertTag);
    }
}
