using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;

        public AccountController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }

        public IActionResult Login()
        {
            //si esta logueado nos envia al index
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel()); //si no nos envìa a la vista
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _userHelper.LoginAsync(model);
                if (result.Succeeded) //si se logueo
                {
                    //y si tiene direcciòn de retorno
                    if (Request.Query.Keys.Contains("ReturnUrl"))
                    {
                        return Redirect(Request.Query["ReturnUrl"].First()); //y direcciona a la direcciòn de retorno
                    }

                    return RedirectToAction("Index", "Home"); //si no tiene direcciòn de retorno devuelve al index
                }

                ModelState.AddModelError(string.Empty, "Email or password incorrect.");
            }

            return View(model); //si hay problemas con la viewmodel nos devuelve al modelo
        }

        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}
