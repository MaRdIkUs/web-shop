using System.ComponentModel.DataAnnotations;

namespace Diplom2.Data.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public override bool Equals(object? obj)
        {
            if (obj is Tag)
                return ((Tag)obj).Id == Id;
            return false;
        }
    }
}
