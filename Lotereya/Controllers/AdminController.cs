using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Lotereya.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.Owin.Security;
using System.Security.Claims;

namespace Lotereya.Controllers
{
    [Authorize]
    public class AdminController : CustomController
    {
        /************************************ACCOUNT**********************************************/
        public ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = true
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Settings", "admin");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser { UserName = model.Login, Email = model.Email };

                IdentityResult result = await UserManager.CreateAsync(user, model.Password);


                if (result.Succeeded)
                {
                    return RedirectToAction("Settings","Admin");
                }
                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Settings", "Admin");
        }
        /*****************************************************************************************/
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
            if (LotTimer.IsStoped())
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

            return PartialView("LotterPartial", 1);
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
            if (id_article.HasValue)
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