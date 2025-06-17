using Diplom2.Data.Interfaces;
using Diplom2.Data.Models;
using Org.BouncyCastle.Math.EC.Multiplier;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Diplom2.Data.DBO
{
    public class NicenessRepository : INiceness
    {
        private readonly ApplicationDbContext _context;
        private readonly IUser _user;
        public NicenessRepository(ApplicationDbContext context, IUser user)
        {
            _context = context;
            _user = user;
        }

        public UserTag? GetByTag(User user, Tag tag)
        {
            var usertag = user.UserTags.Where(t => t.Tag.Id == tag.Id);
            return usertag.Count() != 0 ? usertag.First() : null;
        }

        public async Task<bool> Increase(User user, Tag tag, int increment)
        {
            var userTag = GetByTag(user, tag);
            if (userTag == null)
            {
                Debug.WriteLine(await _user.CreateUserTag(new UserTag() { Count = 0 , User = user, Tag = tag }));
                userTag = GetByTag(user, tag);
            }
            userTag.Count = (uint)(userTag.Count + increment);
            _context.SaveChanges();
            return true;
        }

        public bool Multiply(User user, Tag tag, float multiplier)
        {
            var userTag = GetByTag(user, tag);
            userTag.Count = (uint)(userTag.Count * multiplier);
            _context.SaveChanges();
            return true;
        }

        public bool Set(User user, Tag tag, int value)
        {
            var userTag = GetByTag(user, tag);
            userTag.Count = (uint)value;
            _context.SaveChanges();
            return true;
        }
    }
}
