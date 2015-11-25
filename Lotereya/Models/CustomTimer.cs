using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lotereya.Models
{
    public class CustomTimer
    {
        System.Threading.Timer timer = null;
        public delegate void DisposeEvent();
        public event DisposeEvent onDispose;
        public CustomTimer(System.Threading.TimerCallback callback, object state, int dueTime, int period, DisposeEvent _event)
        {
            onDispose += _event;
            timer = new System.Threading.Timer(callback, state, dueTime, period);
        }

        public void Dispose()
        {
            if (timer != null)
            {
                timer.Dispose();
                timer = null;
                if (onDispose != null)
                    onDispose.Invoke();
            }
        }

        public void FireDisposeEvent()
        {
            if (onDispose != null)
                onDispose.Invoke();
        }
    }
}