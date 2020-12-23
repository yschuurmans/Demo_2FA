using _2FA_Demo.Models;
using _2FA_Demo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _2FA_Demo.Controllers
{
    public class LoginController : Controller
    {
        private readonly ILoginService loginService;
        private readonly ITotpService totpService;

        public LoginController(ILoginService loginService, ITotpService totpService)
        {
            this.loginService = loginService;
            this.totpService = totpService;
        }

        #region LoginRegister
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginView view)
        {
            if (loginService.ValidateCredentials(view.Username, view.Password))
            {
                HttpContext.Session.Clear();

                if (totpService.HasTotpSetup(view.Username))
                {
                    HttpContext.Session.SetString("TotpCheck", view.Username);
                    HttpContext.Session.CommitAsync().Wait();
                    return View("Totp");
                }

                HttpContext.Session.SetString("Username", view.Username);
                HttpContext.Session.CommitAsync().Wait();
                return View("Success");
            }
            return View("Login", new LoginView { Error = "Password incorrect" });
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(LoginView view)
        {
            var result = loginService.RegisterAccount(view.Username, view.Password);
            if(result != null)
            {
                return View(new LoginView { Error = result });
            }
            return Login(view);
        }
        #endregion

        #region Totp

        public IActionResult SetupTotp()
        {
            if (HttpContext.Session.Keys.Contains("Username"))
            {
                var secret = totpService.GenerateSecret();
                HttpContext.Session.SetString("secret", secret);
                HttpContext.Session.CommitAsync().Wait();

                return View(new TotpTokenView
                {
                    TokenUri = totpService.GenerateQRCode(HttpContext.Session.GetString("Username"), secret)
                });
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult SetupTotp(TotpTokenView totpTokenView)
        {
            if (HttpContext.Session.Keys.Contains("Username") && HttpContext.Session.Keys.Contains("secret"))
            {
                var username = HttpContext.Session.GetString("Username");
                var secret = HttpContext.Session.GetString("secret");
                if (totpService.CheckToken(secret, totpTokenView.Token))
                {
                    totpService.SaveSecret(username, secret);
                    return View("Linked");
                }
                return View("SetupTotp", new TotpTokenView { Error = "Token incorrect" });
            }
            return RedirectToAction("Login");
        }




        [HttpPost]
        public IActionResult Totp(TotpTokenView view)
        {
            if (HttpContext.Session.Keys.Contains("TotpCheck"))
            {
                var username = HttpContext.Session.GetString("TotpCheck");

                if(totpService.ValidateToken(username, view.Token))
                {
                    HttpContext.Session.Clear();
                    HttpContext.Session.SetString("Username", username);
                    HttpContext.Session.CommitAsync().Wait();
                    return View("Success");
                }
            }
            return View(new TotpTokenView() { Error = "Token incorrect" });
        }
        #endregion
    }
}
