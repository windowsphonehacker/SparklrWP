using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SparklrWP
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            postsPivot.DataContext = App.PostsViewModel;
            notificationsPivot.DataContext = App.NotificationsViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + MainListBox.SelectedIndex, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            //Updates are handled ONLY by the model. Everything else might screw up the timer.
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
            if (!App.NotificationsViewModel.IsDataLoaded)
            {
                App.NotificationsViewModel.LoadData();
            }
            notificationsPivot.NewCount = 100; //TODO: needs an event maybe to change this
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewPostPage.xaml", UriKind.Relative));
        }

        private void about_click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Sparklr Branding © Jonathan Warner \n\n Application Development Team: \n\n Marocco2 (design!)\n jessenic (code!)\n EaterOfCorpses (code-design!)\n TheInterframe (code-design!)\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);
        }
    }
}