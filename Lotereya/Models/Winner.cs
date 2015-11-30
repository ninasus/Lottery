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
        [Display(Name = "Email")]
        public string email { get; set; }
        [Required]
        [Display(Name = "Телефон")]
        public string phone { get; set; }

        public void Save(string ip, string elements)
        {
            DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities();
            DataBaseLayer.winner model = new DataBaseLayer.winner()
            {
                date = DateTime.Now,
                ip = ip,
                elements = elements,
                email = email,
                name = name,
                phone = phone
            };

            dbc.winners.Add(model);
            dbc.SaveChanges();
        }
    }
}