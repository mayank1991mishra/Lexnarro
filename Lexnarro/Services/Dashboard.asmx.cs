using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Script.Services;
using System.Web.Services;
using System.Collections.Generic;
using Lexnarro.Models;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for Dashboard
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/Dashboard.asmx",
          Description = "<font color='#a31515' size='3'><b>User Dashboard and training summary.</b></font>")]

    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Dashboard : System.Web.Services.WebService
    {
        SqlConnection con = null;
        SqlCommand cmd;
        private LaxNarroEntities db = new LaxNarroEntities();

        //public Dashboard()
        //{
        //    con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString);
        //}

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
        public DataReturn TrainingSummary(string emailID, string finYear)
        {
            DataReturn dr = new DataReturn();

            try
            {
                if (emailID == string.Empty)
                {
                    dr.categ = null;
                    dr.execCateg = null;
                    dr.Status = "Failure";
                    dr.Message = "User Id is required";
                    dr.Requestkey = "TrainingSummary";

                    return dr;
                }
                var v = db.User_Profile.Where(a => a.UserName.Equals(emailID)
                        && a.IsDeleted != "Deleted" &&
                        a.AccountConfirmed == "Yes").FirstOrDefault();

                if (v == null)
                {
                    dr.categ = null;
                    dr.execCateg = null;
                    dr.Status = "Failure";
                    dr.Message = "Account deleted or not confirmed";
                    dr.Requestkey = "TrainingSummary";

                    return dr;
                }

                decimal userid = v.ID;
                string financialYear = string.Empty;
                if (finYear != string.Empty)
                {
                    financialYear = finYear;
                }
                else
                {
                    financialYear = getFinancialYear();
                }


                List<MyCategory> totalCategories = new List<MyCategory>();

                //------Experimental------------
                var userTransactionData = db.User_Training_Status.Where(u => u.User_Id == userid && u.Financial_Year == financialYear).ToList();
                decimal? totalUnits = 0;
                //foreach (var row in userTransactionData)
                //{
                //    totalUnits += Convert.ToInt32(row.Units_Done);
                //}

                var GetUserState = db.User_Profile.Where(u => u.ID == userid).Select(u => u.StateEnrolled).FirstOrDefault();


                totalCategories = (from s in db.State_Category_With_Rule4_Mapping
                                   join c in db.Category_Master on s.CategoryID equals c.ID
                                   where s.StateID == GetUserState && s.Status == "Active"
                                   select new MyCategory
                                   {
                                       CategoryID = s.CategoryID,
                                       Category_Name = c.Name,
                                       ShortName = c.ShortName
                                   }).ToList();


                List<ExistingCategory> swe = new List<ExistingCategory>();
                List<SelectListItem> fy = new List<SelectListItem>();

                foreach (MyCategory d in totalCategories)
                {
                    var existedCompletedCategories = db.User_Training_Status
                        .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                        .Where(x => x.Category_Id == d.CategoryID
                         && x.Financial_Year == finYear && x.User_Id == userid).Select(x => x.Units_Done).Sum();

                    swe.Add(new ExistingCategory
                    {
                        Category_Id = d.CategoryID,
                        Units_Done = (existedCompletedCategories == null) ? 0 : existedCompletedCategories,
                        Category_Name = d.Category_Name,
                        Financial_Year = finYear,
                        Short_Name = d.ShortName
                    });                    

                    totalUnits = totalUnits + ((existedCompletedCategories == null) ? 0 : existedCompletedCategories);
                }

                fy = (from s in db.User_Training_Transaction
                      where s.User_Id == userid
                      orderby s.Date
                      select new SelectListItem
                      {
                          Text = s.Financial_Year,
                          Value = s.Financial_Year
                      }).Distinct().ToList();

                foreach (SelectListItem item in fy)
                {
                    if (item.Value == finYear)
                    {
                        item.Selected = true;
                        break;
                    }
                }

                //foreach (var d in totalCategories)
                //{
                //    var existedCompletedCategories = (from s in db.User_Training_Status
                //                                      join c in db.Category_Master on s.Category_Id equals c.ID
                //                                      where s.Category_Id == d.CategoryID && s.Financial_Year == financialYear
                //                                      && s.User_Id == userid
                //                                      select new ExistingCategory
                //                                      {
                //                                          Category_Id = d.CategoryID,
                //                                          Units_Done = (from od in db.User_Training_Status
                //                                                        where od.Category_Id == d.CategoryID
                //                                                        && s.Financial_Year == financialYear
                //                                                        && od.User_Id == userid
                //                                                        select od.Units_Done).Sum(),
                //                                          Financial_Year = s.Financial_Year,
                //                                          Category_Name = c.Name,
                //                                          Short_Name = c.ShortName
                //                                      }).ToList();

                //    if (existedCompletedCategories.Count > 0)
                //    {
                //        foreach (var e in existedCompletedCategories)
                //        {
                //            swe.Add(new ExistingCategory
                //            {
                //                Category_Id = e.Category_Id,
                //                Units_Done = e.Units_Done,
                //                Category_Name = e.Category_Name,
                //                Financial_Year = e.Financial_Year,
                //                Short_Name = e.Short_Name
                //            });                            

                //            fy = (from s in db.User_Training_Transaction
                //                  where s.User_Id == userid
                //                  orderby s.Date
                //                  select new SelectListItem
                //                  {
                //                      Text = s.Financial_Year,
                //                      Value = s.Financial_Year
                //                  }).Distinct().ToList();

                //            foreach (SelectListItem item in fy)
                //            {
                //                if (item.Value == finYear)
                //                {
                //                    item.Selected = true;
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //    else
                //    {
                //        swe.Add(new ExistingCategory
                //        {
                //            Category_Id = d.CategoryID,
                //            Units_Done = 0,
                //            Category_Name = d.Category_Name,
                //            Financial_Year = "N/A",
                //            Short_Name = d.ShortName
                //        });
                //    }
                //}


                dr.categ = totalCategories;
                dr.execCateg = swe;
                dr.FinancialYear = fy;
                dr.Status = "Success";
                dr.Message = "Data found";
                dr.Requestkey = "TrainingSummary";

                //------End of experiment-------

            }
            catch (Exception)
            {
                dr.categ = null;
                dr.execCateg = null;
                dr.Status = "Failure";
                dr.Message = "Something went wrong. Please try again after some time";
                dr.Requestkey = "TrainingSummary";
            }

            return dr;
        }

        private string getFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
                FinYear = CurYear + "-" + NexYear;
            else
                FinYear = PreYear + "-" + CurYear;

            return FinYear;
        }
    }

    public class DataReturn
    {
        public List<MyCategory> categ { get; set; }
        public List<ExistingCategory> execCateg { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public List<SelectListItem> FinancialYear { get; set; }
    }

    public class MyCategory
    {
        public decimal CategoryID { get; set; }
        public string ShortName { get; set; }
        public string Category_Name { get; set; }
    }

    public class ExistingCategory
    {
        public decimal? Category_Id { get; set; }
        public decimal? Units_Done { get; set; }
        public string Financial_Year { get; set; }
        public string Category_Name { get; set; }
        public string Short_Name { get; set; }
    }

    public class FinancialYear
    {
        public string Financial_Year { get; set; }
    }
}
