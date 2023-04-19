using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeowoofStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using MeowoofStore.Models.StringKeys;

namespace MeowoofStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AccountController(ApplicationDbContext context)
        {
            _context=context;
        }

        public async Task<IActionResult> Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(Member member)
        {
            if (!ModelState.IsValid)
                return View(member);

            member.RoleId = Member.CustomerRole;
            _context.Member.Add(member);
            await _context.SaveChangesAsync();

            var identity = new ClaimsIdentity(new[]
    {
            new Claim(ClaimTypes.Name, member.memberName),
            new Claim(ClaimTypes.Email, member.email),
        }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
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
            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);    
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), HomeController.ControllerName);
        }

        public IActionResult AccessDeny()
        {
            return View();
        }
    }
}
