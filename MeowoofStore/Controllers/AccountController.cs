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

        public IActionResult Register()
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
            new Claim(ClaimTypes.Name, member.MemberName),
            new Claim(ClaimTypes.Email, member.Email),
        }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
        }

        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl; //   [Authorize]傳來的
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            if(!ModelState.IsValid)
                return View(viewModel);

            var member = await _context.Member
                .Where(n => n.Email == viewModel.Email &&n.Password== viewModel.Password)
                .Include(n=>n.Role)
                .SingleOrDefaultAsync();

            if (member == null)
                return NotFound();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,member.Email),
                new Claim(ClaimTypes.Name,member.MemberName),
                new Claim(ClaimTypes.Role,member.Role.RoleName) 
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // 如果上一頁的 URL 是本網站的網址，則使用它作為返回的頁面
            if (Url.IsLocalUrl(viewModel.ReturnUrl))
                return Redirect(viewModel.ReturnUrl);

            // 如果上一頁的 URL 不是本網站的網址，則返回首頁
            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);    
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
        }

        public IActionResult AccessDeny()
        {
            return View();
        }
    }
}
