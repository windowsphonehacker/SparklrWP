using Microsoft.Phone.Controls;
using SparklrWP.Controls;
using SparklrWP.ViewModels;
using System;
using System.Windows.Navigation;

namespace SparklrWP.Pages
{
    public partial class TagsPage : PhoneApplicationPage
    {
        TagViewModel model;

        public TagsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string tag;

            if (NavigationContext.QueryString.TryGetValue("tag", out tag))
            {
                model = new TagViewModel(tag);
                this.DataContext = model;
            }
            else
            {
#if DEBUG
                throw new NotSupportedException("You need to supply a tag");
#endif
            }

            base.OnNavigatedTo(e);
        }

        private void SparklrPostControl_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrPostControl control = sender as SparklrPostControl;

            if (control != null)
            {
                NavigationService.Navigate(new System.Uri("/Pages/DetailsPage.xaml?id=" + control.Post.Id, System.UriKind.Relative));
            }
        }
    }
}