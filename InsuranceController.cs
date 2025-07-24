using System;
using System.Linq;
using System.Web.Mvc;
using InsuranceQuoteCalculator.Models;

namespace InsuranceQuoteCalculator.Controllers
{
    public class InsuranceController : Controller
    {
        private InsuranceDbContext db = new InsuranceDbContext();

        // GET: Insurance
        public ActionResult Index()
        {
            return View(db.Insurees.ToList());
        }

        // GET: Insurance/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insurance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,SpeedingTickets,DUI,FullCoverage")] Insuree insuree)
        {
            if (ModelState.IsValid)
            {
                // Calculate quote
                decimal baseQuote = 50.0m;
                int age = DateTime.Now.Year - insuree.DateOfBirth.Year;

                // Age-based calculations
                if (age <= 18)
                    baseQuote += 100;
                else if (age <= 25)
                    baseQuote += 50;
                else
                    baseQuote += 25;

                // Car year calculations
                if (insuree.CarYear < 2000)
                    baseQuote += 25;
                if (insuree.CarYear > 2015)
                    baseQuote += 25;

                // Car make/model calculations
                if (insuree.CarMake.ToLower() == "porsche")
                {
                    baseQuote += 25;
                    if (insuree.CarModel.ToLower() == "911 carrera")
                        baseQuote += 25;
                }

                // Speeding ticket calculations
                baseQuote += (insuree.SpeedingTickets * 10);

                // DUI calculation
                if (insuree.DUI)
                    baseQuote *= 1.25m;

                // Full coverage calculation
                if (insuree.FullCoverage)
                    baseQuote *= 1.50m;

                insuree.Quote = baseQuote;
                db.Insurees.Add(insuree);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insuree);
        }

        // GET: Insurance/Admin
        [Authorize(Roles = "Admin")]
        public ActionResult Admin()
        {
            var quotes = db.Insurees.ToList();
            return View(quotes);
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
}