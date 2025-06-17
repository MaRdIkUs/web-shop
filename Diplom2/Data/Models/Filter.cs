using System.Text.Json.Serialization;

namespace Diplom2.Data.Models
{
    public class Filter
    {
        public int Id { get; set; }
        public FilterType Type { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string DefaultValue { get; set; }
        public string[] ArrayOfValues()
        {
            return Value.Split(',');
        }
    }
}
