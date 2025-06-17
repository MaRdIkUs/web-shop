using System.ComponentModel.DataAnnotations.Schema;

namespace Diplom2.Data.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
