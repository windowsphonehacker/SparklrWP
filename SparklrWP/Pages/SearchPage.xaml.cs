using Microsoft.Phone.Controls;
using SparklrWP.Utils;
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
            ((TextBox)sender).UpdateBinding();

            if (e.Key == Key.Enter)
                doSearch();
        }

        private void doSearch()
        {
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
    }
}