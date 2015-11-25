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
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Timing>();

        public static int Count { get; private set; }

        public void BeginPlay()
        {
            Count++;
            Clients.All.SendCount(Count);
        }

        public static void EndPlay()
        {
            Count=0;
            hubContext.Clients.All.SendCount(Count);
        }

        /* public override System.Threading.Tasks.Task OnConnected()
        {
            Count++;
            Clients.All.SendCount(Count);
            return base.OnConnected();
        }*/

        /*public override System.Threading.Tasks.Task OnDisconnected(bool stopCalled)
        {
            Count--;
            Clients.All.SendCount(Count);
            return base.OnDisconnected(stopCalled);
        }*/

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

        public void GetCount()
        {
            Clients.Caller.SendCount(Count);
        }             
    }
}