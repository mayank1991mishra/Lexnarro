using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for Country
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/Country.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service gets countries.</b></font>")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Country : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        [WebMethod]
        public ReturnData GetAllCountries()
        {
            ReturnData rd = new ReturnData();

            try
            {
                var country = (from s in db.Country_Master
                              select new { s.ID, s.Name }).ToList();

                if (country.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "Countries not found";
                    rd.Requestkey = "GetAllCountries";
                    return rd;
                }

                List<CountryMaster> countryList = new List<CountryMaster>();

                for (int i = 0; i < country.Count; i++)
                {
                    CountryMaster cm = new CountryMaster();
                    cm.ID = Convert.ToDecimal(country[i].ID);
                    cm.Name = country[i].Name;
                    countryList.Add(cm);
                    rd.Country = countryList;
                }

                rd.Status = "Success";
                rd.Message = "Countries found";
                rd.Requestkey = "GetAllCountries";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetAllCountries";
                return rd;
            }
        }
        public class ReturnData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<CountryMaster> Country { get; set; }
        }

        public class CountryMaster
        {
            public decimal ID { get; set; }
            public string Name { get; set; }
        }
    }    
}
