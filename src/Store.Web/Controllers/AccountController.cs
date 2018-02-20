using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Store.Domain;
using Store.Domain.Account;
using Store.Domain.Products;
using Store.Web.ViewsModels;

namespace Store.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthentication _authentication;

        public AccountController(IAuthentication authentication)  
        {
           _authentication = authentication;
        }

        public IActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var result = await _authentication.Authenticate(model.Email, model.Password);
            if(result) return Redirect("/");
            else 
            {
                ModelState.AddModelError(string.Empty, "Invalid Login attempt.");
                return View(model);
            }
        }
        public async Task<IActionResult> Logout()
        {
            await _authentication.Logout();
            return Redirect("/Account/Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
