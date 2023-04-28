using Azure.Core;
using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.Models.Utilities;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Text.Json;

namespace MeowoofStore.Controllers.Api
{
    [Authorize]
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
        public async Task<ActionResult<IEnumerable<Product>>> GetShoppingList()
        {
            var products = await _context.Product.ToListAsync();

            if (products == null)
                return NotFound("No product !");

            return products;
        }

        [HttpGet("/api/check-stock")]
        public ActionResult CheckStock(int itemId, int quantity)
        {
            var item = _context.Product.Find(itemId);
            if (item == null)
                return NotFound();

            if (item.Stock >= quantity)
                return Ok(new { IsAvailable = true });
 
            return Ok(new { IsAvailable = false });
        }

        [HttpGet("/api/Shopping/GetCartData")]
        public ActionResult<IEnumerable<ShoppingCartViewModel>> GetCartData()
        {
            if (HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
            {
                List<ShoppingCartViewModel>? shoppingCartItemList = GetShoppingCartItemsFromSession();
                return shoppingCartItemList;
            }

            //return View(ViewName.EmptyCart, StringModel.ShoppingCartIsEmpty);
            return BadRequest();
        }
        [HttpDelete("/api/Shopping/DeleteProductsById/{id}")]  //Copy來的
        public async Task<ActionResult> DeleteProductsById(int id)
        {
            var products = await _context.Product.SingleOrDefaultAsync(m => m.Id == id);

            if (products == null)
                return NotFound();  

            List<ShoppingCartViewModel>? shoppingCartViewModels = GetShoppingCartItemsFromSession();
            shoppingCartViewModels.RemoveAll(p => p.Product.Id == id);

            SaveShoppingCartListToSession(shoppingCartViewModels);

            return NoContent();
        }

        private void SaveShoppingCartListToSession(List<ShoppingCartViewModel> shoppingCartViewModelList)
        {
            var serializedJsonString = JsonSerializer.Serialize(shoppingCartViewModelList);
            HttpContext.Session.SetString(ShoppingCartSessionKey.ShoppingCartListKey, serializedJsonString);
        }
        private List<ShoppingCartViewModel>? GetShoppingCartItemsFromSession()
        {
            string? jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
            List<ShoppingCartViewModel>? shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartViewModel>>(jsonString);
            return shoppingCartItemList;
        }
    }
}
