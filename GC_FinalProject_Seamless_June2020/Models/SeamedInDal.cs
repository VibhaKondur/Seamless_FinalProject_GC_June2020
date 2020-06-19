using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GC_FinalProject_Seamless_June2020.Models
{
    public class SeamedInDal
    {
        private readonly string _apiKey;

        public SeamedInDal(IConfiguration configuration)
        {
            _apiKey = configuration.GetSection("ApiKeys")["SeamedInAPI"];
        }

        public SeamedInDal()
        {
        }

        public HttpClient GetClient()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.airtable.com/v0/appFo187B73tuYhyg/");
            client.DefaultRequestHeaders.Add("authorization", $"Bearer {_apiKey}"); //There needs to be a space between Bearer and the api key
            return client;
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

        public async Task<Startups> GetStartups()
        {
            var client = GetClient();
            var response = await client.GetAsync($"Master%20List"); //The "%20" represents the space
            Startups startUpsFull = await response.Content.ReadAsAsync<Startups>();
            return startUpsFull;
        }

        public async Task<List<Record>> GetAllRecords()
        {
            var fullApiResponse = await GetStartups();
            List<Record> listOfRecords = new List<Record>();
            foreach (Record record in fullApiResponse.records)
            {
                listOfRecords.Add(record);
            }

            return listOfRecords;
        }

        public async Task<bool> DoesStartUpExist(string recordId)
        {
            var fullRecordList = await GetAllRecords();
            bool value = fullRecordList.Any(a => a.id == recordId);
            return value;
        }

        public async Task<Record> GetStartUpById(string recordId) //**************
        {
            var fullRecordList = await GetAllRecords();

            return fullRecordList.FirstOrDefault(a => a.id == recordId);

        }

        public async Task<bool> DoesStartUpExistFromName(string startUpName)
        {
            var fullRecordList = await GetAllRecords();
            bool value = fullRecordList.Any(a => a.fields.CompanyName == startUpName);
            return value;
        }

        public async Task<Record> GetStartUpByName(string startUpName) //**************
        {
            var fullRecordList = await GetAllRecords();

            return fullRecordList.FirstOrDefault(a => a.fields.CompanyName == startUpName);
        }


        //-------------------------------------------------------------------------------------------------------
        #region Search Filter Methods
        public async Task<Startups> GetFilteredStartUps(List<string> searchParameters)
        {
            var client = GetClient();
            string searchEndPoint = GetFilteredEndPointFromList(searchParameters);

            var response = await client.GetAsync(searchEndPoint); //The "%20" represents the space
            Startups specificRecords = await response.Content.ReadAsAsync<Startups>();
            return specificRecords;
        }

        public async Task<Startups> GetFilteredStartUps(string searchParameters)
        {
            var client = GetClient();
            string searchEndPoint = GetFilterEndPointFromString(searchParameters);

            var response = await client.GetAsync(searchEndPoint); //The "%20" represents the space
            Startups specificRecords = await response.Content.ReadAsAsync<Startups>();
            return specificRecords;
        }

        public string GetFilteredEndPointFromList(List<string> searchParameters)
        {

            if (!searchParameters.Any())
            {
                return $"Master%20List";
            }
            else
            {
                string finalFormula = "Master%20List?filterByFormula=AND(";
                StringBuilder formulaSecondHalf = new StringBuilder();

                int loopCount = 0;

                foreach (string filterSelectionString in searchParameters)
                {
                    loopCount++;
                    formulaSecondHalf.Append(filterSelectionString);
                    if (searchParameters.Count != loopCount)
                    {
                        formulaSecondHalf = formulaSecondHalf.Append(",");
                    }
                    if (searchParameters.Count == loopCount)
                    {
                        formulaSecondHalf = formulaSecondHalf.Append(")");
                    }
                }
                string encoded2ndHalf = HttpUtility.UrlEncode(formulaSecondHalf.ToString());
                finalFormula += encoded2ndHalf;

                return finalFormula;
            }
        }

        public string GetFilterEndPointFromString(string searchTerm)
        {
            string finalFormula = "Master%20List?filterByFormula=";
            StringBuilder finalParameter = new StringBuilder();

            finalParameter.Append($"AND(OR(FIND('{searchTerm}', {{Alignment}}), FIND('{searchTerm}', {{Technology Areas}}), FIND('{searchTerm}', {{Landscape}})," +
                $" FIND('{searchTerm}', {{Company Name}}), FIND('{searchTerm}', {{Theme(s)}})))");

            string encoded2ndHalf = HttpUtility.UrlEncode(finalParameter.ToString());
            finalFormula += encoded2ndHalf;

            return finalFormula;
        }

        public List<string> ConvertsListsOfFormSelection(List<List<string>> ListOfLists)
        {
            List<string> finalSelectionList = new List<string>();

            foreach (List<string> listToBeConverted in ListOfLists)
            {
                if (listToBeConverted.Any())
                {
                    StringBuilder innerString = new StringBuilder();
                    int loopCount = 0;

                    foreach (string parameter in listToBeConverted)
                    {
                        loopCount++;
                        innerString.Append(parameter);
                        if (listToBeConverted.Count != loopCount)
                        {
                            innerString = innerString.Append(",");
                        }
                    }

                    string convertedString = $"OR({innerString})";

                    finalSelectionList.Add(convertedString);
                }
            }
            return finalSelectionList;
        }
        #endregion

    }
}
