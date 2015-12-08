using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class Winner
    {
        [Required]
        [Display(Name = "Номер розыгрыша")]
        public int id_draw { get; set; }

        [Required]
        [Display(Name = "Ф.И.О.")]
        public string name { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [Display(Name = "Телефон")]
        public string phone { get; set; }

        public void Save(string ip, string elements)
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                DataBaseLayer.winner model = new DataBaseLayer.winner()
                {
                    date = DateTime.Now,
                    ip = ip,
                    elements = elements,
                    email = email,
                    name = name,
                    phone = phone,
                    id_draw = id_draw
                };

                dbc.winners.Add(model);
                dbc.SaveChanges();
            }
        }
    }

    public class WinerViewModel
    {
        [Display(Name = "Номер розыгрыша")]
        public int id_draw { get; set; }

        [Display(Name = "Ф.И.О.")]
        public string name { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Телефон")]
        public string phone { get; set; }

        [Display(Name = "IP")]
        public string ip { get; set; }

        [Display(Name = "Выиграшные номера")]
        public string elements { get; set; }

        [Display(Name = "Джекпот")]
        public int JackPot { get; set; }

        [Display(Name = "Дата")]
        public DateTime date { get; set; }


        public static List<WinerViewModel> Get()
        {
            List<WinerViewModel> model = null;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                var query = from win in dbc.winners
                            join draw in dbc.drawings on win.id_draw equals draw.id_draw
                            orderby win.date descending
                            select new WinerViewModel()
                            {
                                elements = win.elements,
                                email = win.email,
                                id_draw = win.id_draw,
                                ip = win.ip,
                                name = win.name,
                                phone = win.phone,
                                JackPot = draw.JackPot,
                                date = win.date
                            };
                if(query.Any())
                {
                    model = query.ToList();
                }
            }

            return model;
        }
    }
}