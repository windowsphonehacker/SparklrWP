using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;


namespace SparklrWP
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// A collection for ItemViewModel objects.
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        private string _sampleProperty = "Sample Runtime Property Value";
        /// <summary>
        /// Sample ViewModel property; this property is used in the view to display its value using a Binding
        /// </summary>
        /// <returns></returns>
        public string SampleProperty
        {
            get
            {
                return _sampleProperty;
            }
            set
            {
                if (value != _sampleProperty)
                {
                    _sampleProperty = value;
                    NotifyPropertyChanged("SampleProperty");
                }
            }
        }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            App.Client.BeginRequest(loadCallback,
#if DEBUG
 "beacon/stream/2?since=1377357375&n=0&network=1" //Development network
#else
 "beacon/stream/0?since=1377357375&n=0"
#endif
                ); //TODO: fix this hack
        }

        private bool loadCallback(string result)
        {
            JToken token = JObject.Parse(result).SelectToken("data");
            int count = (int)token.SelectToken("length");
            foreach (var t in token.SelectToken("timeline").Children())
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                  {
                      this.Items.Add(new ItemViewModel() { LineOne = (string)t.SelectToken("message") });
                  });
            }
            this.IsDataLoaded = true;
            return this.IsDataLoaded;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}