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

namespace GC_FinalProject_Seamless_June2020.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly SeamedInDal _seamedInDal = new SeamedInDal();

        private readonly IConfiguration configuration;

        public HomeController(IConfiguration configuration)
        {
            this.configuration = configuration;

            this._seamedInDal = new SeamedInDal(configuration);
        }
        

        public async Task<IActionResult> Index()
        {
            var s = _seamedInDal.GetAPIString();
            return View(s);
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
    }
}
