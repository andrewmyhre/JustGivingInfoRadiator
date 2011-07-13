using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace KeyEventsVisualiser.Silverlight
{
    public partial class InformationView : UserControl
    {
        public InformationView()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(InformationView_Loaded);
        }

        void InformationView_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
