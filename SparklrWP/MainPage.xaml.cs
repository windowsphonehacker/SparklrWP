﻿using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrLib.Objects;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;FAIL THE BUILD ;P
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

            this.ApplicationBar.MenuItems.Add(clearCache);
            this.ApplicationBar.MenuItems.Add(cleanCache);
#endif
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
        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
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
            notificationsPivot.Tag = 99; //TODO: Fill this and change from Tag to notificationsviewmodel
            if (!didFriends)
            {
                didFriends = true;
                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Friends> fargs = await App.Client.GetFriendsAsync();

                if (fargs.IsSuccessful)
                {
                    List<int> friends = new List<int>();
                    foreach (int id in fargs.Object.followers)
                    {
                        friends.Add(id);
                    }
                    foreach (int id in fargs.Object.following)
                    {
                        if (!friends.Contains(id)) friends.Add(id);
                    }
                    JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.Username[]> uargs = await App.Client.GetUsernamesAsync(friends.ToArray());
                    foreach (int id in friends)
                    {
                        App.FriendsViewModel.AddFriend(new FriendViewModel(id)
                        {
                            Name = App.Client.Usernames.ContainsKey(id) ? App.Client.Usernames[id] : "User " + id,
                            Image = "http://d.sparklr.me/i/t" + id + ".jpg"
                        });
                    }
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        friendPivot.DataContext = App.FriendsViewModel;
                    });
                }
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
        private Type addedType;
        private void LongListSelector_GroupViewClosing(object sender, GroupViewClosingEventArgs e)
        {
            if (addedType != null)
            {
                TiltEffect.TiltableItems.Remove(addedType);
                addedType = null;
            }
        }

        private void LongListSelector_GroupViewOpened(object sender, GroupViewOpenedEventArgs e)
        {
            var control = e.ItemsControl.Parent as UIElement;
            var generator = e.ItemsControl.ItemContainerGenerator;

            foreach (var item in e.ItemsControl.Items)
            {
                var container = generator.ContainerFromItem(item);
                if (addedType == null)
                {
                    addedType = container.GetType();
                    TiltEffect.TiltableItems.Add(addedType);
                }
                if (container != null)
                {
                    TiltEffect.SetIsTiltEnabled(container, true);
                }
            }

            ScrollViewer.SetVerticalScrollBarVisibility(control, ScrollBarVisibility.Disabled);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {


        }

        private void NotificationCount_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            mainPivot.SelectedItem = notificationsPivot;
        }
        private void StackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Profile.xaml?userId=" + ((StackPanel)sender).Tag, UriKind.Relative));
        }

        private void logout_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml?logout=true", UriKind.Relative));
        }
    }
}
