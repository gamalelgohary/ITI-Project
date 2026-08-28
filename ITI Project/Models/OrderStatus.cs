using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ITI_Project.Models
{
	public class OrderStatus
	{
        public int Id { get; set; }

        [Required]
        public int StatusId { get; set; }//Status Code

        [Required, MaxLength(20)]
        public string? StatusName { get; set; }
    }
}

