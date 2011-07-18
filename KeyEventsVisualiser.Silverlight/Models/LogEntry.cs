using System;

namespace KeyEventsVisualiser.Silverlight.Models
{
    public class LogEntry
    {
        public string Type { get; set; }
        public string[] Attributes { get; set; }
        public DateTime DateTime { get; set; }
    }
}