using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class FavoritesModel
    {
        public int ApiId { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Themes { get; set; }
        public string TechnologyAreas { get; set; }
        public string Landscape { get; set; }
        public string StateProvince { get; set; }
    }

    public class FavoritesListVM
    {
        public List<Favorites> ListOfFavoriteStartUps { get; set; }

        public List<Record> ListofRecords { get; set; }
    }
}
