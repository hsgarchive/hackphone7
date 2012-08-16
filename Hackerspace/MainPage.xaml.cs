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
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls.Maps.Platform;
using Microsoft.Phone.Tasks;

namespace Hackerspace
{
    public partial class MainPage : PhoneApplicationPage
    {
	
        private double Latitude = 1.300932;
        private double Longitude = 103.859974;
        string[] month = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

        }
        private void loadingBar(object sender, EventArgs e)
        {
            this.pb.IsIndeterminate = false;
            this.pb.Visibility = Visibility.Collapsed;
            if (App.ViewModel.currentDate().Month == DateTime.Now.Month && App.ViewModel.currentDate().Year == DateTime.Now.Year)
                DateSelect.Text = "This Month";
            else
                DateSelect.Text = month[App.ViewModel.currentDate().Month-1] + " " + App.ViewModel.currentDate().Year;
        }
        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            
            pb.IsIndeterminate = true;
            pb.Visibility = Visibility.Visible;

            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData(0);
                App.ViewModel.PropertyChanged += loadingBar;
            }
            DateRight.Tap += IncreaseDate;
            DateLeft.Tap += DecreaseDate;
            DateSelect.Tap += SetDate;
            DateSelectIcon.Tap += SetDate;
			
            MapLocation.Tap += CenterMap;
            MapDirections.Tap += FindDirections;
            try
            {
                HSGLocation.SetView(new GeoCoordinate(Latitude, Longitude), 17);

                Image image = new Image();
                image.Source = new BitmapImage(new Uri("hsglogo.png", UriKind.RelativeOrAbsolute));
                image.Stretch = Stretch.None;
                Location location = new GeoCoordinate(Latitude, Longitude);
                PositionOrigin position = PositionOrigin.BottomCenter;
                MapLayer mapLayer = new MapLayer();

                mapLayer.AddChild(image, location, position);
                MapLayer.SetPositionOffset(image, new Point(0, 0));

                HSGLocation.Children.Add(mapLayer);
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to configure map. Please check your network connection.");
            }
        }
		private void CenterMap(object sender, EventArgs e){
			HSGLocation.SetView(new GeoCoordinate(Latitude, Longitude), 17);
		}
        private void FindDirections(object sender, EventArgs e)
        {
            BingMapsDirectionsTask bingMapsDirectionsTask = new BingMapsDirectionsTask();

            GeoCoordinate hsgLocation = new GeoCoordinate(Latitude, Longitude);
            LabeledMapLocation hsgLML = new LabeledMapLocation("Hackerspace.SG", hsgLocation);

            bingMapsDirectionsTask.End = hsgLML;

            bingMapsDirectionsTask.Show();
        }
        private void startpb()
        {
            pb.IsIndeterminate = true;
            pb.Visibility = Visibility.Visible;
            App.ViewModel.PropertyChanged += loadingBar;
        }
        private void SetDate(object sender, EventArgs e)
        {
            startpb();
            App.ViewModel.LoadData(0);
        }
        private void IncreaseDate(object sender, EventArgs e)
        {
            startpb();
            App.ViewModel.LoadData(1);
        }
        private void DecreaseDate(object sender, EventArgs e)
        {
            startpb();
            App.ViewModel.LoadData(-1);
        }
    }
}