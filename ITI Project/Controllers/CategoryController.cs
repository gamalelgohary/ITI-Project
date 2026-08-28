using Humanizer.Localisation;
using ITI_Project.Constants;
using ITI_Project.DTO;
using ITI_Project.Models;
using ITI_Project.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITI_Project.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
   
    public class CategoryController:Controller
    {
        private readonly ICategoryRepository categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }
        public async Task<IActionResult> Index()
        {
            var catgories = await categoryRepository.GetCategory();
            return View(catgories);
        }
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                var categoryToAdd = new Category
                {
                    Id = categoryDTO.Id,
                    CategoryName = categoryDTO.CategoryName
                };
                await categoryRepository.AddCategory(categoryToAdd);
                TempData["successMessage"] = "category added successfully";
                return RedirectToAction(nameof(AddCategory));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Category could not added!";
                return View(categoryDTO);
            }
        }


        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new InvalidOperationException("category is not found");

            var CategoryToUpdate = new CategoryDTO
            {
                Id = id,
                CategoryName = category.CategoryName
            };

            return View(CategoryToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(CategoryDTO categoryDTO)
        {
            if (!ModelState.IsValid)
                return View(categoryDTO);

            try
            {
                var category = new Category
                {
                    Id = categoryDTO.Id,
                    CategoryName = categoryDTO.CategoryName
                };
                await categoryRepository.UpdateCategory(category);
                TempData["successMessage"] = "category is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "category could not updated!";
                return View(categoryDTO);
            }
        }


        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await categoryRepository.GetCategoryById(id);
            if (category == null)
                throw new InvalidOperationException("category is not found");

            await categoryRepository.DeleteCategory(category);
            return RedirectToAction(nameof(Index));
        }
    }
}
