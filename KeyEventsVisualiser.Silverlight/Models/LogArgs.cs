using System;
using System.Collections.Generic;

namespace KeyEventsVisualiser.Silverlight.Models
{
    internal class LogArgs : EventArgs
    {
        public string LogUrl { get; set; }
        public IEnumerable<LogEntry> Log { get; set; }
    }
}