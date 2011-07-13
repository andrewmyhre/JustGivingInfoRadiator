using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using KeyEventsVisualiser.Silverlight.Models;

namespace KeyEventsVisualiser.Silverlight.Services
{
    internal class LogService
    {
        public event EventHandler<LogArgs> LogDownloaded;
        public event EventHandler<UnhandledExceptionEventArgs> OnError;
        private WebClient _http;

        public string GetLogUrl()
        {
            return "http://team1.justgiving.com/logs/v3-KeyEvents.csv";
        }

        public void GetLogAsync(string logUrl)
        {
            _http = new WebClient();
            _http.DownloadStringCompleted += new DownloadStringCompletedEventHandler(c_DownloadStringCompleted);
            _http.DownloadStringAsync(new Uri(logUrl+"?"+DateTime.Now.Ticks.ToString()), logUrl);
        }

        void c_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                string logData = e.Result;

                System.Diagnostics.Debug.WriteLine(logData);

                var log = ParseLog(logData);
                if (LogDownloaded != null)
                    LogDownloaded(this, new LogArgs() {Log = log, LogUrl = (string) e.UserState});
            } catch (Exception ex)
            {
                if (OnError != null)
                    OnError(this, new UnhandledExceptionEventArgs(ex, true));
            }
        }

        private IEnumerable<LogEntry> ParseLog(string logData)
        {
            StringReader sr = new StringReader(logData);
            string line = sr.ReadLine();

            do
            {
                string[] data = line.Split(',');
                LogEntry entry = new LogEntry()
                                     {
                                         DateTime = DateTime.Parse(data[0]),
                                         Type=data[1],
                                         Attributes = data.Skip(2).ToArray()
                                     };
                line = sr.ReadLine();
                yield return entry;
            } while (!string.IsNullOrEmpty(line));
        }
    }
}