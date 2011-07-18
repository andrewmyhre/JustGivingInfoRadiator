using System;
using KeyEventsVisualiser.Silverlight.Models;

namespace KeyEventsVisualiser.Silverlight.Services
{
    internal interface ILogService
    {
        event EventHandler<LogArgs> LogDownloaded;
        event EventHandler<UnhandledExceptionEventArgs> OnError;
        string GetLogUrl();
        void GetLogAsync(string logUrl);
    }
}