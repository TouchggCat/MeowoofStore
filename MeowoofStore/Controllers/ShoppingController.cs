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
            var products = _context.Product?.Select(n => n).ToList();
            if(products.Count>0 )
               return View();

            return View(ViewName.EmptyProduct);
        }
      

        public IActionResult CartView()
        {
            if (HttpContext.Session.Keys.Contains(ShoppingCartSessionKey.ShoppingCartListKey))
            {
                return View();
            }

            return View(ViewName.EmptyCart,StringModel.ShoppingCartIsEmpty);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult CartView(string receiverName, string address, string email)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var member = _context.Member?.Where(n=>n.Email == userEmail).SingleOrDefault();
            Guid orderNumberGuid = Guid.NewGuid();

            var order = MappingOrder(address, email, member.Id, receiverName, orderNumberGuid);
            _context.Order.Add(order);

            // 取得所有需要更新的商品 ID
            List<ShoppingCartViewModel>? shoppingCartViewModels = GetShoppingCartItemsFromSession();
            var productIds = shoppingCartViewModels.Select(item => item.Id).ToList();
            // 從資料庫中查詢這些商品
            var products = _context.Product.Where(p => productIds.Contains(p.Id)).ToList();

            foreach (var item in shoppingCartViewModels)
            {
                var product = products.FirstOrDefault(p => p.Id == item.Id);
                if (product != null)
                    product.Stock -= item.Quantity;

                _context.Product.Update(product);

                var orderDetail = MappingOrderDetail(item.Id, orderNumberGuid, item.Price, item.Quantity, item.TotalPrice);
                _context.OrderDetail.Add(orderDetail);
            }

            _context.SaveChanges();
            RemoveSession(ShoppingCartSessionKey.ShoppingCartListKey);

            return RedirectToAction(nameof(OrderController.MemberOrder),ControllerName.Order);
        }


        private Order MappingOrder(string address, string email, int memberId, string receiverName, Guid orderNumberGuid)
        {
            return new Order()
            {
                Address = address,
                Email = email,
                MemberId = memberId,
                OrderDate = DateTime.Now,
                ReceiverName = receiverName,
                OrderNumber = orderNumberGuid,
                IsShipping = false,
            };
        }

        private OrderDetail MappingOrderDetail(int productId, Guid orderNumber, int price, int quantity, int totalPrice)
        {
            return new OrderDetail()
            {
                ProductId = productId,
                OrderNumber = orderNumber,
                Price = price,
                Quantity = quantity,
                TotalPrice = totalPrice
            };
        }

        private List<ShoppingCartViewModel>? GetShoppingCartItemsFromSession()
        {
            string? jsonString = HttpContext.Session.GetString(ShoppingCartSessionKey.ShoppingCartListKey);
            List<ShoppingCartViewModel>? shoppingCartItemList = JsonSerializer.Deserialize<List<ShoppingCartViewModel>>(jsonString);
            return shoppingCartItemList;
        }

        private void RemoveSession(string SessionKey)
        {
            if (HttpContext.Session.Keys.Contains(SessionKey))
                HttpContext.Session.Remove(SessionKey);
        }

    }
}
