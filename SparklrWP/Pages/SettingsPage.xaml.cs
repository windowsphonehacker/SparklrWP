using Microsoft.Phone.Controls;

namespace SparklrWP.Pages
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            this.DataContext = new SettingsViewModel();
        }
    }
}