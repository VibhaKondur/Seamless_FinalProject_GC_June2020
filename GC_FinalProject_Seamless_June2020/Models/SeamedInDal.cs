using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class SeamedInDal
    {
        private readonly string _apiKey;

        public SeamedInDal()
        {
        }

        public SeamedInDal(IConfiguration configuration)
        {
            _apiKey = configuration.GetSection("ApiKeys")["SeamedInAPI"];
        }

        public Startups GetAPIString()
        {
            string url = $"https://api.airtable.com/v0/appFo187B73tuYhyg/Master%20List?api_key={_apiKey}";
            HttpWebRequest request = WebRequest.CreateHttp(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader rd = new StreamReader(response.GetResponseStream());
            string output = rd.ReadToEnd();

            Startups startups = JsonConvert.DeserializeObject<Startups>(output);
            return startups;
        }

     


    }
}
