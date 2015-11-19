using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class settings
    {
        public int id_settings { get; set; }

        [Display(Name = "Час до розіграшу, сек")]
        public int timeBeforeDraw { get; set; }

        [Display(Name = "Час на розіграш, сек")]
        public int timeDraw { get; set; }

        [Display(Name = "Час після розіграшу, сек")]
        public int timeAfterDraw { get; set; }

        [Display(Name = "Мінімальне значення для вибору")]
        public int minValue { get; set; }

        [Display(Name = "Максимальне значення для вибору")]
        public int maxValue { get; set; }

        [Display(Name = "Мінімальне значення джекпоту")]
        public int minJackPot { get; set; }

        [Display(Name = "Крок джекпоту")]
        public int stepJackPot { get; set; }

        [Display(Name = "Кількість значень, що потрібно вибрати")]
        public int countValue { get; set; }



        public static settings Get()
        {
            DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities();

            var query = from sett in dbc.settings
                        let maxDate = dbc.settings.Select(item => item.date).Max()
                        where sett.date == maxDate
                        select new settings()
                        {
                            id_settings = sett.id_settings,
                            countValue = sett.countValue,
                            maxValue = sett.maxValue,
                            minJackPot = sett.minJackPot,
                            minValue = sett.minValue,
                            stepJackPot = sett.stepJackPot,
                            timeAfterDraw = sett.timeAfterDraw,
                            timeBeforeDraw = sett.timeBeforeDraw,
                            timeDraw = sett.timeDraw
                        };

            settings result = null;

            if (query.Any())
            {
                result = query.First();
            }
            else
            {
                result = SetDefaultValue();
            }

            return result;
        }

        private static settings SetDefaultValue()
        {
            settings model = new settings()
            {
                countValue = 6,
                id_settings = 0,
                maxValue = 42,
                minValue = 1,
                minJackPot = 500,
                stepJackPot = 1,
                timeAfterDraw = 0,
                timeBeforeDraw = 0,
                timeDraw = 100
            };

            return model;
        }

        public static void Insert(settings model)
        {
            try
            {
                DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities();

                dbc.settings.Add((DataBaseLayer.setting)model);
                dbc.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static explicit operator DataBaseLayer.setting(settings model)
        {
            DataBaseLayer.setting result = new DataBaseLayer.setting()
            {
                countValue = model.countValue,
                maxValue = model.maxValue,
                minValue = model.minValue,
                minJackPot = model.minJackPot,
                stepJackPot = model.stepJackPot,
                timeAfterDraw = model.timeAfterDraw,
                timeBeforeDraw = model.timeBeforeDraw,
                timeDraw = model.timeDraw,
                date = DateTime.Now,
                id_settings = 0
            };

            return result;
        }
    }
}