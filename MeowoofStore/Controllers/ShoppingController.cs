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
using System.Web;
using System.Text;
using System.Security.Cryptography;
using System.Diagnostics.Metrics;

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
            var orderNumberGuid = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var member = _context.Member?.Where(n => n.Email == userEmail).SingleOrDefault();
            var customerOrder = MappingOrder(address, email, member.Id, receiverName, orderNumberGuid);
            _context.Order.Add(customerOrder);

            var shoppingCartViewModels = GetShoppingCartItemsFromSession();

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

            return RedirectToAction(nameof(OrderController.MemberOrder), ControllerName.Order);

        }
        public IActionResult PayForOrder(string orderNumber)
        {
            int sumTotalPrice = 0;
            string allProductsName = "";
            var orderdetail = _context.OrderDetail.Where(n => n.OrderNumber == orderNumber).Include(n=>n.Product).ToList();
            foreach (var item in orderdetail)
            {
                allProductsName += item.Product.Name + "#";
                sumTotalPrice += item.TotalPrice;
            }
            allProductsName.Substring(0, allProductsName.Length - 1);

            string encodedOrderNumber = Convert.ToBase64String(Encoding.UTF8.GetBytes(orderNumber));

            var website = "https://localhost:7072";

            var order = new Dictionary<string, string>
        {
            //特店交易編號
            { "MerchantTradeNo",  orderNumber},

            //特店交易時間 yyyy/MM/dd HH:mm:ss
            { "MerchantTradeDate",  DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")},

            //交易金額
            { "TotalAmount",  sumTotalPrice.ToString()},

            //交易描述
            { "TradeDesc",  "無"},

            //商品名稱
            { "ItemName",  allProductsName},

            //允許繳費有效天數(付款方式為 ATM 時，需設定此值)
            { "ExpireDate",  "3"},

            //自訂名稱欄位1
            { "CustomField1",  ""},

            //自訂名稱欄位2
            { "CustomField2",  ""},

            //綠界回傳付款資訊的至 此URL
            { "ReturnURL",  $"{website}/api/Shopping/AddPayInfo"},

            //使用者於綠界 付款完成後，綠界將會轉址至 此URL
            { "OrderResultURL", $"{website}/Shopping/CompleteOrder?encodedOrderNumber={encodedOrderNumber}" },

            //特店編號， 2000132 測試綠界編號
            { "MerchantID",  "2000132"},

            //忽略付款方式9
            { "IgnorePayment",  "GooglePay#WebATM#CVS#BARCODE"},

            //交易類型 固定填入 aio
            { "PaymentType",  "aio"},

            //選擇預設付款方式 固定填入 ALL
            { "ChoosePayment",  "ALL"},

            //CheckMacValue 加密類型 固定填入 1 (SHA256)
            { "EncryptType",  "1"},
        };

            //檢查碼
            order["CheckMacValue"] = GetCheckMacValue(order);

            return View("CheckOut", order);
        }
        [AllowAnonymous]
        public IActionResult CompleteOrder(string encodedOrderNumber)
        {
            string orderNumber = Encoding.UTF8.GetString(Convert.FromBase64String(encodedOrderNumber));

            var order =_context.Order.Where(od=>od.OrderNumber == orderNumber).FirstOrDefault();
            if (order != null)
            {
                order.IsPaid = true;
                _context.SaveChanges();
            }
     
            return RedirectToAction(nameof(OrderController.MemberOrder), ControllerName.Order);
        }
        private Order MappingOrder(string address, string email, int memberId, string receiverName, string orderNumberGuid)
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

        private OrderDetail MappingOrderDetail(int productId, string orderNumber, int price, int quantity, int totalPrice)
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

        /// <summary>
        /// 取得 檢查碼
        /// </summary>
        private string GetCheckMacValue(Dictionary<string, string> order)
        {
            var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();

            var checkValue = string.Join("&", param);

            //測試用的 HashKey
            var hashKey = "5294y06JbISpM5x9";

            //測試用的 HashIV
            var HashIV = "v77hoKGq4kWxNNIS";

            checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";

            checkValue = HttpUtility.UrlEncode(checkValue).ToLower();

            checkValue = GetSHA256(checkValue);

            return checkValue.ToUpper();
        }

        /// <summary>
        /// SHA256 編碼
        /// </summary>
        private string GetSHA256(string value)
        {
            var result = new StringBuilder();
            var sha256 = SHA256.Create();
            var bts = Encoding.UTF8.GetBytes(value);
            var hash = sha256.ComputeHash(bts);

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }

            return result.ToString();
        }

        private void RemoveSession(string SessionKey)
        {
            if (HttpContext.Session.Keys.Contains(SessionKey))
                HttpContext.Session.Remove(SessionKey);
        }
    }
}
