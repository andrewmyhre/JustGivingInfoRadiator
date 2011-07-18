using System;
using System.Collections.Generic;

namespace KeyEventsVisualiser.Silverlight.Models
{
    public class LogArgs : EventArgs
    {
        public string LogUrl { get; set; }
        public IEnumerable<LogEntry> Log { get; set; }
    }
}