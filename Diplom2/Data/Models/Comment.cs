namespace Diplom2.Data.Models
{
    public class Comment
    {
        public int Id { get; set; } 
        public Product Product { get; set; }
        public string UserName { get; set; }
        public string UUID { get; set; }
        public string Content {  get; set; }
        public int Rate { get; set; }
    }
}
