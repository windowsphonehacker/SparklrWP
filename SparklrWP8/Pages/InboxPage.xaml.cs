using Microsoft.Phone.Controls;
using SparklrWP.ViewModels;
using System;
using System.Windows.Controls;

namespace SparklrWP.Pages
{
    public partial class InboxPage : PhoneApplicationPage
    {
        InboxViewModel model;
        public InboxPage()
        {
            InitializeComponent();
            model = new InboxViewModel();
            model.Load();
            this.DataContext = model;
        }

        private void Grid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ConversationModel m = ((Grid)sender).DataContext as ConversationModel;

            if (m != null)
                NavigationService.Navigate(new Uri("/Pages/ChatPage.xaml?id=" + m.From.ToString(), UriKind.Relative));
        }

        private void RefreshIconButton_Click(object sender, System.EventArgs e)
        {
            model.Load();
        }
    }
}