using Lexnarro.HelperClasses;
using Lexnarro.Models;
using Lexnarro.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class CarryOverController : Controller
    {
        private readonly LaxNarroEntities db = null;


        public CarryOverController()
        {
            db = new LaxNarroEntities();
        }



        // GET: CarryOver
        public ActionResult Index(string financialYear)
        {
            decimal userID = Convert.ToDecimal(UserHelper.GetUserId());
            decimal enrolledStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

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

            decimal? carryOverUnits = db.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                    .Where(x => x.StateID == enrolledStateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

            string nextFinYear = GetNewFinancialYear(finYear);
            decimal? carriedOverUnits = db.User_Training_Status.Where(x => x.Financial_Year == nextFinYear
                                       && x.User_Id == userID && x.Received_By_Forwarding == "Yes").Select(x => x.Units_Done).Sum();

            ViewBag.CarryOverUnits = carryOverUnits - (carriedOverUnits == null ? 0 : carriedOverUnits);

            #region Financial year dropdown 
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
            #endregion

            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }



        public ActionResult GetTraining(decimal id, string finYear)
        {
            decimal userId = Convert.ToDecimal(UserHelper.GetUserId());
            decimal enrolledStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

            if (id != userId)
            {
                TempData["message"] = ToasterMessage.Message(ToastType.warning, "Unauthorized access.");
                return RedirectToAction("Index");
            }

            //decimal? totalUnit = 0;

            List<MyCategory> totalCategories = (from s in db.State_Category_With_Rule4_Mapping
                                                join c in db.Category_Master on s.CategoryID equals c.ID
                                                where s.StateID == enrolledStateId && s.Status == "Active"
                                                select new MyCategory
                                                {
                                                    CategoryID = s.CategoryID,
                                                    Category_Name = c.Name,
                                                    ShortName = c.ShortName
                                                }).ToList();

            List<ExistingCategory> swe = new List<ExistingCategory>();

            string isAllCategoriesCompleted = string.Empty;

            foreach (MyCategory d in totalCategories)
            {
                decimal? existedCompletedCategories = db.User_Training_Status
                    .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                    .Where(x => x.Category_Id == d.CategoryID
                     && x.Financial_Year == finYear && x.User_Id == userId).Select(x => x.Units_Done).Sum();

                if (existedCompletedCategories == null)
                {
                    isAllCategoriesCompleted = "false";
                    break;
                }
            }

            decimal? carryOverUnits = db.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                         .Where(x => x.StateID == enrolledStateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

            if (carryOverUnits == 0)
            {
                return Json(new { data = "This state does not allow carry over." }, JsonRequestBehavior.AllowGet);
            }

            string nextFinYear = GetNewFinancialYear(finYear);

            decimal? carriedOverUnits = db.User_Training_Status.Where(x => x.Financial_Year == nextFinYear
                                       && x.User_Id == userId && x.Received_By_Forwarding == "Yes").Select(x => x.Units_Done).Sum();

            if (isAllCategoriesCompleted != "false")
            {
                if (carriedOverUnits >= carryOverUnits)
                {
                    return Json(new { data = "All allowed records has been carried over" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<User_Training_Status> userTraining = db.User_Training_Status
                                               .Include(x => x.User_Training_Transaction)
                                               .Where(x => x.User_Training_Transaction.Forwardable == "Yes" &&
                                               x.Units_Done != 0 && x.Financial_Year == finYear &&
                                               //x.User_Training_Transaction.Has_been_Forwarded == null && 
                                               x.User_Id == userId)
                                               .OrderByDescending(x => x.User_Training_Transaction.Date).ToList();

                    //var userTransactionData = db.User_Training_Status.Where(u => u.User_Id == id && u.Financial_Year == finYear).ToList();

                    int totalUnits = 0;

                    if (userTraining.Count < 1)
                    {
                        return Json(new { data = "No records for carry over found." }, JsonRequestBehavior.AllowGet);
                    }

                    foreach (User_Training_Status row in userTraining)
                    {
                        totalUnits += (int)Math.Floor(Convert.ToDecimal(row.Units_Done));
                    }

                    //ViewBag.TotalUnits = totalUnits;

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
                        rowCount = userTraining.Count
                    });

                    return Json(new { data = training }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { data = "All categories are not completed." }, JsonRequestBehavior.AllowGet);
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
            string FinYear;
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



        public ActionResult DoTask(string financialYear, string ids, string units)
        {
            decimal unts = 0;
            string result = string.Empty;

            decimal? hoursRemoved = 0;
            decimal? unitsRemoved = 0;
            try
            {
                decimal userId = Convert.ToDecimal(UserHelper.GetUserId());
                decimal enrolledStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());

                string nextFinancialYear = GetNewFinancialYear(financialYear);

                ids = ids.Trim('\\', '"', '[', ']');

                string resultIds = ids.RemoverStrs(new[] { "\\", "\"", "[", "]" });

                string resultUnits = units.RemoverStrs(new[] { "\\", "\"", "[", "]" });

                string[] carryOverIds = resultIds.Split(',');

                string[] carryOverUnitsList = resultUnits.Split(',');

                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                    {
                        decimal? carryOverUnits = ctx.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                          .Where(x => x.StateID == enrolledStateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

                        foreach (string item in carryOverUnitsList)
                        {
                            if (Convert.ToDecimal(item) <= 0)
                            {
                                result = "Error - Please select units more then 0 (zero)";
                                transaction.Dispose();
                                goto ErrorCase;
                            }
                            unts = unts + Convert.ToDecimal(item);
                        }

                        if (unts > carryOverUnits)
                        {
                            result = "Error - units";
                            transaction.Dispose();
                            goto ErrorCase;
                        }

                        //i.e limited units to carry over
                        //Actual carry over 

                        for (int j = 0; j < carryOverIds.Length; j++)
                        {
                            decimal id = Convert.ToDecimal(carryOverIds[j]);

                            User_Training_Transaction trainingToCreate = new User_Training_Transaction();
                            User_Training_Status trainingStatusToCreate = new User_Training_Status();


                            //Getting old records to update after carry over and creating new carried over records with same values as before
                            //User_Training_Transaction utt = ctx.User_Training_Transaction.Find(id);
                            //User_Training_Status uts = ctx.User_Training_Status.Where(x=>x.Training_Transaction_ID == id).First();
                            List<CarryOverTraining> userTraining = ctx.User_Training_Status.Include(x => x.User_Profile).Include(x => x.Category_Master)
                                    .Include(x => x.Activity_Master).Include(x => x.Sub_Activity_Master).Include(x => x.User_Training_Transaction)
                                    .Where(x => x.User_Training_Transaction.Id == id && x.Financial_Year == financialYear).Select(a => new CarryOverTraining()
                                    {
                                        TrainingStatusId = a.Id,
                                        TrainingTransactionId = a.User_Training_Transaction.Id,
                                        User_Id = a.User_Id,
                                        Date = a.User_Training_Transaction.Date,
                                        State_Id = a.User_Training_Transaction.State_Id,
                                        Category_Id = a.Category_Id,
                                        Activity_Id = a.Activity_Id,
                                        SubActivity_Id = a.SubActivity_Id,
                                        Hours = a.User_Training_Transaction.Hours,
                                        Provider = a.User_Training_Transaction.Provider,
                                        Financial_Year = a.Financial_Year,
                                        Your_Role = a.User_Training_Transaction.Your_Role,
                                        Forwardable = a.User_Training_Transaction.Forwardable,
                                        Has_been_Forwarded = a.User_Training_Transaction.Has_been_Forwarded,
                                        Descrption = a.User_Training_Transaction.Descrption,
                                        UploadedFile = a.User_Training_Transaction.UploadedFile,
                                        UploadedFileName = a.User_Training_Transaction.UploadedFileName,
                                        Received_By_Forwarding = a.Received_By_Forwarding,
                                        Units_Done = a.Units_Done,
                                        Min_Required_Category_Units = a.Min_Required_Category_Units
                                    }).AsEnumerable().ToList();

                            decimal? activityId = userTraining.Select(y => y.Activity_Id).First();
                            decimal? categoryId = userTraining.Select(y => y.Category_Id).First();
                            decimal? subactivityId = userTraining.Select(y => y.SubActivity_Id).First();
                            DateTime? date = userTraining.Select(y => y.Date).First();

                            //getting hour-units rule to create new hours done according to seleted units
                            IEnumerable<StateActivity__with_Rule2> hourUnits = ctx.StateActivity__with_Rule2
                                               .Include(x => x.Rule2_Master)
                                               .Where(x => x.ActivityID == activityId && x.StateID == enrolledStateId);

                            string hourRule = hourUnits.Select(x => x.Rule2_Master.Name).First();


                            #region Old carried over records
                            ///Getting old carried over record if any so that we can check if 
                            ///same carried over record is already is in database so that 
                            ///we will update old carried over record or we will create a new one.


                            List<User_Training_Status> oldCarriedOverRecord = ctx.User_Training_Status.Where(x => x.Financial_Year == nextFinancialYear
                                    && x.Activity_Id == activityId && x.SubActivity_Id == subactivityId && x.Category_Id == categoryId
                                    && x.User_Training_Transaction.Date == date && x.User_Id == userId
                                    && x.Received_By_Forwarding == "Yes").ToList();

                            if (oldCarriedOverRecord.Count > 0)//Case to update old carry over record.
                            {
                                decimal statusId = oldCarriedOverRecord.Select(x => x.Id).First();
                                decimal transactionId = oldCarriedOverRecord.Select(x => x.Training_Transaction_ID).First();
                                User_Training_Transaction oldCarriedRecord = ctx.User_Training_Transaction.Find(transactionId);
                                User_Training_Status oldCarriedStatusRecord = ctx.User_Training_Status.Find(statusId);

                                //Creating new record i.e carried over record
                                if (hourRule == "1 Unit = 1 Hr")
                                {
                                    if (carryOverUnitsList[j].Contains(".5"))
                                    {
                                        result = "Error - .5 in this category, not allowed";
                                        transaction.Dispose();
                                        goto ErrorCase;
                                    }
                                    else
                                    {
                                        oldCarriedRecord.Hours = oldCarriedRecord.Hours + Convert.ToDecimal(carryOverUnitsList[j]);
                                        oldCarriedStatusRecord.Units_Done =
                                            oldCarriedStatusRecord.Units_Done + Convert.ToDecimal(carryOverUnitsList[j]);

                                        hoursRemoved = unitsRemoved = Convert.ToDecimal(carryOverUnitsList[j]);
                                    }
                                }

                                if (hourRule == "1 Unit = 2 Hr")
                                {
                                    oldCarriedRecord.Hours = oldCarriedRecord.Hours + (Convert.ToDecimal(carryOverUnitsList[j]) * 2);
                                    oldCarriedStatusRecord.Units_Done =
                                        oldCarriedStatusRecord.Units_Done + Convert.ToDecimal(carryOverUnitsList[j]);

                                    hoursRemoved = Convert.ToDecimal(carryOverUnitsList[j]) * 2;
                                    unitsRemoved = Convert.ToDecimal(carryOverUnitsList[j]);
                                }

                                ctx.Entry(oldCarriedStatusRecord).State = EntityState.Modified;
                                ctx.SaveChanges();

                                ctx.Entry(oldCarriedRecord).State = EntityState.Modified;
                                ctx.SaveChanges();

                                decimal? allTraningUnitss = ctx.User_Training_Status
                                    .Where(x => x.User_Id == userId && x.Financial_Year == financialYear).Select(x => x.Units_Done).Sum();

                                if (allTraningUnitss < 10)
                                {
                                    result = "Error - minimum";
                                    transaction.Dispose();
                                    goto ErrorCase;
                                }

                                result = "Success";
                            }
                            #endregion

                            else
                            {
                                //Creating new record i.e carried over record
                                if (hourRule == "1 Unit = 1 Hr")
                                {
                                    if (carryOverUnitsList[j].Contains(".5"))
                                    {
                                        result = "Error - .5 in this category, not allowed";
                                        transaction.Dispose();
                                        goto ErrorCase;
                                    }
                                    else
                                    {
                                        trainingToCreate.Hours = trainingStatusToCreate.Units_Done = Convert.ToDecimal(carryOverUnitsList[j]);
                                        hoursRemoved = unitsRemoved = Convert.ToDecimal(carryOverUnitsList[j]);
                                    }
                                }

                                if (hourRule == "1 Unit = 2 Hr")
                                {
                                    trainingToCreate.Hours = Convert.ToDecimal(carryOverUnitsList[j]) * 2;
                                    trainingStatusToCreate.Units_Done = Convert.ToDecimal(carryOverUnitsList[j]);

                                    hoursRemoved = Convert.ToDecimal(carryOverUnitsList[j]) * 2;
                                    unitsRemoved = Convert.ToDecimal(carryOverUnitsList[j]);
                                }

                                trainingToCreate.Activity_Id = userTraining.Select(y => y.Activity_Id).First();
                                trainingToCreate.Category_Id = userTraining.Select(y => y.Category_Id).First();
                                trainingToCreate.Date = userTraining.Select(y => y.Date).First();
                                trainingToCreate.Descrption = userTraining.Select(y => y.Descrption).First();
                                trainingToCreate.Financial_Year = nextFinancialYear;
                                trainingToCreate.Forwardable = null;
                                trainingToCreate.Has_been_Forwarded = null;
                                trainingToCreate.Provider = userTraining.Select(y => y.Provider).First();
                                trainingToCreate.State_Id = userTraining.Select(y => y.State_Id).First();
                                trainingToCreate.SubActivity_Id = userTraining.Select(y => y.SubActivity_Id).First();
                                trainingToCreate.UploadedFile = userTraining.Select(y => y.UploadedFile).First();
                                trainingToCreate.UploadedFileName = userTraining.Select(y => y.UploadedFileName).First();
                                trainingToCreate.User_Id = userTraining.Select(y => y.User_Id).First();
                                trainingToCreate.Your_Role = userTraining.Select(y => y.Your_Role).First();

                                ctx.User_Training_Transaction.Add(trainingToCreate);
                                ctx.SaveChanges();

                                decimal newly_inserted_id = trainingToCreate.Id; //New carried over record insertion successful.

                                //Now inserting new carried over record in training_transaction_status
                                trainingStatusToCreate.Activity_Id = userTraining.Select(y => y.Activity_Id).First();
                                trainingStatusToCreate.Category_Id = userTraining.Select(y => y.Category_Id).First();
                                trainingStatusToCreate.Financial_Year = nextFinancialYear;
                                trainingStatusToCreate.Min_Required_Category_Units = userTraining.Select(y => y.Min_Required_Category_Units).First();
                                trainingStatusToCreate.Received_By_Forwarding = "Yes";
                                trainingStatusToCreate.SubActivity_Id = userTraining.Select(y => y.SubActivity_Id).First();
                                trainingStatusToCreate.Training_Transaction_ID = newly_inserted_id;
                                trainingStatusToCreate.User_Id = userTraining.Select(y => y.User_Id).First();

                                ctx.User_Training_Status.Add(trainingStatusToCreate);
                                ctx.SaveChanges();
                            }

                            //Carried over records insertion over. Now update part.

                            //Editing units and hours of old record to update accordingly.
                            //First User_Training_Transaction
                            User_Training_Transaction utt = ctx.User_Training_Transaction.Find(userTraining.Select(y => y.TrainingTransactionId).First());
                            utt.Hours = utt.Hours - hoursRemoved;
                            utt.Has_been_Forwarded = "Yes";
                            ctx.Entry(utt).State = EntityState.Modified;
                            ctx.SaveChanges();

                            //Now User_Training_Status
                            User_Training_Status uts = ctx.User_Training_Status.Find(userTraining.Select(y => y.TrainingStatusId).First());
                            uts.Units_Done = uts.Units_Done - unitsRemoved;                            

                            //if (uts.Units_Done == 0.5m || uts.Units_Done < 1)
                            //{
                            //    result = "Error - minimum";
                            //    transaction.Dispose();
                            //    goto ErrorCase;
                            //}

                            ctx.Entry(uts).State = EntityState.Modified;
                            ctx.SaveChanges();

                            decimal? existedCompletedCategories = ctx.User_Training_Status
                                .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                                .Where(x => x.Category_Id == uts.Category_Id
                                 && x.Financial_Year == financialYear
                                 && x.User_Id == userId).Select(x => x.Units_Done).Sum();

                            if (existedCompletedCategories < 1)
                            {
                                result = "Error - minimum";
                                transaction.Dispose();
                                goto ErrorCase;
                            }

                            decimal? allTraningUnits = ctx.User_Training_Status
                                    .Where(x => x.User_Id == userId && x.Financial_Year == financialYear).Select(x => x.Units_Done).Sum();

                            if (allTraningUnits < 10)
                            {
                                result = "Error - minimum";
                                transaction.Dispose();
                                goto ErrorCase;
                            }

                            result = "Success";
                        }
                        transaction.Commit();
                    }
                }
            }
            catch (Exception ee)
            {
                result = ee.ToString();
            }

        //TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
        //return RedirectToAction("Index
        ErrorCase:

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }



        private string GetNewFinancialYear(string financialYear)
        {
            string newFinancialYear = string.Empty;
            try
            {
                int preYear = Convert.ToInt32(financialYear.Split('-')[0]);
                int currentYear = Convert.ToInt32(financialYear.Split('-')[1]);

                newFinancialYear = Convert.ToString(currentYear) + "-" + Convert.ToString(currentYear + 1);
            }
            catch (Exception)
            {
                //throw;
            }
            return newFinancialYear;
        }
    }
}