using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer.Localisation;

namespace ITI_Project.Models
{
	public class Product
	{
        public int Id { get; set; }
        [Required]
        [MaxLength(40)]
        public string? ProductName { get; set; }
        [Required]
        [MaxLength(500)]
        public string? Description { get; set; }

        public string? Image { get; set; }
        public double Price { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category Categories { get; set; } // one Product has one Category
        public List<OrderDetail> OrderDetail { get; set; }
        public List<CartDetail> CartDetail { get; set; }

        public Stock Stock { get; set; }

        [NotMapped]
        public string CategoryName { get; set; }
        [NotMapped]
        public int Quantity { get; set; }

    }
}
