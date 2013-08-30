using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrWP.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class MainPage : PhoneApplicationPage
    {

        bool popupVisible = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            LayoutRoot.DataContext = App.MainViewModel;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

#if DEBUG
            //Debug items for cache cleaning. TODO: Implement them properly in a settings page
            ApplicationBarMenuItem clearCache = new ApplicationBarMenuItem("DEBUG: Clear cache");
            clearCache.Click += (sender, e) =>
            {
                Utils.Caching.Image.ClearImageCache();
            };

            ApplicationBarMenuItem cleanCache = new ApplicationBarMenuItem("DEBUG: Clean cache");
            cleanCache.Click += (sender, e) =>
            {
                Utils.Caching.Image.CleanImageCache();
            };

            ApplicationBarMenuItem garbageCollect = new ApplicationBarMenuItem("DEBUG: Run GC");
            garbageCollect.Click += (sender, e) =>
            {
                GC.Collect();
            };

            this.ApplicationBar.MenuItems.Add(clearCache);
            this.ApplicationBar.MenuItems.Add(cleanCache);
            this.ApplicationBar.MenuItems.Add(garbageCollect);
#endif
            App.BackgroundTask = new Task();
        }

        public bool didFriends = false;
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
            //if (!App.NotificationsViewModel.IsDataLoaded)
            //{
            //    App.NotificationsViewModel.LoadData();
            //}
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.NavigationContext.QueryString.ContainsKey("notification"))
            {
                BorderNotification_Tap(this, new System.Windows.Input.GestureEventArgs());
            }
        }

        private void ApplicationBarIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/NewPostPage.xaml", UriKind.Relative));
        }

        private void about_click(object sender, System.EventArgs e)
        {
            MessageBox.Show("Sparklr Branding © Jonathan Warner \n\n Application Development Team: \n\n Marocco2 (design!)\n jessenic (code!)\n EaterOfCorpses (code-design!)\n TheInterframe (code-design!)\n\n Big Thanks to Jonathan!", "About Sparklr WP V1.0", MessageBoxButton.OK);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popupVisible)
            {
                NotificationDisappear.Begin();
                popupVisible = false;
                e.Cancel = true;
            }
        }

        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Profile.xaml?userId=" + ((StackPanel)sender).Tag, UriKind.Relative));
        }

        private void BorderNotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!popupVisible)
            {
                NotificationAppear.Begin();
                popupVisible = true;
            }
        }
    }
}