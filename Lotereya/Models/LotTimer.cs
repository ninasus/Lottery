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
        private Timer timerStartEvent;
        private Timer timerStartBeforeEvent;
        private Timer timerStartAfterEvent;
        private Timer timer;
        private static bool isStoped;
        public int PeriodAfterEvent;
        public int PeriodEvent;
        public int PeriodBeforeEvent;
        private int Tick;



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
                isStoped = false;
                timerStartBeforeEvent = new Timer(Loter, new DataStructure() { Type = 1, Period = PeriodBeforeEvent }, 1, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent);
                timerStartEvent = new Timer(Loter, new DataStructure() { Type = 2, Period = PeriodEvent }, PeriodBeforeEvent, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent);
                timerStartAfterEvent = new Timer(Loter, new DataStructure() { Type = 3, Period = PeriodAfterEvent }, PeriodEvent + PeriodBeforeEvent+1, PeriodAfterEvent + PeriodEvent + PeriodBeforeEvent);
            }
        }

        private void Loter(object state)
        {
            var data = (DataStructure)state;

            Tick = data.Period;

            if(timer!=null)
            {
                timer.Dispose();
                timer = null;
            }

            timer = new Timer(Sender, data.Type, 0, 1000);
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
            context.Clients.All.addMessage(Helper.GetMessage((int)state), Helper.Converter(hours) + ":" + Helper.Converter(minutes) + ":" + Helper.Converter(second));

            
            if (Tick <= 0)
            {

                timer.Dispose();
                timer = null;

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
}