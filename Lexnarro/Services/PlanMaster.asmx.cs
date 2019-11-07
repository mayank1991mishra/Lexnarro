using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for PlanMaster
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/PlanMaster.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service gets countries.</b></font>")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class PlanMaster : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        [WebMethod]
        public ReturnData GetPlans()
        {
            ReturnData rd = new ReturnData();

            try
            {
                var plans = (from s in db.Plan_Master
                             join c in db.Rate_Card on s.Plan_ID equals c.Plan_Id
                             where c.Status == "Active"
                             select new { s.Plan_ID, c.Rate_Id, s.Plan, c.Amount}).ToList();

                if (plans.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "Plans not found";
                    rd.Requestkey = "GetPlans";
                    return rd;
                }

                List<PlanList> PlanList = new List<PlanList>();

                for (int i = 0; i < plans.Count; i++)
                {
                    PlanList cm = new PlanList();
                    cm.Plan_ID = Convert.ToDecimal(plans[i].Plan_ID);
                    cm.Rate_Id = plans[i].Rate_Id;
                    cm.Plan = plans[i].Plan;
                    cm.Amount = plans[i].Amount;
                    PlanList.Add(cm);
                    rd.Plans = PlanList;
                }

                rd.Status = "Success";
                rd.Message = "Plans found";
                rd.Requestkey = "GetPlans";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetPlans";
                return rd;
            }
        }
        public class ReturnData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<PlanList> Plans { get; set; }
        }

        public class PlanList
        {
            public decimal Plan_ID { get; set; }
            public string Plan { get; set; }
            public decimal Rate_Id { get; set; }
            public decimal? Amount { get; set; }
        }
    }
}
