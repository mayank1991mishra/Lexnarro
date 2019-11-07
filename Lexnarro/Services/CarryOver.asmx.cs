using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web.Services;
using System.Web.Mvc;
using static Lexnarro.Services.Training;
using Lexnarro.HelperClasses;
using Lexnarro.ViewModels;

namespace Lexnarro.Services
{
    /// <summary>
    /// Summary description for CarryOver
    /// </summary>
    [WebService(Namespace = "http://www.lexnarro.com.au/services/carryOver.asmx",
          Description = "<font color='#a31515' size='3'><b>This web service carry records forward from one CPD year to next CPD year.</b></font>")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class CarryOver : System.Web.Services.WebService
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        [WebMethod]
        public ReturnDataNew GetCarryOverRecords(string stateId, string finYear, string User_Id)
        {
            ReturnDataNew rd = new ReturnDataNew();
            decimal state_Id = Convert.ToDecimal(stateId);
            decimal userID = Convert.ToDecimal(User_Id);

            decimal? carryOverUnits = db.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                    .Where(x => x.StateID == state_Id).Select(x => x.Rule3_Master.CarryOverUnits).First();

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
            #endregion

            if (carryOverUnits == 0)
            {
                rd.Status = "Failure";
                rd.Message = "This state does not allow carry over.";
                rd.Requestkey = "GetCarryOverRecords";
                rd.MaximumUnitsAllowed = "0";
                rd.FinancialYear = fy;
                return rd;
            }

            try
            {
                string nextFinYear = GetNewFinancialYear(finYear);
                var carriedOverUnits = db.User_Training_Status.Where(x => x.Financial_Year == nextFinYear
                                           && x.User_Id == userID && x.Received_By_Forwarding == "Yes").Select(x => x.Units_Done).Sum();

                decimal? CarryOverUnits = carryOverUnits - (carriedOverUnits == null ? 0 : carriedOverUnits);

                

                if (carriedOverUnits >= carryOverUnits)
                {
                    //Send info no records/units for carry over found or all records/units has been carried over.
                    rd.Status = "Failure";
                    rd.Message = "No records/units for carry over found or all records/units has been carried over";
                    rd.Requestkey = "GetCarryOverRecords";
                    rd.MaximumUnitsAllowed = "0";
                    rd.FinancialYear = fy;
                    return rd;
                }
                else
                {
                    List<User_Training_Status> userTraining = db.User_Training_Status
                                               .Include(x => x.User_Training_Transaction)
                                               .Where(x => x.User_Training_Transaction.Forwardable == "Yes" &&
                                               x.Units_Done != 0 && x.Financial_Year == finYear &&
                                               x.User_Id == userID && x.User_Training_Transaction.State_Id == state_Id)
                                               .OrderByDescending(x => x.User_Training_Transaction.Date).ToList();

                    if (userTraining.Count <= 0)
                    {
                        rd.Status = "Failure";
                        rd.Message = "No records/units for carry over found or all records/units has been carried over";
                        rd.Requestkey = "GetCarryOverRecords";
                        rd.MaximumUnitsAllowed = "0";
                        rd.FinancialYear = fy;
                        return rd;
                    }

                    decimal totalUnits = 0;

                    foreach (User_Training_Status row in userTraining)
                    {
                        totalUnits += Convert.ToDecimal(row.Units_Done);
                    }

                    

                    List<UserTrainingTransaction> sd = new List<UserTrainingTransaction>();
                    foreach (var row in userTraining)
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

                        var unitsDone = row.Units_Done;


                        training.CategoryId = row.Category_Id;
                        training.CategoryName = row.User_Training_Transaction.Category_Master.Name;
                        training.FirstName = row.User_Training_Transaction.User_Profile.FirstName;
                        training.Id = row.Id;
                        training.TransactionId = row.Training_Transaction_ID;
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

                        rd.Status = "Success";
                        rd.Message = "Carry over records found";
                        rd.Requestkey = "GetCarryOverRecords";
                        rd.MaximumUnitsAllowed = CarryOverUnits.ToString();
                        rd.FinancialYear = fy;
                    }
                    return rd;
                }
            }
            catch (Exception y)
            {
                rd.Status = "Failure";
                rd.Message = "Something went wrong. Please try after some time." + y.ToString();
                rd.Requestkey = "DetailTraining";
                rd.UserTraining = null;
                rd.MaximumUnitsAllowed = carryOverUnits.ToString();
                rd.FinancialYear = fy;
                return rd;
            }
        }



        [WebMethod]
        public ReturnData DoCarryOver(string financialYear, string ids, string units, string userId, string stateId)
        {
            decimal unts = 0;
            string result = string.Empty;
            decimal user_Id = Convert.ToDecimal(userId);
            decimal enrolledStateId = Convert.ToDecimal(stateId);

            decimal? hoursRemoved = 0;
            decimal? unitsRemoved = 0;

            ReturnData rd = new ReturnData();

            try
            {
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
                                //result = "Error - Please select units more then 0 (zero)";
                                transaction.Dispose();
                                rd.Status = "Error";
                                rd.Message = "Please select units more then 0 (Zero)";
                                rd.Requestkey = "DoCarryOver";
                                return rd;
                            }
                            unts = unts + Convert.ToDecimal(item);
                        }

                        if (unts > carryOverUnits)
                        {
                            transaction.Dispose();
                            rd.Status = "Error";
                            rd.Message = "Selected units are more then the allowed units for carry over";
                            rd.Requestkey = "DoCarryOver";
                            return rd;
                        }

                        //if (unts > carryOverUnits)
                        //{
                        //    transaction.Dispose();
                        //    rd.Status = "Error";
                        //    rd.Message = "Selected units are greater then allowed no. of units";
                        //    rd.Requestkey = "DoCarryOver";
                        //    return rd;
                        //}

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
                                    && x.User_Training_Transaction.Date == date && x.User_Id == user_Id
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
                                        rd.Status = "Error";
                                        rd.Message = "*.5 units are not allowed in this category";
                                        rd.Requestkey = "DoCarryOver";
                                        return rd;
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

                                //var allTraningUnitss = ctx.User_Training_Status
                                //    .Where(x => x.User_Id == user_Id && x.Financial_Year == financialYear).Select(x => x.Units_Done).Sum();

                                //if (allTraningUnitss < 10)
                                //{
                                //    result = "Error - minimum";
                                //    transaction.Dispose();
                                //    rd.Status = "Error";
                                //    rd.Message = "Minimum 10 units should be left in this financial year.";
                                //    rd.Requestkey = "DoCarryOver";
                                //    return rd;
                                //}

                                //result = "Success";
                            }
                            #endregion

                            else
                            {
                                //Creating new record i.e carried over record
                                if (hourRule == "1 Unit = 1 Hr")
                                {
                                    if (carryOverUnitsList[j].Contains(".5"))
                                    {
                                        transaction.Dispose();
                                        rd.Status = "Error";
                                        rd.Message = "*.5 units are not allowed in this category";
                                        rd.Requestkey = "DoCarryOver";
                                        return rd;
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

                            //if (uts.Units_Done == 0.5m)
                            //{
                            //    transaction.Dispose();
                            //    rd.Status = "Error";
                            //    rd.Message = "Less then minimum allowed units(1) left after this carry over which is not allowed";
                            //    rd.Requestkey = "DoCarryOver";
                            //    return rd;
                            //}

                            ctx.Entry(uts).State = EntityState.Modified;
                            ctx.SaveChanges();

                            decimal? existedCompletedCategories = ctx.User_Training_Status
                               .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                               .Where(x => x.Category_Id == uts.Category_Id
                                && x.Financial_Year == financialYear
                                && x.User_Id == user_Id).Select(x => x.Units_Done).Sum();

                            if (existedCompletedCategories < 1)
                            {
                                result = "Error - minimum";
                                transaction.Dispose();
                                rd.Status = "Error";
                                rd.Message = "Less then minimum allowed units(i.e 1) left after this carry over which is not allowed";
                                rd.Requestkey = "DoCarryOver";
                                return rd;
                            }

                            var allTraningUnitss = ctx.User_Training_Status
                                    .Where(x => x.User_Id == user_Id && x.Financial_Year == financialYear).Select(x => x.Units_Done).Sum();

                            if (allTraningUnitss < 10)
                            {
                                result = "Error - minimum";
                                transaction.Dispose();
                                rd.Status = "Error";
                                rd.Message = "Minimum 10 units should be left in this financial year.";
                                rd.Requestkey = "DoCarryOver";
                                return rd;
                            }

                            result = "Success";
                        }

                        transaction.Commit();

                        rd.Status = "Success";
                        rd.Message = "Selected units are carried over successfully.";
                        rd.Requestkey = "DoCarryOver";
                        return rd;
                    }
                }
            }
            catch (Exception ee)
            {
                rd.Status = "Error";
                rd.Message = "Something went wrong." + ee.ToString();
                rd.Requestkey = "DoCarryOver";
                return rd;
            }           
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
                throw;
            }
            return newFinancialYear;
        }
    }

    public class ReturnDataNew
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public string Requestkey { get; set; }
        public string MaximumUnitsAllowed{ get; set; }
        public List<UserTrainingTransaction> UserTraining { get; set; }
        public List<SelectListItem> FinancialYear { get; set; }
    }
}
