using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class Startups
    {
        public Record[] records { get; set; }
    }

    public class Record
    {
        public string id { get; set; }
        public Fields fields { get; set; }
        public DateTime createdTime { get; set; }
    }

    public class Fields
    {
        [JsonProperty(PropertyName = "Company Name")]
        public string CompanyName { get; set; }

        [JsonProperty(PropertyName = "Date Added")]
        public string DateAdded { get; set; }

        public string Scout { get; set; }

        public string Source { get; set; }

        [JsonProperty(PropertyName = "Company Website")]
        public string CompanyWebsite { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        [JsonProperty(PropertyName = "Two Line Company Summary")]
        public string TwoLineCompanySummary { get; set; }

        public string Alignment { get; set; }

        [JsonProperty(PropertyName = "Theme(s)")]
        public string Themes { get; set; }

        public string Uniqueness { get; set; }

        public string Team { get; set; }

        public string Raised { get; set; }

        [JsonProperty(PropertyName = "Review Date")]
        public string ReviewDate { get; set; }

        [JsonProperty(PropertyName = "Technology Areas")]
        public string TechnologyAreas { get; set; }

        public string Landscape { get; set; }
        public string Stage { get; set; }

        [JsonProperty(PropertyName = "State/Province")]
        public string StateProvince { get; set; }
    }

}
