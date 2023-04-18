using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeowoofStore.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace MeowoofStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context=context;
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if(!ModelState.IsValid)
                return View(viewModel);

            var member = _context.Member
                .Where(n => n.email == viewModel.email &&n.password== viewModel.password)
                .Include(n=>n.Role)
                .SingleOrDefault();

            if (member == null)
                return NotFound();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,member.email),
                new Claim(ClaimTypes.Name,member.memberName),
                new Claim(ClaimTypes.Role,member.Role.roleName) 
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            string ControllerName="Home";
            return RedirectToAction(nameof(HomeController.Index), ControllerName);
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string ControllerName = "Home";
            return RedirectToAction(nameof(HomeController.Index), ControllerName);
        }

        public IActionResult AccessDeny()
        {
            return View();
        }
    }
}
