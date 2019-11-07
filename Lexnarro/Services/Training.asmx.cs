using Lexnarro.Models;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for Training
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/Training.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service creates/edit user training.</b></font>")]

    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class Training : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();
        private SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["LaxNarroConnectionString"].ConnectionString);
        private SqlCommand cmd = new SqlCommand();
        private SqlDataAdapter sda = new SqlDataAdapter();
        private DataTable dt = new DataTable();


        private string GetFinancialYear()
        {
            int CurrentYear = DateTime.Today.Year;
            int PreviousYear = DateTime.Today.Year - 1;
            int NextYear = DateTime.Today.Year + 1;
            string PreYear = PreviousYear.ToString();
            string NexYear = NextYear.ToString();
            string CurYear = CurrentYear.ToString();
            string FinYear = null;

            if (DateTime.Today.Month > 3)
            {
                FinYear = CurYear + "-" + NexYear;
            }
            else
            {
                FinYear = PreYear + "-" + CurYear;
            }

            return FinYear;
        }

        [WebMethod]
        public ReturnData GetTraining(string email, string finYear)
        {
            ReturnData rd = new ReturnData();
            try
            {
                if (email == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "User name is required";
                    rd.Requestkey = "GetTraining";
                    rd.UserTraining = null;
                    return rd;
                }               

                var user_Training_Transaction = db.User_Training_Status.Include(u => u.User_Training_Transaction)
                    .Include(u => u.Activity_Master).Include(u => u.Category_Master).Include(u => u.User_Profile)
               .Where(u => u.User_Profile.EmailAddress == email && u.Financial_Year == finYear && u.Units_Done != 0).ToList();

                if (user_Training_Transaction.Count > 0)
                {
                    rd.Status = "Success";
                    rd.Message = "Training data found";
                    rd.Requestkey = "GetTraining";
                    List<UserTrainingTransaction> sd = new List<UserTrainingTransaction>();
                    List<SelectListItem> fy = new List<SelectListItem>();
                    foreach (var row in user_Training_Transaction)
                    {
                        UserTrainingTransaction training = new UserTrainingTransaction();
                        training.ActivityId = row.Activity_Id;//Convert.ToDecimal(row["Activity_Id"].ToString());
                        training.ActivityName = row.Activity_Master.Name;

                        if (row.SubActivity_Id != null)
                        {
                            training.SubActivityId = row.SubActivity_Id;
                            training.SubActivityName = row.Sub_Activity_Master.Name;
                        }
                        else
                        {
                            training.SubActivityId = null;
                            training.SubActivityName = null;
                        }


                        //if (row.User_Training_Transaction.UploadedFile != null)
                        //{
                        //    //Save the Byte Array as File.
                        //    string filePath = "~/Reports/" + row.User_Training_Transaction.UploadedFileName;
                        //    File.WriteAllBytes(Server.MapPath(filePath), row.User_Training_Transaction.UploadedFile);

                        //    training.ImageLink = "https://lexnarro.com.au/Reports/" + row.User_Training_Transaction.UploadedFileName;
                        //    training.FileName = row.User_Training_Transaction.UploadedFileName;
                        //}


                        training.CategoryId = row.Category_Id;
                        training.CategoryName = row.Category_Master.Name;
                        training.FirstName = row.User_Profile.FirstName;
                        training.Id = row.Id;
                        training.User_Id = row.User_Id;
                        training.Date = row.User_Training_Transaction.Date;
                        training.StateId = row.User_Training_Transaction.State_Id;
                        training.StateName = row.User_Training_Transaction.State_Master.Name;
                        training.TransactionId = row.Training_Transaction_ID;
                        training.Hours = row.User_Training_Transaction.Hours;
                        training.Units = row.Units_Done;
                        training.Has_been_Forwarded = row.User_Training_Transaction.Has_been_Forwarded;
                        training.Forwardable = row.User_Training_Transaction.Forwardable;
                        training.Provider = row.User_Training_Transaction.Provider;
                        training.Financial_Year = row.User_Training_Transaction.Financial_Year;
                        training.Descrption = row.User_Training_Transaction.Descrption;
                        training.Your_Role = row.User_Training_Transaction.Your_Role;
                        sd.Add(training);
                        rd.UserTraining = sd;                        
                    }

                    var userId = user_Training_Transaction.Select(x => x.User_Id).First();

                    fy = (from s in user_Training_Transaction
                          where s.User_Id == userId
                          orderby s.User_Training_Transaction.Date
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

                    rd.FinancialYear = fy;

                    return rd;
                }
                else
                {
                    rd.Status = "Failure";
                    rd.Message = "No data found for this financial year.";
                    rd.Requestkey = "GetTraining";
                    rd.UserTraining = null;
                    rd.FinancialYear = null;
                    return rd;
                }
            }
            catch (Exception er)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetTraining";
                rd.UserTraining = null;
                rd.FinancialYear = null;
                return rd;
            }
        }

        [WebMethod]
        public ReturnData DetailTraining(string id)
        {
            ReturnData rd = new ReturnData();

            try
            {
                decimal recordID = Convert.ToDecimal(id);

                if (id == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "No Record found";
                    rd.Requestkey = "DetailTraining";
                    rd.UserTraining = null;
                    return rd;
                }

                IEnumerable<User_Training_Status> utt = db.User_Training_Status.Include(x=>x.User_Training_Transaction).Where(x => x.Id == recordID).ToList();

                if (utt.Count() > 0)
                {
                    rd.Status = "Success";
                    rd.Message = "Training data found";
                    rd.Requestkey = "GetTraining";
                    List<UserTrainingTransaction> sd = new List<UserTrainingTransaction>();
                    foreach (var row in utt)
                    {
                        UserTrainingTransaction training = new UserTrainingTransaction();
                        training.ActivityId = row.Activity_Id;

                        training.ActivityName = row.Activity_Master.Name;

                        if (row.SubActivity_Id != null)
                        {
                            training.SubActivityId = row.SubActivity_Id;
                            training.SubActivityName = row.User_Training_Transaction.Sub_Activity_Master.Name;
                        }
                        else
                        {
                            training.SubActivityId = null;
                            training.SubActivityName = null;
                        }



                        //----------------------------------------------File Upload---------------------------------------
                        if (row.User_Training_Transaction.UploadedFile != null)
                        {
                            //string imageArray = Convert.ToBase64String(row.User_Training_Transaction.UploadedFile);

                            //int fileSize = imageArray.Length;

                            //if (fileSize > (1000000))
                            //{
                            //    rd.Status = "Failure";
                            //    rd.Message = "File size limit is 1 MB";
                            //    rd.Requestkey = "CreateTraining";
                            //    return rd;
                            //}

                            //Save the Byte Array as File.
                            string filePath = "~/Reports/" + row.User_Training_Transaction.UploadedFileName;
                            File.WriteAllBytes(Server.MapPath(filePath), row.User_Training_Transaction.UploadedFile);

                            training.ImageLink = "https://lexnarro.com.au/Reports/" + row.User_Training_Transaction.UploadedFileName;
                            training.FileName = row.User_Training_Transaction.UploadedFileName;
                        }                        
                        //-------------------------------------------------------------------------------------------------



                        var unitsDone = row.Units_Done;


                        training.CategoryId = row.Category_Id;
                        training.TransactionId = row.Training_Transaction_ID;
                        training.CategoryName = row.User_Training_Transaction.Category_Master.Name;
                        training.FirstName = row.User_Training_Transaction.User_Profile.FirstName;
                        training.Id = row.Id;
                        training.User_Id = row.User_Id;
                        training.Date = row.User_Training_Transaction.Date;
                        training.StateId = row.User_Training_Transaction.State_Id;
                        training.Units = unitsDone;
                        training.StateName = row.User_Training_Transaction.State_Master.Name;
                        training.Hours = row.User_Training_Transaction.Hours;
                        training.Has_been_Forwarded = row.User_Training_Transaction.Has_been_Forwarded;
                        training.Forwardable = row.User_Training_Transaction.Forwardable;
                        training.Provider = row.User_Training_Transaction.Provider;
                        training.Financial_Year = row.Financial_Year;
                        training.Descrption = row.User_Training_Transaction.Descrption;
                        training.Your_Role = row.User_Training_Transaction.Your_Role;
                        sd.Add(training);
                        rd.UserTraining = sd;
                    }
                    return rd;
                }
                else
                {
                    rd.Status = "Failure";
                    rd.Message = "No Data Found";
                    rd.Requestkey = "DetailTraining";
                    rd.UserTraining = null;
                    return rd;
                }
            }
            catch (Exception y)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time." + y.ToString();
                rd.Requestkey = "DetailTraining";
                rd.UserTraining = null;
                return rd;
            }
        }

        [WebMethod]
        public ReturnCategoryList GetCategories(string stateId)
        {
            ReturnCategoryList rd = new ReturnCategoryList();

            try
            {
                if (stateId == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "State Id is required for categories";
                    rd.Requestkey = "GetCategories";
                    rd.UserCategories = null;
                    return rd;
                }

                decimal stateid = Convert.ToDecimal(stateId);

                var categories = (from s in db.State_Category_With_Rule4_Mapping
                                  join c in db.Category_Master on s.CategoryID equals c.ID
                                  where s.StateID == stateid
                                  select new { c.ID, c.Name }).ToList();

                if (categories.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "Categories not found";
                    rd.Requestkey = "GetCategories";
                    rd.UserCategories = null;
                    return rd;
                }

                List<Category_Master> categoryList = new List<Category_Master>();

                for (int i = 0; i < categories.Count; i++)
                {
                    Category_Master cm = new Category_Master();
                    cm.ID = Convert.ToDecimal(categories[i].ID);
                    cm.Name = categories[i].Name;
                    categoryList.Add(cm);
                    rd.UserCategories = categoryList;
                }

                rd.Status = "Success";
                rd.Message = "Categories list found";
                rd.Requestkey = "GetCategories";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetCategories";
                rd.UserCategories = null;
                return rd;
            }
        }

        [WebMethod]
        public ReturnActivityList GetActivities(string stateId)
        {
            ReturnActivityList rd = new ReturnActivityList();

            try
            {
                decimal stateid = Convert.ToDecimal(stateId);

                if (stateId == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "State Id is required for activities";
                    rd.Requestkey = "GetActivities";
                    rd.UserActivities = null;
                    return rd;
                }

                var activities = (from s in db.State_Activity_Mapping
                                  join c in db.Activity_Master on s.ActivityID equals c.ID
                                  where s.StateID == stateid
                                  select new { c.ID, c.Name }).ToList();

                if (activities.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "Activities not found";
                    rd.Requestkey = "GetActivities";
                    rd.UserActivities = null;
                    return rd;
                }

                List<Activity_Master> activityList = new List<Activity_Master>();

                for (int i = 0; i < activities.Count; i++)
                {
                    Activity_Master cm = new Activity_Master();
                    cm.ID = Convert.ToDecimal(activities[i].ID);
                    cm.Name = activities[i].Name;
                    activityList.Add(cm);
                    rd.UserActivities = activityList;
                }

                rd.Status = "Success";
                rd.Message = "Activity list found";
                rd.Requestkey = "GetActivities";
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time.";
                rd.Requestkey = "GetActivities";
                rd.UserActivities = null;
                return rd;
            }
        }

        [WebMethod]
        public ReturnSubActivityList GetSubActivities(string stateId, string activityId)
        {
            ReturnSubActivityList rd = new ReturnSubActivityList();
            List<Sub_Activity_Master> subActivityList = new List<Sub_Activity_Master>();

            decimal stateID = Convert.ToDecimal(stateId);
            decimal activityID = Convert.ToDecimal(activityId);

            try
            {
                if (stateId == string.Empty || activityId == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "State Id and activity id is required for activities";
                    rd.Requestkey = "GetSubActivities";
                    Sub_Activity_Master cm = new Sub_Activity_Master
                    {
                        ID = "NULL",
                        Name = "N/A",
                        ShortName = "N/A",
                        StateID = stateID,
                        Activity_ID = activityID
                    };
                    subActivityList.Add(cm);
                    rd.UserSubActivities = subActivityList;
                    return rd;
                }

                List<Lexnarro.Models.Sub_Activity_Master> subActivity = (from s in db.StateActivitySubActivityWithRule1
                                                                         join c in db.Sub_Activity_Master on s.SubActivityID equals c.ID
                                                                         join x in db.Activity_Master on s.ActivityID equals x.ID
                                                                         where s.StateID == stateID && s.ActivityID == activityID
                                                                         select c).ToList();

                if (subActivity.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "SubActivities not found";
                    rd.Requestkey = "GetSubActivities";
                    Sub_Activity_Master cm = new Sub_Activity_Master
                    {
                        ID = "NULL",
                        Name = "N/A",
                        ShortName = "N/A",
                        StateID = stateID,
                        Activity_ID = activityID
                    };
                    subActivityList.Add(cm);
                    rd.UserSubActivities = subActivityList;
                    return rd;
                }

                rd.Status = "Success";
                rd.Message = "SubActivities found";
                rd.Requestkey = "GetSubActivities";
                for (int i = 0; i < subActivity.Count; i++)
                {
                    Sub_Activity_Master cm = new Sub_Activity_Master
                    {
                        ID = subActivity[i].ID.ToString(),
                        Name = subActivity[i].Name,
                        ShortName = subActivity[i].ShortName,
                        Activity_ID = subActivity[i].Activity_ID,
                        StateID = subActivity[i].StateID
                    };
                    subActivityList.Add(cm);
                    rd.UserSubActivities = subActivityList;
                }
                return rd;
            }
            catch (Exception)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please contact administrator.";
                rd.Requestkey = "GetSubActivities";
                Sub_Activity_Master cm = new Sub_Activity_Master
                {
                    ID = "NULL",
                    Name = "N/A",
                    ShortName = "N/A",
                    StateID = stateID,
                    Activity_ID = activityID
                };
                subActivityList.Add(cm);
                rd.UserSubActivities = subActivityList;
                return rd;
            }
        }

        [WebMethod]
        public ReturnData CreateTraining(string User_Id, string Date, string State_Id, string Category_Id, string Activity_Id,
            string SubActivity_Id, string Hours, string Provider, string Your_Role, string Descrption, string FileName, string File)
        {
            ReturnData rd = new ReturnData();
            User_Training_Transaction user_Training_Transaction = new User_Training_Transaction();
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    if (User_Id == string.Empty && Date == string.Empty && State_Id == string.Empty && Category_Id == string.Empty
                        && Activity_Id == string.Empty && Hours == string.Empty && Provider == string.Empty)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Information missing";
                        rd.Requestkey = "CreateTraining";
                        return rd;
                    }

                    decimal userId = Convert.ToDecimal(User_Id);
                    decimal stateId = Convert.ToDecimal(State_Id);
                    decimal categoryId = Convert.ToDecimal(Category_Id);
                    decimal activityId = Convert.ToDecimal(Activity_Id);
                    DateTime date = Convert.ToDateTime(Date);
                    decimal? subActivityId;


                    //-----------Checking for no of records and demo account-----------

                    var demoRecords = (from a in db.User_Transaction_Master
                                       join b in db.User_Profile on a.User_ID equals b.ID
                                       join c in db.User_Training_Transaction on a.User_ID equals c.User_Id
                                       join d in db.Plan_Master on a.PlanID equals d.Plan_ID
                                       where a.User_ID == userId && d.Plan == "Demo"
                                       select c);

                    var noOfRecords = demoRecords.Count();

                    if (noOfRecords == 5)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Demo users can only save 5 records.";
                        rd.Requestkey = "CreateTraining";
                        return rd;
                    }
                    //-----------------------------------------------------------------



                    var categories = (from s in db.State_Category_With_Rule4_Mapping
                                      join c in db.Category_Master on s.CategoryID equals c.ID
                                      where s.StateID == stateId && c.ID == categoryId
                                      select new { c.ID, c.Name }).ToList();

                    if (categories.Count <= 0)
                    {
                        rd.Status = "Failure";
                        rd.Message = "This category does not belong to the state enrolled.";
                        rd.Requestkey = "CreateTraining";
                        return rd;
                    }


                    if (SubActivity_Id != string.Empty)
                        subActivityId = Convert.ToDecimal(SubActivity_Id);
                    else
                        subActivityId = null;

                    DateTime trainingDate = Convert.ToDateTime(Date);

                    if (trainingDate > DateTime.Today)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Date cannot be greater then today's date.";
                        rd.Requestkey = "CreateTraining";
                        return rd;
                    }

                    int CurrentYear = trainingDate.Year;
                    int PreviousYear = trainingDate.Year - 1;
                    int NextYear = trainingDate.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;

                    if (trainingDate.Month > 3)
                        FinYear = CurYear + "-" + NexYear;
                    else
                        FinYear = PreYear + "-" + CurYear;

                    var getMinUnits = db.State_Category_With_Rule4_Mapping.
                        Include(u => u.Category_Master).Include(u => u.State_Master).Include(u => u.Rule4_Master)
                        .Where(u => u.CategoryID == categoryId && u.StateID == stateId)
                        .Select(u => new { MinUnits = u.Rule4_Master.MinUnits }).FirstOrDefault();

                    var getUnitsDone = db.StateActivity__with_Rule2.
                        Include(u => u.Activity_Master).Include(u => u.State_Master).Include(u => u.Rule2_Master)
                        .Where(u => u.ActivityID == activityId && u.StateID == stateId)
                        .Select(u => new
                        {
                            Unit = u.Rule2_Master.Unit,
                            Hours = u.Rule2_Master.Hours
                        }).FirstOrDefault();

                    if (getMinUnits == null || getUnitsDone == null)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Incorrect mapping selected";
                        rd.Requestkey = "CreateTraining";
                        return rd;
                    }
                    else
                    {

                        #region checking if 10 units has been created in each category and mark new record as forwardable.
                        decimal? totalUnits = 0;

                        //decimal GetUserStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());


                        List<MyCategory> totalCategories = (from s in db.State_Category_With_Rule4_Mapping
                                                            join c in db.Category_Master on s.CategoryID equals c.ID
                                                            where s.StateID == stateId && s.Status == "Active"
                                                            select new MyCategory
                                                            {
                                                                CategoryID = s.CategoryID,
                                                                Category_Name = c.Name,
                                                                ShortName = c.ShortName
                                                            }).ToList();

                        //ViewBag.TotalCat = totalCategories;

                        //List<ExistingCategory> swe = new List<ExistingCategory>();

                        //foreach (MyCategory d in totalCategories)
                        //{
                        //    decimal? existedCompletedCategories = db.User_Training_Status
                        //        .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                        //        .Where(x => x.Category_Id == d.CategoryID && x.User_Training_Transaction.Has_been_Forwarded == null
                        //         && x.Financial_Year == FinYear && x.User_Id == userId).Select(x => x.Units_Done).Sum();

                        //    swe.Add(new ExistingCategory
                        //    {
                        //        Category_Id = d.CategoryID,
                        //        Units_Done = (existedCompletedCategories == null) ? 0 : existedCompletedCategories,
                        //        Category_Name = d.Category_Name,
                        //        Financial_Year = FinYear,
                        //        Short_Name = d.ShortName
                        //    });

                        //    totalUnits = totalUnits + ((existedCompletedCategories == null) ? 0 : existedCompletedCategories);
                        //}
                        decimal hoursPerUnit = (decimal)getUnitsDone.Unit / (decimal)getUnitsDone.Hours;
                        decimal units_done = 0;
                        decimal hoursDone = Convert.ToDecimal(Hours);
                        units_done = hoursDone * hoursPerUnit;

                        decimal? carryOverUnits = db.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                                    .Where(x => x.StateID == stateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

                        var allTraningUnits = db.User_Training_Status
                            .Where(x => x.User_Id == userId && x.Financial_Year == FinYear).Select(x => x.Units_Done).Sum();

                        decimal? carry_ovarable = allTraningUnits + units_done;


                        if (carry_ovarable > 10 && (date.Month >= 1 && date.Month <= 3))
                        {
                            user_Training_Transaction.Forwardable = "Yes";
                            //user_Training_Transaction.Has_been_Forwarded = "No";
                        }

                        //if (totalUnits >= 10 && (date.Month >= 1 && date.Month <= 3))
                        //{
                        //    user_Training_Transaction.Forwardable = "Yes";
                        //}
                        #endregion
                        

                        User_Training_Status user_Training_Status = new User_Training_Status
                        {
                            User_Id = Convert.ToDecimal(userId),
                            Financial_Year = FinYear,
                            Category_Id = categoryId,
                            Min_Required_Category_Units = getMinUnits.MinUnits,
                            Activity_Id = activityId,
                            SubActivity_Id = subActivityId
                        };


                        
                        user_Training_Status.Units_Done = units_done;
                        user_Training_Status.Received_By_Forwarding = null;

                        user_Training_Transaction.Date = date;
                        user_Training_Transaction.Provider = Provider;
                        user_Training_Transaction.State_Id = stateId;
                        user_Training_Transaction.Category_Id = categoryId;
                        user_Training_Transaction.Activity_Id = activityId;
                        user_Training_Transaction.SubActivity_Id = subActivityId;
                        user_Training_Transaction.Financial_Year = FinYear;
                        user_Training_Transaction.User_Id = Convert.ToDecimal(userId);
                        user_Training_Transaction.Your_Role = Your_Role;
                        user_Training_Transaction.Has_been_Forwarded = null;
                        user_Training_Transaction.Hours = hoursDone;
                        user_Training_Transaction.Financial_Year = FinYear;
                        user_Training_Transaction.Descrption = Descrption;
                        //user_Training_Transaction.Forwardable = null;



                        //----------------------------------------------File Upload---------------------------------------
                        if (!string.IsNullOrEmpty(File))
                        {
                            byte[] imageArray = Convert.FromBase64String(File);

                            int fileSize = imageArray.Length;

                            if (fileSize > (1000000))
                            {
                                rd.Status = "Failure";
                                rd.Message = "File size limit is 1 MB";
                                rd.Requestkey = "CreateTraining";
                                return rd;
                            }

                            if (string.IsNullOrEmpty(FileName))
                            {
                                rd.Status = "Failure";
                                rd.Message = "File name cannot be empty";
                                rd.Requestkey = "CreateTraining";
                                return rd;
                            }

                            user_Training_Transaction.UploadedFileName = FileName;
                            user_Training_Transaction.UploadedFile = imageArray;
                        }
                        //-------------------------------------------------------------------------------------------------


                        List<User_Training_Transaction> getRecord = db.User_Training_Transaction.Where(u => u.Activity_Id == activityId &&
                        u.Category_Id == categoryId && u.Date == date &&
                        u.Financial_Year == FinYear && u.Hours == hoursDone
                        && u.Provider == Provider && u.State_Id == stateId &&
                        u.User_Id == userId && u.Your_Role == Your_Role).ToList();


                        if (getRecord.Count > 0)
                        {
                            rd.Status = "Failure";
                            rd.Message = "Record already exits";
                            rd.Requestkey = "CreateTraining";
                            return rd;
                        }
                        else
                        {
                            string sqlQuery = "";

                            if (subActivityId == null)
                            {
                                sqlQuery = @"SELECT User_Training_Transaction.User_Id, User_Training_Transaction.Activity_Id, 
                                User_Training_Transaction.State_Id, Rule1_Master.Name, Rule1_Master.Min, Rule1_Master.Max, 
                                COUNT(User_Training_Transaction.User_Id) AS TotalRecords
                                FROM            User_Training_Transaction INNER JOIN
                                StateActivitySubActivityWithRule1 ON User_Training_Transaction.Activity_Id = 
                                StateActivitySubActivityWithRule1.ActivityID AND User_Training_Transaction.State_Id = 
                                StateActivitySubActivityWithRule1.StateID INNER JOIN Rule1_Master ON 
                                StateActivitySubActivityWithRule1.Rule1ID = Rule1_Master.Id
                                WHERE (User_Training_Transaction.State_Id = " + stateId +
                                ") AND (User_Training_Transaction.Activity_Id = " + activityId +
                                ") AND (User_Training_Transaction.User_Id = " + userId +
                                ") AND (User_Training_Transaction.Financial_Year = '" + FinYear +
                                "') GROUP BY User_Training_Transaction.User_Id, User_Training_Transaction.State_Id, " +
                                "User_Training_Transaction.Activity_Id, Rule1_Master.Min, Rule1_Master.Max, Rule1_Master.Name";
                            }

                            if (subActivityId != null)
                            {
                                sqlQuery = @"SELECT User_Training_Transaction.User_Id, User_Training_Transaction.Activity_Id, 
                                User_Training_Transaction.State_Id, Rule1_Master.Name, Rule1_Master.Min, Rule1_Master.Max, 
                                COUNT(User_Training_Transaction.User_Id) AS TotalRecords
                                FROM User_Training_Transaction INNER JOIN
                                StateActivitySubActivityWithRule1 ON User_Training_Transaction.Activity_Id = 
                                StateActivitySubActivityWithRule1.ActivityID AND User_Training_Transaction.State_Id = 
                                StateActivitySubActivityWithRule1.StateID AND 
                                User_Training_Transaction.SubActivity_Id = StateActivitySubActivityWithRule1.SubActivityID 
                                INNER JOIN Rule1_Master ON StateActivitySubActivityWithRule1.Rule1ID = Rule1_Master.Id
                                WHERE (User_Training_Transaction.State_Id = " + stateId +
                                ") AND (User_Training_Transaction.Activity_Id = " + activityId +
                                ") AND (User_Training_Transaction.User_Id = " + userId +
                                ") AND (User_Training_Transaction.Financial_Year = '" + FinYear +
                                "') AND (User_Training_Transaction.SubActivity_Id = " + subActivityId +
                                ") GROUP BY User_Training_Transaction.User_Id, User_Training_Transaction.State_Id, " +
                                "User_Training_Transaction.Activity_Id, Rule1_Master.Min, Rule1_Master.Max, Rule1_Master.Name, " +
                                "User_Training_Transaction.SubActivity_Id";
                            }

                            using (LaxNarroEntities ctx = new LaxNarroEntities())
                            {
                                List<Custom_User_Training_Trnsaction> transactionRecords = ctx.Database.SqlQuery<Custom_User_Training_Trnsaction>(sqlQuery).ToList();

                                if (transactionRecords.Count > 0)
                                {
                                    decimal? maxValue = transactionRecords[0].Max;
                                    decimal? minValue = transactionRecords[0].Min;
                                    int totalRecords = transactionRecords[0].TotalRecords;
                                    string rule1Name = transactionRecords[0].Name;

                                    if (rule1Name == "0 <= Infinity")
                                    {
                                        db.User_Training_Transaction.Add(user_Training_Transaction);
                                        db.SaveChanges();
                                        decimal newly_inserted_id = user_Training_Transaction.Id;
                                        user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                        db.User_Training_Status.Add(user_Training_Status);
                                        db.SaveChanges();
                                        transaction.Commit();
                                        rd.Status = "Success";
                                        rd.Message = "Created successfully.";
                                        rd.Requestkey = "CreateTraining";
                                        return rd;
                                    }

                                    else if (rule1Name == "0 <=3")
                                    {
                                        if (totalRecords < 3)
                                        {
                                            db.User_Training_Transaction.Add(user_Training_Transaction);
                                            db.SaveChanges();
                                            decimal newly_inserted_id = user_Training_Transaction.Id;
                                            user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                            db.User_Training_Status.Add(user_Training_Status);
                                            db.SaveChanges();

                                            transaction.Commit();
                                            rd.Status = "Success";
                                            rd.Message = "Created successfully.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                        else
                                        {
                                            rd.Status = "Failure";
                                            rd.Message = "Maximum number of records in this activity is already saved.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                    }
                                    else if (rule1Name == "0 <=4")
                                    {
                                        if (totalRecords < 4)
                                        {
                                            db.User_Training_Transaction.Add(user_Training_Transaction);
                                            db.SaveChanges();
                                            decimal newly_inserted_id = user_Training_Transaction.Id;
                                            user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                            db.User_Training_Status.Add(user_Training_Status);
                                            db.SaveChanges();

                                            transaction.Commit();

                                            rd.Status = "Success";
                                            rd.Message = "Created successfully.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                        else
                                        {
                                            rd.Status = "Failure";
                                            rd.Message = "Maximum number of records in this activity is already saved.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                    }
                                    else if (rule1Name == "0 <=5")
                                    {
                                        if (totalRecords < 5)
                                        {
                                            db.User_Training_Transaction.Add(user_Training_Transaction);
                                            db.SaveChanges();
                                            decimal newly_inserted_id = user_Training_Transaction.Id;
                                            user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                            db.User_Training_Status.Add(user_Training_Status);
                                            db.SaveChanges();

                                            transaction.Commit();
                                            rd.Status = "Success";
                                            rd.Message = "Created successfully.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                        else
                                        {
                                            rd.Status = "Failure";
                                            rd.Message = "Maximum number of records in this activity is already saved.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                    }

                                    else if (rule1Name == "0 <=10")
                                    {
                                        if (totalRecords < 10)
                                        {
                                            db.User_Training_Transaction.Add(user_Training_Transaction);
                                            db.SaveChanges();
                                            decimal newly_inserted_id = user_Training_Transaction.Id;
                                            user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                            db.User_Training_Status.Add(user_Training_Status);
                                            db.SaveChanges();

                                            transaction.Commit();
                                            rd.Status = "Success";
                                            rd.Message = "Created successfully.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                        else
                                        {
                                            rd.Status = "Failure";
                                            rd.Message = "Maximum number of records in this activity is already saved.";
                                            rd.Requestkey = "CreateTraining";
                                            return rd;
                                        }
                                    }
                                    else
                                    {
                                        rd.Status = "Failure";
                                        rd.Message = "Something went wrong. Please try again after some time[1]";
                                        rd.Requestkey = "CreateTraining";
                                        return rd;
                                    }
                                }
                                else
                                {
                                    db.User_Training_Transaction.Add(user_Training_Transaction);
                                    db.SaveChanges();
                                    decimal newly_inserted_id = user_Training_Transaction.Id;
                                    user_Training_Status.Training_Transaction_ID = newly_inserted_id;
                                    db.User_Training_Status.Add(user_Training_Status);
                                    db.SaveChanges();

                                    transaction.Commit();

                                    rd.Status = "Success";
                                    rd.Message = "Created successfully.";
                                    rd.Requestkey = "CreateTraining";
                                    return rd;
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    rd.Status = "Failure";
                    rd.Message = "Something went wrong. Please try again after some time[2]";
                    rd.Requestkey = "CreateTraining";
                    return rd;
                }
            }
        }

        [WebMethod]
        public ReturnData EditTraining(string recordId, string User_Id, string Date, string State_Id, string Category_Id, string Activity_Id,
            string SubActivity_Id, string Hours, string Provider, string Your_Role, string Descrption, string FileName, string File)
        {
            ReturnData rd = new ReturnData();
            User_Training_Transaction user_Training_Transaction = new User_Training_Transaction();

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                if (recordId == string.Empty && User_Id == string.Empty && Date == string.Empty && 
                    State_Id == string.Empty && Category_Id == string.Empty
                    && Activity_Id == string.Empty && Hours == string.Empty && Provider == string.Empty)
                {
                    rd.Status = "Failure";
                    rd.Message = "Information missing";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }

                decimal id = Convert.ToDecimal(recordId);

                User_Training_Status ut = db.User_Training_Status.Where(x => x.Training_Transaction_ID == id).First();

                string finYear = GetFinancialYear();

                if (ut.Financial_Year != finYear)
                {
                    rd.Status = "Failure";
                    rd.Message = "Only current CPD years records are editable.";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }

                

                if (ut.Received_By_Forwarding == "Yes")
                {
                    rd.Status = "Failure";
                    rd.Message = "Carried over records are not editable.";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }

                
                decimal userId = Convert.ToDecimal(User_Id);
                decimal stateId = Convert.ToDecimal(State_Id);
                decimal categoryId = Convert.ToDecimal(Category_Id);
                decimal activityId = Convert.ToDecimal(Activity_Id);
                DateTime date = Convert.ToDateTime(Date);               


                if (date > DateTime.Today)
                {
                    rd.Status = "Failure";
                    rd.Message = "Date cannot be greater then today's date.";
                    rd.Requestkey = "CreateTraining";
                    return rd;
                }

                var categories = (from s in db.State_Category_With_Rule4_Mapping
                                  join c in db.Category_Master on s.CategoryID equals c.ID
                                  where s.StateID == stateId && c.ID == categoryId
                                  select new { c.ID, c.Name }).ToList();

                if (categories.Count <= 0)
                {
                    rd.Status = "Failure";
                    rd.Message = "This category does not belong to the state enrolled.";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }

                decimal? subActivityId;

                if (SubActivity_Id != string.Empty)
                    subActivityId = Convert.ToDecimal(SubActivity_Id);
                else
                    subActivityId = null;

                try
                {
                    user_Training_Transaction = db.User_Training_Transaction.Find(id);
                    //user_Training_Transaction.Id = Id;
                    user_Training_Transaction.Date = date;
                    user_Training_Transaction.Provider = Provider;
                    user_Training_Transaction.State_Id = stateId;
                    user_Training_Transaction.Category_Id = categoryId;
                    user_Training_Transaction.Activity_Id = activityId;
                    user_Training_Transaction.SubActivity_Id = subActivityId;
                    //user_Training_Transaction.Financial_Year = FinYear;
                    user_Training_Transaction.User_Id = userId;
                    user_Training_Transaction.Your_Role = Your_Role;
                    user_Training_Transaction.Has_been_Forwarded = null;
                    user_Training_Transaction.Hours = Convert.ToDecimal(Hours);
                    user_Training_Transaction.Forwardable = null;
                    user_Training_Transaction.Descrption = Descrption;

                    //if (File != null)
                    //{

                    //    user_Training_Transaction.UploadedFileName = FileName;
                    //    user_Training_Transaction.UploadedFile = File;
                    //}


                    //----------------------------------------------File Upload---------------------------------------
                    if (!string.IsNullOrEmpty(File))
                    {
                        byte[] imageArray = Convert.FromBase64String(File);

                        int fileSize = imageArray.Length;

                        if (fileSize > (1000000))
                        {
                            rd.Status = "Failure";
                            rd.Message = "File size limit is 1 MB";
                            rd.Requestkey = "EditTraining";
                            return rd;
                        }

                        if (string.IsNullOrEmpty(FileName))
                        {
                            rd.Status = "Failure";
                            rd.Message = "File name cannot be empty";
                            rd.Requestkey = "EditTraining";
                            return rd;
                        }

                        user_Training_Transaction.UploadedFileName = FileName;
                        user_Training_Transaction.UploadedFile = imageArray;
                    }
                    //-------------------------------------------------------------------------------------------------


                    db.Entry(user_Training_Transaction).State = EntityState.Modified;
                    db.SaveChanges();


                    decimal userid = user_Training_Transaction.User_Id;

                    User_Training_Status user_Training_Status = db.User_Training_Status.
                        FirstOrDefault(u => u.Training_Transaction_ID == user_Training_Transaction.Id);

                    var getMinUnits = db.State_Category_With_Rule4_Mapping.
                       Include(u => u.Category_Master).Include(u => u.State_Master).Include(u => u.Rule4_Master)
                       .Where(u => u.CategoryID == user_Training_Transaction.Category_Id && u.StateID == user_Training_Transaction.State_Id)
                       .Select(u => new { MinUnits = u.Rule4_Master.MinUnits }).FirstOrDefault();

                    var getUnitsDone = db.StateActivity__with_Rule2.
                        Include(u => u.Activity_Master).Include(u => u.State_Master).Include(u => u.Rule2_Master)
                        .Where(u => u.ActivityID == user_Training_Transaction.Activity_Id && u.StateID == user_Training_Transaction.State_Id)
                        .Select(u => new
                        {
                            Unit = u.Rule2_Master.Unit,
                            Hours = u.Rule2_Master.Hours
                        }).FirstOrDefault();

                    if (getMinUnits == null || getUnitsDone == null)
                    {
                        rd.Status = "Failure";
                        rd.Message = "Incorrect mapping selected";
                        rd.Requestkey = "EditTraining";
                        return rd;
                    }

                    //int userDoneHours = 0;
                    //bool dataType = int.TryParse(user_Training_Transaction.Hours, out userDoneHours);

                    decimal hours = Convert.ToDecimal(Hours);

                    decimal hoursPerUnit = Convert.ToDecimal(getUnitsDone.Unit / getUnitsDone.Hours);

                    user_Training_Status.Category_Id = user_Training_Transaction.Category_Id;
                    user_Training_Status.Activity_Id = user_Training_Transaction.Activity_Id;
                    user_Training_Status.Financial_Year = user_Training_Transaction.Financial_Year;
                    user_Training_Status.Min_Required_Category_Units = getMinUnits.MinUnits;
                    user_Training_Status.SubActivity_Id = user_Training_Transaction.SubActivity_Id;

                    decimal units_done = 0;
                    units_done = hours * hoursPerUnit;
                    user_Training_Status.Units_Done = units_done;
                    user_Training_Status.Received_By_Forwarding = user_Training_Transaction.Has_been_Forwarded;

                    db.Entry(user_Training_Status).State = EntityState.Modified;
                    db.SaveChanges();

                    transaction.Commit();
                    rd.Status = "Success";
                    rd.Message = "changes saved successfully.";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }
                catch (Exception ee)
                {
                    transaction.Rollback();
                    rd.Status = "Failure";
                    rd.Message = "Something went wrong. Please try again after some time[2]";
                    rd.Requestkey = "EditTraining";
                    return rd;
                }
            }
        }

        [WebMethod]
        public ReturnData DeleteTraining(string Id)
        {
            ReturnData rd = new ReturnData();

            if (Id == string.Empty)
            {
                rd.Status = "Failure";
                rd.Message = "No Record found";
                rd.Requestkey = "DeleteTraining";
                return rd;
            }

            decimal id = Convert.ToDecimal(Id);

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    User_Training_Status user_Training_Status = db.User_Training_Status.Find(id); 

                     User_Training_Transaction user_Training_Transaction = db.User_Training_Transaction.
                        FirstOrDefault(u => u.Id == user_Training_Status.Training_Transaction_ID);

                    if (user_Training_Status.Received_By_Forwarding == "Yes")
                    {
                        string prevCPDYear = Convert.ToString(Convert.ToInt32
                            (user_Training_Status.Financial_Year.Split('-')[0]) - 1) + "-" +
                            user_Training_Status.Financial_Year.Split('-')[0];


                        User_Training_Status oldCarriedOverRecord = db.User_Training_Status.Where(x => x.Financial_Year == prevCPDYear
                                    && x.Activity_Id == user_Training_Status.Activity_Id
                                    && x.SubActivity_Id == user_Training_Status.SubActivity_Id
                                    && x.Category_Id == user_Training_Status.Category_Id
                                    && x.User_Training_Transaction.Date == user_Training_Status.User_Training_Transaction.Date
                                    && x.User_Id == user_Training_Status.User_Id).FirstOrDefault();

                        User_Training_Transaction oldCarryTransaction = db.User_Training_Transaction
                            .Find(oldCarriedOverRecord.Training_Transaction_ID);

                        if (oldCarriedOverRecord != null)
                        {
                            oldCarriedOverRecord.Units_Done = user_Training_Status.Units_Done + oldCarriedOverRecord.Units_Done;
                            db.Entry(oldCarriedOverRecord).State = EntityState.Modified;
                            db.SaveChanges();

                            oldCarryTransaction.Hours = oldCarryTransaction.Hours + user_Training_Transaction.Hours;
                            db.Entry(oldCarryTransaction).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    db.User_Training_Status.Remove(user_Training_Status);
                    db.SaveChanges();

                    db.User_Training_Transaction.Remove(user_Training_Transaction);
                    db.SaveChanges();

                    transaction.Commit();

                    rd.Status = "Success";
                    rd.Message = "Deleted Successfully.";
                    rd.Requestkey = "DeleteTraining";
                    return rd;
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    rd.Status = "Failure";
                    rd.Message = "Something went wrong. Please try after some time.";
                    rd.Requestkey = "DeleteTraining";
                    return rd;
                }
            }
        }



        public class ReturnSubActivityList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<Sub_Activity_Master> UserSubActivities { get; set; }
        }

        public class ReturnActivityList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<Activity_Master> UserActivities { get; set; }
        }

        public class ReturnData
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<UserTrainingTransaction> UserTraining { get; set; }
            public List<SelectListItem> FinancialYear { get; set; }
        }

        public class ReturnCategoryList
        {
            public string Status { get; set; }
            public string Message { get; set; }
            public string Requestkey { get; set; }
            public List<Category_Master> UserCategories { get; set; }
        }

        public class UserTrainingTransaction
        {
            public decimal Id { get; set; }
            public decimal TransactionId { get; set; }
            public decimal User_Id { get; set; }
            public string FirstName { get; set; }
            public DateTime? Date { get; set; }
            public decimal? StateId { get; set; }
            public string StateName { get; set; }
            public decimal? CategoryId { get; set; }
            public string CategoryName { get; set; }
            public decimal? ActivityId { get; set; }
            public string ActivityName { get; set; }
            public decimal? SubActivityId { get; set; }
            public string SubActivityName { get; set; }
            public decimal? Hours { get; set; }
            public string Provider { get; set; }
            public string Financial_Year { get; set; }
            public string Your_Role { get; set; }
            public string Forwardable { get; set; }
            public string Has_been_Forwarded { get; set; }
            public string Descrption { get; set; }
            public decimal? Units { get; set; }
            public string ImageLink { get; set; }
            public string FileName { get; set; }
        }

        public class Activity_Master
        {
            public decimal ID { get; set; }
            public string Name { get; set; }
        }

        public class Category_Master
        {
            public decimal ID { get; set; }
            public string Name { get; set; }
        }

        public partial class Sub_Activity_Master
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public decimal Activity_ID { get; set; }
            public string ShortName { get; set; }
            public decimal StateID { get; set; }
        }
    }
}
