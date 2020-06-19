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
using Newtonsoft.Json;

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
        
		[Authorize]
        public async Task<IActionResult> Index()
        {
			string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
			try
			{
				Users thisUser = _context.Users.Where(x => x.UserId == id).First();
			}
			catch
			{
				return RedirectToAction("RegisterUser");
			}

			

            return RedirectToAction("SearchPage", "SeamedInDB");
        }

        public async Task<IActionResult> SearchResults(List<string> source, List<string> scout, List<string> alignment, List<string> theme, List<string> technologyArea,
            List<string> landscape, List<string> country, List<string> state, List<string> city, List<string> stage, string dateAdded1st, string dateAdded2nd, string dateReviewed1st, string dateReviewed2nd)
        {
            List<List<string>> listOfLists = new List<List<string>>();

            listOfLists.Add(source);
            listOfLists.Add(scout);
            listOfLists.Add(alignment);
            listOfLists.Add(theme);
            listOfLists.Add(technologyArea);
            listOfLists.Add(landscape);
            listOfLists.Add(country);
            listOfLists.Add(state);
            listOfLists.Add(city);
            listOfLists.Add(stage);

            List<string> convertedList = _seamedInDal.ConvertsListsOfFormSelection(listOfLists);
            Startups foundStartups = await _seamedInDal.GetFilteredStartUps(convertedList);

			string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

			Users thisUser = _context.Users.Where(x => x.UserId == id).First();

			var rankedStartups = Ranking(foundStartups, thisUser);

			return View(rankedStartups);
		}

        public async Task<IActionResult> StartupProfile(string name)
		{
			string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

			AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

			Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

			ViewBag.AspUser = thisAspUser;

			ViewBag.User = thisUser;

			Startups startups = await _seamedInDal.GetStartups();

			var rankedStartups = Ranking(startups, thisUser).ToList();

			ViewBag.Startups = rankedStartups;

			Record s = await _seamedInDal.GetStartUpByName(name);

			return View(s);
		}

		public async Task<IActionResult> UserProfile(string name)
		{
			string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

			AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

			Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

			ViewBag.AspUser = thisAspUser;

			ViewBag.User = thisUser;

			Startups startups = await _seamedInDal.GetStartups();

			var rankedStartups = Ranking(startups, thisUser).ToList();

			ViewBag.Startups = rankedStartups;

			return View(thisUser);
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
				AspNetUsers thisUser = _context.AspNetUsers.Where(x => x.Id == u.UserId).First();
				thisUser.Roles = u.UserType;
				_context.Users.Add(u);
				_context.SaveChanges();
				return RedirectToAction("Index");
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
