using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;


namespace Hackerspace
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private HttpWebRequest req;
        private WebResponse response;
        DateTime tsdt,tedt;

        public MainViewModel()
        {
            this.Items = new ObservableCollection<CalendarViewModel>();

            tsdt = DateTime.Now;
            tedt = tsdt.AddMonths(1);
        }
        public ObservableCollection<CalendarViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";

        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }
        public DateTime currentDate()
        {
            return tsdt;
        }

        public void LoadData(int i)
        {
            if (i > 0)
            {
                tsdt = tsdt.AddMonths(1);
            }
            else if (i < 0)
            {

                tsdt = tsdt.AddMonths(-1);
            }
            else
            {
                tsdt = DateTime.Now;
            }


            tedt = tsdt.AddMonths(1);
            string key = "AIzaSyBUzYV_2fENxbHgUU9ur5_F-cQuJTbbHHA";
            req = (HttpWebRequest)WebRequest.Create(@"https://www.googleapis.com/calendar/v3/calendars/mengwong%40hackerspace.sg/events?orderBy=startTime&singleEvents=true&timeMax=" + tedt.Year + "-" + tedt.Month + "-1T00%3A00%3A00%2B08%3A00&timeMin=" + tsdt.Year + "-" + tsdt.Month + "-1T00%3A00%3A00%2B08%3A00&key=" + key);
            req.Method = "GET";

            req.BeginGetResponse(jsonGetRequestStreamCallback, null);
        }
        
        private void jsonGetRequestStreamCallback(IAsyncResult result)
        {
            try
            {
                response = req.EndGetResponse(result);
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {

                    string jsonstr = reader.ReadToEnd();

                    //remove all events
                    Deployment.Current.Dispatcher.BeginInvoke(() => { this.Items.Clear(); });

                    CalendarEvents values = JsonConvert.DeserializeObject<CalendarEvents>(jsonstr);
                    if (values.items != null)
                    {
                        foreach (CalendarEvents.itemclass events in values.items)
                        {
                            CalendarViewModel evt = new CalendarViewModel();
                            evt.eventDate = events.start.dateTime;
                            evt.eventTitle = events.summary;
                            Deployment.Current.Dispatcher.BeginInvoke(() => { this.Items.Add(evt); });
                        }
                    }
                    else
                    {
                        CalendarViewModel evt = new CalendarViewModel();

                        evt.eventTitle = "No events.";

                        Deployment.Current.Dispatcher.BeginInvoke(() => { this.Items.Add(evt); });
                    }
                    response.Close();


                    this.IsDataLoaded = true;
                    Deployment.Current.Dispatcher.BeginInvoke(() => { this.NotifyPropertyChanged("DataLoaded"); });
                    reader.Close();
                }
            }
            catch (Exception)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() => { MessageBox.Show("Unable to load events. Please check your network connection."); });
                this.IsDataLoaded = false;
                Deployment.Current.Dispatcher.BeginInvoke(() => { this.NotifyPropertyChanged("DataLoaded"); });
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
    
    public class CalendarEvents
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string summary { get; set; }
        public string description { get; set; }
        public string updated { get; set; }
        public string timeZone { get; set; }
        public string accessRole { get; set; }

        public itemclass[] items;

        public class itemclass
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public string status { get; set; }
            public string htmlLink { get; set; }
            public string created { get; set; }
            public string updated { get; set; }
            public string summary { get; set; }
            public string description { get; set; }
            public personclass creator;
            public personclass organizer;
            public dt start;
            public dt end;
            public string iCalUID { get; set; }
            public int sequence { get; set; }
            public bool guestsCanInviteOthers { get; set; }
            public bool guestsCanSeeOtherGuests { get; set; }

            public class personclass 
            {
                public string email { get; set; }
                public string displayName { get; set; }
                public bool self { get; set; }
 
            }
            public class dt
            {
                public string dateTime { get; set; }

            }

        }
        
    


    }

}