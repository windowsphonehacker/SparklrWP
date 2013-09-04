extern alias ImageToolsDLL;
using Microsoft.Phone.Controls;
using SparklrLib.Objects;
using SparklrLib.Objects.Responses;
using SparklrLib.Objects.Responses.Work;
using System;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class Profile : PhoneApplicationPage
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
        }

        private static ItemViewModel generateItemViewModel(SparklrLib.Objects.Responses.Work.Timeline item)
        {
            return new ItemViewModel(item.id)
            {
                AuthorId = item.from,
                Message = item.message,
                CommentCount = item.commentcount ?? 0,
                ImageUrl = !String.IsNullOrEmpty(item.meta) ? String.Format("http://d.sparklr.me/i/t{0}", item.meta) : null
            };
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

        private void MentionButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri(String.Format("/NewPostPage.xaml?content={0}", HttpUtility.UrlEncode(String.Format(model.Handle))), UriKind.Relative));
        }

        private async void FollowButton_Click(object sender, System.Windows.RoutedEventArgs e)
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
    }
}