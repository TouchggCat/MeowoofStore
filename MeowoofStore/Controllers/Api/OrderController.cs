using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MeowoofStore.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private ApplicationDbContext _context { get; set; }
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("/api/Order/{id}")]
        public async Task<ActionResult<OrderDetail>> ChangeIsShippingProperty(int id)
        {
            var order = await _context.Order.Where(od=>od.Id==id).SingleOrDefaultAsync();
            if (order == null)
                return NotFound();

            order.IsShipping =!order.IsShipping;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
            return Ok("ChangedProperty");
        }
    }
}
