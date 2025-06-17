namespace Diplom2.Data.Models
{
    public class History
    {
        public int Id { get; set; }
        public Product Purchase { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
