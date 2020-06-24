using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class SearchResultsVM
    {
        public string SearchString { get; set; }

        public List<Users> UsersList { get; set; }

        public List<Tuple<int, Record>> ResultsList { get; set; }
    }
}
