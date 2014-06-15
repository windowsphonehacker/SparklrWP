using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SparklrForWindowsPhone.Resources;
using SparklrForWindowsPhone.Helpers;
using SparklrSharp;
using SparklrSharp.Extensions;
using SparklrSharp.Sparklr;

namespace SparklrForWindowsPhone.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        /// <summary>
        /// Sample property that returns a localized string
        /// </summary>
        
        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Performs an initial load of the stram data
        /// </summary>
        public async void LoadData()
        {
            GlobalLoadingIndicator.Start();

            // Sample data; replace with real data
            Stream everythingStream = await Stream.InstanciateStreamAsync("everything", Housekeeper.ServiceConnection);

            foreach(Post p in everythingStream.Posts)
            {
                Items.Add(new ItemViewModel() {
                    LineOne = p.Author.Handle,
                    LineTwo = p.Content,
                    LineThree = p.CommentCount.ToString()
                });
            }

            this.IsDataLoaded = true;

            GlobalLoadingIndicator.Stop();
        }
    }
}