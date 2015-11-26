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
        public delegate void StartEvent();
        public event DisposeEvent onDispose;
        public event StartEvent onStart;
        public CustomTimer(System.Threading.TimerCallback callback, object state, int dueTime, int period, DisposeEvent _eventDispose, StartEvent _eventStart)
        {
            onDispose += _eventDispose;
            onStart += _eventStart;
                     
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

        public void FireStartEvent()
        {
            if (onStart != null)
                onStart.Invoke();
        }
    }
}