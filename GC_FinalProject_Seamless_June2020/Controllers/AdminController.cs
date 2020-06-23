using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Data;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IServiceProvider serviceProvider;
        private readonly SeamedInDBContext _context;
        private readonly SeamedInDal _seamedInDal;

        public AdminController(IServiceProvider serviceProvider, SeamedInDBContext context, IConfiguration configuration)
        {
            this.serviceProvider = serviceProvider;
            _context = context;
            _seamedInDal = new SeamedInDal(configuration);
        }

        public async Task<IActionResult> IndexAsync()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;

            var users = _context.AspNetUsers.ToList();
            return View(users);
        }

        public async Task<IActionResult> AdminView()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;
            var users = _context.Users.ToList();
            return View(users);
        }

        public async Task<IActionResult> AddRole(string id, string usertype)
        {
            await CreateRoles(id, usertype);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateForm(string id)
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            var user = _context.Users.Where(x => x.UserId == id).First();

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;
            return View(user);
        }

        public async Task<IActionResult> Update(string id, string newtype, string newtheme, string newname, string newtech, string newlandscape, string newindustry, string newphone,
            string newemail, string newwebsite, string newpic, string newcity, string newstate, string newcountry)
        {
            var user = _context.Users.Where(x => x.UserId == id).First();


            if (ModelState.IsValid)
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
                user.ProfilePicture = newpic;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("AdminView");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id != "76b4ba6b-8a94-4fe9-9f64-6b2d386d50fd")
            {
                var user = _context.Users.Where(x => x.UserId == id).First();

                _context.Users.Remove(user);


                await _context.SaveChangesAsync();
            }
            return RedirectToAction("AdminView");
        }

        public async Task<IActionResult> DeleteAccount(string id)
        {
            if (id != "76b4ba6b-8a94-4fe9-9f64-6b2d386d50fd")
            {

                var user1 = _context.Users.Where(x => x.UserId == id).ToList();
                var user2 = _context.Favorites.Where(x => x.UserId == id).ToList();
                var user = _context.AspNetUsers.Where(x => x.Id == id).First();
                _context.AspNetUsers.Remove(user);
                if (user1.Count > 0)
                {
                    _context.Users.RemoveRange(user1);
                }
                if (user2.Count > 0)
                {
                    _context.Favorites.RemoveRange(user2);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        private async Task CreateRoles(string id, string usertype)
        {
            if (id != "76b4ba6b-8a94-4fe9-9f64-6b2d386d50fd")
            {
                var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

                var roleCheck = await RoleManager.RoleExistsAsync(usertype);
                if (!roleCheck)
                {
                    await RoleManager.CreateAsync(new IdentityRole(usertype));
                }

                IdentityUser user = await UserManager.FindByIdAsync(id);
                await UserManager.AddToRoleAsync(user, usertype);
                if (usertype == "Admin")
                {
                    var user1 = _context.AspNetUsers.Find(id);
                    user1.Roles = "Admin";
                    await _context.SaveChangesAsync();
                    await UserManager.RemoveFromRoleAsync(user, "Startup");
                    await UserManager.RemoveFromRoleAsync(user, "Partner");
                    await UserManager.RemoveFromRoleAsync(user, "Seamless");
                }
                else if (usertype == "Startup")
                {
                    var user1 = _context.AspNetUsers.Find(id);
                    user1.Roles = "Startup";
                    await _context.SaveChangesAsync();
                    await UserManager.RemoveFromRoleAsync(user, "Admin");
                    await UserManager.RemoveFromRoleAsync(user, "Partner");
                    await UserManager.RemoveFromRoleAsync(user, "Seamless");
                    _ = await UserManager.UpdateSecurityStampAsync(user);

                }
                else if (usertype == "Partner")
                {
                    var user1 = _context.AspNetUsers.Find(id);
                    user1.Roles = "Partner";
                    await _context.SaveChangesAsync();
                    await UserManager.RemoveFromRoleAsync(user, "Admin");
                    await UserManager.RemoveFromRoleAsync(user, "Startup");
                    await UserManager.RemoveFromRoleAsync(user, "Seamless");
                    _ = await UserManager.UpdateSecurityStampAsync(user);

                }
                else if (usertype == "Seamless")
                {
                    var user1 = _context.AspNetUsers.Find(id);
                    user1.Roles = "Seamless";
                    await _context.SaveChangesAsync();
                    await UserManager.RemoveFromRoleAsync(user, "Admin");
                    await UserManager.RemoveFromRoleAsync(user, "Startup");
                    await UserManager.RemoveFromRoleAsync(user, "Partner");
                    _ = await UserManager.UpdateSecurityStampAsync(user);

                }
            }
        }

        public IEnumerable<StartupRank> Ranking(Startups startups, Users user)
        {
            List<StartupRank> rankedList = new List<StartupRank>();

            //do algorithm and add to rankedList accordingly

            foreach (Record startup in startups.records)
            {
                int rank = 0;
                //if statements

                if (startup.fields.Country != null && user.Country != null && (startup.fields.Country == user.Country))
                {
                    rank += 1;
                }

                if (startup.fields.Alignment != null && user.Name != null && startup.fields.Alignment.Contains(user.Name))
                {
                    rank += 5;
                }

                if (startup.fields.Themes != null && user.Theme != null && startup.fields.Themes.Contains(user.Theme))
                {
                    rank += 3;
                }

                if (startup.fields.TechnologyAreas != null && user.Technology != null && startup.fields.TechnologyAreas.Contains(user.Technology))
                {
                    rank += 3;
                }

                if (startup.fields.Landscape != null && user.Landscape != null && startup.fields.Landscape == user.Landscape)
                {
                    rank += 4;
                }

                StartupRank s = new StartupRank(startup, rank);
                rankedList.Add(s);
            }

            IEnumerable<StartupRank> sortedStartups =
                from startup in rankedList
                orderby startup.Rank descending
                select startup;

            return sortedStartups;

        }

    }
}
