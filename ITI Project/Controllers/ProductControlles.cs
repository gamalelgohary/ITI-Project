using System.Threading.Tasks;
using ITI_Project.Constants;
using ITI_Project.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.DotNet.Scaffolding.Shared;
using ITI_Project.Repository;
using ITI_Project.DTO;
using ITI_Project.Models;

namespace Product_mvc.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class ProductController : Controller
    {
        private readonly IProductRepository ProductRepository;
        private readonly ICategoryRepository categoryRepository;
        private readonly IFileServices fileServices;

        public ProductController(IProductRepository ProductRepository
            , ICategoryRepository categoryRepository
            , IFileServices fileServices)
        {
            this.ProductRepository = ProductRepository;
            this.categoryRepository = categoryRepository;
            this.fileServices = fileServices;
        }

        public async Task<IActionResult> Index()
        {
            var product = await ProductRepository.GetProducts();
            return View(product);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ShowProductsCards(string sTerm = "", int CategoryId = 0)
        {
            var products = await ProductRepository.DisplayProducts(sTerm, CategoryId);
            var categories = await ProductRepository.GetCategory();
            var productModel = new ProductDisplayModel()
            {
                Products = products,
                Categories = categories,
                sTerm = sTerm,
                CategoryId = CategoryId
            };

            return View(productModel);
        }
        public async Task<IActionResult> AddProduct()
        {
            var CategorySelectedList = (await categoryRepository.GetCategory()).Select(category => new SelectListItem
            {
                Text = category.CategoryName,
                Value = category.Id.ToString()
            });
            var ProductDTO = new ProductDTO
            {
                CategoryList = CategorySelectedList
            };
            return View(ProductDTO);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductDTO ProductDTO)
        {
            if (!ModelState.IsValid)
                return View(ProductDTO);

            try
            {
                if (ProductDTO.ImageFile != null)
                {
                    if (ProductDTO.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not exceed 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await fileServices.SaveFile(ProductDTO.ImageFile, allowedExtensions);
                    ProductDTO.Image = imageName;
                }
                var Product = new Product
                {
                    ProductName = ProductDTO.ProductName,
                    Description = ProductDTO.Description,
                    Price = ProductDTO.Price,
                    Image = ProductDTO.Image,
                    CategoryId = ProductDTO.CategoryId
                };
                await ProductRepository.AddProduct(Product);
                TempData["successMessage"] = "Product is added successfully";
                return RedirectToAction(nameof(AddProduct));
            }

            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(ProductDTO);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(ProductDTO);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data";
                return View(ProductDTO);
            }

        }

        public async Task<IActionResult> UpdateProduct(int id)
        {
            var Product = await ProductRepository.GetProductById(id);
            if (Product == null)
            {
                TempData["errorMessage"] = $"Product with the id: {id} does not found";
                return RedirectToAction(nameof(Index));
            }

            var CategorySelectedList = (await categoryRepository.GetCategory()).Select(Category => new SelectListItem
            {
                Text = Category.CategoryName,
                Value = Category.Id.ToString(),
                Selected = Category.Id == Product.CategoryId
            });

            var ProductDTO = new ProductDTO
            {
                ProductName = Product.ProductName,
                CategoryList = CategorySelectedList,
                Description = Product.Description,
                CategoryId = Product.CategoryId,
                Price = Product.Price,
                Image = Product.Image
            };
            return View(ProductDTO);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProduct(ProductDTO ProductDTO)
        {
            var CategorySelectList = (await categoryRepository.GetCategory()).Select(Category => new SelectListItem
            {
                Text = Category.CategoryName,
                Value = Category.Id.ToString(),
                Selected = Category.Id == ProductDTO.CategoryId
            });
            ProductDTO.CategoryList = CategorySelectList;


            if (!ModelState.IsValid)
            {
                return View(ProductDTO);
            }

            try
            {
                string oldImage = "";
                if (ProductDTO.ImageFile != null)
                {
                    if (ProductDTO.ImageFile.Length > 1 * 1024 * 1024)
                    {
                        throw new InvalidOperationException("Image file can not exceed 1 MB");
                    }
                    string[] allowedExtensions = [".jpeg", ".jpg", ".png"];
                    string imageName = await fileServices.SaveFile(ProductDTO.ImageFile, allowedExtensions);
                    // hold the old image name. Because we will delete this image after updating the new
                    oldImage = ProductDTO.Image;
                    ProductDTO.Image = imageName;
                }

                var Product = new Product
                {
                    Id = ProductDTO.Id,
                    ProductName = ProductDTO.ProductName,
                    Description = ProductDTO.Description,
                    Price = ProductDTO.Price,
                    CategoryId = ProductDTO.CategoryId,
                    Image = ProductDTO.Image
                };
                await ProductRepository.UpdateProduct(Product);
                // if image is updated, then delete it from the folder too
                if (!string.IsNullOrWhiteSpace(oldImage))
                {
                    fileServices.DeleteFile(oldImage);
                }
                TempData["successMessage"] = "Product is updated successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(ProductDTO);
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
                return View(ProductDTO);
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on saving data";
                return View(ProductDTO);
            }

        }


        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var Product = await ProductRepository.GetProductById(id);
                if (Product == null)
                {
                    TempData["errorMessage"] = $"Book with the id: {id} does not found";
                }
                else
                {
                    await ProductRepository.DeleteProduct(Product);
                    if (!string.IsNullOrWhiteSpace(Product.Image))
                    {
                        fileServices.DeleteFile(Product.Image);
                    }

                }
            }

            catch (InvalidOperationException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (FileNotFoundException ex)
            {
                TempData["errorMessage"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["errorMessage"] = "Error on deleting the data";
            }
            return RedirectToAction(nameof(Index));

        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> FilterProducts(string sTerm = "", int CategoryId = 0)
        {
            var products = await ProductRepository.DisplayProducts(sTerm, CategoryId);

            var model = new ProductDisplayModel
            {
                Products = products.ToList(),
                Categories = (await ProductRepository.GetCategory()).ToList(),
                CategoryId = CategoryId,
                sTerm = sTerm
            };

            return PartialView("_ProductsCardsPartial", model);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ProductDetails(int id)
        {
            var product = await ProductRepository.GetProductById(id);
            if (product == null) return NotFound();
            return View("ProductDetails", product);
        }

    }
}
