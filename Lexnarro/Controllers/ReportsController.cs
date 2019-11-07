using Lexnarro.Models;
using Lexnarro.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    public class ReportsController : Controller
    {
        private readonly LaxNarroEntities db = null;

        public ReportsController()
        {
            db = new LaxNarroEntities();
        }


        // GET: Reports
        public ActionResult ACT(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                //&& a.Units_Done != 0
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm
                                }).ToList();
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult NSW(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                && a.Units_Done != 0 && a.Received_By_Forwarding == null
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm
                                }).ToList();

                #region Carry over records
                //string nextFinYear = GetNewFinancialYear(f);

                //List<User_Training_Status> carriedOverRecords = db.User_Training_Status.Include(x => x.User_Training_Transaction)
                //    .Include(x => x.User_Profile).Include(x => x.Activity_Master).Include(x => x.Category_Master)
                //    .Include(x => x.Sub_Activity_Master).Where(x => x.User_Id == v && x.Received_By_Forwarding == "Yes"
                //    && x.Financial_Year == nextFinYear).ToList();

                //ViewBag.carriedOverRecords = carriedOverRecords;

                string strt = f.Split('-')[0];
                string end = f.Split('-')[1];

                //string previousStartYear = (Convert.ToInt32(strt) - 1).ToString();
                //string previousEndYear = (Convert.ToInt32(end) - 1).ToString();

                //string previousFinancialYear = previousStartYear + "-" + previousEndYear;

                List<User_Training_Status> previousYearCarryOverRecords = db.User_Training_Status.Include(x => x.User_Training_Transaction)
                    .Include(x => x.User_Profile).Include(x => x.Activity_Master).Include(x => x.Category_Master)
                    .Include(x => x.Sub_Activity_Master).Where(x => x.User_Id == v && x.Received_By_Forwarding == "Yes"
                    && x.Financial_Year == f).ToList();

                ViewBag.carriedOverRecords = previousYearCarryOverRecords;
                #endregion


                ViewBag.startYear = strt;
                ViewBag.endYear = end;

            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult NT(decimal v, string f)
        {
            IEnumerable<Training> userTrainingCurrentYear = null;
            IEnumerable<Training> userTrainingPreviousYear = null;
            IEnumerable<Training> userTrainingNextYear = null;
            try
            {
                decimal? unitsDone = 0;
                userTrainingCurrentYear = (from a in db.User_Training_Status
                                           join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                           join c in db.User_Profile on a.User_Id equals c.ID
                                           join d in db.State_Master on b.State_Id equals d.ID
                                           join e in db.Activity_Master on a.Activity_Id equals e.ID
                                           join h in db.Category_Master on a.Category_Id equals h.ID
                                           join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                           from r in subTraining.DefaultIfEmpty()
                                           where a.User_Id == v && a.Financial_Year == f
                                           && a.Units_Done != 0 && a.Received_By_Forwarding == null
                                           orderby b.Date ascending
                                           select new Training()
                                           {
                                               Date = b.Date,
                                               Hours = b.Hours,
                                               Provider = b.Provider,
                                               Financial_Year = b.Financial_Year,
                                               Forwardable = b.Forwardable,
                                               Has_been_Forwarded = b.Has_been_Forwarded,
                                               Descrption = b.Descrption,
                                               Received_By_Forwarding = a.Received_By_Forwarding,
                                               Units_Done = a.Units_Done,
                                               Min_Required_Category_Units = a.Min_Required_Category_Units,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               OtherName = c.OtherName,
                                               StreetNumber = c.StreetNumber,
                                               StreetName = c.StreetName,
                                               PostCode = c.PostCode,
                                               Suburb = c.Suburb,
                                               LawSocietyNumber = c.LawSocietyNumber,
                                               EmailAddress = c.EmailAddress,
                                               PhoneNumber = c.PhoneNumber,
                                               Address = c.Address,
                                               Your_Role = b.Your_Role,
                                               CountryName = c.Country_Master.Name,
                                               StateName = c.State_Master1.Name,
                                               StateShortName = c.State_Master1.ShortName,
                                               ActivityName = e.Name,
                                               ActivityShortName = e.ShortName,
                                               CategoryName = h.Name,
                                               CategoryShortName = h.ShortName,
                                               SubactivityName = r.Name,
                                               SubactivityShortName = r.ShortName,
                                               Firm = c.Firm
                                           }).ToList();

                ViewBag.startYear = userTrainingCurrentYear.Select(x => x.Financial_Year).First().Split('-')[0];
                ViewBag.endYear = userTrainingCurrentYear.Select(x => x.Financial_Year).First().Split('-')[1];

                foreach (Training item in userTrainingCurrentYear)
                {
                    unitsDone += item.Units_Done;
                }
                               


                #region  Carry over from previous cpd year
                userTrainingPreviousYear = (from a in db.User_Training_Status
                                            join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                            join c in db.User_Profile on a.User_Id equals c.ID
                                            join d in db.State_Master on b.State_Id equals d.ID
                                            join e in db.Activity_Master on a.Activity_Id equals e.ID
                                            join h in db.Category_Master on a.Category_Id equals h.ID
                                            join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                            from r in subTraining.DefaultIfEmpty()
                                            where a.User_Id == v && b.Financial_Year == f
                                            && a.Received_By_Forwarding == "Yes"
                                            orderby b.Date ascending
                                            select new Training()
                                            {
                                                Date = b.Date,
                                                Hours = b.Hours,
                                                Provider = b.Provider,
                                                Financial_Year = b.Financial_Year,
                                                Forwardable = b.Forwardable,
                                                Has_been_Forwarded = b.Has_been_Forwarded,
                                                Descrption = b.Descrption,
                                                Received_By_Forwarding = a.Received_By_Forwarding,
                                                Units_Done = a.Units_Done,
                                                Min_Required_Category_Units = a.Min_Required_Category_Units,
                                                FirstName = c.FirstName,
                                                LastName = c.LastName,
                                                OtherName = c.OtherName,
                                                StreetNumber = c.StreetNumber,
                                                StreetName = c.StreetName,
                                                PostCode = c.PostCode,
                                                Suburb = c.Suburb,
                                                LawSocietyNumber = c.LawSocietyNumber,
                                                EmailAddress = c.EmailAddress,
                                                PhoneNumber = c.PhoneNumber,
                                                Address = c.Address,
                                                Your_Role = b.Your_Role,
                                                CountryName = c.Country_Master.Name,
                                                StateName = c.State_Master1.Name,
                                                StateShortName = c.State_Master1.ShortName,
                                                ActivityName = e.Name,
                                                ActivityShortName = e.ShortName,
                                                CategoryName = h.Name,
                                                CategoryShortName = h.ShortName,
                                                SubactivityName = r.Name,
                                                SubactivityShortName = r.ShortName,
                                                Firm = c.Firm
                                            }).ToList();

                ViewBag.previousFinancialYearRecords = userTrainingPreviousYear;
                #endregion



                foreach (Training item in userTrainingPreviousYear)
                {
                    unitsDone += item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;

                #region Carry over for next cpd year
                userTrainingNextYear = (from a in db.User_Training_Status
                                            join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                            join c in db.User_Profile on a.User_Id equals c.ID
                                            join d in db.State_Master on b.State_Id equals d.ID
                                            join e in db.Activity_Master on a.Activity_Id equals e.ID
                                            join h in db.Category_Master on a.Category_Id equals h.ID
                                            join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                            from r in subTraining.DefaultIfEmpty()
                                            where a.User_Id == v && b.Financial_Year == f
                                            && a.Units_Done != 0 && b.Forwardable == "Yes"
                                            orderby b.Date ascending
                                            select new Training()
                                            {
                                                Date = b.Date,
                                                Hours = b.Hours,
                                                Provider = b.Provider,
                                                Financial_Year = b.Financial_Year,
                                                Forwardable = b.Forwardable,
                                                Has_been_Forwarded = b.Has_been_Forwarded,
                                                Descrption = b.Descrption,
                                                Received_By_Forwarding = a.Received_By_Forwarding,
                                                Units_Done = a.Units_Done,
                                                Min_Required_Category_Units = a.Min_Required_Category_Units,
                                                FirstName = c.FirstName,
                                                LastName = c.LastName,
                                                OtherName = c.OtherName,
                                                StreetNumber = c.StreetNumber,
                                                StreetName = c.StreetName,
                                                PostCode = c.PostCode,
                                                Suburb = c.Suburb,
                                                LawSocietyNumber = c.LawSocietyNumber,
                                                EmailAddress = c.EmailAddress,
                                                PhoneNumber = c.PhoneNumber,
                                                Address = c.Address,
                                                Your_Role = b.Your_Role,
                                                CountryName = c.Country_Master.Name,
                                                StateName = c.State_Master1.Name,
                                                StateShortName = c.State_Master1.ShortName,
                                                ActivityName = e.Name,
                                                ActivityShortName = e.ShortName,
                                                CategoryName = h.Name,
                                                CategoryShortName = h.ShortName,
                                                SubactivityName = r.Name,
                                                SubactivityShortName = r.ShortName,
                                                Firm = c.Firm
                                            }).ToList();

                ViewBag.nextFinancialYearRecords = userTrainingNextYear;
                #endregion

            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTrainingCurrentYear);
        }


        public ActionResult QLD(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                decimal? unitsDone = 0;

                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm
                                }).ToList();

                foreach (Training item in userTraining)
                {
                    unitsDone = unitsDone + item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult SA(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                decimal? unitsDone = 0;

                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm
                                }).ToList();

                foreach (Training item in userTraining)
                {
                    unitsDone = unitsDone + item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult TAS(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                decimal? unitsDone = 0;

                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm,
                                    UploadedFileName = b.UploadedFileName
                                }).ToList();

                foreach (Training item in userTraining)
                {
                    unitsDone = unitsDone + item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult VIC(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                decimal? unitsDone = 0;

                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm,
                                    UploadedFileName = b.UploadedFileName
                                }).ToList();

                foreach (Training item in userTraining)
                {
                    unitsDone = unitsDone + item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;

                ViewBag.startYear = userTraining.Select(x => x.Financial_Year).First().Split('-')[0];
                ViewBag.endYear = userTraining.Select(x => x.Financial_Year).First().Split('-')[1];
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
        }


        public ActionResult WA(decimal v, string f)
        {
            IEnumerable<Training> userTraining = null;
            try
            {
                decimal? unitsDone = 0;

                userTraining = (from a in db.User_Training_Status
                                join b in db.User_Training_Transaction on a.Training_Transaction_ID equals b.Id
                                join c in db.User_Profile on a.User_Id equals c.ID
                                join d in db.State_Master on b.State_Id equals d.ID
                                join e in db.Activity_Master on a.Activity_Id equals e.ID
                                join h in db.Category_Master on a.Category_Id equals h.ID
                                join g in db.Sub_Activity_Master on a.SubActivity_Id equals g.ID into subTraining
                                from r in subTraining.DefaultIfEmpty()
                                where a.User_Id == v && a.Financial_Year == f
                                orderby b.Date ascending
                                select new Training()
                                {
                                    Date = b.Date,
                                    Hours = b.Hours,
                                    Provider = b.Provider,
                                    Financial_Year = b.Financial_Year,
                                    Forwardable = b.Forwardable,
                                    Has_been_Forwarded = b.Has_been_Forwarded,
                                    Descrption = b.Descrption,
                                    Received_By_Forwarding = a.Received_By_Forwarding,
                                    Units_Done = a.Units_Done,
                                    Min_Required_Category_Units = a.Min_Required_Category_Units,
                                    FirstName = c.FirstName,
                                    LastName = c.LastName,
                                    OtherName = c.OtherName,
                                    StreetNumber = c.StreetNumber,
                                    StreetName = c.StreetName,
                                    PostCode = c.PostCode,
                                    Suburb = c.Suburb,
                                    LawSocietyNumber = c.LawSocietyNumber,
                                    EmailAddress = c.EmailAddress,
                                    PhoneNumber = c.PhoneNumber,
                                    Address = c.Address,
                                    Your_Role = b.Your_Role,
                                    CountryName = c.Country_Master.Name,
                                    StateName = c.State_Master1.Name,
                                    StateShortName = c.State_Master1.ShortName,
                                    ActivityName = e.Name,
                                    ActivityShortName = e.ShortName,
                                    CategoryName = h.Name,
                                    CategoryShortName = h.ShortName,
                                    SubactivityName = r.Name,
                                    SubactivityShortName = r.ShortName,
                                    Firm = c.Firm,
                                    UploadedFileName = b.UploadedFileName
                                }).ToList();

                foreach (Training item in userTraining)
                {
                    unitsDone = unitsDone + item.Units_Done;
                }

                ViewBag.unitsDone = unitsDone;
            }
            catch (Exception)
            {
                //throw;
            }
            return View(userTraining);
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
}