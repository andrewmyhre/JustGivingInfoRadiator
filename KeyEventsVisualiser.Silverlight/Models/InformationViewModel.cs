using System.ComponentModel;

namespace KeyEventsVisualiser.Silverlight.Models
{
    public class InformationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _mainMetricValue;
        public string MainMetricValue
        {
            get { return _mainMetricValue; }
            set { _mainMetricValue = value;
            NotifyPropertyChanged("MainMetricValue");
            }
        }

        private string _mainMetricType;
        public string MainMetricType
        {
            get { return _mainMetricType; }
            set { _mainMetricType = value; NotifyPropertyChanged("MainMetricType"); }
        }

        private string _extraMetricInfo;
        public string ExtraMetricInfo
        {
            get { return _extraMetricInfo; }
            set { _extraMetricInfo = value; NotifyPropertyChanged("ExtraMetricInfo"); }
        }


        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}