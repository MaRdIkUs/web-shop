using Diplom2.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Diplom2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IUser _user;
        public ProfileController(IUser user) {
            _user = user;
        }

        [HttpGet(Name = "GetUser")]
        public async Task<object> GetAsync()
        {
            //
            if (User.FindFirst(ClaimTypes.NameIdentifier) == null)
                return null;
            var ids = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var usernames = User.FindFirst("preferred_username").Value;
            var roles = User.FindFirst("client_roles").Value;
            var emails = User.FindFirst(ClaimTypes.Email).Value;
            ClaimsIdentity identity = (ClaimsIdentity)User.Identity;
            identity.AddClaim(new(ClaimTypes.Role, roles));
            return new {
                id = ids,
                username = usernames,
                role = roles,
                email = emails
            };
        }
    }
}
