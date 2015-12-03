using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lotereya.Models;

namespace Lotereya.Binders
{
    public class ArticleBinder:IModelBinder
    {
        public object BindModel(ControllerContext context, ModelBindingContext bindingContext)
        {
            article model = new article();
            var valueProvider = bindingContext.ValueProvider;

            model.id_article = (int)valueProvider.GetValue("id_article").ConvertTo(typeof(int));
            model.JackPot = (int)valueProvider.GetValue("JackPot").ConvertTo(typeof(int));
            model.place = (string)valueProvider.GetValue("place").ConvertTo(typeof(string));
           
            string date = (string)valueProvider.GetValue("date").ConvertTo(typeof(string));

            if ((date != null) && (date != ""))
            {
                model.date = DateTime.Parse(date+" 00:00:00");
            }

            return model;
        }
    }
}