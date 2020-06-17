using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class SearchPageVM
    {
        public List<string> sourcesList { get; set; }
        public List<string> scoutsList { get; set; }
        public List<string> alignmentsList { get; set; }
        public List<string> themesList { get; set; }
        public List<string> technologyAreasList { get; set; }
        public List<string> landscapesList { get; set; }
        public List<string> countriesList { get; set; }
        public List<string> statesList { get; set; }
        public List<string> citiesList { get; set; }
        public List<string> stagesList { get; set; }

        public SearchPageVM()
        {
            this.sourcesList =  new List<string>();
            this.scoutsList = new List<string>();
            this.alignmentsList = new List<string>();
            this.themesList = new List<string>();
            this.technologyAreasList = new List<string>();
            this.landscapesList = new List<string>();
            this.countriesList = new List<string>();
            this.statesList = new List<string>();
            this.citiesList = new List<string>();
            this.stagesList = new List<string>();
        }
    }
}
