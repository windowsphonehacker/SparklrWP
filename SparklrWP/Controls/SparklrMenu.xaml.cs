using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using SparklrWP.Controls;
using SparklrWP.Utils;
using SparklrWP.Utils.Extensions;
using SparklrWP.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace SparklrWP.Controls
{
    public partial class SparklrMenu : UserControl
    {
        bool popupVisible = false;
        public SparklrMenu()
        {
            InitializeComponent();
        }
       
        protected void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (popupVisible)
            {
                NotificationDisappear.Begin();
                popupVisible = false;
                e.Cancel = true;
            }
        }
        
        private void BorderNotification_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (!popupVisible)
            {
                NotificationAppear.Begin();
                popupVisible = true;
            }
        }
    }
}
