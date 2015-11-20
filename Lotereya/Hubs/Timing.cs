using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;

namespace Lotereya.Hubs
{
    public class Timing : Hub
    {
        static bool isTimerRun;

        public static int Count { get; private set; }

        public override System.Threading.Tasks.Task OnConnected()
        {
            Count++;
            Clients.All.SendCount(Count);
            return base.OnConnected();
        }

        public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Count--;
            Clients.All.SendCount(Count);
            return base.OnDisconnected(stopCalled);
        }

        static Timing()
        {
            isTimerRun = false;
            Count = 0;
        }
        public void Send(string name, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(name, message);
        }

        /*public void Timer()
        {
            if (!isTimerRun)
            {
                isTimerRun = true;
                int i = 0;
                while (true)
                {
                    Thread.Sleep(5000);
                    Clients.All.addNewMessageToPage("TIMER", "Tic-tac " + i);
                    i++;
                }
            }
        }*/
    }
}