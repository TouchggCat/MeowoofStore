using AutoMapper;
using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowoofStore.Controllers.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private IWebHostEnvironment _environment; //取路徑用
        private readonly ApplicationDbContext _context;
        private readonly IMapper _iMapper;

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment,IMapper mapper)
        {
            _context = context;
            _environment = environment;
            _iMapper = mapper;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _context.Product;

            if (products == null)
                return NotFound();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public ActionResult<Product> GetProductsById(int id)
        {
            var products = _context.Product.SingleOrDefault(m => m.Id == id);

            if (products == null)
                return NotFound();

            return Ok(products);
        }

        [HttpPost]
        public ActionResult<Product> CreateProduct(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (viewModel.Photo != null)
            {
                SaveProductPhoto(viewModel, FolderPath._Images_ProductImages);
            }
            Product product = new Product()
            {
                Name = viewModel.Name,
                Description = viewModel.Description,
                Price = viewModel.Price,
                Stock = viewModel.Stock,
                ImageString = viewModel.ImageString,
            };
            _context.Product.Add(product);
            _context.SaveChanges();
            viewModel.Id = product.Id;

            return CreatedAtAction(nameof(GetProductsById), new { id = product.Id }, viewModel);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteProductsById(int id ,string imageString)
        {
            var products = _context.Product.SingleOrDefault(m => m.Id == id);

            if (products == null)
                return NotFound();
            if (!String.IsNullOrEmpty(imageString))
                DeleteProductPhoto(FolderPath._Images_ProductImages, imageString);
            _context.Product.Remove(products);
            _context.SaveChanges();
            return NoContent();
        }

        private void SaveProductPhoto(ProductViewModel viewModel, string folderPath)
        {
            string photoName = Guid.NewGuid().ToString() + ".jpg";
            viewModel.ImageString = photoName;
            //抓路徑 IWebHostEnvironment(見下方)
            using(var stream = new FileStream(_environment.WebRootPath + folderPath + photoName, FileMode.Create))
            {
                viewModel.Photo.CopyTo(stream);
            }
        }

        private void DeleteProductPhoto(string folderPath, string photoName)
        {
            string filePath = Path.Combine(_environment.WebRootPath + folderPath, photoName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
