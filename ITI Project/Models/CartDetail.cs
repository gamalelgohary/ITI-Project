using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ITI_Project.Models
{
	public class CartDetail
	{
		public int Id { get; set; }

		[Required, ForeignKey("ShoppingCart")]
		public int ShoppingCart_Id { get; set; }    
		public ShoppingCart ShoppingCart { get; set; }   

		[Required]
		public int ProductId { get; set; }    
		public Product Product { get; set; }   

		[Required]
		public int Quantity { get; set; }

		[Required]
		public double UnitPrice { get; set; }
	}
}
