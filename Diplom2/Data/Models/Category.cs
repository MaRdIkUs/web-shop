namespace Diplom2.Data.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Filter> Filters { get; set; }
        public List<Product> Products { get; set; }
        public bool Copy(Category category) { 
            Name = category.Name;
            Filters = category.Filters;
            Products = category.Products;
            return true;
        }
    }
}
