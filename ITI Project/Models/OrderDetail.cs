using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ITI_Project.Models
{
	public class OrderDetail
	{
        public int Id { get; set; }
        [Required]
        public double UnitPrice { get; set; }
        [Required]
        public int Quantity { get; set; }

        [Required]
        public int OrderId { get; set; }//FK
        public Order Order { get; set; } //Nav Property  

        public int ProductId { get; set; }//FK
        public Product Product { get; set; } //Nav Property  
    }
}

