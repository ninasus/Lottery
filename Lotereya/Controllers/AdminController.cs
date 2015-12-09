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
using System.Data.Entity;

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
                        IsPersistent = model.RememberMe
                    }, claim);
                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("Settings", "admin");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");
                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
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
                    return RedirectToAction("Settings", "Admin");
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

        public ActionResult UserProfile()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UserProfile(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId().ToString(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user,
                                            DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties
                    {
                        IsPersistent = false
                    }, claim);
                }
                ViewData["Success"] = "Пароль успешно изменен!";
                return View(model);
            }
            AddErrors(result);
            return View(model);
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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }


        public ActionResult Setup()
        {
            return View();
        }

        public ActionResult OptionsList(string grupa)
        {
            List<OptionsViewModel> fields = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                fields = (from g in dbc.options
                          where g.option_group == grupa
                          select new OptionsViewModel
                          {
                              id_option = g.id_option,
                              option_group = g.option_group,
                              option_key = g.option_key,
                              value = g.value,
                              field_type = g.field_type,
                              option_name = g.option_name
                          }).ToList();
            }

            return PartialView(fields);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> UpdateOption(string grupa, IEnumerable<OptionsViewModel> fields)
        {
            try
            {
                using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
                {
                    if (fields != null)
                    {
                        foreach (var f in fields)
                        {
                            var result = from opt in dbc.options
                                         where (opt.option_group == grupa && opt.id_option == f.id_option)
                                         select opt;

                            if (result.Count() != 0)
                            {
                                var item = await result.FirstOrDefaultAsync();
                                item.value = f.value;
                            }

                            await dbc.SaveChangesAsync();
                        }
                    }                    
                }
                return Json(new { success = true, Message = "<div class=\"alert alert-success\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Сохранено!</strong></div>" });
            }
            catch
            {
                return Json(new { success = false, Message = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Ошибка!</strong> Форма заполнена не корректно.</div>" });
            }
        }
    }
}