using Humanizer.Localisation;
using ITI_Project.Data;
using ITI_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace ITI_Project.Repository
{
    public class HomeRepository:IHomeRepository
    {
        private readonly ApplicationDbContext context;
        public HomeRepository(ApplicationDbContext context)
        {
            this.context = context;
        }
        
    }
}
