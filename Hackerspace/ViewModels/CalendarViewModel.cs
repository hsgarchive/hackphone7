using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Hackerspace
{
    public class CalendarViewModel : INotifyPropertyChanged
    {
        private string _eventTitle;
        public string eventTitle
        {
            get
            {
                return _eventTitle;
            }
            set
            {
                if (value != _eventTitle)
                {
                    _eventTitle = value;
                    NotifyPropertyChanged("eventTitle");
                }
            }
        }

        private string _eventDate;
        public string eventDate
        {
            get
            {
                return _eventDate;
            }
            set
            {
                if (value != _eventDate)
                {
                    DateTime timeD = DateTime.Parse(value);
                    _eventDate = timeD.ToLongDateString() + " " + timeD.ToShortTimeString();
                    NotifyPropertyChanged("eventDate");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}