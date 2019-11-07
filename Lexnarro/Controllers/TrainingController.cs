using Lexnarro.HelperClasses;
using Lexnarro.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class TrainingController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: User_Training_Transaction

        public ActionResult Index(string financialYear)
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            #region DELETE EXISTING FILE

            try
            {
                if (System.IO.File.Exists(Server.MapPath("~/Reports/TrainingReport_" + userID + ".pdf")))
                {
                    System.IO.File.Delete(Server.MapPath("~/Reports/TrainingReport_" + userID + ".pdf"));
                }
            }
            catch (Exception)
            {
                //throw;
            }

            #endregion

            string finYear = string.Empty;

            if (financialYear == null)
            {
                finYear = GetFinancialYear();
            }
            else
            {
                finYear = financialYear;
            }

            ViewBag.fy = GetFinancialYear(); 
            List<User_Transaction_Master> demoUser = db.User_Transaction_Master.Include(x => x.Plan_Master)
                .Where(x => x.Plan_Master.Plan == "Demo" && x.User_ID == userID).ToList();

            List<User_Training_Transaction> userRecordsCount = db.User_Training_Transaction.Where(x => x.User_Id == userID).ToList();

            if (demoUser.Count > 0)
            {
                ViewBag.userPlan = "Demo";

                if (userRecordsCount.Count >= 0 && userRecordsCount.Count <= 4)
                {
                    ViewBag.RowCount = userRecordsCount.Count;
                }
                else
                {
                    ViewBag.RowCount = userRecordsCount.Count;
                }
            }
            else
            {
                ViewBag.userPlan = "Not Demo";

                if (userRecordsCount.Count > 0)
                {
                    ViewBag.RowCount = userRecordsCount.Count;
                }
                else
                {
                    ViewBag.RowCount = 0;
                }
            }

            ViewBag.CurrentCPDYear = GetFinancialYear();

            List<SelectListItem> fy = new List<SelectListItem>();

            fy = (from s in db.User_Training_Transaction
                  where s.User_Id == userID
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

            ViewBag.financialYear = fy;

            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }


        public ActionResult GetTraining(decimal id, string finYear)
        {
            decimal userId = Convert.ToDecimal(UserHelper.GetUserId());

            if (id != userId)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }

            List<User_Training_Status> userTraining = db.User_Training_Status.Include(x => x.User_Profile).Include(x => x.Category_Master)
                 .Include(x => x.Activity_Master).Include(x => x.Sub_Activity_Master).Include(x => x.User_Training_Transaction)
                 .Where(x => x.User_Id == userId && x.Financial_Year == finYear && x.Units_Done != 0).ToList();

            //var userTransactionData = db.User_Training_Status.Where(u => u.User_Id == id && u.Financial_Year == finYear).ToList();

            decimal? totalUnits = 0;

            totalUnits = userTraining.Select(x => x.Units_Done).Sum();

            //foreach (User_Training_Status row in userTraining)
            //{
            //    totalUnits += (int)Math.Floor(Convert.ToDecimal(row.Units_Done));
            //}

            ViewBag.TotalUnits = totalUnits;

            var training = userTraining.Select(S => new
            {
                S.Id,
                S.User_Id,
                S.Training_Transaction_ID,
                Date = S.User_Training_Transaction.Date.ToString().Split(' ')[0],
                S.User_Profile.FirstName,
                S.User_Profile.LastName,
                Country = S.User_Profile.Country_Master.Name,
                StateEnrolled = S.User_Profile.State_Master1.Name,
                Category = S.Category_Master.Name,
                Activity = S.Activity_Master.Name,
                SubActivity = (S.SubActivity_Id == null) ? "" : S.Sub_Activity_Master.Name,
                S.User_Training_Transaction.Hours,
                S.User_Training_Transaction.Provider,
                S.Financial_Year,
                S.Units_Done,
                S.User_Profile.EmailAddress,
                TotalUnits = totalUnits,
                S.Received_By_Forwarding,
                S.User_Training_Transaction.Has_been_Forwarded,
                rowCount = userTraining.Count
            });

            return Json(new { data = training }, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult GetData(string finYear)
        //{
        //    string userRole = UserHelper.GetUserRole();
        //    decimal userId = Convert.ToDecimal(UserHelper.GetUserId());
        //    string finanacialYear = string.Empty;

        //    if (finYear == null)
        //    {
        //        finanacialYear = GetFinancialYear();
        //    }
        //    else
        //    {
        //        finanacialYear = finYear;
        //    }

        //    //IQueryable<User_Training_Status> user_Training_Transaction = db.User_Training_Status.Include(u => u.Activity_Master)
        //    //    .Include(u => u.Category_Master).Include(u => u.User_Profile).Include(u => u.User_Training_Transaction)
        //    //    .Where(u => u.Financial_Year == finanacialYear);

        //    //if (userRole == "User")
        //    //{
        //        var user_Training_Transaction = db.User_Training_Status.Include(u => u.Activity_Master)
        //       .Include(u => u.Category_Master).Include(u => u.User_Profile)
        //       .Include(u => u.User_Training_Transaction).Where(u => u.User_Id == userId && u.Financial_Year == finanacialYear);
        //    //}


        //    var subData = user_Training_Transaction.ToList().Select(S => new
        //    {
        //        S.Id,
        //        State = S.User_Profile.State_Master1.Name,
        //        Category = S.Category_Master.Name,
        //        Activity = S.Activity_Master.Name,
        //        Subactivity = (S.SubActivity_Id == null) ? "" : S.Sub_Activity_Master.Name,
        //        Units = S.Units_Done,
        //        S.User_Training_Transaction.Hours,
        //        S.Financial_Year,
        //        S.User_Training_Transaction.Forwardable,
        //        S.User_Training_Transaction.Has_been_Forwarded,
        //        S.User_Training_Transaction.Provider,
        //        S.User_Training_Transaction.Descrption,
        //        S.User_Training_Transaction.UploadedFileName,
        //        Role = S.User_Training_Transaction.Your_Role,
        //        Date = S.User_Training_Transaction.Date.ToString().Split(' ')[0],
        //        S.User_Id
        //    });

        //    return Json(new { data = subData }, JsonRequestBehavior.AllowGet);
        //}


        public ActionResult Details(decimal? id)
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            User_Training_Transaction user_Training_Transaction = db.User_Training_Transaction.Find(id);

            if (user_Training_Transaction.User_Id != userID)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }
            

            if (user_Training_Transaction == null)
            {
                return HttpNotFound();
            }

            //CheckCurrentUserPlan(userID);

            //Create financial year viewbag and check it to wheather to show edit button or not.
            ViewBag.financialYear = GetFinancialYear();

            return View(user_Training_Transaction);
        }


        // GET: User_Training_Transaction/Create
        public ActionResult Create()
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            //ViewBag.CurrentDate = DateTime.Today;

            //var state = (from s in db.User_Profile
            //             join c in db.State_Master on s.StateEnrolled equals c.ID
            //             where s.ID == userID
            //             select new { c.ID, c.Name }).ToList();

            //var ds = state.ToList();

            decimal stateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

            //-----------Checking for no of records and demo account-----------

            var demoRecords = (from a in db.User_Transaction_Master
                               join b in db.User_Profile on a.User_ID equals b.ID
                               join c in db.User_Training_Transaction on a.User_ID equals c.User_Id
                               join d in db.Plan_Master on a.PlanID equals d.Plan_ID
                               where a.User_ID == userID && d.Plan == "Demo"
                               select c);

            var noOfRecords = demoRecords.Count();

            if (noOfRecords == 5)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.error, "Demo users can only save 5 records.");
                return RedirectToAction("Index");
            }
            //-----------------------------------------------------------------


            //-------------Empty sub activity list on page load-------------
            List<SelectListItem> subActivityList = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "NULL",
                    Text = "N/A"
                }
            };
            ViewBag.SubActivity_Id = subActivityList;



            //-----------Getting activity list for a state-------------------

            var activities = (from s in db.State_Activity_Mapping
                              join c in db.Activity_Master on s.ActivityID equals c.ID
                              where s.StateID == stateId
                              select new { c.ID, c.Name }).ToList();

            List<SelectListItem> activityList = new List<SelectListItem>();

            for (int i = 0; i < activities.Count; i++)
            {
                activityList.Add(new SelectListItem
                {
                    Value = Convert.ToString(activities[i].ID),
                    Text = activities[i].Name
                });
            }

            ViewBag.Activity_Id = activityList;

            //-----------Getting category list for a state-------------------

            var categories = (from s in db.State_Category_With_Rule4_Mapping
                              join c in db.Category_Master on s.CategoryID equals c.ID
                              where s.StateID == stateId
                              select new { c.ID, c.Name }).ToList();

            List<SelectListItem> categoryList = new List<SelectListItem>();

            for (int i = 0; i < categories.Count; i++)
            {
                categoryList.Add(new SelectListItem
                {
                    Value = Convert.ToString(categories[i].ID),
                    Text = categories[i].Name
                });
            }

            ViewBag.stateName = UserHelper.GetUserStateEnrolledName();
            ViewBag.Category_Id = categoryList;

            return View();
        }


        // POST: User_Training_Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User_Training_Transaction user_Training_Transaction, HttpPostedFileBase postedFile)
        {
            decimal userid = Convert.ToDecimal(UserHelper.GetUserId());
            decimal stateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    #region VARIOUS DROPDOWNs INITIALISATION
                    //-----------Getting activity list for a state-------------------

                    var activities = (from s in db.State_Activity_Mapping
                                      join c in db.Activity_Master on s.ActivityID equals c.ID
                                      where s.StateID == stateId
                                      select new { c.ID, c.Name }).ToList();

                    List<SelectListItem> activityList = new List<SelectListItem>();

                    for (int i = 0; i < activities.Count; i++)
                    {
                        activityList.Add(new SelectListItem
                        {
                            Value = Convert.ToString(activities[i].ID),
                            Text = activities[i].Name
                        });
                    }
                    string activityid = Convert.ToString(user_Training_Transaction.Activity_Id);
                    foreach (SelectListItem item in activityList)
                    {
                        if (item.Value == activityid)
                        {
                            item.Selected = true;
                            break;
                        }
                    }

                    ViewBag.Activity_Id = activityList;

                    ////////////////////-----------Sub Activity----------////////////////////

                    var subActivities = (from s in db.StateActivitySubActivityWithRule1
                                         join c in db.Sub_Activity_Master on s.SubActivityID equals c.ID
                                         join x in db.Activity_Master on s.ActivityID equals x.ID
                                         where s.StateID == stateId && s.ActivityID == user_Training_Transaction.Activity_Id
                                         select new { c.ID, c.Name }).ToList();

                    List<SelectListItem> subActivityList = new List<SelectListItem>();

                    if (subActivities.Count > 0)
                    {
                        for (int i = 0; i < subActivities.Count; i++)
                        {
                            if (subActivities[i].Name != "Private Study")
                            {
                                subActivityList.Add(new SelectListItem
                                {
                                    Value = Convert.ToString(subActivities[i].ID),
                                    Text = subActivities[i].Name
                                });
                            }
                        }

                        ViewBag.SubActivity_Id = subActivityList;
                    }

                    if (ViewBag.SubActivity_Id == null)
                    {
                        //-------------Empty sub activity list-------------
                        subActivityList = new List<SelectListItem>();
                        subActivityList.Add(new SelectListItem
                        {
                            Value = "NULL",
                            Text = "N/A"
                        });
                        ViewBag.SubActivity_Id = subActivityList;
                    }


                    //-----------Getting category list for a state-------------------

                    var categories = (from s in db.State_Category_With_Rule4_Mapping
                                      join c in db.Category_Master on s.CategoryID equals c.ID
                                      where s.StateID == stateId
                                      select new { c.ID, c.Name }).ToList();

                    List<SelectListItem> categoryList = new List<SelectListItem>();

                    for (int i = 0; i < categories.Count; i++)
                    {
                        categoryList.Add(new SelectListItem
                        {
                            Value = Convert.ToString(categories[i].ID),
                            Text = categories[i].Name
                        });
                    }
                    string categoryid = Convert.ToString(user_Training_Transaction.Category_Id);
                    foreach (SelectListItem item in categoryList)
                    {
                        if (item.Value == categoryid)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                    ViewBag.Category_Id = categoryList;
                    #endregion                    

                    DateTime date = Convert.ToDateTime(user_Training_Transaction.Date);

                    if (date > DateTime.Today)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.error, "Cannot create a training of future date.");
                        goto ErrorCase;
                    }

                    int CurrentYear = date.Year;
                    int PreviousYear = date.Year - 1;
                    int NextYear = date.Year + 1;
                    string PreYear = PreviousYear.ToString();
                    string NexYear = NextYear.ToString();
                    string CurYear = CurrentYear.ToString();
                    string FinYear = null;

                    if (date.Month > 3)
                        FinYear = CurYear + "-" + NexYear;
                    else
                        FinYear = PreYear + "-" + CurYear;                  



                    user_Training_Transaction.State_Id = stateId;

                    var getMinUnits = db.State_Category_With_Rule4_Mapping.
                        Include(u => u.Category_Master).Include(u => u.State_Master).Include(u => u.Rule4_Master)
                        .Where(u => u.CategoryID == user_Training_Transaction.Category_Id && u.StateID == stateId)
                        .Select(u => new { u.Rule4_Master.MinUnits }).FirstOrDefault();

                    var getUnitsDone = db.StateActivity__with_Rule2.
                        Include(u => u.Activity_Master).Include(u => u.State_Master).Include(u => u.Rule2_Master)
                        .Where(u => u.ActivityID == user_Training_Transaction.Activity_Id && u.StateID == stateId)
                        .Select(u => new
                        {
                            u.Rule2_Master.Unit,
                            u.Rule2_Master.Hours
                        }).FirstOrDefault();

                    if (getMinUnits == null || getUnitsDone == null)
                        ViewBag.message = ToasterMessage.Message(ToastType.error, "Incorrect mapping selected");
                    else
                    {
                        int userDoneHours = 0;
                        //bool dataType = int.TryParse(user_Training_Transaction.Hours, out userDoneHours);

                        decimal hoursPerUnit = (decimal)getUnitsDone.Unit / (decimal)getUnitsDone.Hours;

                        User_Training_Status user_Training_Status = new User_Training_Status
                        {
                            User_Id = userid,
                            Financial_Year = FinYear,
                            Category_Id = user_Training_Transaction.Category_Id,
                            Min_Required_Category_Units = getMinUnits.MinUnits,
                            Activity_Id = user_Training_Transaction.Activity_Id,
                            SubActivity_Id = user_Training_Transaction.SubActivity_Id
                        };
                        decimal? units_done = 0;
                        units_done = user_Training_Transaction.Hours * hoursPerUnit;
                        user_Training_Status.Units_Done = units_done;

                        //------------------Something is here -- remember it 
                        user_Training_Status.Received_By_Forwarding = user_Training_Transaction.Has_been_Forwarded;

                        user_Training_Transaction.Financial_Year = FinYear;
                        user_Training_Transaction.User_Id = userid;


                        //--------------------------------------------File-------------------------------------------------

                        if (postedFile != null)
                        {
                            string[] allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
                            string extension = Path.GetExtension(postedFile.FileName);
                            if (allowedExtensions.Contains(extension.ToLower()))
                            {
                                int fileSize = postedFile.ContentLength;

                                if (fileSize <= (1000000))
                                {
                                    user_Training_Transaction.UploadedFileName = Path.GetFileName(postedFile.FileName);
                                    using (BinaryReader reader = new BinaryReader(postedFile.InputStream))
                                    {
                                        user_Training_Transaction.UploadedFile = reader.ReadBytes(postedFile.ContentLength);
                                    }
                                }
                                else
                                {
                                    ViewBag.message = ToasterMessage.Message(ToastType.info, "File size limit is 1 MB");
                                    goto ErrorCase;
                                }
                            }
                            else
                            {
                                ViewBag.message = ToasterMessage.Message(ToastType.info, "Only pdf and Image files are allowed");
                                goto ErrorCase;
                            }
                        }

                        //-------------------------------------------------------------------------------------------------

                        //if (ModelState.IsValid)
                        //{
                        List<User_Training_Transaction> getRecord = db.User_Training_Transaction.Where(u => u.Activity_Id == user_Training_Transaction.Activity_Id &&
                        u.Category_Id == user_Training_Transaction.Category_Id && u.Date == user_Training_Transaction.Date &&
                        u.Financial_Year == user_Training_Transaction.Financial_Year && u.Forwardable == user_Training_Transaction.Forwardable
                        && u.Has_been_Forwarded == user_Training_Transaction.Has_been_Forwarded && u.Hours == user_Training_Transaction.Hours
                        && u.Provider == user_Training_Transaction.Provider && u.State_Id == stateId &&
                        u.User_Id == user_Training_Transaction.User_Id && u.Your_Role == user_Training_Transaction.Your_Role).ToList();

                        if (getRecord.Count > 0)
                        {
                            ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                            return View();
                        }
                        else
                        {
                            string sqlQuery = "";

                            if (user_Training_Transaction.SubActivity_Id == null)
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
                                ") AND (User_Training_Transaction.Activity_Id = " + user_Training_Transaction.Activity_Id +
                                ") AND (User_Training_Transaction.User_Id = " + user_Training_Transaction.User_Id +
                                ") AND (User_Training_Transaction.Financial_Year = '" + FinYear +
                                "') GROUP BY User_Training_Transaction.User_Id, User_Training_Transaction.State_Id, " +
                                "User_Training_Transaction.Activity_Id, Rule1_Master.Min, Rule1_Master.Max, Rule1_Master.Name";
                            }

                            if (user_Training_Transaction.SubActivity_Id != null)
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
                                ") AND (User_Training_Transaction.Activity_Id = " + user_Training_Transaction.Activity_Id +
                                ") AND (User_Training_Transaction.User_Id = " + user_Training_Transaction.User_Id +
                                ") AND (User_Training_Transaction.Financial_Year = '" + FinYear +
                                "') AND (User_Training_Transaction.SubActivity_Id = " + user_Training_Transaction.SubActivity_Id +
                                ") GROUP BY User_Training_Transaction.User_Id, User_Training_Transaction.State_Id, " +
                                "User_Training_Transaction.Activity_Id, Rule1_Master.Min, Rule1_Master.Max, Rule1_Master.Name, " +
                                "User_Training_Transaction.SubActivity_Id";
                            }


                            using (LaxNarroEntities ctx = new LaxNarroEntities())
                            {
                                //var records = ctx.Database.ExecuteSqlCommand(sqlQuery);
                                List<Custom_User_Training_Trnsaction> transactionRecords = ctx.Database
                                    .SqlQuery<Custom_User_Training_Trnsaction>(sqlQuery).ToList();

                                decimal enrolledStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());
                                //string enrolledStateName = UserHelper.GetUserStateEnrolledName();

                                //User_Profile userRecord = db.User_Profile.Find(userId);

                                decimal? carryOverUnits = db.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                                    .Where(x => x.StateID == enrolledStateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

                                var allTraningUnits = db.User_Training_Status
                                    .Where(x=>x.User_Id == userid && x.Financial_Year == FinYear).Select(x=>x.Units_Done).Sum();

                                decimal? carry_ovarable = allTraningUnits + units_done;


                                if (carry_ovarable > 10 && (date.Month >= 1 && date.Month <= 3))
                                {
                                    user_Training_Transaction.Forwardable = "Yes";
                                    //user_Training_Transaction.Has_been_Forwarded = "No";
                                }

                                //else
                                //{
                                //    user_Training_Transaction.Forwardable = null;
                                //    user_Training_Transaction.Has_been_Forwarded = null;
                                //    user_Training_Status.Received_By_Forwarding = null;
                                //}

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
                                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                        return RedirectToAction("Index", new { financialYear = FinYear });
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
                                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                            //return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            TempData["message"] = ToasterMessage.Message(ToastType.info, "Maximum number of records in this activity is already saved.");
                                            //return RedirectToAction("Index");
                                        }
                                        return RedirectToAction("Index", new { financialYear = FinYear });
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
                                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                            //return RedirectToAction("Index");
                                        }
                                        else
                                        {
                                            TempData["message"] = ToasterMessage.Message(ToastType.info, "Maximum number of records in this activity is already saved.");
                                            //return RedirectToAction("Index");
                                        }
                                        return RedirectToAction("Index", new { financialYear = FinYear });
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
                                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                            return RedirectToAction("Index", new { financialYear = FinYear });
                                        }
                                        else
                                        {
                                            TempData["message"] = ToasterMessage.Message(ToastType.info, "Maximum number of records in this activity is already saved.");
                                            return RedirectToAction("Index", new { financialYear = FinYear });
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
                                            TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                            return RedirectToAction("Index", new { financialYear = FinYear });
                                        }
                                        else
                                        {
                                            TempData["message"] = ToasterMessage.Message(ToastType.info, "Maximum number of records in this activity is already saved.");
                                            return RedirectToAction("Index", new { financialYear = FinYear });
                                        }
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
                                    TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                                    return RedirectToAction("Index", new { financialYear = FinYear });
                                }
                            }
                        }
                    }
                }
                catch (DbEntityValidationException e)
                {
                    transaction.Rollback();
                    string errorMessage = string.Empty;

                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                            errorMessage = ve.ErrorMessage;

                            ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                        }
                    }
                    return View();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
                }
            }

        ErrorCase:

            return View();
        }


        // GET: User_Training_Transaction/Edit/5
        public ActionResult Edit(decimal? id)
        {
            User_Training_Transaction user_Training_Transaction = null;

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                string finYear = GetFinancialYear();

                user_Training_Transaction = db.User_Training_Transaction.Find(id);

                if (user_Training_Transaction.Financial_Year != finYear)
                {
                    TempData["message"] = ToasterMessage.Message(ToastType.warning, "Only current CPD year records are editable.");
                    return RedirectToAction("Index");
                }

                User_Training_Status ut = db.User_Training_Status.Where(x=>x.Training_Transaction_ID == user_Training_Transaction.Id).First();

                if (ut.Received_By_Forwarding == "Yes")
                {
                    TempData["message"] = ToasterMessage.Message(ToastType.warning, "Carried over records are not editable.");
                    return RedirectToAction("Index");
                }

                if (user_Training_Transaction == null)
                {
                    return HttpNotFound();
                }

                decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

                if (user_Training_Transaction.User_Id != userID)
                {
                    TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                    return RedirectToAction("Index");
                }                

                ViewBag.State_Id = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name", user_Training_Transaction.State_Id);

                string stateEnrolledName = UserHelper.GetUserStateEnrolledName();

                decimal stateEnrolledId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

                ViewBag.stateName = stateEnrolledName;

                //-----------Getting activity list for a state-------------------

                var activities = (from s in db.State_Activity_Mapping
                                  join c in db.Activity_Master on s.ActivityID equals c.ID
                                  where s.StateID == stateEnrolledId
                                  select new { c.ID, c.Name }).ToList();

                List<SelectListItem> activityList = new List<SelectListItem>();

                for (int i = 0; i < activities.Count; i++)
                {
                    activityList.Add(new SelectListItem
                    {
                        Value = Convert.ToString(activities[i].ID),
                        Text = activities[i].Name
                    });
                }
                string activityid = Convert.ToString(user_Training_Transaction.Activity_Id);
                foreach (SelectListItem item in activityList)
                {
                    if (item.Value == activityid)
                    {
                        item.Selected = true;
                        break;
                    }
                }

                ViewBag.Activity_Id = activityList;

                //-----------Getting category list for a state-------------------

                var categories = (from s in db.State_Category_With_Rule4_Mapping
                                  join c in db.Category_Master on s.CategoryID equals c.ID
                                  where s.StateID == stateEnrolledId
                                  select new { c.ID, c.Name }).ToList();

                List<SelectListItem> categoryList = new List<SelectListItem>();

                for (int i = 0; i < categories.Count; i++)
                {
                    categoryList.Add(new SelectListItem
                    {
                        Value = Convert.ToString(categories[i].ID),
                        Text = categories[i].Name
                    });
                }
                string categoryid = Convert.ToString(user_Training_Transaction.Category_Id);
                foreach (SelectListItem item in categoryList)
                {
                    if (item.Value == categoryid)
                    {
                        item.Selected = true;
                        break;
                    }
                }
                ViewBag.Category_Id = categoryList;


                var subActivities = (from s in db.StateActivitySubActivityWithRule1
                                     join c in db.Sub_Activity_Master on s.SubActivityID equals c.ID
                                     join x in db.Activity_Master on s.ActivityID equals x.ID
                                     where s.StateID == stateEnrolledId && s.ActivityID == user_Training_Transaction.Activity_Id
                                     select new { c.ID, c.Name }).ToList();

                List<SelectListItem> subActivityList = new List<SelectListItem>();

                string subactivityid = "";

                if (subActivities.Count > 0)
                {
                    for (int i = 0; i < subActivities.Count; i++)
                    {
                        if (subActivities[i].Name != "Private Study")
                        {
                            subActivityList.Add(new SelectListItem
                            {
                                Value = Convert.ToString(subActivities[i].ID),
                                Text = subActivities[i].Name
                            });
                            subactivityid = Convert.ToString(subActivities[i].ID);
                        }
                    }

                    ViewBag.SubActivity_Id = subActivityList;
                }

                if (ViewBag.SubActivity_Id == null)
                {
                    //-------------Empty sub activity list-------------
                    subActivityList = new List<SelectListItem>();
                    subActivityList.Add(new SelectListItem
                    {
                        Value = "NULL",
                        Text = "N/A"
                    });
                    ViewBag.SubActivity_Id = subActivityList;
                }

                //------------------------------------------------------------------------------------------------------

                ViewBag.fileUploader = user_Training_Transaction.UploadedFileName;
            }
            catch (DbEntityValidationException e)
            {
                string errorMessage = string.Empty;

                foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (DbValidationError ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                        errorMessage = ve.ErrorMessage;

                        ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                    }
                }
                return View();
            }
            catch (Exception)
            {
                ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
            }

            return View(user_Training_Transaction);
        }



        // POST: User_Training_Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User_Training_Transaction user_Training_Transaction, HttpPostedFileBase postedFile)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    User_Training_Transaction user_Training_Transaction1 = db.User_Training_Transaction
                        .Find(user_Training_Transaction.Id);

                    decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

                    if (user_Training_Transaction1.User_Id != userID)
                    {
                        TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                        return RedirectToAction("Index", new { financialYear = user_Training_Transaction1.Financial_Year });
                    }

                    if (postedFile != null)
                    {
                        string[] allowedExtensions = new[] { ".pdf", ".png", ".jpg", ".jpeg" };
                        string extension = Path.GetExtension(postedFile.FileName);

                        if (allowedExtensions.Contains(extension.ToLower()))
                        {
                            int fileSize = postedFile.ContentLength;

                            if (fileSize <= (1000000))
                            {
                                user_Training_Transaction1.UploadedFileName = System.IO.Path.GetFileName(postedFile.FileName);
                                using (BinaryReader reader = new System.IO.BinaryReader(postedFile.InputStream))
                                {
                                    user_Training_Transaction1.UploadedFile = reader.ReadBytes(postedFile.ContentLength);
                                }
                            }
                            else
                            {
                                ViewBag.Error1 = "File size limit is 1 MB";
                                goto ErrorCase;
                            }
                        }
                        else
                        {
                            ViewBag.Error2 = "Only pdf and Image files are allowed";
                            goto ErrorCase;
                        }
                    }

                    user_Training_Transaction1.Date = user_Training_Transaction.Date;
                    user_Training_Transaction1.Activity_Id = user_Training_Transaction.Activity_Id;
                    user_Training_Transaction1.SubActivity_Id = user_Training_Transaction.SubActivity_Id;
                    user_Training_Transaction1.Category_Id = user_Training_Transaction.Category_Id;
                    user_Training_Transaction1.Hours = user_Training_Transaction.Hours;
                    user_Training_Transaction1.Provider = user_Training_Transaction.Provider;
                    user_Training_Transaction1.Descrption = user_Training_Transaction.Descrption;
                    user_Training_Transaction1.Your_Role = user_Training_Transaction.Your_Role;


                    db.Entry(user_Training_Transaction1).State = EntityState.Modified;
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
                            u.Rule2_Master.Unit,
                            u.Rule2_Master.Hours
                        }).FirstOrDefault();


                    int userDoneHours = 0;
                    //bool dataType = int.TryParse(user_Training_Transaction.Hours, out userDoneHours);

                    decimal hoursPerUnit = Convert.ToDecimal(getUnitsDone.Unit / getUnitsDone.Hours);

                    user_Training_Status.Category_Id = user_Training_Transaction.Category_Id;
                    user_Training_Status.Activity_Id = user_Training_Transaction.Activity_Id;
                    user_Training_Status.Financial_Year = user_Training_Transaction.Financial_Year;
                    user_Training_Status.Min_Required_Category_Units = getMinUnits.MinUnits;
                    user_Training_Status.Activity_Id = user_Training_Transaction.Activity_Id;
                    user_Training_Status.SubActivity_Id = user_Training_Transaction.SubActivity_Id;

                    decimal? units_done = 0;
                    units_done = user_Training_Transaction.Hours * hoursPerUnit;
                    user_Training_Status.Units_Done = units_done;
                    user_Training_Status.Received_By_Forwarding = user_Training_Transaction.Has_been_Forwarded;

                    db.Entry(user_Training_Status).State = EntityState.Modified;
                    db.SaveChanges();

                    transaction.Commit();
                    TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated successfully");
                    return RedirectToAction("Index", new { financialYear = user_Training_Transaction1.Financial_Year });

                }
                catch (DbEntityValidationException e)
                {
                    string errorMessage = string.Empty;

                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                            errorMessage = ve.ErrorMessage;

                            ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                        }
                    }
                    return View();
                }
                catch (Exception)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
                }

                return View();
            }

        ErrorCase:

            ViewBag.State_Id = new SelectList(db.State_Master.OrderBy(z => z.Name), "ID", "Name", user_Training_Transaction.State_Id);

            decimal stateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());
            ViewBag.stateName = UserHelper.GetUserStateEnrolledName();

            var subActivities = (from s in db.StateActivitySubActivityWithRule1
                                 join c in db.Sub_Activity_Master on s.SubActivityID equals c.ID
                                 join x in db.Activity_Master on s.ActivityID equals x.ID
                                 where s.StateID == stateId && s.ActivityID == user_Training_Transaction.Activity_Id
                                 select new { c.ID, c.Name }).ToList();

            List<SelectListItem> subActivityList = new List<SelectListItem>();

            if (subActivities.Count > 0)
            {
                for (int i = 0; i < subActivities.Count; i++)
                {
                    if (subActivities[i].Name != "Private Study")
                    {
                        subActivityList.Add(new SelectListItem
                        {
                            Value = Convert.ToString(subActivities[i].ID),
                            Text = subActivities[i].Name
                        });
                    }
                }

                ViewBag.SubActivity_Id = subActivityList;
            }

            if (ViewBag.SubActivity_Id == null)
            {
                //-------------Empty sub activity list-------------
                subActivityList = new List<SelectListItem>();
                subActivityList.Add(new SelectListItem
                {
                    Value = "NULL",
                    Text = "N/A"
                });
                ViewBag.SubActivity_Id = subActivityList;
            }

            //-----------Getting activity list for a state-------------------

            var activities = (from s in db.State_Activity_Mapping
                              join c in db.Activity_Master on s.ActivityID equals c.ID
                              where s.StateID == stateId
                              select new { c.ID, c.Name }).ToList();

            List<SelectListItem> activityList = new List<SelectListItem>();

            for (int i = 0; i < activities.Count; i++)
            {
                activityList.Add(new SelectListItem
                {
                    Value = Convert.ToString(activities[i].ID),
                    Text = activities[i].Name
                });
            }
            string activityid = Convert.ToString(user_Training_Transaction.Activity_Id);
            foreach (SelectListItem item in activityList)
            {
                if (item.Value == activityid)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.Activity_Id = activityList;

            //-----------Getting category list for a state-------------------

            var categories = (from s in db.State_Category_With_Rule4_Mapping
                              join c in db.Category_Master on s.CategoryID equals c.ID
                              where s.StateID == stateId
                              select new { c.ID, c.Name }).ToList();

            List<SelectListItem> categoryList = new List<SelectListItem>();

            for (int i = 0; i < categories.Count; i++)
            {
                categoryList.Add(new SelectListItem
                {
                    Value = Convert.ToString(categories[i].ID),
                    Text = categories[i].Name
                });
            }
            string categoryid = Convert.ToString(user_Training_Transaction.Category_Id);
            foreach (SelectListItem item in categoryList)
            {
                if (item.Value == categoryid)
                {
                    item.Selected = true;
                    break;
                }
            }
            ViewBag.Category_Id = categoryList;

            return View();
        }


        //GET: User_Training_Transaction/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User_Training_Transaction user_Training_Transaction = db.User_Training_Transaction.Find(id);

            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

            if (user_Training_Transaction.User_Id != userID)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }

            if (user_Training_Transaction == null)
            {
                return HttpNotFound();
            }
            return View(user_Training_Transaction);
        }


        // POST: User_Training_Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            using (DbContextTransaction transaction = db.Database.BeginTransaction())
            {
                try
                {
                    User_Training_Transaction user_Training_Transaction = db.User_Training_Transaction.Find(id);

                    decimal userID = Convert.ToDecimal(UserHelper.GetUserId());

                    if (user_Training_Transaction.User_Id != userID)
                    {
                        TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                        return RedirectToAction("Index", new { financialYear = user_Training_Transaction.Financial_Year });
                    }                    

                    User_Training_Status user_Training_Status = db.User_Training_Status
                        .FirstOrDefault(u => u.Training_Transaction_ID == user_Training_Transaction.Id);

                    if (user_Training_Transaction.Has_been_Forwarded == "Yes" && user_Training_Status != null)
                    {
                        TempData["message"] = ToasterMessage.Message(ToastType.warning, "carry over record exist corresponding to this record, please delete carried over record first.");
                        return RedirectToAction("Index", new { financialYear = user_Training_Transaction.Financial_Year });
                    }

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
                            oldCarryTransaction.Has_been_Forwarded = null;
                            db.Entry(oldCarryTransaction).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    db.User_Training_Status.Remove(user_Training_Status);
                    db.SaveChanges();

                    db.User_Training_Transaction.Remove(user_Training_Transaction);
                    db.SaveChanges();

                    transaction.Commit();

                    TempData["message"] = ToasterMessage.Message(ToastType.success, "Deleted successfully");
                    return RedirectToAction("Index", new { financialYear = user_Training_Transaction.Financial_Year });
                }
                catch (DbEntityValidationException e)
                {
                    string errorMessage = string.Empty;

                    foreach (DbEntityValidationResult eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);

                        foreach (DbValidationError ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                            errorMessage = ve.ErrorMessage;

                            ViewBag.message = ViewBag.message + ToasterMessage.Message(ToastType.error, errorMessage);
                        }
                    }
                    return View();
                }
                catch (Exception)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.error, "Something went wrong");
                }
            }
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }



        //    public JsonResult CategoryList(int StateID)
        //    {
        //        var state = from s in db.State_Category_With_Rule4_Mapping
        //                    join c in db.Category_Master on s.CategoryID equals c.ID
        //                    where s.StateID == StateID
        //                    select new { c.ID, c.Name };
        //        return Json(new SelectList(state.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult ActivityList(int StateID)
        //    {
        //        var state = from s in db.StateActivity__with_Rule2
        //                    join c in db.Activity_Master on s.ActivityID equals c.ID
        //                    where s.StateID == StateID
        //                    select new { c.ID, c.Name };
        //        return Json(new SelectList(state.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        //    }

        //    public JsonResult UserState(int userID)
        //    {
        //        var state = from s in db.User_Training_Transaction
        //                    join c in db.State_Master on s.State_Id equals c.ID
        //                    where s.User_Id == userID
        //                    select new { c.ID, c.Name };
        //        return Json(new SelectList(state.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        //    }


        public JsonResult SubActivityList(int stateID, int activityID)
        {
            //------------------Getting sub activity list for state------------------

            var state = (from s in db.StateActivitySubActivityWithRule1
                         join c in db.Sub_Activity_Master on s.SubActivityID equals c.ID
                         join x in db.Activity_Master on s.ActivityID equals x.ID
                         where s.StateID == stateID && s.ActivityID == activityID
                         select new { c.ID, c.Name }).ToList();

            //List<SelectListItem> subActivityList = new List<SelectListItem>();

            //if (subActivity.Count > 0)
            //{
            //    for (int i = 0; i < subActivity.Count; i++)
            //    {
            //        subActivityList.Add(new SelectListItem
            //        {
            //            Value = Convert.ToString(subActivity[i].ID),
            //            Text = subActivity[i].Name
            //        });
            //    }

            //    ViewBag.SubActivity_Id = subActivityList;
            //}

            return Json(new SelectList(state.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        //    //public JsonResult UserState(int userID)
        //    //{
        //    //    var state = from s in db.User_Training_Transaction
        //    //                join c in db.State_Master on s.State_Id equals c.ID
        //    //                where s.User_Id == userID
        //    //                select new { c.ID, c.Name };

        //    //    IQueryable<State_Master> df = new List<State_Master>
        //    //    {
        //    //        state.ID, state.Name
        //    //    };


        //    //    return Json(new SelectList(state.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        //    //}

        //private void CheckCurrentUserPlan(string userID)
        //{
        //    try
        //    {
        //        decimal uss = Convert.ToDecimal(userID);

        //        Plan_Master plan_id = db.Plan_Master.FirstOrDefault(x => x.Plan == "Demo");

        //        decimal planID = Convert.ToDecimal(plan_id.Plan_ID);

        //        List<User_Transaction_Master> demoUser = (from s in db.User_Transaction_Master
        //                                                  where s.PlanID == planID && s.User_ID == uss
        //                                                  select s).ToList();

        //        if (demoUser.Count > 0)
        //        {
        //            ViewBag.userPlan = "Demo";

        //            List<User_Training_Transaction> userRecordsCount = (from s in db.User_Training_Transaction
        //                                                                where s.User_Id == uss
        //                                                                select s).ToList();

        //            if (userRecordsCount.Count >= 0 && userRecordsCount.Count <= 4)
        //            {
        //                ViewBag.RowCount = userRecordsCount.Count;
        //            }
        //            else
        //            {
        //                ViewBag.RowCount = userRecordsCount.Count;
        //            }
        //        }
        //        else
        //        {
        //            ViewBag.userPlan = "Not Demo";

        //            List<User_Training_Transaction> userRecordsCount = (from s in db.User_Training_Transaction
        //                                                                where s.User_Id == uss
        //                                                                select s).ToList();

        //            if (userRecordsCount.Count > 0)
        //            {
        //                ViewBag.RowCount = userRecordsCount.Count;
        //            }
        //            else
        //            {
        //                ViewBag.RowCount = 0;
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        //throw;
        //    }
        //}



        public void ExportExcel()
        {
            decimal id = Convert.ToDecimal(UserHelper.GetUserId());

            //string finYear = TempData["finYear"].ToString();

            IQueryable<User_Training_Transaction> data = db.User_Training_Transaction.Include(u => u.Activity_Master).
                Include(u => u.Category_Master).Include(u => u.State_Master).Include(u => u.User_Profile)
                .Where(u => u.User_Id == id);

            List<Excel_User_Training_Trnsaction> _temp = new List<Excel_User_Training_Trnsaction>();

            foreach (User_Training_Transaction item in data)
            {
                _temp.Add(new Excel_User_Training_Trnsaction()
                {
                    Name = item.User_Profile.FirstName,
                    State = item.State_Master.Name,
                    Category = item.Category_Master.Name,
                    SubActivity = (item.SubActivity_Id == null) ? "" : item.Sub_Activity_Master.Name,
                    Activity = item.Activity_Master.Name,
                    Provider = item.Provider,
                    Hours = item.Hours,
                    Date = item.Date,
                    Financial_Year = item.Financial_Year,
                    Your_Role = item.Your_Role,
                    Forwardable = item.Forwardable,
                    Has_Been_Forwarded = item.Has_been_Forwarded,
                    Description = (item.Descrption == null) ? "" : item.Descrption
                });
            }
            System.Web.UI.WebControls.GridView grid = new System.Web.UI.WebControls.GridView();
            grid.DataSource = _temp;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=TrainingDetails.xls");
            Response.ContentType = "application/vnd.ms-excel";
            StringWriter sw = new StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Write(sw.ToString());
            Response.End();
        }


        public ActionResult SendTrainingReport(string finYear)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    string userEmail = UserHelper.GetUserMail();

                    string body = string.Empty;

                    string userEnrolledState = UserHelper.GetUserStateEnrolledName();

                    decimal userId = Convert.ToDecimal(UserHelper.GetUserId());

                    string stateShortName = UserHelper.GetUserStateEnrolledShortName();

                    mailMessage.From = new MailAddress("mail@lexnarro.com.au", "Lex Narro");
                    mailMessage.To.Add(new MailAddress(userEmail));
                    mailMessage.Subject = "Lex Narro CPD Records for " + finYear;
                    //mailMessage.Body = "<p>CPD Records.</p>";

                    using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplate/CPDRecords.html")))
                    {
                        body = reader.ReadToEnd();
                    }

                    body = body.Replace("{name}", UserHelper.GetUserName());

                    mailMessage.Body = body;

                    HtmlToPdf converter = new HtmlToPdf();
                    string pdf_page_size = "A4";
                    PdfPageSize pageSize = (PdfPageSize)Enum.Parse(typeof(PdfPageSize),
                        pdf_page_size, true);
                    string pdf_orientation = "Portrait";
                    PdfPageOrientation pdfOrientation =
                        (PdfPageOrientation)Enum.Parse(typeof(PdfPageOrientation),
                        pdf_orientation, true);
                    int webPageWidth = 800;
                    int webPageHeight = 0;

                    converter.Options.PdfPageSize = pageSize;
                    converter.Options.MarginBottom = 30;
                    converter.Options.MarginRight = 20;
                    converter.Options.MarginTop = 30;
                    converter.Options.PdfPageOrientation = pdfOrientation;
                    converter.Options.WebPageWidth = webPageWidth;
                    converter.Options.WebPageHeight = webPageHeight;

                    //Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath

                    string myurl = Request.Url.Scheme + "://" + Request.Url.Authority
                        + "/Reports/" + stateShortName + "?v=" + userId + "&f=" + finYear;

                    //string myurl = "https://www.lexnarro.com.au/Reports/" + stateShortName + ".aspx?v=" + userId + "&f=" + finYear;

                    PdfDocument doc = converter.ConvertUrl(myurl);
                    doc.Save(Server.MapPath("~/Reports/TrainingReport_" + userId + ".pdf"));
                    doc.Close();

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath("~/Reports/TrainingReport_" + userId + ".pdf")));


                    //// training image Attachment

                    var image = from s in db.User_Training_Transaction
                                where s.User_Id == userId && s.Financial_Year == finYear
                                select new { s.UploadedFile, s.UploadedFileName };

                    /////
                    foreach (var array in image)
                    {
                        if (array.UploadedFile != null)
                        {
                            mailMessage.Attachments.Add(new Attachment(new MemoryStream(array.UploadedFile), array.UploadedFileName));
                        }
                    }

                    mailMessage.IsBodyHtml = true;
                    SmtpClient client = new SmtpClient();

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "mail.lexnarro.com.au";
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 25;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                    smtp.Send(mailMessage);

                    var categories = (new
                    {
                       status = "success"
                    });

                    return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception dd)
            {
                var categories = (new
                {
                    status = dd.ToString()
                });
                return Json(new { data = categories }, JsonRequestBehavior.AllowGet);
            }
        }


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
        

        public class stateList
        {
            private decimal ID { get; set; }

            private String Name { get; set; }
        }
    }
}

