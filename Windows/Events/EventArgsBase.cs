using System;

namespace Overlay.Windows
{
    public class EventArgsBase : EventArgs
    {
        public string Message { get; set; }

        public EventArgsBase(string message)
        {
            Message = message;
        }
    }
}
