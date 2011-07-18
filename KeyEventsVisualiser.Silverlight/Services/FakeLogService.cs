using System;
using KeyEventsVisualiser.Silverlight.Models;

namespace KeyEventsVisualiser.Silverlight.Services
{
    public class FakeLogService : ILogService
    {
        static Random random = new Random();
        public event EventHandler<LogArgs> LogDownloaded;
        public event EventHandler<UnhandledExceptionEventArgs> OnError;
        public string GetLogUrl()
        {
            return "";
        }

        public void GetLogAsync(string logUrl)
        {
            if (LogDownloaded != null)
            {
                var args = new LogArgs();
                args.Log = new LogEntry[]
                               {
                                   new LogEntry(){DateTime=DateTime.Now.AddMinutes(random.Next(100)), Type="DONATION", Attributes = new []{"amount=10"}}
                               };
                LogDownloaded(this, args);
            }
        }
    }
}