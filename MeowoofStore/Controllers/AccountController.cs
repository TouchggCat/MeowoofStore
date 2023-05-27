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
            member.IsEnabled = false;

            var callbackUrl = $"https://localhost:7072/Account/RegisterConfirmed?token=";
            callbackUrl=AddTokenToCallbackUrl(member,callbackUrl);

            var mailBody = $"<h1>MeoWoof會員一{member.MemberName}，您好:</h1>" +
                                         $"<br><h2>請點擊連結完成註冊<a href='{callbackUrl}'>請點我</a></h2>" +
                                         $"<br>" +
                                         $"<h2>如未註冊請忽略本信件</h2>";
            var mailSubject = "[MeoWoof會員]一註冊確認信";
            MailMessage mail = IntegrateMailMessage(viewModel.Email, mailSubject, mailBody);
            SendMail(mail);

            try
            {
                byte[] salt = PasswordAndSaltProcess.SaltGenerator();

                string hashPassword = PasswordAndSaltProcess.HashEnteredPassword(salt,member.Password);

                await SaveSaltAndPasswordToDB(member, salt, hashPassword);

                return Content("<script>alert('註冊信件已送出，請至信箱查看');window.location.href='https://localhost:7072/'</script>", "text/html", System.Text.Encoding.UTF8);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "註冊時發生錯誤，請稍後再試。");
            }

            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
        }

        public async Task <IActionResult> RegisterConfirmed(string token)
        {
            string email = ValidateToken(token);

            var member = await _context.Member.Where(m => m.Email == email).FirstOrDefaultAsync();
            if (member != null)
            {
                await Set_IsEnable_PropertyToTrue(member);

                return View(ViewName.RegisterConfirmed, StringModel.RegisterConfirmed);
            }

            return View(ViewName.RegisterConfirmed, StringModel.RegisterFailed);
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


            if (!IsValidLogin(viewModel))
                return View(ViewName.RegisterConfirmed, StringModel.RegisterFailed);

            await SignInByClaimIdentity(member);

            // 如果上一頁的 URL 是本網站的網址，則使用它作為返回的頁面
            if (Url.IsLocalUrl(viewModel.ReturnUrl))
                return Redirect(viewModel.ReturnUrl);

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
                return Content("<script>alert('未註冊的帳號，請確認輸入是否正確');window.location.href='https://localhost:7072/Account/Login'</script>", "text/html", System.Text.Encoding.UTF8);

            var callbackUrl = "https://localhost:7072/Account/ResetPassword?token=";
            callbackUrl = AddTokenToCallbackUrl(member, callbackUrl);
            var mailBody = $"<h1>MeoWoof會員一{member.MemberName}，您好:</h1><br><h2>如欲重新設定密碼<a href='{callbackUrl}'>請點我</a></h2>";
            var mailSubject = "[MeoWoof會員]一密碼重設通知信";
            MailMessage mail = IntegrateMailMessage(userEmail, mailSubject, mailBody);
            SendMail(mail);

            return Content("<script>alert('信件已送出，請至信箱查看');window.location.href='https://localhost:7072/'</script>", "text/html", System.Text.Encoding.UTF8);
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

            string email = ValidateToken(viewModel.Token);

            var member = await _context.Member.Where(m => m.Email == email).FirstOrDefaultAsync();

            await Set_IsEnable_PropertyToTrue(member);
            try
            {
                byte[] salt = PasswordAndSaltProcess.SaltGenerator();
                string hashPassword = PasswordAndSaltProcess.HashEnteredPassword(salt, viewModel.Password);

                await SaveSaltAndPasswordToDB(member, salt, hashPassword);

                return RedirectToAction(nameof(Login));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "重設密碼發生錯誤，請稍後再試。");
            }
            return RedirectToAction(nameof(HomeController.Index), ControllerName.Home);
        }
        private async Task Set_IsEnable_PropertyToTrue(Member? member)
        {
            member.IsEnabled = true;
            await _context.SaveChangesAsync();
        }

        private string AddTokenToCallbackUrl(Member? member,string callbackUrl)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,member.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            string token = GenerateJWT(claimsIdentity);
            callbackUrl += token;
            return callbackUrl;
        }

        private string ValidateToken(string token )
        {
            try
            {
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
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
                ClaimsIdentity claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                var emailClaim = claimsIdentity.FindFirst(ClaimTypes.Email);
                var email = emailClaim.Value;

                return email;
            }
            catch (SecurityTokenException ex)
            {
                // 處理驗證例外情況
                throw new Exception("Token validation failed.", ex);
            }
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

        private static void SendMail(MailMessage mail)
        {
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
        }

        private static MailMessage IntegrateMailMessage(string userEmail,string mailSubject,string mailBody)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(SendingMailServiceKey.sendMailServiceAccount, "MeoWoofStore");   //前面是發信email後面是顯示的名稱
            mail.To.Add(userEmail);  //收信者email from 參數
            mail.Subject = mailSubject;
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;   //內容使用html
            mail.Body = mailBody;
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            return mail;
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


        private bool IsValidLogin(LoginViewModel loginViewModel)
        {
            var member = _context.Member.SingleOrDefault(m => m.Email == loginViewModel.Email);

            if (member == null||member.IsEnabled==false)
                return false;

            var hashedPasswordFromDb = member.Password;

            // Compute the hash of the user-provided password using the retrieved salt
            var hashPassword = PasswordAndSaltProcess
                                    .HashEnteredPassword(member.Salt, loginViewModel.Password);

            if (hashPassword != hashedPasswordFromDb)
                return false;

            return true;
        }
    }
}
