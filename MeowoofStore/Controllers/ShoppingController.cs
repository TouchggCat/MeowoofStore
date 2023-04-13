using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using MeowoofStore.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NuGet.Protocol.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MeowoofStore.Controllers
{
    public class ShoppingController : Controller
    {
        private ApplicationDbContext _context { get; set; }
        public ShoppingController(ApplicationDbContext context)
        {
            _context= context;
        }
        public IActionResult List()
        {
            var products = _context.Product.Select(n => n);
            if(products != null )
               return View(products);

            return View("NullView");
        }
        public IActionResult AddToCart(int id) 
        {
            int defaultCount = 1;
            AddToCartViewModel viewModel=new AddToCartViewModel();
            var product = _context.Product.SingleOrDefault(n => n.Id == id);
            if(product != null)
            {
                viewModel.id = product.Id;
                viewModel.price = product.Price;
                viewModel.Name = product.Name;
                viewModel.count = defaultCount;
                return View(viewModel);
            }

            return View("NullView");
        }

        [HttpPost]
        public IActionResult AddToCart(AddToCartViewModel viewModel)
        {
            var product = _context.Product.SingleOrDefault(n => n.Id == viewModel.id);
            if (product == null)
                return View("NullView");

            string? jsonString = "";
            List<ShoppingCartItem>? shoppingCartItemList = null;
            if (!HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
                shoppingCartItemList = new List<ShoppingCartItem>();
            else
            {
                jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
                shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartItem>>(jsonString);
            }
            ShoppingCartItem shoppingCartItem = new ShoppingCartItem()
            {
                Id = viewModel.id,
                Price = viewModel.price,
                ProductId = viewModel.id,
                Quantity = viewModel.count,
                Product = product
            };
            shoppingCartItemList.Add(shoppingCartItem);
            jsonString = JsonSerializer.Serialize(shoppingCartItemList);
            HttpContext.Session.SetString(ShoppingCartSessionKey.ShoppingCartListKey, jsonString);
            return RedirectToAction("List");
        }

        public IActionResult CartView()
        {
            if (HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
            {
                string? jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
                List<ShoppingCartItem>? shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartItem>>(jsonString);
                return View(shoppingCartItemList);
            }

            return View("EmptyCart");
        }
    }
}
