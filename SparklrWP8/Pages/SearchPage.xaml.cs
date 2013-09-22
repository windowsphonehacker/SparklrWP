using Microsoft.Phone.Controls;
using SparklrWP.Controls;
using SparklrWP.Utils;
using SparklrWP.ViewModels;
using System.Windows.Controls;
using System.Windows.Input;

namespace SparklrWP.Pages
{
    public partial class SearchPage : PhoneApplicationPage
    {
        SearchViewModel model;

        public SearchPage()
        {
            InitializeComponent();
            model = new SearchViewModel();
            this.DataContext = model;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                doSearch();
        }

        private void doSearch()
        {
            searchTextBox.UpdateBinding();
            this.Focus();
            model.Search();
        }

        private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            searchTextBox.Focus();
        }

        private void searchTextBox_ActionIconTapped(object sender, System.EventArgs e)
        {
            doSearch();
        }

        private void UserStackPanel_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            UserItemViewModel i = ((StackPanel)sender).DataContext as UserItemViewModel;
            if (i != null)
            {
                NavigationService.Navigate(new System.Uri("/Pages/ProfilePage.xaml?userId=" + i.Id.ToString(), System.UriKind.Relative));
            }
        }

        private void SparklrText_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            SparklrPostControl control = sender as SparklrPostControl;
            if (control != null)
            {
                NavigationService.Navigate(new System.Uri("/Pages/DetailsPage.xaml?id=" + control.Post.Id, System.UriKind.Relative));
            }
        }
    }
}