using MeowoofStore.Data;
using MeowoofStore.Models;
using MeowoofStore.Models.StringKeys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeowoofStore.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles =nameof(RoleName.Administrator))]
        public IActionResult List()
        {
            var order = _context.Order.ToList();

            if (order.Count == 0)
                return View(ViewName.EmptyOrder);

            return View(order);
        }

        [Authorize(Roles = nameof(RoleName.Administrator))]
        public IActionResult AdminOrderDetail(Guid OrderNumber)
        {
            var orderDetail = _context.OrderDetail
                                             .Where(n => n.OrderNumber == OrderNumber)
                                             .Include(n => n.Product).ToList();

            return View(orderDetail);
        }

        [Authorize(Roles = nameof(RoleName.Administrator))]
        public IActionResult AdminDeleteOrder(Guid OrderNumber)
        {
            var order = _context.Order
                                             .Where(n => n.OrderNumber == OrderNumber).SingleOrDefault();
            if(order!=null)
                 _context.Remove(order);

            var orderDetail = _context.OrderDetail
                                  .Where(n => n.OrderNumber == OrderNumber).ToList();
            if(orderDetail!=null&&orderDetail.Count>0)
                _context.Remove(orderDetail);

            _context.SaveChanges();
            return RedirectToAction(nameof(List));
        }

        public IActionResult MemberOrder()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var member = _context.Member.Where(n => n.Email == userEmail).SingleOrDefault();
            var order = _context.Order.Where(n => n.MemberId == member.Id).ToList();

            if (order == null || order.Count == 0)
                return View(ViewName.EmptyCart, StringModel.OrderIsEmpty);

            return View(order);
        }

        public IActionResult MemberOrderDetail(Guid OrderNumber)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var order = _context.Order.Include(n => n.Member)                          //確認為登入者訂單號
                .Where(n => n.Email == userEmail && n.OrderNumber == OrderNumber);

            if (order == null)
                return View(ViewName.EmptyCart);

            var orderDetail = _context.OrderDetail
                                             .Where(n => n.OrderNumber == OrderNumber)
                                             .Include(n => n.Product).ToList();

            return View(orderDetail);
        }

    }
}
