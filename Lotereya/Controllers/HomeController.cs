using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lotereya.Models;
using System.IO;
using Lotereya.Helpers;

namespace Lotereya.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            settings model = settings.Get();
            Session["elements"] = null;

            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                List<OptionsViewModel> fields = (from g in dbc.options
                          select new OptionsViewModel
                          {
                              id_option = g.id_option,
                              option_group = g.option_group,
                              option_key = g.option_key,
                              value = g.value,
                              field_type = g.field_type,
                              option_name = g.option_name
                          }).ToList();

                string title = fields.Where(f => f.id_option == 6).FirstOrDefault().value;
                if (!String.IsNullOrEmpty(title))
                    ViewBag.Title = title;
                else
                    ViewBag.Title = "Бесплатная онлайн лотерея";
                ViewBag.Description = fields.Where(f => f.id_option == 7).FirstOrDefault().value;
                ViewBag.Keywords = fields.Where(f => f.id_option == 8).FirstOrDefault().value;

                ViewBag.rules = fields.Where(f => f.id_option == 4).FirstOrDefault().value;


                ViewBag.code = fields.Where(f => f.id_option == 2).FirstOrDefault().value;
            }

            return View(model);
        }

        public ActionResult HistoryPartial()
        {
            var model = articleView.Get();
            return PartialView(model);
        }

        public ActionResult GamePartial()
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                List<OptionsViewModel> fields = (from g in dbc.options
                                                 select new OptionsViewModel
                                                 {
                                                     id_option = g.id_option,
                                                     option_group = g.option_group,
                                                     option_key = g.option_key,
                                                     value = g.value,
                                                     field_type = g.field_type,
                                                     option_name = g.option_name
                                                 }).ToList();

                string title = fields.Where(f => f.id_option == 6).FirstOrDefault().value;

                ViewBag.code = fields.Where(f => f.id_option == 2).FirstOrDefault().value;
            }
            return PartialView();
        }

        public ActionResult ResultPartial(int thereWinners, int isWinner)
        {
            if (!Convert.ToBoolean(thereWinners))
                ViewData["resultText"] = "Никто не выиграл джэкпот";
            else
                if (!Convert.ToBoolean(isWinner))
                    ViewData["resultText"] = "В этом розыграше был выигран джэкпот";
                else
                {
                    ViewData["resultText"] = "Поздравляем! Вы выиграли джэкпот";
                    ViewData["result"] = 1;
                }

            return PartialView();
        }

        [HttpPost]
        public ActionResult Winner(Winner model)
        {
            if (ModelState.IsValid)
            {
                string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (ip == null)
                    ip = Request.ServerVariables["REMOTE_ADDR"];

                if (ip == null)
                    ip = Request.UserHostAddress;

                string elements = "";
                if (Session["elements"] != null)
                {
                    int[] array = (int[])Session["elements"];

                    foreach (int i in array)
                    {
                        elements = elements + i + ",";
                    }
                    elements.Trim().Trim(',');
                }
                else
                    elements = "Игра в несколько окон";


                model.Save(ip, elements);

                try
                {
                    string messageBody;
                    using (var sr = new StreamReader(Server.MapPath("\\Views\\Shared\\EmailTemplate\\") + "WinnerEmail.cshtml"))
                    {
                        messageBody = sr.ReadToEnd();
                    }
                    messageBody = string.Format(messageBody, model.id_draw, model.name, model.email, model.phone);

                    CoreHelper.SendMail(null, "free-online-lottery.com", null, null, "Новый победитель", messageBody);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Message = "<div class=\"alert alert-warning\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Ошибка!</strong>  Не удалось отправить письмо.<br/>" + ex.Message + "</div>" });
                }

                return Json(new { success = true, Message = "<div class=\"alert alert-success\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Отправлено!</strong>  Спасибо, Ваше сообщение отпревлено.</div>" });
            }
            return Json(new { success = false, Message = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Ошибка!</strong>  Форма заполнена не корректно.</div>" });
            
        }

        public JsonResult AddElement(int element, int number, int count)
        {
            int[] array;
            if (Session["elements"] != null)
            {
                array = (int[])Session["elements"];
                array[number] = element;

            }
            else
            {
                array = new int[count];
                array[0] = element;
            }
            Session["elements"] = array;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RemoveElement(int element)
        {
            int[] array;
            if (Session["elements"] != null)
            {
                array = (int[])Session["elements"];
                var newArray = array.Where(item => item != element).ToList();
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < newArray.Count)
                        array[i] = newArray[i];
                    else
                        array[i] = 0;
                }

                Session["elements"] = array;
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ClearElement()
        {
            Session["elements"] = null;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoElements(int min, int max, int count)
        {
            int[] array = Lotereya.Models.GenerateRandomValues.Get(count, min, max);

            Session["elements"] = array;

            return Json(array, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OptionPartial(int id)
        {
            OptionsViewModel field = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                field = (from g in dbc.options
                         where g.id_option == id
                          select new OptionsViewModel
                          {
                              id_option = g.id_option,
                              option_group = g.option_group,
                              option_key = g.option_key,
                              value = g.value,
                              field_type = g.field_type,
                              option_name = g.option_name
                          }).FirstOrDefault();
            }

            return PartialView(field);
        }
    }
}