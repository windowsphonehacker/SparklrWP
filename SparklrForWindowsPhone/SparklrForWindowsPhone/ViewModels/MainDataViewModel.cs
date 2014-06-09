
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using Telerik.Windows.Controls;

namespace SparklrForWindowsPhone.ViewModels
{
    public class MainDataViewModel : ViewModelBase
    {
        private ObservableCollection<DataItemViewModel> items;

        /// <summary>
        /// Initializes the items.
        /// </summary>
        private void InitializeItems()
        {
            this.items = new ObservableCollection<DataItemViewModel>();
            for (int i = 1; i <= 7; i++)
            {
                this.items.Add(new DataItemViewModel()
                {
                    ImageSource = new Uri("Images/Frame.png", UriKind.RelativeOrAbsolute),
                    ImageThumbnailSource = new Uri("Images/FrameThumbnail.png", UriKind.RelativeOrAbsolute),
                    Title = "Title " + i,
                    Information = "Information " + i,
                    Group = (i % 2 == 0) ? "EVEN" : "ODD"
                });
            }
        }

        /// <summary>
        /// A collection for <see cref="DataItemViewModel"/> objects.
        /// </summary>
        public ObservableCollection<DataItemViewModel> Items
        {
            get
            {
                if (this.items == null)
                {
                    this.InitializeItems();
                }
                return this.items;
            }
            private set
            {
                this.items = value;
            }
        }
    }
}
