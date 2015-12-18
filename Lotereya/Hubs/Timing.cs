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
            draw.CountPlay = draw.CountPlay + 1;

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
            Draw draw = Draw.Instance();

            if(priceElement!=null)
            {
                if(winners!=null)
                {
                    if(winners.Count()>0)
                    {
                        hubContext.Clients.AllExcept(winners).drawResult(1, 0, priceElement, draw.id_draw);
                        hubContext.Clients.Clients(winners).drawResult(1, 1, priceElement, draw.id_draw);
                    }
                    else
                    {
                        hubContext.Clients.All.drawResult(0, 0, priceElement, draw.id_draw);
                    }
                }
            }
        }

        public static void sendJackPot()
        {
            Draw draw = Draw.Instance();
            hubContext.Clients.All.SendJackPot(draw.JackPot);
        }

        public void getJackPot()
        {
            Draw draw = Draw.Instance();
            Clients.All.SendJackPot(draw.JackPot);
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