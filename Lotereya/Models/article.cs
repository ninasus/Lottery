using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    [System.Web.Mvc.ModelBinder(typeof(Lotereya.Binders.ArticleBinder))]
    public class article
    {
        public int id_article { get; set; }

        [Display(Name = "Месторасположение")]
        public string place { get; set; }
        [Display(Name = "Джекпот")]
        public int JackPot { get; set; }
        [Display(Name = "Дата")]
        public DateTime date { get; set; }

        public static List<article> Get()
        {
            List<article> model = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                model = dbc.articles.Select(item => new article() { id_article = item.id_article, date = item.date, JackPot = item.JackPot, place = item.place }).ToList();
            }

            return model;
        }

        public static List<article> Get(int year)
        {
            List<article> model = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                model = dbc.articles.Where(item => item.date.Year == year).Select(item => new article() { id_article = item.id_article, date = item.date, JackPot = item.JackPot, place = item.place }).ToList();
            }

            return model;
        }

        public static article GetById(int id_article)
        {
            article model = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                model = dbc.articles.Where(item => item.id_article == id_article).Select(item => new article() { id_article = item.id_article, date = item.date, JackPot = item.JackPot, place = item.place }).First();
            }

            return model;
        }

        public void Insert()
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                DataBaseLayer.article model = new DataBaseLayer.article() { date = date, place = place, JackPot = JackPot };
                dbc.articles.Add(model);
                dbc.SaveChanges();
            }
        }

        public void Edit()
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                var item = dbc.articles.FirstOrDefault(it => it.id_article == id_article);

                if(item!=null)
                {
                    item.JackPot = JackPot;
                    item.place = place;
                    item.date = date;

                    dbc.SaveChanges();
                }
            }
        }

        public static void Delete(int id_article)
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                var item = dbc.articles.FirstOrDefault(it => it.id_article == id_article);
                if (item != null)
                {
                    dbc.articles.Remove(item);
                    dbc.SaveChanges();
                }
            }
        }
    }

    public class articleView
    {
        public int id_article { get; set; }
              
        public string place { get; set; }
        
        public int JackPot { get; set; }
        
        public DateTime date { get; set; }

        public int year { get; set; }

        public static List<articleView> Get()
        {
            List<articleView> model = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                model = dbc.get_history().Select(item => new articleView() { id_article = item.id_article, date = item.date, JackPot = item.JackPot, place = item.place, year = item.year }).ToList();
            }

            return model;
        }
    }
    
}