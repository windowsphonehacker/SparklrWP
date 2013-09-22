using Microsoft.Phone.Controls;
using SparklrWP.ViewModels;

namespace SparklrWP.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        SettingsViewModel model;
        public SettingsPage()
        {
            InitializeComponent();
            model = new SettingsViewModel();
            DataContext = model;
        }

        private void ClearButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            model.ClearCache();
        }

        private void CleanButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            model.CleanCache();
        }
    }
}