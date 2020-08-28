﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OnSale.Common.Enums;
using OnSale.Web.Data;
using OnSale.Web.Data.Entities;
using OnSale.Web.Models;
using System;
using System.Threading.Tasks;

namespace OnSale.Web.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;

        //UserManager administra los user de la Tabla Users
        //RoleManager administra los roles de la clase IdentityRole
        //SignInManager clase que permite loguear y desloguear
        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager,
                          SignInManager<User> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<User> AddUserAsync(AddUserViewModel model, Guid imageId, UserType userType)
        {
            User user = new User
            {
                Address = model.Address,
                Document = model.Document,
                Email = model.Username,
                FirstName = model.FirstName,
                LastName = model.LastName,
                ImageId = imageId,
                PhoneNumber = model.PhoneNumber,
                City = await _context.Cities.FindAsync(model.CityId),
                UserName = model.Username,
                UserType = userType
            };

            //crea un user 
            IdentityResult result = await _userManager.CreateAsync(user, model.Password);
            if (result != IdentityResult.Success) //IdentityResult evalua si puede o no crear el user
            {
                return null;
            }

            User newUser = await GetUserAsync(model.Username); //obtenemos el user
            await AddUserToRoleAsync(newUser, user.UserType.ToString()); //asignamos un role
            return newUser;
        }


        public async Task AddUserToRoleAsync(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName); //verifica si el role existe
            if (!roleExists) //si no existe
            {
                await _roleManager.CreateAsync(new IdentityRole //creamos el role
                {
                    Name = roleName
                });
            }
        }

        public async Task<User> GetUserAsync(string email)
        {
            return await _context.Users
                .Include(u => u.City)
                .FirstOrDefaultAsync(u => u.Email == email); //devuelve el user que encontrò que es el email
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginViewModel model)
        {
            return await _signInManager.PasswordSignInAsync(
                model.Username,
                model.Password,
                model.RememberMe,
                false); //paràmetro que bloquea la sesiòn despuès de tres intentos siempre y cuando este en true
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> ValidatePasswordAsync(User user, string password)
        {
            return await _signInManager.CheckPasswordSignInAsync(user, password, false);
        }

    }

}
