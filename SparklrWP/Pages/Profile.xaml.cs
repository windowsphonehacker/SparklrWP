extern alias ImageToolsDLL;
using Microsoft.Phone.Controls.Updated;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses;
using SparklrLib.Objects.Responses.Work;
using SparklrWP.Controls;
using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class Profile : Microsoft.Phone.Controls.PhoneApplicationPage
    {
        ProfileViewModel model;
        ListPicker filterModePicker;

        public Profile()
        {
            InitializeComponent();

            model = new ProfileViewModel();
            this.DataContext = model;
            model.PropertyChanged += model_PropertyChanged;
        }

        async void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "BackgroundImage")
            {
                ImageBrush image = new ImageBrush();
                image.ImageSource = (BitmapImage)await Utils.Caching.Image.LoadCachedImageFromUrlAsync<BitmapImage>(model.BackgroundImage);
                image.Stretch = Stretch.UniformToFill;
                image.Opacity = 0;
                MainPanorama.Background = image;
                Storyboard.SetTarget(FadeInAnimation, image);
                FadeInStoryboard.Begin();
            }
        }

        public bool dataLoaded = false;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (dataLoaded) return;
            dataLoaded = true;
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("userId", out selectedIndex))
            {
                //model.ID = int.Parse(selectedIndex);

                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.User> usargs;

                int id;

                if (Int32.TryParse(selectedIndex, out id))
                {
                    model.ID = id;
                    usargs = await App.Client.GetUserAsync(model.ID);
                }
                else
                {
                    usargs = await App.Client.GetUserAsync(selectedIndex);
                    model.ID = (usargs != null && usargs.IsSuccessful) ? usargs.Object.user : -1;
                }

                if (usargs.IsSuccessful)
                {
                    refreshUserDetails(usargs);

                    foreach (var item in usargs.Object.timeline)
                    {
                        model.Posts.Add(generateItemViewModel(item));
                    }

                    model.Active = model.Posts;

                    //Load mentions
                    usargs = await App.Client.GetUserMentionsAsync(model.ID);

                    if (usargs.IsSuccessful)
                    {
                        foreach (var item in usargs.Object.timeline)
                        {
                            model.Mentions.Add(generateItemViewModel(item));
                        }
                    }

                    //Load photos
                    usargs = await App.Client.GetUserPhotosAsync(model.ID);

                    if (usargs.IsSuccessful)
                    {
                        foreach (var item in usargs.Object.timeline)
                        {
                            model.Photos.Add(generateItemViewModel(item));
                        }
                    }
                }
            }
        }

        private void refreshUserDetails(JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.User> usargs)
        {
            model.Handle = "@" + usargs.Object.handle;
            model.BackgroundImage = "http://d.sparklr.me/i/b" + model.ID + ".jpg";
            model.ProfileImage = "http://d.sparklr.me/i/" + model.ID + ".jpg";
            model.Following = usargs.Object.following;

            model.Bio = usargs.Object.bio;
            if (model.Bio.Trim() == "")
            {
                model.Bio = usargs.Object.name + " is too shy to write something about his/herself maybe check again later!";
            }

            updateProfileTile();
        }

        private async void updateProfileTile()
        {
            if (await Utils.TilesCreator.CreateProfileTileImages(model.ID))
            {
                userProfileTile.Source = Utils.TilesCreator.LoadProfileTileImage(model.ID, Utils.TileSize.Wide);
            }
        }

        private static PostItemViewModel generateItemViewModel(SparklrLib.Objects.Responses.Work.Timeline t)
        {
            PostItemViewModel i = new PostItemViewModel(
                                t.id,
                                t.from,
                                t.message,
                                null,
                                null,
                                t.commentcount ?? 0,
                                null,
                                t.from == App.Client.UserId,
                                !String.IsNullOrEmpty(t.meta) ? "http://d.sparklr.me/i/t" + t.imageUrl : null,
                                t.network,
                                t.modified ?? t.time,
                                t.time,
                                t.via);
            i.FillNamesAndImages();
            return i;
        }

        private void filterModePicker_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            filterModePicker = (ListPicker)sender;
        }

        private void filterModePicker_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (filterModePicker == null)
                filterModePicker = (ListPicker)sender;

            if (filterModePicker.SelectedItem != null)
            {
                switch (((ListPickerItem)filterModePicker.SelectedItem).Tag.ToString())
                {
                    case "posts":
                        model.Active = model.Posts;
                        break;
                    case "mentions":
                        model.Active = model.Mentions;
                        break;
                    case "photos":
                        model.Active = model.Photos;
                        break;
                }
            }
        }

        private void MentionButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri(String.Format("/NewPostPage.xaml?content={0}", HttpUtility.UrlEncode(String.Format(model.Handle))), UriKind.Relative));
        }

        private async void FollowButton_Click(object sender, EventArgs e)
        {
            if (!model.Following)
            {
                JSONRequestEventArgs<Generic> response = await App.Client.FollowAsync(model.ID);

                if (response.IsSuccessful)
                {
                    Utils.Helpers.Notify(String.Format("You are now following {0}", model.Handle));
                }
            }
            else
            {
                JSONRequestEventArgs<Generic> response = await App.Client.UnfollowAsync(model.ID);

                if (response.IsSuccessful)
                {
                    Utils.Helpers.Notify(String.Format("You are no longer following {0}", model.Handle));
                }
            }

            App.MainViewModel.RefreshFriends();
            JSONRequestEventArgs<User> userargs = await App.Client.GetUserAsync(model.ID);
            if (userargs.IsSuccessful)
                refreshUserDetails(userargs);
        }

        private void MessageButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ChatPage.xaml?id=" + model.ID, UriKind.Relative));
        }

        private void SparklrPostControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrPostControl control = sender as SparklrPostControl;
            if (control != null)
                NavigationService.Navigate(new Uri("/Pages/DetailsPage.xaml?id=" + control.Post.Id, UriKind.Relative));
        }

        private async void PinProfileMenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!await Utils.TilesCreator.PinUserprofile(model.ID, App.Client))
            {
                MessageBox.Show("We could not create a tile for this user. Maybe he's already on your startscreen?", "We're sorry :(", MessageBoxButton.OK);
            }
        }
    }
}