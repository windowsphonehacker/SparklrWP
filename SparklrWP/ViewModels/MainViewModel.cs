﻿using System;
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
using System.Threading;


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

        public int LastTime = 1377357375;
        private bool isInLoadCycle = false;

        /// <summary>
        /// Creates and adds a few ItemViewModel objects into the Items collection.
        /// </summary>
        public void LoadData()
        {
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    while (isInLoadCycle)
                    {
                    }
                    isInLoadCycle = true;
                    GlobalLoading.Instance.IsLoading = true;
                    App.Client.BeginRequest(loadCallback,
#if DEBUG
 "beacon/stream/2?since=" + LastTime + "&n=0&network=1" //Development network
#else
 "beacon/stream/0?since="+LastTime+"&n=0"
#endif
                        ); //TODO: fix this hack
                    Thread.Sleep(10000);
                }
            }));
            t.Start();
        }
        private bool loadCallback(string result)
        {
            if (result == null || result == "")
            {
                GlobalLoading.Instance.IsLoading = false;
                return false;
            }
            SparklrLib.Objects.Responses.Beacon.Stream stream = JsonConvert.DeserializeObject<SparklrLib.Objects.Responses.Beacon.Stream>(result);
            if (stream == null || stream.data == null)
            {
                GlobalLoading.Instance.IsLoading = false;
                return true;
            }
            int count = stream.data.length;
            foreach (var t in stream.data.timeline)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                  {
                      this.Items.Add(new ItemViewModel() { LineOne = t.message });
                  });
                if (LastTime < t.time)
                {
                    LastTime = t.time;
                }
                if (LastTime < t.modified)
                {
                    LastTime = t.modified;
                }
            }
            isInLoadCycle = false;
            GlobalLoading.Instance.IsLoading = false;
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