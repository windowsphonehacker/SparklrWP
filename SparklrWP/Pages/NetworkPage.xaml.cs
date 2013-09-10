using Microsoft.Phone.Controls;
using SparklrWP.Controls;
using System;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class NetworkPage : PhoneApplicationPage
    {
        NetworkViewModel model;
        public NetworkPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string network;

            if (NavigationContext.QueryString.TryGetValue("network", out network))
            {
                model = new NetworkViewModel(network);
                DataContext = model;
            }

            base.OnNavigatedTo(e);
        }

        private void postsExtendedListBox_TopRefresh(object sender, System.EventArgs e)
        {
            model.Refresh();
        }

        private void SparklrPostControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrPostControl control = sender as SparklrPostControl;
            if (control != null)
            {
                NavigationService.Navigate(new Uri("/Pages/DetailsPage.xaml?id=" + control.Post.Id, UriKind.Relative));
            }
        }
    }
}