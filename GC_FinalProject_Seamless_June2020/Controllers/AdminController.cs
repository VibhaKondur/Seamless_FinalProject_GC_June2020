using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
        [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ApplicationDbContext _context;
        public AdminController(IServiceProvider serviceProvider, ApplicationDbContext context)
        {
            this.serviceProvider = serviceProvider;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AdminView(string email)
        {
            await CreateRoles(email);
            return RedirectToAction("Index");
        }

        private async Task CreateRoles( string email)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            //here in this line we are adding Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck)
            {
                //here in this line we are creating admin role and seed it to the database
                await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }
            //here we are assigning the Admin role to the User that we have registered above 
            //Now, we are assinging admin role to this user("Ali@gmail.com"). When will we run this project then it will
            //be assigned to that user.
            IdentityUser user = await UserManager.FindByEmailAsync(email);
            await UserManager.AddToRoleAsync(user, "Admin");
        }

    }
}
