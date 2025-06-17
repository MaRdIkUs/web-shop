namespace Diplom2.Data.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Specs> Specs { get; set; }
        public double Price { get; set; }
        public Category Category { get; set; }
        public int ImagesCount { get; set; }
        public List<Tag> Tags { get; set; }
        public long Popularirty { get; set; }
        public uint Count { get; set; }
        public bool Copy(Product product)
        {
            Id = product.Id;
            Name = product.Name;
            Description = product.Description;
            Specs = product.Specs;
            Price = product.Price;
            Category = product.Category;
            ImagesCount = product.ImagesCount;
            Tags = product.Tags;
            Popularirty = product.Popularirty;
            Count = product.Count;
            return true;
        }
    }
    public static class EntityGenerator
    {
        public class Specs
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Tag
        {
            public string Name { get; set; }
        }
            static void Main1(string[] args)
            {
                // 2. Генерация 100 продуктов
                var rnd = new Random();
                var categories = Enum.GetValues(typeof(Category)).Cast<Category>().ToArray();
                var sampleSpecsKeys = new[] { "Weight", "Color", "Size", "Material", "Power" };
                var sampleTagNames = new[] { "New", "Sale", "Popular", "Limited", "Eco" };

                var products = Enumerable.Range(1, 100).Select(i => new Product
                {
                    Id = i,
                    Name = $"Product {i}",
                    Description = $"Description for product {i}",
                    Specs = sampleSpecsKeys
                        .OrderBy(_ => rnd.Next())
                        .Take(rnd.Next(1, 4))
                        .Select(k => new Data.Models.Specs { Name = k, Value = $"{rnd.Next(1, 100)}" })
                        .ToList(),
                    Price = Math.Round(rnd.NextDouble() * 1000, 2),
                    Category = categories[rnd.Next(categories.Length)],
                    ImagesCount = rnd.Next(1, 6),
                    /*Tags = sampleTagNames
                        .OrderBy(_ => rnd.Next())
                        .Take(rnd.Next(0, 3))
                        .Select(n => new Tag { Name = n })
                        .ToList(),
                    */
                    Popularirty = rnd.NextInt64(0, 10000),
                    Count = (uint)rnd.Next(0, 500)
                }).ToList();
                /*
                // 3. Сериализация в JSON
                string json = JsonConvert.SerializeObject(products, Formatting.Indented);
                Console.WriteLine("=== JSON DATA ===");
                Console.WriteLine(json);

                // 4. Десериализация обратно в объекты
                var deserialized = JsonConvert.DeserializeObject<List<Product>>(json);

                Console.WriteLine();
                Console.WriteLine($"Считано объектов: {deserialized.Count}");
                Console.WriteLine($"Первый продукт: Id={deserialized[0].Id}, Name={deserialized[0].Name}");
                */
            }
    }
}