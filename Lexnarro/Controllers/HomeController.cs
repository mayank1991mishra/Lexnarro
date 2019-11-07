using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly LaxNarroEntities db = null;


        public HomeController()
        {
            db = new LaxNarroEntities();
        }


        public ActionResult Index(string financialYear)
        {
            string Role = UserHelper.GetUserRole();

            //CarryOver();

            if (Role != "Admin")
            {
                decimal userId = Convert.ToDecimal(UserHelper.GetUserId());
                //IQueryable<Plan_Master> plan_master = from s in db.Plan_Master
                //                                      where s.Plan != "Demo"
                //                                      select s;

                //ViewBag.PM = plan_master;

                string finYear = string.Empty;

                if (financialYear != null)
                    finYear = financialYear;
                else
                    finYear = GetFinancialYear();

                //---Days left in CPD Yaer---
                int year = Convert.ToInt32(GetFinancialYear().Split('-')[1]);

                DateTime cpdEndDate = new DateTime(year, 03, 31);

                ViewBag.totalDays = (cpdEndDate - DateTime.Today).Days;
                //---------------------------

                decimal? totalUnits = 0;

                decimal GetUserStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());


                List<MyCategory> totalCategories = (from s in db.State_Category_With_Rule4_Mapping
                                                    join c in db.Category_Master on s.CategoryID equals c.ID
                                                    where s.StateID == GetUserStateId && s.Status == "Active"
                                                    select new MyCategory
                                                    {
                                                        CategoryID = s.CategoryID,
                                                        Category_Name = c.Name,
                                                        ShortName = c.ShortName
                                                    }).ToList();

                ViewBag.TotalCat = totalCategories;

                List<ExistingCategory> swe = new List<ExistingCategory>();

                foreach (MyCategory d in totalCategories)
                {
                    decimal? existedCompletedCategories = db.User_Training_Status
                        .Include(x => x.Category_Master).Include(x => x.User_Training_Transaction)
                        .Where(x => x.Category_Id == d.CategoryID
                         && x.Financial_Year == finYear 
                         && x.User_Id == userId).Select(x => x.Units_Done).Sum();

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

                ViewBag.TotalUnits = totalUnits;

                ViewBag.Cat = swe;

                List<SelectListItem> fy = new List<SelectListItem>();

                fy = (from s in db.User_Training_Transaction
                      where s.User_Id == userId
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
            }
            if (Role == "Admin")
            {
                System.Threading.Thread th = new System.Threading.Thread(CheckAndSendReminders);
                th.Start();
            }
            return View();
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


        private void CheckAndSendReminders()
        {
            try
            {
                DateTime dt = DateTime.Today;
                Plan_Master plan_id = db.Plan_Master.FirstOrDefault(x => x.Plan == "Demo");
                decimal planID = Convert.ToDecimal(plan_id.Plan_ID);
                List<decimal> totalUserList = (from s in db.User_Transaction_Master
                                               where s.PlanID == planID && s.End_Date > dt
                                               select s.User_ID).ToList();

                List<string> firstReminderUserList = new List<string>();
                List<string> secondReminderUserList = new List<string>();
                List<string> thirdReminderUserList = new List<string>();
                List<string> fourthReminderUserList = new List<string>();
                List<string> fifithReminderUserList = new List<string>();
                List<string> sixthReminderUserList = new List<string>();

                #region CHECK REMINDER STATUS

                foreach (decimal item in totalUserList)
                {

                    List<MailLog> userRow = (from s in db.MailLogs
                                             where (s.UserID == item && s.LastReminderStatus != "Deactive")
                                             select s).ToList();


                    if (userRow.Count > 0)
                    {
                        string status = "";
                        if (userRow[0].LastReminderStatus != null)
                        {
                            status = userRow[0].LastReminderStatus.ToString();

                        }
                        else
                        {
                            status = "";
                        }
                        foreach (MailLog x in userRow)
                        {
                            switch (status)
                            {
                                case "":
                                    if (x.ReminderOne <= dt)
                                    {
                                        firstReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderTwo <= dt)
                                    {
                                        secondReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderThree <= dt)
                                    {
                                        thirdReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFour <= dt)
                                    {
                                        fourthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFive <= dt)
                                    {
                                        fifithReminderUserList.Add(x.UserID.ToString());

                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;


                                case "2":

                                    if (x.ReminderTwo <= dt)
                                    {
                                        secondReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderThree <= dt)
                                    {
                                        thirdReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFour <= dt)
                                    {
                                        fourthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFive <= dt)
                                    {
                                        fifithReminderUserList.Add(x.UserID.ToString());

                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;

                                case "3":

                                    if (x.ReminderThree <= dt)
                                    {
                                        thirdReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFour <= dt)
                                    {
                                        fourthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFive <= dt)
                                    {
                                        fifithReminderUserList.Add(x.UserID.ToString());

                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;

                                case "4":

                                    if (x.ReminderFour <= dt)
                                    {
                                        fourthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderFive <= dt)
                                    {
                                        fifithReminderUserList.Add(x.UserID.ToString());

                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;

                                case "5":

                                    if (x.ReminderFive <= dt)
                                    {
                                        fifithReminderUserList.Add(x.UserID.ToString());

                                    }
                                    else
                                    {
                                        break;
                                    }
                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;

                                case "6":

                                    if (x.ReminderSix <= dt)
                                    {
                                        sixthReminderUserList.Add(x.UserID.ToString());
                                    }
                                    else
                                    {
                                        break;
                                    }
                                    break;

                            }
                        }

                    }

                }
                #endregion

                #region SEND MAIL                

                SendMail(firstReminderUserList, "2");
                SendMail(secondReminderUserList, "3");
                SendMail(thirdReminderUserList, "4");
                SendMail(fourthReminderUserList, "5");
                SendMail(fifithReminderUserList, "6");
                SendMail(sixthReminderUserList, "7");
                #endregion

            }
            catch (Exception)
            {
                //throw;
            }
        }


        private string SendMail(string firstName, string lastName, string emailID, int? days, DateTime? expirationDate, string emailTemplate)
        {
            string result = "";
            try
            {
                Mail mailHelper = new Mail();

                string body = string.Empty;

                using (StreamReader reader = new StreamReader(Server.MapPath(emailTemplate)))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("{name}", firstName + " " + lastName);
                body = body.Replace("{Email}", emailID);

                if (days != null && expirationDate != null)
                {
                    body = body.Replace("{expirationDate}", Convert.ToString(expirationDate));
                    if (days > 0)
                    {
                        body = body.Replace("{days}", Convert.ToString("will expire in " + days));
                        body = body.Replace("{ExtraInfo}", Convert.ToString("Your account will be locked after 7 days of expiration date."));
                    }
                    else
                    {
                        body = body.Replace("{days}", Convert.ToString("has expired on " + days));
                        body = body.Replace("{ExtraInfo}", Convert.ToString(""));
                    }
                }


                //MailMessage mail = new MailMessage();
                //mail.To.Add(emailID);
                //mail.From = new MailAddress("jajmail.testing@gmail.com");
                //mail.Subject = "Lexnarro - sample subject";

                //mail.Body = body;
                //mail.IsBodyHtml = true;

                //SmtpClient smtp = new SmtpClient();
                //smtp.Host = "smtp.gmail.com";
                //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                //smtp.Port = 587;
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = false;
                //smtp.Credentials = new System.Net.NetworkCredential("jajmail.testing@gmail.com", "testing@1234");
                //smtp.Send(mail);

                MailMessage mail = new MailMessage();
                mail.To.Add(emailID);
                mail.From = new MailAddress("mail@lexnarro.com.au");
                mail.Subject = "Lexnarro - Reminder";

                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.lexnarro.com.au";
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Port = 25;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential("mail@lexnarro.com.au", "lexnarro@123");
                smtp.Send(mail);

                result = "success";

                result = "success";
            }
            catch (Exception)
            {
                result = "";
            }

            return result;
        }


        private void SendMail(List<string> userIds, string status)
        {
            try
            {

                foreach (string item in userIds)
                {
                    decimal id = Convert.ToDecimal(item);
                    List<User_Profile> GetUserData = (db.User_Profile.Where(u => u.ID == id)).ToList();
                    string mailResult = "";
                    if (status == "2")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName,
                            GetUserData[0].EmailAddress, 1, DateTime.Today, "~/EmailTemplate/firstMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                    if (status == "3")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName, GetUserData[0].EmailAddress,
                            1, DateTime.Today, "~/EmailTemplate/secondMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                    if (status == "4")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName, GetUserData[0].EmailAddress,
                            1, DateTime.Today, "~/EmailTemplate/thirdMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                    if (status == "5")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName, GetUserData[0].EmailAddress,
                            1, DateTime.Today, "~/EmailTemplate/fourthMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                    if (status == "6")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName, GetUserData[0].EmailAddress,
                            1, DateTime.Today, "~/EmailTemplate/fifthMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                    if (status == "7")
                    {
                        mailResult = SendMail(GetUserData[0].FirstName, GetUserData[0].LastName, GetUserData[0].EmailAddress,
                            1, DateTime.Today, "~/EmailTemplate/sixthMail.html");
                        if (mailResult == "Success")
                        {
                            UpdateMailLog(item, status);
                        }
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }


        private void UpdateMailLog(string userID, string statusValue)
        {
            try
            {
                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    decimal id = Convert.ToDecimal(userID);
                    MailLog user = ctx.MailLogs.FirstOrDefault(x => x.UserID == id);
                    user.LastReminderStatus = statusValue;
                    ctx.SaveChanges();
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }


        private void CarryOver()
        {
            try
            {
                decimal userId = Convert.ToDecimal(UserHelper.GetUserId());
                decimal enrolledStateId = Convert.ToDecimal(UserHelper.GetUserStateEnrolledId());
                //string enrolledStateName = UserHelper.GetUserStateEnrolledName();

                using (LaxNarroEntities ctx = new LaxNarroEntities())
                {
                    using (DbContextTransaction transaction = ctx.Database.BeginTransaction())
                    {

                        User_Profile userRecord = ctx.User_Profile.Find(userId);

                        //How many units are allowed to carry over
                        decimal? carryOverUnits = ctx.State_Rule3_Marriage.Include(x => x.Rule3_ID)
                            .Where(x => x.StateID == enrolledStateId).Select(x => x.Rule3_Master.CarryOverUnits).First();

                        //decimal? carryOverUnits = 100;//For testing infinte carryover

                        if (carryOverUnits == 0)
                        {
                            //No carry over allowed for some states
                            return;
                        }

                        //For old records if any....current_financial_year is null should update it current financial year
                        if (userRecord.Current_Financial_year == null)
                        {
                            userRecord.Current_Financial_year = GetFinancialYear();
                            ctx.Entry(userRecord).State = EntityState.Modified;
                            ctx.SaveChanges();
                            transaction.Commit();
                        }
                        else
                        {
                            int currentFinancialYearStart = Convert.ToInt32(GetFinancialYear().Split('-')[0]);
                            int currentFinancialYearEnd = Convert.ToInt32(GetFinancialYear().Split('-')[1]);

                            int databaseFinancialYearStart = Convert.ToInt32(userRecord.Current_Financial_year.Split('-')[0]);
                            int databaseFinancialYearEnd = Convert.ToInt32(userRecord.Current_Financial_year.Split('-')[1]);

                            if (false)//(GetFinancialYear() == userRecord.Current_Financial_year)
                            {
                                //No carry over should occur since the financial year is same
                            }
                            else if (true)//(databaseFinancialYearStart < currentFinancialYearStart) //Do carry over here.
                            {
                                List<User_Training_Status> userTrainingTransaction = ctx.User_Training_Status
                                    .Include(x=>x.User_Training_Transaction)
                                    .Include(x => x.Activity_Master).Include(x => x.Category_Master).Include(x => x.Sub_Activity_Master)
                                    .Where(x => x.Financial_Year == userRecord.Current_Financial_year)
                                    .Where(x => x.User_Id == userId).OrderByDescending(x => x.User_Training_Transaction.Date).ToList();

                                decimal? totalUnitsDone = 0;

                                foreach (User_Training_Status item in userTrainingTransaction)
                                {
                                    totalUnitsDone += item.Units_Done;
                                }

                                if (totalUnitsDone < 10)
                                {
                                    //No carryover. Minimum criteria of minimum 10 units is not fullfiled.
                                }
                                else
                                {
                                    bool unitsDoneStatus = false;
                                    //Getting all categories of the state enrolled
                                    List<MyCategory> totalCategories = (from s in ctx.State_Category_With_Rule4_Mapping
                                                                        join c in ctx.Category_Master on s.CategoryID equals c.ID
                                                                        where s.StateID == enrolledStateId && s.Status == "Active"
                                                                        select new MyCategory
                                                                        {
                                                                            CategoryID = s.CategoryID,
                                                                            Category_Name = c.Name,
                                                                            ShortName = c.ShortName
                                                                        }).OrderBy(x => x.ShortName).ToList();

                                    //checking if all the categories has been completed for carryover
                                    foreach (MyCategory item in totalCategories)
                                    {
                                        //Another minimum criteria that user should have completed at least one unit in every category
                                        List<decimal?> unitsDone = userTrainingTransaction.Where(x => x.Category_Id == item.CategoryID)
                                            .Select(x => x.Units_Done).ToList();

                                        if (unitsDone.Count >= 1)
                                            unitsDoneStatus = true;
                                        else
                                        {
                                            unitsDoneStatus = false;
                                            break;
                                        }
                                    }

                                    if (unitsDoneStatus)
                                    {
                                        List<User_Training_Status> transferableRecords = ctx.User_Training_Status
                                        .Include(x => x.User_Training_Transaction)
                                        .Where(x => x.User_Training_Transaction.Forwardable == "Yes" &&
                                        x.User_Training_Transaction.Has_been_Forwarded == null && x.User_Id == userId)
                                        .OrderByDescending(x => x.User_Training_Transaction.Date).ToList();

                                        decimal? transferableUnits = totalUnitsDone - 10;

                                        decimal? unitsLeftToTransfer = 0;

                                        if (transferableRecords.Count > 0)
                                        {
                                            if (carryOverUnits == 3)
                                            {
                                                #region Case When Transferable units are 3
                                                for (int i = 0; i < transferableRecords.Count; i++)
                                                {
                                                    decimal oldTrainingTransactionId = transferableRecords[i].User_Training_Transaction.Id;
                                                    decimal oldTrainingStatusId = transferableRecords[i].Id;

                                                    User_Training_Status record = transferableRecords[i];

                                                    User_Training_Transaction oldTraining = ctx.User_Training_Transaction
                                                        .Where(x => x.Id == record.Training_Transaction_ID).First();

                                                    IEnumerable<StateActivity__with_Rule2> hourUnits = ctx.StateActivity__with_Rule2
                                                        .Include(x => x.Rule2_Master)
                                                        .Where(x => x.ActivityID == record.Activity_Id
                                                        && x.StateID == record.User_Training_Transaction.State_Id).AsEnumerable();

                                                    string hourRule = hourUnits.Select(x => x.Rule2_Master.Name).First();

                                                    User_Training_Transaction utt = new User_Training_Transaction();
                                                    User_Training_Status uts = new User_Training_Status();

                                                    User_Training_Transaction oldUtt = ctx.User_Training_Transaction
                                                           .Find(oldTrainingTransactionId);

                                                    User_Training_Status oldUts = ctx.User_Training_Status
                                                            .Find(oldTrainingStatusId);


                                                    #region When Units Done is Greater Then 3
                                                    if (record.Units_Done >= 3)
                                                    {
                                                        //calculating hours and units for new record
                                                        if (unitsLeftToTransfer == 0)
                                                            uts.Units_Done = 3;
                                                        else
                                                            uts.Units_Done = unitsLeftToTransfer;

                                                        if (hourRule == "1 Unit = 1 Hr")
                                                        {
                                                            utt.Hours = uts.Units_Done;
                                                            //oldUtt.Hours = record.User_Training_Transaction.Hours - uts.Units_Done;

                                                            if (unitsLeftToTransfer != 0)
                                                            {
                                                                oldUtt.Hours = record.User_Training_Transaction.Hours - uts.Units_Done;
                                                                oldUtt.Forwardable = null;
                                                                oldUtt.Has_been_Forwarded = null;
                                                            }
                                                            else
                                                                oldUtt.Has_been_Forwarded = "Yes";
                                                        }
                                                        if (hourRule == "1 Unit = 2 Hr")
                                                        {
                                                            utt.Hours = uts.Units_Done * 2;
                                                            //oldUtt.Hours = record.User_Training_Transaction.Hours - (uts.Units_Done * 2);

                                                            if (unitsLeftToTransfer != 0)
                                                            {
                                                                oldUtt.Hours = record.User_Training_Transaction.Hours - (uts.Units_Done * 2);
                                                                oldUtt.Forwardable = null;
                                                                oldUtt.Has_been_Forwarded = null;
                                                            }
                                                            else
                                                                oldUtt.Has_been_Forwarded = "Yes";
                                                        }


                                                        //Inserting new carry over record into user_Training_transaction
                                                        utt.Activity_Id = record.Activity_Id;
                                                        utt.Category_Id = record.Category_Id;
                                                        utt.Date = record.User_Training_Transaction.Date;
                                                        utt.Descrption = record.User_Training_Transaction.Descrption;
                                                        utt.Financial_Year = "2019-2020";//GetFinancialYear();
                                                        utt.Forwardable = null;
                                                        utt.Has_been_Forwarded = null;
                                                        //utt.Hours = record.User_Training_Transaction.Hours;
                                                        utt.Provider = record.User_Training_Transaction.Provider;
                                                        utt.State_Id = record.User_Training_Transaction.State_Id;
                                                        utt.SubActivity_Id = record.SubActivity_Id;
                                                        utt.UploadedFile = record.User_Training_Transaction.UploadedFile;
                                                        utt.UploadedFileName = record.User_Training_Transaction.UploadedFileName;
                                                        utt.User_Id = record.User_Id;
                                                        utt.Your_Role = record.User_Training_Transaction.Your_Role;
                                                        utt.UploadedFile = record.User_Training_Transaction.UploadedFile;
                                                        utt.UploadedFileName = record.User_Training_Transaction.UploadedFileName;

                                                        ctx.User_Training_Transaction.Add(utt);
                                                        ctx.SaveChanges();

                                                        decimal newly_inserted_id = utt.Id;

                                                        uts.Activity_Id = record.Activity_Id;
                                                        uts.Category_Id = record.Category_Id;
                                                        uts.Financial_Year = "2019-2020";//GetFinancialYear();
                                                        uts.Min_Required_Category_Units = record.Min_Required_Category_Units;
                                                        uts.Received_By_Forwarding = "Yes";
                                                        uts.SubActivity_Id = record.SubActivity_Id;
                                                        uts.Training_Transaction_ID = newly_inserted_id;
                                                        uts.User_Id = record.User_Id;

                                                        ctx.User_Training_Status.Add(uts);
                                                        ctx.SaveChanges();

                                                        //Updating User_Training_Transaction old record
                                                       
                                                        //oldUtt.Has_been_Forwarded = "Yes";
                                                        ctx.Entry(oldUtt).State = EntityState.Modified;
                                                        ctx.SaveChanges();

                                                        //Updating User_Training_Status old record
                                                        oldUts.Units_Done = record.Units_Done - unitsLeftToTransfer;
                                                        ctx.Entry(oldUts).State = EntityState.Modified;
                                                        ctx.SaveChanges();

                                                        unitsLeftToTransfer = 0;
                                                        break;
                                                    }
                                                    #endregion

                                                    //When units done is 1 or 2 
                                                    #region When Units Done is 1 or 2
                                                    if (record.Units_Done < 3)
                                                    {
                                                        //calculating hours and units for new record
                                                        //uts.Units_Done = record.Units_Done;
                                                        //if (hourRule == "1 Unit = 1 Hr")
                                                        //{
                                                        //    utt.Hours = "" + record.Units_Done + "";
                                                        //}
                                                        //if (hourRule == "1 Unit = 2 Hr")
                                                        //{
                                                        //    utt.Hours = "" + record.Units_Done * 2 + "";
                                                        //}

                                                        if (unitsLeftToTransfer == 0)
                                                            uts.Units_Done = record.Units_Done;
                                                        else
                                                            uts.Units_Done = unitsLeftToTransfer;

                                                        if (hourRule == "1 Unit = 1 Hr")
                                                        {
                                                            utt.Hours = uts.Units_Done ;

                                                            if (unitsLeftToTransfer != 0)
                                                            {
                                                                oldUtt.Hours = record.User_Training_Transaction.Hours - uts.Units_Done;
                                                                oldUtt.Forwardable = null;
                                                                oldUtt.Has_been_Forwarded = null;
                                                            }
                                                            else
                                                                oldUtt.Has_been_Forwarded = "Yes";
                                                        }
                                                        if (hourRule == "1 Unit = 2 Hr")
                                                        {
                                                            utt.Hours = uts.Units_Done * 2 ;
                                                            if (unitsLeftToTransfer != 0)
                                                            {
                                                                oldUtt.Hours = record.User_Training_Transaction.Hours - (uts.Units_Done * 2);
                                                                oldUtt.Forwardable = null;
                                                                oldUtt.Has_been_Forwarded = null;
                                                            }
                                                            else
                                                                oldUtt.Has_been_Forwarded = "Yes";
                                                        }

                                                        //Inserting new carry over record
                                                        utt.Activity_Id = record.Activity_Id;
                                                        utt.Category_Id = record.Category_Id;
                                                        utt.Date = record.User_Training_Transaction.Date;
                                                        utt.Descrption = record.User_Training_Transaction.Descrption;
                                                        utt.Financial_Year = "2019-2020";//GetFinancialYear();
                                                        utt.Forwardable = null;
                                                        utt.Has_been_Forwarded = null;
                                                        //utt.Hours = record.User_Training_Transaction.Hours;
                                                        utt.Provider = record.User_Training_Transaction.Provider;
                                                        utt.State_Id = record.User_Training_Transaction.State_Id;
                                                        utt.SubActivity_Id = record.SubActivity_Id;
                                                        utt.UploadedFile = record.User_Training_Transaction.UploadedFile;
                                                        utt.UploadedFileName = record.User_Training_Transaction.UploadedFileName;
                                                        utt.User_Id = record.User_Id;
                                                        utt.Your_Role = record.User_Training_Transaction.Your_Role;

                                                        ctx.User_Training_Transaction.Add(utt);
                                                        ctx.SaveChanges();

                                                        decimal newly_inserted_id = utt.Id;

                                                        uts.Activity_Id = record.Activity_Id;
                                                        uts.Category_Id = record.Category_Id;
                                                        uts.Financial_Year = "2019-2020";//GetFinancialYear();
                                                        uts.Min_Required_Category_Units = record.Min_Required_Category_Units;
                                                        uts.Received_By_Forwarding = "Yes";
                                                        uts.SubActivity_Id = record.SubActivity_Id;
                                                        uts.Training_Transaction_ID = newly_inserted_id;
                                                        uts.User_Id = record.User_Id;

                                                        ctx.User_Training_Status.Add(uts);
                                                        ctx.SaveChanges();

                                                        //Updating User_Training_Transaction old record
                                                        //User_Training_Transaction oldUtt = ctx.User_Training_Transaction
                                                        //    .Find(oldTrainingTransactionId);
                                                        
                                                        ctx.Entry(oldUtt).State = EntityState.Modified;
                                                        ctx.SaveChanges();

                                                        //Updating User_Training_Status old record
                                                        //User_Training_Status oldUts = ctx.User_Training_Status
                                                        //    .Find(oldTrainingStatusId);
                                                        oldUts.Units_Done = record.Units_Done - unitsLeftToTransfer;
                                                        ctx.Entry(oldUts).State = EntityState.Modified;
                                                        ctx.SaveChanges();

                                                        unitsLeftToTransfer = 3 - uts.Units_Done;

                                                        if (unitsLeftToTransfer > 0)
                                                            continue;
                                                        else
                                                            break;
                                                    }
                                                    #endregion
                                                }

                                                userRecord.Current_Financial_year = "2019-2020";//GetFinancialYear();
                                                ctx.Entry(userRecord).State = EntityState.Modified;
                                                ctx.SaveChanges();

                                                transaction.Commit();
                                                #endregion
                                            }

                                            if (carryOverUnits == 100)
                                            {
                                                #region Case When Transferable Units are Infinite

                                                if (carryOverUnits == 100)
                                                {
                                                    decimal? unitsToTransfer = transferableRecords.Select(c => c.Units_Done).Sum();

                                                    for (int i = 0; i < transferableRecords.Count; i++)
                                                    {
                                                        decimal oldTrainingTransactionId = transferableRecords[i].User_Training_Transaction.Id;
                                                        decimal oldTrainingStatusId = transferableRecords[i].Id;

                                                        //Insering new User_Training_Transaction's carried over records
                                                        User_Training_Transaction utt = transferableRecords.Where(x => x.Training_Transaction_ID
                                                        == transferableRecords[i].Training_Transaction_ID).Select(x => new User_Training_Transaction
                                                        {
                                                            User_Id = transferableRecords[i].User_Id,
                                                            Date = transferableRecords[i].User_Training_Transaction.Date,
                                                            State_Id = transferableRecords[i].User_Training_Transaction.State_Id,
                                                            Category_Id = transferableRecords[i].Category_Id,
                                                            Activity_Id = transferableRecords[i].Activity_Id,
                                                            SubActivity_Id = transferableRecords[i].SubActivity_Id,
                                                            Hours = transferableRecords[i].User_Training_Transaction.Hours,
                                                            Provider = transferableRecords[i].User_Training_Transaction.Provider,
                                                            Financial_Year = "2019-2020", //GetFinancialYear();
                                                            Your_Role = transferableRecords[i].User_Training_Transaction.Your_Role,
                                                            Forwardable = null,
                                                            Has_been_Forwarded = null,
                                                            Descrption = transferableRecords[i].User_Training_Transaction.Descrption,
                                                            UploadedFile = transferableRecords[i].User_Training_Transaction.UploadedFile,
                                                            UploadedFileName = transferableRecords[i].User_Training_Transaction.UploadedFileName
                                                        }).First();

                                                        ctx.User_Training_Transaction.Add(utt);
                                                        ctx.SaveChanges();


                                                        //Inserting new User_Training_Status's carried over records
                                                        User_Training_Status uts = transferableRecords[i];
                                                        uts.Training_Transaction_ID = utt.Id;
                                                        uts.Activity_Id = transferableRecords[i].Activity_Id;
                                                        uts.Category_Id = transferableRecords[i].Category_Id;
                                                        uts.Min_Required_Category_Units = transferableRecords[i].Min_Required_Category_Units;
                                                        uts.SubActivity_Id = transferableRecords[i].SubActivity_Id;
                                                        uts.Units_Done = transferableRecords[i].Units_Done;
                                                        uts.User_Id = transferableRecords[i].User_Id;
                                                        uts.Received_By_Forwarding = "Yes";
                                                        uts.Financial_Year = "2019-2020";
                                                        ctx.User_Training_Status.Add(uts);
                                                        ctx.SaveChanges();


                                                        //updating transfered records so that is may not show on dashboard and index
                                                        //Updating User_Training_Transaction
                                                        User_Training_Transaction oldUtt = ctx.User_Training_Transaction
                                                            .Find(oldTrainingTransactionId);
                                                        oldUtt.Has_been_Forwarded = "Yes";
                                                        ctx.Entry(oldUtt).State = EntityState.Modified;
                                                        ctx.SaveChanges();

                                                        unitsToTransfer = unitsToTransfer - uts.Units_Done;

                                                        if (unitsToTransfer > 0)
                                                            continue;
                                                        else
                                                            break;
                                                    }

                                                    userRecord.Current_Financial_year = "2019-2020";//GetFinancialYear();
                                                    ctx.Entry(userRecord).State = EntityState.Modified;
                                                    ctx.SaveChanges();

                                                    transaction.Commit();
                                                }

                                                #endregion
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
        
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
}