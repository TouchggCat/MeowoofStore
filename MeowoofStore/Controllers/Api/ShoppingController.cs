using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MeowoofStore.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShoppingController : ControllerBase
    {
        private ApplicationDbContext _context { get; set; }
        public ShoppingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Product>> GetShoppingList()
        {
            var products = _context.Product;

            if (products == null)
                return NotFound("No product !");

            return products;
        }
    }
}
