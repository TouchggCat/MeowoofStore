using AutoMapper;
using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.Models.Utilities;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MeowoofStore.Controllers.Api
{
    [Authorize(Roles = "Administrator")]
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _iMapper;
        private readonly PhotoProcess _photoProcess;
        public ProductsController(ApplicationDbContext context, IMapper mapper, PhotoProcess photoProcess)
        {
            _context = context;
            _iMapper = mapper;
            _photoProcess = photoProcess;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _context.Product.ToListAsync();

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
        public async Task<ActionResult<Product>> CreateProduct(ProductViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (viewModel.Photo != null)
            {
               await _photoProcess.CreatePhoto<ProductViewModel>(viewModel, FolderPath._Images_ProductImages,
                nameof(viewModel.Photo), nameof(viewModel.ImageString));
            }
            var product = _iMapper.Map<Product>(viewModel);
            _context.Product.Add(product);
            _context.SaveChanges();
            viewModel.Id = product.Id;

            return CreatedAtAction(nameof(GetProductsById), new { id = product.Id }, viewModel);
        }

        [HttpDelete("{id}")]
        public  async Task<ActionResult> DeleteProductsById(int id ,string imageString)
        {
            var products = _context.Product.SingleOrDefault(m => m.Id == id);

            if (products == null)
                return NotFound();
            if (!String.IsNullOrEmpty(imageString))
               await  _photoProcess.DeletePhoto(FolderPath._Images_ProductImages, imageString);
            _context.Product.Remove(products);
            _context.SaveChanges();
            return NoContent();
        }

    }
}
