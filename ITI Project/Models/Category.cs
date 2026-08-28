using System.ComponentModel.DataAnnotations;

namespace ITI_Project.Models
{
	public class Category
	{
		
			public int Id { get; set; }
			[Required]
			[MaxLength(40)]
			public string CategoryName { get; set; }
			public List<Product> Products { get; set; }//one category has many products
		
	}
}

