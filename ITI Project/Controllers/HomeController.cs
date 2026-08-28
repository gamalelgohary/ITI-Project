using System.Diagnostics;
using ITI_Project.DTO;
using ITI_Project.Models;
using ITI_Project.Repository;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeRepository homeRepository;

        public HomeController(ILogger<HomeController> logger, IHomeRepository homeRepository)
        {
            _logger = logger;
            this.homeRepository = homeRepository;
        }

        public  IActionResult Index(string sTerm = "", int CategoryId = 0)
        {
            //var products = await homeRepository.DisplayProducts(sTerm, CategoryId);
            //var categories = await homeRepository.GetCategory();
            //var productModel = new ProductDisplayModel()
            //{
            //    Products = products,
            //    Categories = categories,
            //    sTerm = sTerm,
            //    CategoryId = CategoryId
            //};

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


       

        
        public IActionResult BMIPlan()
        {
            return View();
        }

      
    }
}
