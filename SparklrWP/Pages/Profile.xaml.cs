using Microsoft.Phone.Controls;
using SparklrLib.Objects;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class Profile : PhoneApplicationPage
    {
        ProfileViewModel model;

        public Profile()
        {
            InitializeComponent();
            model = new ProfileViewModel();
            this.DataContext = model;
        }

        public bool dataLoaded = false;

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (dataLoaded) return;
            dataLoaded = true;
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("userId", out selectedIndex))
            {
                model.ID = int.Parse(selectedIndex);

                JSONRequestEventArgs<SparklrLib.Objects.Responses.Work.User> usargs = await App.Client.GetUserAsync(model.ID);

                if (usargs.IsSuccessful)
                {
                    this.Dispatcher.BeginInvoke(() =>
                    {
                        model.Handle = "@" + usargs.Object.handle;
                        model.BackgroundImage = "http://d.sparklr.me/i/b" + model.ID + ".jpg";
                        model.ProfileImage = "http://d.sparklr.me/i/" + model.ID + ".jpg";

                        model.Bio = usargs.Object.bio;
                        if (model.Bio.Trim() == "")
                        {
                            model.Bio = usargs.Object.name + " is too shy to write something about his/herself maybe check again later!";
                        }
                    });
                }
            }
        }


    }
}