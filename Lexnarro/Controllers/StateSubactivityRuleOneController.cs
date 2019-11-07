using Lexnarro.HelperClasses;
using Lexnarro.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StateSubactivityRuleOneController : Controller
    {
        private LaxNarroEntities db = new LaxNarroEntities();

        // GET: StateActivitySubActivityWithRule1
        public ActionResult Index()
        {
            if (TempData["message"] != null)
            {
                ViewBag.message = TempData["message"].ToString();
            }

            return View();
        }


        public ActionResult GetData()
        {
            var stateActivitySubActivityWithRules = db.StateActivitySubActivityWithRule1.Include(s => s.Activity_Master).Include(s => s.Rule1_Master).Include(s => s.State_Master)
                .Include(s => s.Sub_Activity_Master).Where(s => s.Status == "Active");

            var subData = stateActivitySubActivityWithRules.ToList().Select(S => new {
                S.Id,
                State = S.State_Master.Name,
                Activity = S.Activity_Master.Name,
                RuleOne = S.Rule1_Master.Name,
                Subactivity = (S.SubActivityID == null) ? "" : S.Sub_Activity_Master.Name,
                StartDate = S.StartDate.ToShortDateString(),
                EndDate = S.EndDate.ToString().Split(' ')[0],
                S.Status
            });

            return Json(new { data = subData }, JsonRequestBehavior.AllowGet);
        }


        // GET: StateActivitySubActivityWithRule1/Details/5
        public ActionResult Details(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1 = db.StateActivitySubActivityWithRule1.Find(id);

            if (stateActivitySubActivityWithRule1 == null)
            {
                return HttpNotFound();
            }

            return View(stateActivitySubActivityWithRule1);
        }



        // GET: StateActivitySubActivityWithRule1/Create
        public ActionResult Create()
        {
            ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.SubActivityID = new SelectList(db.Sub_Activity_Master.OrderBy(m => m.Name), "ID", "Name");
            ViewBag.Rule1ID = new SelectList(db.Rule1_Master, "Id", "Name");

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name");

            return View();
        }


        // POST: StateActivitySubActivityWithRule1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,StateID,ActivityID,SubActivityID,Rule1ID,StartDate,EndDate,Status")]
        StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.ActivityID);

                ViewBag.SubActivityID = new SelectList(db.Sub_Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.SubActivityID);

                ViewBag.Rule1ID = new SelectList(db.Rule1_Master.OrderBy(m => m.Name), "Id", "Name", stateActivitySubActivityWithRule1.Rule1ID);

                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.StateID);

                if (stateActivitySubActivityWithRule1.StartDate > stateActivitySubActivityWithRule1.EndDate)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                    return View();
                }
                else
                {
                    List<StateActivitySubActivityWithRule1> getName = db.StateActivitySubActivityWithRule1.
                            Where(u => u.StateID == stateActivitySubActivityWithRule1.StateID
                            && u.ActivityID == stateActivitySubActivityWithRule1.ActivityID
                            && u.SubActivityID == stateActivitySubActivityWithRule1.SubActivityID
                            && u.Rule1ID == stateActivitySubActivityWithRule1.Rule1ID
                            && u.StartDate == stateActivitySubActivityWithRule1.StartDate
                            && u.EndDate == stateActivitySubActivityWithRule1.EndDate
                            && u.Status == stateActivitySubActivityWithRule1.Status).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.StateActivitySubActivityWithRule1.Add(stateActivitySubActivityWithRule1);
                        db.SaveChanges();
                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Saved successfully");
                        return RedirectToAction("Index");
                    }
                }
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


        // GET: StateActivitySubActivityWithRule1/Edit/5
        public ActionResult Edit(decimal? id)
        {
            decimal userid = Convert.ToDecimal(UserHelper.GetUserId());

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1 = db.StateActivitySubActivityWithRule1.Find(id);

            if (stateActivitySubActivityWithRule1 == null)
            {
                return HttpNotFound();
            }

            ViewBag.Rule1ID = new SelectList(db.Rule1_Master.OrderBy(m => m.Name), "Id", "Name", stateActivitySubActivityWithRule1.Rule1ID);

            ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.StateID);

            var state = (from s in db.User_Profile
                         join c in db.State_Master on s.StateEnrolled equals c.ID
                         where s.ID == userid
                         select new { c.ID, c.Name }).ToList();

            var ds = state.ToList();

            decimal stateid = state[0].ID;
            ViewBag.stateName = state[0].Name;

            //-----------Getting activity list for a state-------------------

            var activities = (from s in db.State_Activity_Mapping
                              join c in db.Activity_Master on s.ActivityID equals c.ID
                              where s.StateID == stateActivitySubActivityWithRule1.StateID
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
            string activityid = Convert.ToString(stateActivitySubActivityWithRule1.ActivityID);
            foreach (SelectListItem item in activityList)
            {
                if (item.Value == activityid)
                {
                    item.Selected = true;
                    break;
                }
            }

            ViewBag.ActivityID = activityList;


            var subActivities = (from s in db.Sub_Activity_Master
                                 join x in db.Activity_Master on s.Activity_ID equals x.ID
                                 where s.StateID == stateActivitySubActivityWithRule1.StateID && s.Activity_ID == stateActivitySubActivityWithRule1.ActivityID
                                 select new { s.ID, s.Name }).ToList();

            List<SelectListItem> subActivityList = new List<SelectListItem>();

            string subactivityid = "";

            if (subActivities.Count > 0)
            {
                for (int i = 0; i < subActivities.Count; i++)
                {
                    subActivityList.Add(new SelectListItem
                    {
                        Value = Convert.ToString(subActivities[i].ID),
                        Text = subActivities[i].Name
                    });
                    subactivityid = Convert.ToString(subActivities[i].ID);
                }

                ViewBag.SubActivityID = subActivityList;
            }

            if (ViewBag.SubActivityID == null)
            {
                //-------------Empty sub activity list-------------
                subActivityList = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = "NULL",
                        Text = "N/A"
                    }
                };
                ViewBag.SubActivityID = subActivityList;
            }
            return View(stateActivitySubActivityWithRule1);
        }


        // POST: StateActivitySubActivityWithRule1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,StateID,ActivityID,SubActivityID,Rule1ID,StartDate,EndDate,Status")] StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1)
        {
            try
            {
                ViewBag.ActivityID = new SelectList(db.Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.ActivityID);

                ViewBag.SubActivityID = new SelectList(db.Sub_Activity_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.SubActivityID);

                ViewBag.Rule1ID = new SelectList(db.Rule1_Master.OrderBy(m => m.Name), "Id", "Name", stateActivitySubActivityWithRule1.Rule1ID);

                ViewBag.StateID = new SelectList(db.State_Master.OrderBy(m => m.Name), "ID", "Name", stateActivitySubActivityWithRule1.StateID);
                //if (ModelState.IsValid)
                //{
                if (stateActivitySubActivityWithRule1.StartDate > stateActivitySubActivityWithRule1.EndDate)
                {
                    ViewBag.message = ToasterMessage.Message(ToastType.info, "End date should be greater then start date");
                    return View();
                }
                else
                {
                    List<StateActivitySubActivityWithRule1> getName = db.StateActivitySubActivityWithRule1.
                            Where(u => u.StateID == stateActivitySubActivityWithRule1.StateID
                            && u.ActivityID == stateActivitySubActivityWithRule1.ActivityID
                            && u.SubActivityID == stateActivitySubActivityWithRule1.SubActivityID
                            && u.Rule1ID == stateActivitySubActivityWithRule1.Rule1ID
                            && u.StartDate == stateActivitySubActivityWithRule1.StartDate
                            && u.EndDate == stateActivitySubActivityWithRule1.EndDate
                            && u.Status == stateActivitySubActivityWithRule1.Status && u.Id != stateActivitySubActivityWithRule1.Id).ToList();

                    if (getName.Count > 0)
                    {
                        ViewBag.message = ToasterMessage.Message(ToastType.info, "Record already exist");
                        return View();
                    }
                    else
                    {
                        db.Entry(stateActivitySubActivityWithRule1).State = EntityState.Modified;
                        db.SaveChanges();
                        TempData["message"] = ToasterMessage.Message(ToastType.success, "Updated Successfully");
                        return RedirectToAction("Index");
                    }
                }
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


        // GET: StateActivitySubActivityWithRule1/Delete/5
        public ActionResult Delete(decimal? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1 = db.StateActivitySubActivityWithRule1.Find(id);

            if (stateActivitySubActivityWithRule1 == null)
            {
                return HttpNotFound();
            }

            return View(stateActivitySubActivityWithRule1);
        }



        // POST: StateActivitySubActivityWithRule1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal? id)
        {
            try
            {
                StateActivitySubActivityWithRule1 stateActivitySubActivityWithRule1 = db.StateActivitySubActivityWithRule1.Find(id);
                db.StateActivitySubActivityWithRule1.Remove(stateActivitySubActivityWithRule1);
                db.SaveChanges();
                TempData["message"] = ToasterMessage.Message(ToastType.success, "Deleted successfully");
                return RedirectToAction("Index");
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


        public JsonResult RuleDetails()
        {
            var ruleList = from s in db.Rule1_Master select new { s.Id, Name = string.Concat(s.Name, " Min - ", s.Min, ", Max - ", s.Max) };

            return Json(new SelectList(ruleList.ToArray().Distinct(), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult ActivityList(int StateID)
        {
            var state = from s in db.State_Activity_Mapping
                        join c in db.Activity_Master on s.ActivityID equals c.ID
                        where s.StateID == StateID
                        select new { c.ID, c.Name };
            return Json(new SelectList(state.ToArray().Distinct().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }


        public JsonResult SubActivityList(int ActivityID, int StateID)
        {
            decimal state_id = Convert.ToDecimal(StateID);
            var state = from s in db.Sub_Activity_Master
                        join c in db.Activity_Master on s.Activity_ID equals c.ID
                        join d in db.State_Master on s.StateID equals d.ID
                        where s.Activity_ID == ActivityID && s.StateID == state_id
                        select new { s.ID, s.Name };
            return Json(new SelectList(state.ToArray().Distinct().OrderBy(m => m.Name), "ID", "Name"), JsonRequestBehavior.AllowGet);
        }
    }
}
