using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Entities;
using OnSale.Common.Enums;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Helpers;
using OnSale.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OnSale.Web.Controllers
{

    public class AccountController : Controller
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly ICombosHelper _combosHelper;
        private readonly IBlobHelper _blobHelper;

        public AccountController(DataContext context, IUserHelper userHelper, ICombosHelper combosHelper, IBlobHelper blobHelper)
        {
            _context = context;
            _userHelper = userHelper;
            _combosHelper = combosHelper;
            _blobHelper = blobHelper;
        }

        public IActionResult NotAuthorized()
        {
            return View();
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

        public IActionResult Register()
        {
            AddUserViewModel model = new AddUserViewModel
            {
                Countries = _combosHelper.GetComboCountries(),
                Departments = _combosHelper.GetComboDepartments(0), //para que muestre el combo en blanco
                Cities = _combosHelper.GetComboCities(0),
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AddUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                Guid imageId = Guid.Empty; //no se sabe si subieron imàgen

                if (model.ImageFile != null)
                {
                    imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users"); //subimos la imàgen
                }

                User user = await _userHelper.AddUserAsync(model, imageId, UserType.User);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "This email is already used.");
                    model.Countries = _combosHelper.GetComboCountries();
                    model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
                    model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
                    return View(model);
                }

                LoginViewModel loginViewModel = new LoginViewModel
                {
                    Password = model.Password,
                    RememberMe = false,
                    Username = model.Username
                };

                //almacena el logueo
                var result2 = await _userHelper.LoginAsync(loginViewModel);

                if (result2.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            model.Countries = _combosHelper.GetComboCountries();
            model.Departments = _combosHelper.GetComboDepartments(model.CountryId);
            model.Cities = _combosHelper.GetComboCities(model.DepartmentId);
            return View(model);
        }


        public JsonResult GetDepartments(int countryId)
        {
            Country country = _context.Countries
                .Include(c => c.Departments)
                .FirstOrDefault(c => c.Id == countryId);
            if (country == null)
            {
                return null;
            }

            return Json(country.Departments.OrderBy(d => d.Name));
        }

        public JsonResult GetCities(int departmentId)
        {
            Department department = _context.Departments
                .Include(d => d.Cities)
                .FirstOrDefault(d => d.Id == departmentId);
            if (department == null)
            {
                return null;
            }

            return Json(department.Cities.OrderBy(c => c.Name));
        }



    }

}
