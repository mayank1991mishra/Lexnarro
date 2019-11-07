
using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Services;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for States
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/States.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service gets user enrolled state.</b></font>")]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class States : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        [WebMethod]
        public ReturnAllStates GetAllStates(decimal countryId)
        {
            ReturnAllStates rd = new ReturnAllStates();

            try
            {
                var states = (from s in db.State_Master
                              where s.Country_ID == countryId
                                  select new { s.ID, s.Name, s.ShortName }).ToList();

                if (states.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "States not found";
                    rd.Requestkey = "GetAllStates";
                    rd.States = null;
                    return rd;
                }

                List<StateMaster> stateList = new List<StateMaster>();

                for (int i = 0; i < states.Count; i++)
                {
                    StateMaster cm = new StateMaster();
                    cm.Id = Convert.ToDecimal(states[i].ID);
                    cm.Name = states[i].Name;
                    cm.ShortName = states[i].ShortName;
                    stateList.Add(cm);
                    rd.States = stateList;
                }

                rd.Status = "Success";
                rd.Message = "States found";
                rd.Requestkey = "GetAllStates";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetAllStates";
                rd.States = null;
                return rd;
            }
        }


        [WebMethod]
        public ReturnAllStates GetUserEnrolledState(string User_ID)
        {
            ReturnAllStates rd = new ReturnAllStates();

            try
            {
                decimal id = Convert.ToDecimal(User_ID);

                var states = (from s in db.User_Profile
                              join c in db.State_Master on s.StateEnrolled equals c.ID
                              where s.ID == id
                              select new { c.ID, c.Name, c.ShortName }).ToList();

                if (states.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "State not found";
                    rd.Requestkey = "GetUserEnrolledState";
                    rd.States = null;
                    return rd;
                }

                List<StateMaster> stateList = new List<StateMaster>();

                for (int i = 0; i < states.Count; i++)
                {
                    StateMaster cm = new StateMaster();
                    cm.Id = Convert.ToDecimal(states[i].ID);
                    cm.Name = states[i].Name;
                    cm.ShortName = states[i].ShortName;
                    stateList.Add(cm);
                    rd.States = stateList;
                }

                rd.Status = "Success";
                rd.Message = "State found";
                rd.Requestkey = "GetUserEnrolledState";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetUserEnrolledState";
                return rd;
            }
        }
    }

    public class ReturnAllStates
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public List<StateMaster> States { get; set; }
    }

    public class StateMaster
    {
        public decimal Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
