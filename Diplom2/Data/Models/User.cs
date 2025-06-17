using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Diplom2.Data.Models
{
    public class User
    {
        [Key]
        public string UUID { get; set; }
        public List<History> History { get; set; } = new();
        public List<UserTag> UserTags { get; set; } = new();
        public List<Purchase> Cart { get; set; } = new();
        public User(string UUID) {
            this.UUID = UUID;
        }
        public bool Copy(User user) { 
            Cart = user.Cart;
            History = user.History;
            UserTags = user.UserTags;
            return true;
        }
    }
}
