using Diplom2.Data.Models;

namespace Diplom2.Data.Interfaces
{
    public interface INiceness
    {
        public Task<bool> Increase(User user, Tag tag, int increment);
        public bool Set(User user, Tag tag, int value);
        public UserTag GetByTag(User user, Tag tag);
        public bool Multiply(User user, Tag tag, float multiplier);
    }
}
