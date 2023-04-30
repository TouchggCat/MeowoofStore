using AutoMapper;
using Azure.Core;
using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.Models.Utilities;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
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
        private readonly IMapper _lMapper;
        public ShoppingController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _lMapper = mapper;
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

        [HttpGet("/api/Shopping/LoadCartItem/{id}")]
        public ActionResult LoadCartItem(int id)
        {
            int defaultQuantity = 1;
            var product = _context.Product?.SingleOrDefault(n => n.Id == id);
            if (product != null)
            {
                var viewModel = _lMapper.Map<AddToCartViewModel>(product);
                viewModel.Quantity = defaultQuantity;

                return Ok(viewModel);
            }

            return NotFound("沒有此產品");
        }

        [HttpPost("/api/Shopping/AddToCart/")]
        public ActionResult AddToCart(AddToCartViewModel viewModel)
        {
            var product = _context.Product?.SingleOrDefault(n => n.Id == viewModel.id);
            if (product == null)
                return NotFound();


            List<ShoppingCartViewModel>? shoppingCartViewModelList = GetShoppingCartListFromSessionOrNewCart();

            var shoppingCartViewModel = shoppingCartViewModelList.FirstOrDefault(n => n.Id == viewModel.id);

            if (shoppingCartViewModel == null)
            {
                shoppingCartViewModel = MappingToShoppingCartViewModel(product, viewModel);

                shoppingCartViewModelList.Add(shoppingCartViewModel);
            }
            else
            {
                shoppingCartViewModel.Quantity += viewModel.Quantity;
            }
            // 檢查是否已經包含相同的商品，如果是，增加數量並退出方法
            //if (IsSameItemAlreadyExistInCart(shoppingCartViewModelList, viewModel.id,viewModel.Quantity))
            //    return Ok("AddedSameItem");

            SaveShoppingCartListToSession(shoppingCartViewModelList);

            return Ok("SuccessAddToCart");
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

            if (shoppingCartViewModels.Count == 0)
                RemoveSession(ShoppingCartSessionKey.ShoppingCartListKey);
            else
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

        private List<ShoppingCartViewModel> GetShoppingCartListFromSessionOrNewCart()
        {
            if (!HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
                return new List<ShoppingCartViewModel>();

            var jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
            return JsonSerializer.Deserialize<List<ShoppingCartViewModel>>(jsonString);
        }

        private bool IsSameItemAlreadyExistInCart(List<ShoppingCartViewModel> shoppingCartViewModelList, int productId, int quantity)
        {
            var existingItem = shoppingCartViewModelList.SingleOrDefault(x => x.Product.Id == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                SaveShoppingCartListToSession(shoppingCartViewModelList);
                return true;
            }
            return false;
        }

        private ShoppingCartViewModel MappingToShoppingCartViewModel(Product product, AddToCartViewModel viewModel)
        {
            var shoppingCartViewModel = _lMapper.Map<ShoppingCartViewModel>(viewModel);
            shoppingCartViewModel.Product = product;
            return shoppingCartViewModel;
        }


        private void RemoveSession(string SessionKey)
        {
            if (HttpContext.Session.Keys.Contains(SessionKey))
                HttpContext.Session.Remove(SessionKey);
        }
    }
}
