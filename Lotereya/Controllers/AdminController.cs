using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lotereya.Models;

namespace Lotereya.Controllers
{
    public class AdminController : CustomController
    {
        //
        // GET: /Admin/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View(settings.Get());
        }

        [HttpPost]
        public ActionResult Settings(settings model)
        {
            if (ModelState.IsValid)
                try
                {
                    settings.Insert(model);

                    ViewData["Info"] = "Налаштування збережено";
                }
                catch (Exception ex)
                {
                    ViewData["Error"] = _ErrorChatcher.ShowError(ex.Message, "Помилка");
                }

            return View(model);
        }


        public ActionResult Winners()
        {
            return View();
        }

        public ActionResult Lotter()
        {
            LotTimer lotter = LotTimer.Instance();
            if(LotTimer.IsStoped())
            {
                var sets = settings.Get();
                lotter.PeriodAfterEvent = sets.timeAfterDraw * 1000;
                lotter.PeriodBeforeEvent = sets.timeBeforeDraw * 1000;
                lotter.PeriodEvent = sets.timeDraw * 1000;
                lotter.Count = sets.countValue;
                lotter.minValue = sets.minValue;
                lotter.maxValue = sets.maxValue;
                lotter.stepJackPot = sets.stepJackPot;
                lotter.JackPot = sets.minJackPot;
                lotter.Start();
            }
            else
            {
                lotter.Stop();
            }

            return PartialView("LotterPartial",1);
        }

        public ActionResult WinnersPartial()
        {
            var model = WinerViewModel.Get();
            return PartialView(model);
        }

        public ActionResult ArticlePartial()
        {
            var model = article.Get();
            return PartialView(model);
        }

        public ActionResult ArticleEditPartial(int? id_article)
        {
            if(id_article.HasValue)
            {
                var model = article.GetById(id_article.Value);
                return PartialView(model);
            }
            else
            {
                return PartialView();
            }
        }


        public ActionResult articleEdit(article model)
        {
            if (model.id_article == 0)
                model.Insert();
            else
                model.Edit();

            var model2 = article.Get();
            return PartialView("ArticlePartial", model2);
        }

        public ActionResult articleDelete(int id_article)
        {
            article.Delete(id_article);
            return Json(1, JsonRequestBehavior.AllowGet);
        }
    }
}