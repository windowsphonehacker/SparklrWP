using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrWP.Controls;
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

            //App.MainViewModel.BeforeItemAdded += MainViewModel_BeforeItemAdded;
            //App.MainViewModel.AfterItemAdded += MainViewModel_AfterItemAdded;

            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

#if DEBUG

            ApplicationBarMenuItem garbageCollect = new ApplicationBarMenuItem("DEBUG: Run GC");
            garbageCollect.Click += (sender, e) =>
            {
                GC.Collect();
            };

            this.ApplicationBar.MenuItems.Add(garbageCollect);
#endif
            App.BackgroundTask = new Task();
        }

        /*
        double distanceFromBottom = -1;

        void MainViewModel_AfterItemAdded(object sender, EventArgs e)
        {
            if (distanceFromBottom >= 0)
            {
                MainListBox.UnderlyingListBox.UpdateLayout();
                MainListBox.UpdateLayout();
                ScrollViewer v = MainListBox.UnderlyingListBox.FindScrollViewer();
                v.ScrollToVerticalOffset(v.ScrollableHeight - distanceFromBottom);
                MainListBox.UnderlyingListBox.UpdateLayout();
                MainListBox.UpdateLayout();

#if DEBUG
                v = MainListBox.UnderlyingListBox.FindScrollViewer();
                App.logger.log("Distance from bottom updated: {0} (Height: {1}, Offset: {2})", v.ScrollableHeight - v.VerticalOffset, v.ScrollableHeight, v.VerticalOffset);
#endif
            }
        }

        void MainViewModel_BeforeItemAdded(object sender, EventArgs e)
        {
            ScrollViewer v = MainListBox.UnderlyingListBox.FindScrollViewer();
            if (v != null)
            {
                distanceFromBottom = v.ScrollableHeight - v.VerticalOffset;
#if DEBUG
                App.logger.log("Distance from bottom is: {0} (Height: {1}, Offset: {2})", distanceFromBottom, v.ScrollableHeight, v.VerticalOffset);
#endif
            }
            else
            {
                distanceFromBottom = -1;
            }
        }
         * */

        // Handle selection changed on ListBox
        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.UnderlyingListBox.SelectedIndex == -1)
                return;

            // Navigate to the new page
            NavigationService.Navigate(new Uri("/Pages/DetailsPage.xaml?selectedItem=" + MainListBox.UnderlyingListBox.SelectedIndex, UriKind.Relative));

            // Reset selected index to -1 (no selection)
            MainListBox.UnderlyingListBox.SelectedIndex = -1;
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
            if (this.NavigationContext.QueryString.ContainsKey("notification") && e.NavigationMode == NavigationMode.New)
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
            NavigationService.Navigate(new Uri("/Pages/About.xaml", UriKind.Relative));
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

        private void MainListBox_TopRefresh(object sender, EventArgs e)
        {
            App.MainViewModel.Update();
        }

        private void MainListBox_LoadMore(object sender, EventArgs e)
        {
            //TODO: Fix this
            //App.MainViewModel.LoadMore();
        }

        private void SearchIconButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SearchPage.xaml", UriKind.Relative));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/InboxPage.xaml", UriKind.Relative));
        }

        private void Notification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrTextBlock control = sender as SparklrTextBlock;

            if (control != null)
            {
                NotificationViewModel m = (NotificationViewModel)control.DataContext;
                if (m.NavigationUri != null)
                    NavigationService.Navigate(m.NavigationUri);
            }
        }

        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/SettingsPage.xaml", UriKind.Relative));
        }

        private void networkNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                navigateToNetwork(networkNameTextBox.Text);
            }
        }

        private void networkNameTextBox_ActionIconTapped(object sender, EventArgs e)
        {
            navigateToNetwork(networkNameTextBox.Text);
        }

        private void NetworkTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock control = sender as TextBlock;
            if (control != null)
            {
                navigateToNetwork(control.Text);
            }
        }

        private void navigateToNetwork(string name)
        {
            name = NetworkHelpers.UnformatNetworkName(name);
            NavigationService.Navigate(new Uri("/Pages/NetworkPage.xaml?network=" + name.EncodeUrl(), UriKind.Relative));
        }
    }
}