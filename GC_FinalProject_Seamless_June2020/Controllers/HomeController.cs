using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly SeamedInDal _seamedInDal;
        private readonly SeamedInDBContext _context;


        public HomeController(IConfiguration configuration, SeamedInDBContext context)
        {

            _seamedInDal = new SeamedInDal(configuration);

            _context = context;
        }
        

        public async Task<IActionResult> Index()
        {
            var startUp = await _seamedInDal.GetStartups();
            return View(startUp);
        }

        public async Task<IActionResult> SearchResults()
        {
            var s = await _seamedInDal.GetStartups();

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            Users thisUser = _context.Users.Where(x => x.UserId == id).First();

            var rankedStartups = Ranking(s, thisUser);

			return View(rankedStartups);
        }

		public async Task<IActionResult> StartupProfile(string name)
		{
			Record s = await _seamedInDal.GetStartUpByName(name);

			return View(s);
		}

		public IActionResult RegisterUser()
		{
			string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

			AspNetUsers thisUser = _context.AspNetUsers.Where(x => x.Id == id).First();

			return View(thisUser);
		}

		public async Task<IActionResult> AddUser(Users u)
		{
			if (ModelState.IsValid)
			{
				_context.Users.Add(u);
				_context.SaveChanges();
				return RedirectToAction("SearchResults");
			}
			else
			{
				return View();
			}

			
		}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
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

				if (startup.fields.TechnologyAreas != null && user.Technology != null && startup.fields.TechnologyAreas.Contains(user.Technology) )
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
