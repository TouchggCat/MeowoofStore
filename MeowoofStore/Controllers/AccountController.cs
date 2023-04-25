using MeowoofStore.Data;
using MeowoofStore.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MeowoofStore.ViewModels;
using Microsoft.EntityFrameworkCore;
using MeowoofStore.Models.StringKeys;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using MeowoofStore.Models.Utilities;

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
        public async Task<IActionResult> Register(Member? member)
        {
            if (!ModelState.IsValid)
                return View(member);

            try
            {
                // Generate a random salt value
                byte[] salt = PasswordAndSaltProcess.SaltGenerator();

                // Hash the user's password
                //string hashPassword = HashUserPassword(member, salt);
                string hashPassword = PasswordAndSaltProcess.HashEnteredPassword(member, salt,nameof(member.Password));


                // Save the salt and hashed password to your database
                await SaveSaltAndPasswordToDB(member, salt, hashPassword);

                await SignInByClaimIdentity(member);

                return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while registering. Please try again later.");
            }

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
            if (!ModelState.IsValid)
                return View(viewModel);

            var member = await _context.Member
                                       .Where(m => m.Email == viewModel.Email)
                                       .Include(n => n.Role)
                                       .SingleOrDefaultAsync();

            if (!IsValidLogin(viewModel, _context))
                return NotFound();

            await SignInByClaimIdentity(member);

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



        private async Task SaveSaltAndPasswordToDB(Member? member, byte[] salt, string hashPassword)
        {
            member.RoleId = Member.CustomerRole;
            member.Password = hashPassword;
            member.Salt = salt;
            _context.Member.Add(member);
            await _context.SaveChangesAsync();
        }

        private async Task SignInByClaimIdentity(Member? member)
        {
                member = await _context.Member
                                       .Where(m => m.Email == member.Email)
                                       .Include(n => n.Role)
                                       .SingleOrDefaultAsync();  //TODO 想辦法在其他地方載入?

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,member.Email),
                new Claim(ClaimTypes.Name,member.MemberName),
                new Claim(ClaimTypes.Role,member.Role.RoleName)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }


        private bool IsValidLogin(LoginViewModel loginViewModel, ApplicationDbContext _context)
        {
            var member = _context.Member.SingleOrDefault(m => m.Email == loginViewModel.Email);

            if (member == null)
                return false;

            var hashedPasswordFromDb = member.Password;

            // Compute the hash of the user-provided password using the retrieved salt
            var hashPassword = PasswordAndSaltProcess
                                    .HashEnteredPassword(loginViewModel, member.Salt, nameof(loginViewModel.Password));

            // Compare the computed hash with the stored hash
            if (hashPassword != hashedPasswordFromDb)
                return false;

            return true;
        }
    }
}
