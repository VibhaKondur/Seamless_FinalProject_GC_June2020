using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    public class SeamedInDBController : Controller
    {
        private readonly SeamedInDal _seamedInDal;
        private readonly SeamedInDBContext _context;
        private readonly string _apiKey;

        public SeamedInDBController(SeamedInDBContext context, IConfiguration configuration)
        {
            _context = context;
            _seamedInDal = new SeamedInDal(configuration);
        }

        #region Views
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Testing()
        {
            return View();
        }

        #endregion


        #region CRUD Functions
        //Displays list of User's favorite StartUps
        [Authorize]
        public async Task<IActionResult> DisplayListOfFavoriteStartUps()
        {
            List<Users> usersFavoritesStartUps = new List<Users>();
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoritesOfUser = _context.Favorites.Where(x => x.UserId == id).ToList();
            usersFavoritesStartUps = await _seamedInDal.GetFavoriteStartUpsList(favoritesOfUser);
            return View(usersFavoritesStartUps);
        }



        //Adds a new favorite StartUp to the user's list of favorite StartUps
        [Authorize]
        public IActionResult AddAFavoriteStartUpToList(int id)
        {

            Favorites favorite = new Favorites
            {
                ApiId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            };


            if (_context.Favorites.Where(x => (x.ApiId == id) && (x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)).ToList().Count > 0)
            {
                return RedirectToAction("Favorites");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Favorites.Add(favorite);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return RedirectToAction("Favorites");
        }

        //Removes a new favorite StartUp from the user's list of favorite StartUps
        [Authorize]
        public IActionResult RemoveAFavoriteStartUpFromList(int id)
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Favorites found = _context.Favorites.FirstOrDefault(x => (x.ApiId == id) && (x.UserId == uid));
            if (found != null)
            {
                try
                {
                    _context.Favorites.Remove(found);
                    _context.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                }


            }
            return RedirectToAction("Favorites");
        }

        //Updates the user's list of Favorite StartUps
        [Authorize]
        public async Task<ActionResult> UpdateListOfFavoriteStartUps(int id, Users updatedListOfFavoriteStartUps)
        {
            if (id != updatedListOfFavoriteStartUps.Id || !ModelState.IsValid)
            {
                return BadRequest();
            }
            else
            {
                _context.Entry(updatedListOfFavoriteStartUps).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
        #endregion

        #region Search Result Methods
        public async Task<Startups> GetStartupsFromSelections(List<string> source, List<string> scout, List<string> alignment, List<string> theme, List<string> technologyArea,
            List<string> landscape, List<string> country, string dateAdded1st, string dateAdded2nd, string dateReviewed1st, string dateReviewed2nd)
        {
            List<List<string>> listOfLists = new List<List<string>>();
            listOfLists.Add(source);
            listOfLists.Add(scout);
            listOfLists.Add(alignment);
            listOfLists.Add(theme);
            listOfLists.Add(technologyArea);
            listOfLists.Add(landscape);
            listOfLists.Add(country);

            List<string> convertedList = _seamedInDal.ConvertsListsOfFormSelection(listOfLists);
            Startups foundStartups = await _seamedInDal.GetFilteredStartUps(convertedList);
            return foundStartups;
        }

        #endregion
    }
}
