using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using KeyEventsVisualiser.Silverlight.Models;
using KeyEventsVisualiser.Silverlight.Services;

namespace KeyEventsVisualiser.Silverlight
{
    public partial class MainPage : UserControl
    {
        private bool _initialising = true;
        private int _currentSlide = 0;
        private Storyboard _slideChangeTimer;
        private Storyboard _dataUpdateTimer;
        private IEnumerable<LogEntry> _log;

        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("loaded.");

            UpdateLogData();
        }

        private void UpdateLogData()
        {
            var logService = new LogService();
            string logUrl = logService.GetLogUrl();
            logService.LogDownloaded += new EventHandler<LogArgs>(logService_LogDownloaded);
            logService.OnError += new EventHandler<UnhandledExceptionEventArgs>(logService_OnError);
            logService.GetLogAsync(logUrl);
        }

        void DataDownloadedStartInterface()
        {
            AddSlideIndicators();
            ActivateSlide(0);
            BeginSlideSequence();
            StartUpdateTimer();
        }

        private void StartUpdateTimer()
        {
            _dataUpdateTimer = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 1;
            anim.To = 0.999d;
            anim.Duration = new Duration(TimeSpan.FromSeconds(10));
            _dataUpdateTimer.Children.Add(anim);
            _dataUpdateTimer.Completed += new EventHandler(_dataUpdateTimer_Completed);
            Storyboard.SetTarget(anim, LayoutRoot);
            Storyboard.SetTargetProperty(anim, new PropertyPath(UIElement.OpacityProperty));
            LayoutRoot.Resources.Add("downloadTimer", _dataUpdateTimer);
            _dataUpdateTimer.Begin();
            System.Diagnostics.Debug.WriteLine("start download timer.");
        }

        void _dataUpdateTimer_Completed(object sender, EventArgs e)
        {
            UpdateLogData();
            _dataUpdateTimer.Begin();
        }

        private void SetUpViewsAndViewModels(int slideCount)
        {
            for (int i = 0; i < slideCount; i++)
            {
                var view = new InformationView();
                view.DataContext = new InformationViewModel();
                infoContainer.Children.Add(view);
            }
        }

        private void SetViewModelDataFromLog(IEnumerable<LogEntry> log)
        {
            //set all donations
            SetDonationViewModelDataFromLog(0, _log.Where(l => l.Type == "DONATION"), "All Donations");

            // direct donations
            SetDonationViewModelDataFromLog(1, _log.Where(l => l.Type == "DIRECT-DONATION"), "Direct Donations");

            // sponsorship donations
            SetDonationViewModelDataFromLog(2, _log.Where(l => l.Type == "SPONSORED-DONATION"), "Sponsorship Donations");

            // sms donations
            SetDonationViewModelDataFromLog(3, _log.Where(l => l.Type == "SMS-DONATION"), "SMS Donations");

            // page creations
            var view = infoContainer.Children[4] as InformationView;
            var viewModel = view.DataContext as InformationViewModel;
            viewModel.MainMetricType = "Page Creations";
            viewModel.MainMetricValue = _log.Where(l => l.Type == "PAGE-CREATION").Count().ToString();

            // user activations
            view = infoContainer.Children[5] as InformationView;
            viewModel = view.DataContext as InformationViewModel;
            viewModel.MainMetricType = "Page Activations";
            viewModel.MainMetricValue = _log.Where(l => l.Type == "PAGE-ACTIVATION").Count().ToString();

            // user registrations
            view = infoContainer.Children[6] as InformationView;
            viewModel = view.DataContext as InformationViewModel;
            viewModel.MainMetricType = "User Registrations";
            viewModel.MainMetricValue = _log.Where(l => l.Type == "USER-REGISTRATION").Count().ToString();
        }

        private void SetDonationViewModelDataFromLog(int slideIndex, IEnumerable<LogEntry> donations, string metricTitle)
        {
            var donationsView = infoContainer.Children[slideIndex] as InformationView;
            var viewModel = donationsView.DataContext as InformationViewModel;
            var amounts = (from d in donations
                           select (from a in d.Attributes
                                   where a.Contains("amount=")
                                   select decimal.Parse(a.Substring(a.IndexOf('=') + 1))).First());
            viewModel.MainMetricType = metricTitle;
            viewModel.MainMetricValue = string.Format("{0}", donations.Count());
            viewModel.ExtraMetricInfo = string.Format("{0:c}", amounts.Sum());
        }

        private void AddUserRegistrationsSlide(IEnumerable<LogEntry> userRegistrationEntries)
        {
            var viewModel = new InformationViewModel()
            {
                MainMetricType = "User Registrations",
                MainMetricValue = string.Format("{0}",userRegistrationEntries.Count())
            };

            InformationView view = new InformationView();
            view.DataContext = viewModel;
            infoContainer.Children.Add(view);
        }

        private void AddPageCreationsSlide(IEnumerable<LogEntry> pageCreationEntries)
        {
            var pageCreations = new InformationViewModel()
                                    {
                                        MainMetricType = "Page Creations",
                                        MainMetricValue =
                                            string.Format("{0}", pageCreationEntries.Count())
                                    };
            InformationView view = new InformationView();
            view.DataContext = pageCreations;
            infoContainer.Children.Add(view);
        }


        private void BeginSlideSequence()
        {
            _slideChangeTimer = new Storyboard();
            DoubleAnimation anim = new DoubleAnimation();
            anim.From = 1;
            anim.To = 0.999d;
            anim.Duration = new Duration(TimeSpan.FromSeconds(1));
            _slideChangeTimer.Children.Add(anim);
            _slideChangeTimer.Completed += new EventHandler(timer_Completed);
            Storyboard.SetTarget(anim, LayoutRoot);
            Storyboard.SetTargetProperty(anim, new PropertyPath(UIElement.OpacityProperty));
            LayoutRoot.Resources.Add("timer", _slideChangeTimer);
            _slideChangeTimer.Begin();
            System.Diagnostics.Debug.WriteLine("start slide timer.");
        }

        void timer_Completed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tick.");
            NextSlide();
            _slideChangeTimer.Begin();
        }

        private void NextSlide()
        {
            _currentSlide++;
            if (_currentSlide >= infoContainer.Children.Count)
                _currentSlide = 0;

            DeactivateSlides();
            ActivateSlide(_currentSlide);
        }

        private void DeactivateSlides()
        {
            foreach (var control in infoContainer.Children)
                control.Visibility = System.Windows.Visibility.Collapsed;
            foreach (Rectangle slide in slides.Children)
                slide.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }

        private void ActivateSlide(int slideIndex)
        {
            infoContainer.Children[slideIndex].Visibility = System.Windows.Visibility.Visible;
            ((Rectangle)slides.Children[slideIndex]).Style = Application.Current.Resources["ActiveSlide"] as Style;
            ((Rectangle)slides.Children[slideIndex]).Fill = new SolidColorBrush(Color.FromArgb(255, 42, 66, 198));
        }

        void logService_OnError(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (ex == null) return;
            while (ex.InnerException != null)
                ex = ex.InnerException;
            error.Text = ex.Message;
        }

        void logService_LogDownloaded(object sender, LogArgs e)
        {
            if (_log == null)
            {
                _log = e.Log;

                SetUpViewsAndViewModels(7);
                SetViewModelDataFromLog(_log);

                DataDownloadedStartInterface();
            } else
            {
                _log = e.Log;
                SetViewModelDataFromLog(_log);
            }
        }

        private void AddSlideIndicators()
        {
            int count = infoContainer.Children.Count;
            for(int i=0; i<count; i++)
            {
                Rectangle r = new Rectangle();
                r.Width = 30;
                r.Height = 30;
                r.Fill = new SolidColorBrush(Colors.Black);
                r.RadiusX = 5;
                r.RadiusY = 5;
                r.Margin = new Thickness(10);
                r.Style = Application.Current.Resources["InactiveSlide"] as Style;

                slides.ColumnDefinitions.Add(new ColumnDefinition());
                r.SetValue(Grid.ColumnProperty, i);
                slides.Children.Add(r);
            }
        }
    }
}
