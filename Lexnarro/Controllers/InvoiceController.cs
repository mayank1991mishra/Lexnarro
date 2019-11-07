using Lexnarro.Models;
using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Lexnarro.Controllers
{
    public class InvoiceController : Controller
    {
        private LaxNarroEntities db = null;

        public InvoiceController()
        {
            db = new LaxNarroEntities();
        }
        // GET: Invoice
        public ActionResult Index(decimal v, decimal r)
        {
            User_Transaction_Master data = db.User_Transaction_Master.Include(x => x.Plan_Master)
                .Include(x => x.User_Profile).Where(x => x.User_ID == v && x.Id == r).First();

            if (data.Plan_Master.Plan == "Yearly")
            {
                ViewBag.term = "365";


                decimal oneyearamt = data.Amount;


                oneyearamt = (oneyearamt / 11) * 10;
                ViewBag.amt = Math.Round(oneyearamt,2);

                decimal oneyeargst = (oneyearamt / 10);
                ViewBag.gst = Math.Round(oneyeargst,2);


                ViewBag.total = ViewBag.amt + ViewBag.gst;
            }
            else
            {
                ViewBag.term = "730";

                decimal twoyearamt = data.Amount;


                twoyearamt = (twoyearamt / 11) * 10;
                ViewBag.amt = Math.Round(twoyearamt,2);

                decimal twoyeargst = (twoyearamt / 10);
                ViewBag.gst = Math.Round(twoyeargst, 2);

                ViewBag.total = ViewBag.amt + ViewBag.gst;
            }

            return View(data);
        }
    }
}