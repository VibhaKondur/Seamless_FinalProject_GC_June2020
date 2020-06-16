using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    public class SeamedInDBController : Controller
    {
        private readonly SeamedInDal _seamedInDal = new SeamedInDal();
        private readonly SeamedInDBContext _context;
        private readonly string _apiKey;

        public SeamedInDBController (SeamedInDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

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
            if (ModelState.IsValid)
            {
                _context.Favorites.Add(favorite);
                _context.SaveChanges();
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
                _context.Favorites.Remove(found);
                _context.SaveChanges();
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



    }
}
