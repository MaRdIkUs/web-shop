using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom2.Data.Models
{
    [PrimaryKey(nameof(TagId), nameof(UserId))]
    public class UserTag
    {
        public int TagId { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; }
        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        public uint Count { get; set; }
    }
}
