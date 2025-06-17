using Diplom2.Data.Analyzer;
using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

namespace Diplom2.Data.DBO
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _context;
        private readonly SHA256 _hashf = SHA256.Create();
        private readonly ILocalGeoService _localGeoService;

        public UserRepository(ApplicationDbContext context, ILocalGeoService localGeoService)
        {
            _context = context;
            _localGeoService = localGeoService;
        }

        public async Task<User?> Create(User userPreferences)
        {
            _context.Users.Add(userPreferences);
            await _context.SaveChangesAsync();
            return await Get(userPreferences.UUID);
        }

        public async Task<int> Delete(string UUID)
        {
            var preferencesList = _context.Users.Where(p => p.UUID == UUID);
            if (preferencesList.Count() != 0)
            {
                _context.Users.Remove(preferencesList.First());
                return await _context.SaveChangesAsync();
            }
            return -1;
        }

        public async Task<List<User>> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> Get(string UUID)
        {
            var user = _context.Users.Include(u => u.UserTags).Where(p => p.UUID == UUID);
            User? result;
            if (user.Count() > 0) {
                result = user.First();
                result.UserTags.ForEach(ut =>
                {
                    ut.User = result;
                    ut.Tag = _context.Tags.Where(t => t.Id == ut.TagId).First();
                });
            }
            else
                result = null;
            return result;
        }

        public async Task<int> Update(string UUID, User user)
        {
            _context.Users.First(u => u.UUID == UUID).Copy(user);
            return await _context.SaveChangesAsync();
        }

        public async Task<User?> Get(ClaimsPrincipal remote_user, IPAddress address)
        {
            var claim = remote_user.FindFirst(ClaimTypes.NameIdentifier);
            string userId;
            if (claim == null)
            {
                var remoteIp = address;
                var location = _localGeoService.GetLocation(remoteIp);
                var countryHash = Convert.ToBase64String(_hashf.ComputeHash(Encoding.UTF8.GetBytes(location.City + location.Country)));
                userId = countryHash;
            }
            else
                userId = claim.Value;
            var user = await Get(userId);
            if (user == null)
                user = await Create(new Data.Models.User(userId));
            return user;
        }

        public async Task<int> CreateUserTag(UserTag usertTag)
        {
            _context.UserTags.Add(usertTag);
            return await _context.SaveChangesAsync();
        }
    }
}
