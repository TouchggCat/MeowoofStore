using MeowoofStore.Data;
using MeowoofStore.Models;
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

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetProducts()
        {
            var products = _context.Product.ToList().Select(n=>n);

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
        public ActionResult DeleteProductsById(int id)
        {
            var products = _context.Product.SingleOrDefault(m => m.Id == id);

            if (products == null)
                return NotFound();

            _context.Product.Remove(products);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
