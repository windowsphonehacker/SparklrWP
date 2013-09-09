using System;
using System.ComponentModel;

namespace SparklrWP
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        public bool LoadAnimatedGIFs
        {
            get
            {
                return Settings.LoadGIFsInStream;
            }
            set
            {
                if (Settings.LoadGIFsInStream != value)
                {
                    Settings.LoadGIFsInStream = value;
                    NotifyPropertyChanged("LoadAnimatedGIFs");
                }
            }
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