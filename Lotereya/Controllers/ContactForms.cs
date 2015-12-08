using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Text;
using System.Text.RegularExpressions;
using Lotereya.Helpers;
using Lotereya.Models;
using System.IO;

namespace Lotereya.Controllers
{
    public class ContactFormsController : CustomController
    {
        public ActionResult HomeLetter()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult HomeLetter(HomeLetterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string messageBody;
                    using (var sr = new StreamReader(Server.MapPath("\\Views\\Shared\\EmailTemplate\\") + "HomeLetterEmail.cshtml"))
                    {
                        messageBody = sr.ReadToEnd();
                    }
                    messageBody = string.Format(messageBody, model.Name, model.Email, model.Message);

                    CoreHelper.SendMail(null, "pfree-online-lottery.com", null, null, "Новое сообщение", messageBody);

                }
                catch (Exception ex)
                {
                    return Json(new { success = false, Message = "<div class=\"alert alert-warning\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Ошибка!</strong>  Не удалось отправить письмо.<br/>" + ex.Message + "</div>" });
                }

                return Json(new { success = true, Message = "<div class=\"alert alert-success\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Отправлено!</strong>  Спасибо, Ваше сообщение отпревлено.</div>" });
            }
            return Json(new { success = false, Message = "<div class=\"alert alert-danger\"><button class=\"close\" data-dismiss=\"alert\"> × </button> <i class=\"fa fa-times-circle\"></i> <strong>Ошибка!</strong>  Форма заполнена не корректно.</div>" });
        }
    }

}