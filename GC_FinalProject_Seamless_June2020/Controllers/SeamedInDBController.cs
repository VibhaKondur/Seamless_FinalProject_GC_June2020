using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public async Task<IActionResult> SearchPage()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;
            SearchPageVM searchPageVM = await GetStartUpColumnCategoryValues();

            return View(searchPageVM);
        }

        public async Task<IActionResult> AboutUs()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;
            return View();
        }

        #endregion


        #region CRUD Functions
        //Displays list of User's favorite StartUps
        [Authorize]
        public async Task<IActionResult> DisplayListOfFavoriteStartUps()
        {
            var model = new FavoritesListVM();

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoritesOfUser = await _context.Favorites.Where(x => x.UserId == id).ToListAsync();

            foreach (Favorites favorites in favoritesOfUser)
            {
                var favorite = await _seamedInDal.GetStartUpById(favorites.ApiId.ToString());

                FavoritesModel favoriteModel = new FavoritesModel();
                favoriteModel.ApiId = int.Parse(favorite.id);
                favoriteModel.CompanyName = favorite.fields.CompanyName;
                favoriteModel.City = favorite.fields.City;
                favoriteModel.Country = favorite.fields.Country;
                favoriteModel.Themes = favorite.fields.Themes;
                favoriteModel.TechnologyAreas = favorite.fields.TechnologyAreas;
                favoriteModel.Landscape = favorite.fields.Landscape;
                favoriteModel.StateProvince = favorite.fields.StateProvince;

                model.ListOfFavoriteStartUps.Add(favoriteModel);
            }

            return View(model);
        }

        //favorite action method to find list of user's favorites
        public async Task<IActionResult> Favorites()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            AspNetUsers thisAspUser = _context.AspNetUsers.Where(x => x.Id == uid).First();

            Users thisUser = _context.Users.Where(x => x.UserId == uid).First();

            ViewBag.AspUser = thisAspUser;

            ViewBag.User = thisUser;

            Startups startups = await _seamedInDal.GetStartups();

            var rankedStartups = Ranking(startups, thisUser).ToList();

            ViewBag.Startups = rankedStartups;

            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            List<Favorites> f = _context.Favorites.Where(x => x.UserId == id).ToList();

            List<Record> records = new List<Record>();

            foreach (Favorites favorites in f)
            {
                Record record = await _seamedInDal.GetStartUpById(favorites.ApiId);

                records.Add(record);
            }

            return View(records);
        }

        //error handling for user not being able to add duplicate startUps to their favorites list
        private bool CheckIfEntryExists(string id)
        {
            var listOfFavorites = _context.Favorites.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();

            bool exists = false;

            ViewBag.id = exists;

            foreach (Favorites favor in listOfFavorites)
            {
                if (favor.ApiId == id)
                {
                    return true;
                }
            }

            return exists;
        }

        //Adds a new favorite StartUp to the user's list of favorite StartUps
        [Authorize]
        public IActionResult AddAFavoriteStartUpToList(string id)
        {

            Favorites favorite = new Favorites
 
            {
                ApiId = id,
                UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value
            };
            
            var listOfFavorites = _context.Favorites.Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value).ToList();

            if (!CheckIfEntryExists(id))
            {
                if (ModelState.IsValid)
                {
                    _context.Favorites.Add(favorite);
                    _context.SaveChanges();
                }

            }      
            
            if (listOfFavorites.Count > 0)
            {
                return RedirectToAction("Favorites");
            }

            return RedirectToAction("Favorites");
        }


        //Removes a favorite StartUp from the user's list of favorite StartUps
        [Authorize]
        public IActionResult RemoveAFavoriteStartUpFromList(string id, string subject)
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

        //Clears all favorites view if user needs
        [Authorize]
        public IActionResult ClearFavoritesView()
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Favorites> clearFavorites = _context.Favorites.Where(x => x.UserId == uid).ToList();

            if(clearFavorites != null)
            {
                _context.Favorites.RemoveRange(clearFavorites);
                _context.SaveChanges();
            }
            return RedirectToAction("SearchPage");
        }

        //Adds / edits comments
        [Authorize]
        public IActionResult AddComment(string id, string comment)
        {
            string uid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Favorites found = _context.Favorites.FirstOrDefault(x => (x.ApiId == id) && (x.UserId == uid));
            var foundIt = _context.Favorites.Find(id);
            
            foundIt.CommentSection = comment;

            _context.Entry(foundIt).State = EntityState.Modified;
            _context.Update(foundIt);
            _context.SaveChanges();

            return RedirectToAction("Favorites");
        }

        #endregion

        #region Search Result Methods

        public async Task<SearchPageVM> GetStartUpColumnCategoryValues()
        {
            SearchPageVM searchPageVM = new SearchPageVM();
            Startups currentApiData = await _seamedInDal.GetStartups();

            foreach (Record startUp in currentApiData.records)
            {
                AddToRespectiveList(startUp.fields.Source, searchPageVM.sourcesList);
                AddToRespectiveList(startUp.fields.Scout, searchPageVM.scoutsList);
                AddToRespectiveList(startUp.fields.Landscape, searchPageVM.landscapesList);
                AddToRespectiveList(startUp.fields.Country, searchPageVM.countriesList);
                AddToRespectiveList(startUp.fields.StateProvince, searchPageVM.statesList);
                AddToRespectiveList(startUp.fields.City, searchPageVM.citiesList);
                AddToRespectiveList(startUp.fields.Stage, searchPageVM.stagesList);

                AddToRespectiveListMultiValue(startUp.fields.Alignment, searchPageVM.alignmentsList);
                AddToRespectiveListMultiValue(startUp.fields.Themes, searchPageVM.themesList);
                AddToRespectiveListMultiValue(startUp.fields.TechnologyAreas, searchPageVM.technologyAreasList);
            }

            return searchPageVM;
        }

        public void AddToRespectiveList(string checkedColumnValue, List<string> respectiveColumnList)
        {
            if (!(string.IsNullOrEmpty(checkedColumnValue)) && (!respectiveColumnList.Contains(checkedColumnValue)))
            {
                respectiveColumnList.Add(checkedColumnValue);
                respectiveColumnList.Sort();
            }

        }

        public void AddToRespectiveListMultiValue(string checkedColumnValue, List<string> respectiveColumnList)
        {
            if (!(string.IsNullOrEmpty(checkedColumnValue)) && (!respectiveColumnList.Contains(checkedColumnValue)))
            {
                if (checkedColumnValue.Contains(","))
                {
                    List<String> splitStringList = checkedColumnValue.Split(",").ToList();
                    foreach (string splitString in splitStringList)
                    {
                        if (!respectiveColumnList.Contains(splitString.Trim()))
                        {
                            respectiveColumnList.Add(splitString.Trim().Replace("\"", ""));
                        }
                    }
                }
                else
                {
                    respectiveColumnList.Add(checkedColumnValue);
                }
            }
            respectiveColumnList.Sort();
        }
        #endregion


        #region Ranking Method From HomeController
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
        #endregion
    }


}
