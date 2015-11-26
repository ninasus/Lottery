using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Lotereya.Models
{
    public class LotTimer
    {
        private static LotTimer instance;
        private CustomTimer timerStartEvent;
        private CustomTimer timerStartBeforeEvent;
        private CustomTimer timerStartAfterEvent;
        private CustomTimer timer;
        private static bool isStoped;

        public int PeriodAfterEvent;
        public int PeriodEvent;
        public int PeriodBeforeEvent;
        public int Count;
        public int minValue;
        public int maxValue;

        private int Tick;
        private Draw draw;


        protected LotTimer()
        {
            isStoped = true;
        }

        public static bool IsStoped()
        {
            if (instance == null)
                isStoped = true;

            return isStoped;
        }

        public static LotTimer Instance()
        {
            if (instance == null)
                instance = new LotTimer();
            return instance;
        }

        public void Start()
        {
            if (isStoped)
            {
                var context = GlobalHost.ConnectionManager.GetHubContext<Lotereya.Hubs.Timing>();
                context.Clients.All.startDraw();
                Lotereya.Hubs.Timing.SendCount();

                isStoped = false;
                timerStartBeforeEvent = new CustomTimer(Loter, new DataStructure() { Type = 1, Period = PeriodBeforeEvent }, 1, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent, null, timerStartBeforeEvent_onStart);
                timerStartEvent = new CustomTimer(Loter, new DataStructure() { Type = 2, Period = PeriodEvent }, PeriodBeforeEvent, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent, timerStartEvent_onDispose,null);
                timerStartAfterEvent = new CustomTimer(Loter, new DataStructure() { Type = 3, Period = PeriodAfterEvent }, PeriodEvent + PeriodBeforeEvent + 1, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent, null,null);
            }
        }

        private void timerStartBeforeEvent_onStart()
        {
            Draw.Clear();
            draw = Draw.Instance();
            draw.PriceElements = GenerateRandomValues.Get(Count, minValue, maxValue);
            //draw.Count = Count;
        }

        private void timerStartEvent_onDispose()
        {
            string[] winners = null;
            int[] priceElement = null;
            if (draw != null)
            {
                winners = draw.Winners.ToArray();
                priceElement = draw.PriceElements;
            }

            Lotereya.Hubs.Timing.EndPlay(winners, priceElement);

        }

        private void Loter(object state)
        {
            var data = (DataStructure)state;

            if (data.Type == 1)
            {
                timerStartBeforeEvent.FireStartEvent();
            }

            Tick = data.Period;

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
                if (data.Type == 3)
                    timerStartEvent.FireDisposeEvent();
            }

            timer = new CustomTimer(Sender, state, 0, 1000, null,null);
            //var context = GlobalHost.ConnectionManager.GetHubContext<Lotereya.Hubs.Timing>();
            //context.Clients.All.addMessage("Начался розыграш");
        }

        private void Sender(object state)
        {
            Tick -= 1000;
            var context = GlobalHost.ConnectionManager.GetHubContext<Lotereya.Hubs.Timing>();
            int hours = Tick / 3600000;
            int minutes = (Tick - hours * 3600000) / 60000;
            int second = (Tick - hours * 3600000 - minutes * 60000) / 1000;
            context.Clients.All.addMessage(Helper.GetMessage(((DataStructure)state).Type), Helper.Converter(hours) + ":" + Helper.Converter(minutes) + ":" + Helper.Converter(second));

            if (Tick <= 0)
            {
                timer.Dispose();
                timer = null;
                if (((DataStructure)state).Type == 2)
                    timerStartEvent.FireDisposeEvent();
            }
        }

        public void Stop()
        {
            isStoped = true;

            if (timerStartBeforeEvent != null)
            {
                timerStartBeforeEvent.Dispose();
                timerStartBeforeEvent = null;
            }

            if (timerStartEvent != null)
            {
                timerStartEvent.Dispose();
                timerStartEvent = null;
            }

            if (timerStartAfterEvent != null)
            {
                timerStartAfterEvent.Dispose();
                timerStartAfterEvent = null;
            }

            if (timer != null)
            {
                timer.Dispose();
                timer = null;
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<Lotereya.Hubs.Timing>();
            context.Clients.All.endDraw();
        }
    }

    public class DataStructure
    {
        public int Type { get; set; }
        public int Period { get; set; }
    }

    public class Helper
    {
        public static string Converter(int data)
        {
            return (data < 10 ? "0" + data.ToString() : data.ToString());
        }

        public static string GetMessage(int type)
        {
            string str = "";

            switch (type)
            {
                case 1:
                    str = "До запуску лотереї залишилось: ";
                    break;
                case 2:
                    str = "До завершення лотереї залишилось: ";
                    break;
                case 3:
                    str = "До початку лотереї залишилось: ";
                    break;
            }

            return str;
        }
    }

    public class GenerateRandomValues
    {
        public static int[] Get(int count,int min,int max)
        {
            int[] array = new int[count];

            int i = 0;

            Random rnd = new Random();

            while (true)
            {
                int element = rnd.Next(min, max + 1);
                if (!array.Any(item => item == element))
                {
                    array[i] = element;
                    i++;
                }

                if (i == count)
                    break;
            }

            return array;
        }
    }
}