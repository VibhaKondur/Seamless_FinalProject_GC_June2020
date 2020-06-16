using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GC_FinalProject_Seamless_June2020.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    public class SeamedInController : Controller
    {

        private readonly SeamedInDal _seamedInDal;


        public SeamedInController(IConfiguration configuration)
        {

            _seamedInDal = new SeamedInDal(configuration);
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> Testing()
        {
            List<string> filterSelectionList = new List<string>() { "{Scout} = 'Mark'", "{Country} = 'China'" };
            
            Startups passedIn = await _seamedInDal.GetFilteredStartUps(filterSelectionList);
            return View(passedIn);
        }
    }
}
