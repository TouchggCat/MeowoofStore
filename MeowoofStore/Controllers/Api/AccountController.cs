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
    public class AccountController : ControllerBase
    {
        private ApplicationDbContext _context { get; set; }
        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<Product>> GetUserInfo()
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var userInfo = await _context.Member.Where(n => n.Email == userEmail).SingleOrDefaultAsync();

            if (userInfo == null)
                return NotFound("No User !");

            return Ok(userInfo);
        }
    }
}
