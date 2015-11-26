using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading;

using Lotereya.Models;

namespace Lotereya.Hubs
{
    public class Timing : Hub
    {
     
        private static IHubContext hubContext = GlobalHost.ConnectionManager.GetHubContext<Timing>();

        public static int Count { get; private set; }

        public void BeginPlay(int[] array)
        {
            Count++;

            Draw draw = Draw.Instance();

            string removableConnection = draw.Winners.FirstOrDefault(item => item == Context.ConnectionId);
            if(removableConnection!=null)
            {
                draw.Winners.Remove(removableConnection);
            }

            bool IsWrong = false;
            foreach(int i in draw.PriceElements)
            {
                if(!array.Any(item=>item==i))
                {
                    IsWrong = true;
                    break;
                }
            }

            if(!IsWrong)
            {
                draw.Winners.Add(Context.ConnectionId);
            }

            Clients.All.SendCount(Count);
        }

        public static void EndPlay(string[] winners, int[] priceElement)
        {
            Count=0;
            hubContext.Clients.All.SendCount(Count);

            if(priceElement!=null)
            {
                if(winners!=null)
                {
                    if(winners.Count()>0)
                    {
                        hubContext.Clients.AllExcept(winners).drawResult(1, 0, priceElement);
                        hubContext.Clients.Clients(winners).drawResult(1, 1, priceElement);
                    }
                    else
                    {
                        hubContext.Clients.All.drawResult(0, 0, priceElement);
                    }
                }
            }
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
        
        public static void SendCount()
        {
            hubContext.Clients.All.SendCount(Count);
        }
    }
}