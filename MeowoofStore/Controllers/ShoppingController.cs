using MeowoofStore.Data;
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
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Policy;
using System.Security.Claims;
using MeowoofStore.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Execution;

namespace MeowoofStore.Controllers
{
    [Authorize]
    public class ShoppingController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _lMapper;
        public ShoppingController(ApplicationDbContext context,IMapper mapper)
        {
            _context= context;
            _lMapper= mapper;
        }
        public IActionResult List()
        {
            var products = _context.Product.Select(n => n);
            if(products != null )
               return View(products);

            return View(ViewName.NullView);
        }
        public IActionResult AddToCart(int id) 
        {
            int defaultQuantity = 1;
            //AddToCartViewModel viewModel=new AddToCartViewModel();
            var product = _context.Product.SingleOrDefault(n => n.Id == id);
            if(product != null)
            {
                var viewModel = _lMapper.Map<AddToCartViewModel>(product);
                viewModel.Quantity = defaultQuantity;
                return View(viewModel);
            }

            return View(ViewName.NullView);
        }

        [HttpPost]
        public IActionResult AddToCart(AddToCartViewModel viewModel)
        {
            var product = _context.Product.SingleOrDefault(n => n.Id == viewModel.id);
            if (product == null)
                return View(ViewName.NullView);

            string? jsonString = "";
            List<ShoppingCartViewModel>? shoppingCartItemList = null;
            if (!HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
                shoppingCartItemList = new List<ShoppingCartViewModel>();
            else
            {
                jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
                shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartViewModel>>(jsonString);
            }

            var shoppingCartViewModel=_lMapper.Map<ShoppingCartViewModel>(viewModel);
            shoppingCartViewModel.Product=product;

            shoppingCartItemList.Add(shoppingCartViewModel);
            jsonString = JsonSerializer.Serialize(shoppingCartItemList);
            HttpContext.Session.SetString(ShoppingCartSessionKey.ShoppingCartListKey, jsonString);
            return RedirectToAction(nameof(List));
        }

        public IActionResult CartView()
        {
            if (HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
            {
                string? jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
                List<ShoppingCartViewModel>? shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartViewModel>>(jsonString);
                return View(shoppingCartItemList);
            }

            return View(ViewName.EmptyCart,StringModel.ShoppingCartIsEmpty);
        }

        [HttpPost]
        public IActionResult CartView(string receiverName, string address, string email, List<ShoppingCartViewModel> shoppingCartViewModels)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var member = _context.Member.Where(n=>n.Email == userEmail).SingleOrDefault();
            Guid OrderNumberGuid = Guid.NewGuid();

             var order = new Order()
            {
                Address = address,
                Email = email,
                MemberId = member.Id,
                OrderDate = DateTime.Now,
                ReceiverName = receiverName,
                OrderNumber = OrderNumberGuid,
                IsShopping = false,
             };
            _context.Order.Add(order);

            OrderDetail? orderDetail = null;

            foreach (var  item in shoppingCartViewModels)
            {
                orderDetail = new OrderDetail()
                {
                    ProductId= item.Id,
                    OrderNumber=OrderNumberGuid,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    TotalPrice = item.TotalPrice
                };
                _context.OrderDetail.Add(orderDetail);
            }
            _context.SaveChanges();
            RemoveSession(ShoppingCartSessionKey.ShoppingCartListKey);

            return RedirectToAction(nameof(OrderController.MemberOrder),ControllerName.Order);
        }

        private void RemoveSession(string SessionKey)
        {
            if (HttpContext.Session.Keys.Contains(SessionKey))
                HttpContext.Session.Remove(SessionKey);
        }

    }
}
