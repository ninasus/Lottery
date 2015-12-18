using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class Draw
    {
        private static Draw instance { get; set; }
        //public static int CountElement {get;set;}
        //public int Count { get; set; }
        public int id_draw { get; set; }
        public int JackPot { get; set; }
        public int[] PriceElements { get; set; }
        public List<string> Winners { get; set; }

        public int CountPlay {get;set;}

        protected Draw()
        {
            //PriceElements = new int[CountElement];
            Winners = new List<string>();
        }

        public static Draw Instance()
        {
            if (instance == null)
                instance = new Draw();
            return instance;
        }

        public static void Clear()
        {
            instance = null;
        }

        public static DataBaseLayer.create_draw_Result Create(int JackPot, int stepJackPot)
        {
            DataBaseLayer.create_draw_Result result;
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {
                result = dbc.create_draw(JackPot, stepJackPot).FirstOrDefault();
            }

            return result;
        }

        public void Save()
        {
            using (DataBaseLayer.LoterejaEntities dbc = new DataBaseLayer.LoterejaEntities())
            {

                var result = dbc.drawings.Where(item => item.id_draw == id_draw).FirstOrDefault();

                if (result != null)
                {
                    result.count_winner = Winners.Count;
                    result.count_player = CountPlay;

                    if (PriceElements != null)
                    {
                        string elements = "";
                        foreach (int i in PriceElements)
                        {
                            elements = elements + i + ",";
                        }
                        elements.Trim().Trim(',');

                        result.result = elements;
                    }

                    dbc.SaveChanges();
                }
            }
        }
    }
}