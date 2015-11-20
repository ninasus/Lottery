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

        public ActionResult GamePartial()
        {
            return PartialView();
        }

        public JsonResult AddElement(int element, int number, int count)
        {
            int[] array;
            if(Session["elements"]!=null)
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
                for(int i=0;i<array.Length;i++)
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
            int[] array = new int[count];

            int i = 0;

            Random rnd = new Random();

            while(true)
            {
                int element = rnd.Next(min, max);
                if(!array.Any(item=>item==element))
                {
                    array[i] = element;
                    i++;
                }

                if (i == count)
                    break;
            }

            Session["elements"] = array;

            return Json(array, JsonRequestBehavior.AllowGet);
        }
    }
}