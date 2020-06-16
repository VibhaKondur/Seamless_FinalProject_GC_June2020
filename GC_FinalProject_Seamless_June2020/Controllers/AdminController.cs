using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Data;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.DependencyInjection;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
        [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly SeamedInDBContext _context;
        public AdminController(IServiceProvider serviceProvider, SeamedInDBContext context)
        {
            this.serviceProvider = serviceProvider;
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var users = _context.AspNetUsers.ToList();
            return View(users);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminView()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(string email, string usertype)
        {
            await CreateRoles(email, usertype);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(string email, string newtype, string newtheme, string newname, string newtech, string newlandscape, string newindustry, string newphone,
            string newemail, string newwebsite, string newpic, string newcity, string newstate, string newcountry)
        {
            var users = _context.Users.Where(x => x.EmailAddress == email).ToList();
            foreach (Users user in users)
            {
                if (user.EmailAddress == email)
                {
                    user.UserType = newtype;
                    user.Theme = newtheme;
                    user.Name = newname;
                    user.Technology = newtech;
                    user.Landscape = newlandscape;
                    user.Industry = newindustry;
                    user.PhoneNumber = newphone;
                    user.EmailAddress = newemail;
                    user.Website = newwebsite;
                    user.City = newcity;
                    user.StateProvince = newstate;
                    user.Country = newcountry;
                }
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string email)
        {
            var users =  _context.Users.Where(x => x.EmailAddress == email).ToList();
            foreach(Users user in users)
            {
                if(user.EmailAddress == email)
                {
                    _context.Users.Remove(user);
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        private async Task CreateRoles( string email, string usertype)
        {
            if (email != "example@example.com")
            {
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
                //here in this line we are adding Admin Role
                var roleCheck = await RoleManager.RoleExistsAsync(usertype);
                if (!roleCheck)
                {
                    //here in this line we are creating admin role and seed it to the database
                    await RoleManager.CreateAsync(new IdentityRole(usertype));
                }
                //here we are assigning the Admin role to the User that we have registered above 
                //Now, we are assinging admin role to this user("Ali@gmail.com"). When will we run this project then it will
                //be assigned to that user.
                IdentityUser user = await UserManager.FindByEmailAsync(email);
                await UserManager.AddToRoleAsync(user, usertype);
                if (usertype == "Admin")
                {
                    await UserManager.RemoveFromRoleAsync(user, "Startup");
                }
                else if (usertype == "Startup")
                {
                    await UserManager.RemoveFromRoleAsync(user, "Admin");
                    _ = await UserManager.UpdateSecurityStampAsync(user);
                    
                }
                
            }
        }

    }
}
