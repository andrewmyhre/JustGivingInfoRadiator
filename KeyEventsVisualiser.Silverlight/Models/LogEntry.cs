using System;

namespace KeyEventsVisualiser.Silverlight.Models
{
    internal class LogEntry
    {
        public string Type { get; set; }
        public string[] Attributes { get; set; }
        public DateTime DateTime { get; set; }
    }
}