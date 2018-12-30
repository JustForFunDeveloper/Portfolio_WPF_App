using System;

namespace Portfolio_WPF_App.ViewModels.Handler
{
    public static class EventRaiser
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>> handler, object sender, T value)
        {
            if (handler != null)
            {
                handler(sender, new EventArgs<T>(value));
            }
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T value) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, value);
            }
        }

        public static void Raise<T>(this EventHandler<EventArgs<T>> handler, object sender, EventArgs<T> value)
        {
            if (handler != null)
            {
                handler(sender, value);
            }
        }
    }
}
