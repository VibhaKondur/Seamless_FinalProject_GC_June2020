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
            SearchPageVM searchPageVM = await GetStartUpColumnCategoryValues();

            return View(searchPageVM);
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

                else
                {

                    FavoritesListVM fVM = new FavoritesListVM();
                    fVM.ErrorMessage = "Duplicate found. Please return to search results to add another favorite";
                    return View(fVM);
                    
                }
            }      
            
            if (listOfFavorites.Count > 0)
            {
                return RedirectToAction("Favorites");
            }

            return RedirectToAction("Favorites");
        }


        //Removes a new favorite StartUp from the user's list of favorite StartUps
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
                            respectiveColumnList.Add(splitString.Trim());
                        }
                    }
                }
                else
                {
                    respectiveColumnList.Add(checkedColumnValue);
                }
            }
            respectiveColumnList.Sort();

            /*foreach(string columnValues in respectiveColumnList)
            {
                string removedSpecialChars = Regex.Replace(columnValues, "[^A-Za-z0-9 -]", "");
                respectiveColumnList.Select(a => a.Replace(removedSpecialChars));
            }*/
        }
        #endregion
    }


}
