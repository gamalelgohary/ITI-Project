using Humanizer.Localisation;
using ITI_Project.Models;

namespace ITI_Project.DTO
{
    public class ProductDisplayModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string sTerm { get; set; } = "";
        public int CategoryId { get; set; } = 0;
    }
}
