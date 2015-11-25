using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class Draw
    {
        private static Draw instance { get; set; }
        public static int CountElement {get;set;}
        public int Count { get; set; }
        public int[] PriceElements { get; set; }
        public List<Guid> Winners { get; set; }
        
        protected Draw()
        {
            PriceElements = new int[CountElement];
        }

        public static Draw Instance()
        {
            if (instance == null)
                instance = new Draw();
            return instance;
        }
    }
}