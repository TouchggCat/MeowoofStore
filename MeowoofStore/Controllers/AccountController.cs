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
using AutoMapper;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Data;
using System;
using static System.Net.WebRequestMethods;

namespace MeowoofStore.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _IMapper;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext context,IMapper mapper,IConfiguration configuration)
        {
            _context=context;
            _IMapper=mapper;
            _configuration=configuration;
        }

        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel? viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            var member = _IMapper.Map<Member>(viewModel);

            try
            {
                // Generate a random salt value
                byte[] salt = PasswordAndSaltProcess.SaltGenerator();

                // Hash the user's password
                //string hashPassword = HashUserPassword(member, salt);
                string hashPassword = PasswordAndSaltProcess.HashEnteredPassword(salt,member.Password);


                // Save the salt and hashed password to your database
                await SaveSaltAndPasswordToDB(member, salt, hashPassword);

                await SignInByClaimIdentity(member);

                return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "註冊時發生錯誤，請稍後再試。");
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

        public IActionResult ForgetPassword(string userEmail)
        {
            var member = _context.Member.Where(m => m.Email == userEmail).SingleOrDefault();
            if (member == null)
                return Content("<script>alert('未註冊的帳號，請確認輸入是否正確');window.location.href='https://localhost:7072/Login/ForgetPassword'</script>", "text/html", System.Text.Encoding.UTF8);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,member.Email)
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            string token = GenerateJWT(claimsIdentity);
            var callbackUrl = $"https://localhost:7072/Account/ResetPassword?token={token}";

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SendingMailServiceKey.sendMailServiceAccount, "MeoWoofStore");   //前面是發信email後面是顯示的名稱
            mail.To.Add(userEmail);  //收信者email from 參數
            mail.Subject = "[MeoWoof會員]一密碼重設通知信"; 
            mail.SubjectEncoding = System.Text.Encoding.UTF8; 
            mail.IsBodyHtml = true;   //內容使用html
            mail.Body = $"<h1>MeoWoof會員一{member.MemberName}，您好:</h1><br><h2>如欲重新設定密碼<a href='{callbackUrl}'>請點我</a></h2>";
            mail.BodyEncoding = System.Text.Encoding.UTF8;  
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.Host = "smtp.gmail.com"; //設定google server
                    client.Port = 587;              //google port
                    client.Credentials = new NetworkCredential(SendingMailServiceKey.sendMailServiceAccount, SendingMailServiceKey.sendMailServicePassword);
                    client.EnableSsl = true;
                    client.Send(mail);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                mail.Dispose();
            }
            return Content("<script>alert('信件已送出，請至信箱查看');window.location.href='https://localhost:7072/'</script>", "text/html", System.Text.Encoding.UTF8);    //localhost版本
                                                                                                                                                                 //return Content("<script>alert('信件已送出，請至信箱查看');window.location.href='http://localhost/Home/index'</script>", "text/html", System.Text.Encoding.UTF8);  //IIS版本
                                                                                                                                                                 //window.location.href跳轉業面
        }

        public IActionResult ResetPassword(string token)
        {
            ViewBag.Token = token;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  ResetPassword(ResetPasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JWT:KEY"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };
            SecurityToken validatedToken;
            var claimsPrincipal = tokenHandler.ValidateToken(viewModel.Token, tokenValidationParameters, out validatedToken);
            ClaimsIdentity claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
            var email = emailClaim.Value;


            var member =_context.Member.Where(m=>m.Email == email).FirstOrDefault();

            try
            {
                // Generate a random salt value
                byte[] salt = PasswordAndSaltProcess.SaltGenerator();

                // Hash the user's password
                //string hashPassword = HashUserPassword(member, salt);
                string hashPassword = PasswordAndSaltProcess.HashEnteredPassword(salt, viewModel.Password);


                // Save the salt and hashed password to your database
                await SaveSaltAndPasswordToDB(member, salt, hashPassword);

                await SignInByClaimIdentity(member);

                return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "註冊時發生錯誤，請稍後再試。");
            }
               return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
        }

            private string GenerateJWT(ClaimsIdentity claimsIdentity)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:KEY"]));

            var jwt = new JwtSecurityToken
            (
                issuer : _configuration["JWT:Issuer"],
                audience: _configuration["JWT:audience"],
                claims:claimsIdentity.Claims,
                expires:DateTime.Now.AddMinutes(10),
                signingCredentials:new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
            );
                
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }


        private async Task SaveSaltAndPasswordToDB(Member? member, byte[] salt, string hashPassword)
        {
            //member.RoleId = Member.AdminRole;    新增管理者
            member.RoleId = Member.CustomerRole;  
            member.Password = hashPassword;
            member.Salt = salt;
            if(member.Id==0)
              _context.Member.Add(member);

            await _context.SaveChangesAsync();
        }

        private async Task SignInByClaimIdentity(Member? member)
        {
            member = await _context.Member
                  .Include(m => m.Role)
                  .SingleOrDefaultAsync(m => m.Email == member.Email);

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
                                    .HashEnteredPassword(member.Salt, loginViewModel.Password);

            // Compare the computed hash with the stored hash
            if (hashPassword != hashedPasswordFromDb)
                return false;

            return true;
        }
    }
}
