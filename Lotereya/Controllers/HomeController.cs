using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lotereya.Models;

namespace Lotereya.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            settings model = settings.Get();

            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            LotTimer model = LotTimer.Instance();
            model.PeriodBeforeEvent = 10000;
            model.PeriodEvent = 20000;
            model.PeriodAfterEvent = 30000;

            model.Start();

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}