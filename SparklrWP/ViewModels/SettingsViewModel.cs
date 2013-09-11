using SparklrWP.Utils;
using System;
using System.ComponentModel;

namespace SparklrWP.ViewModels
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

        public double FontSize
        {
            get
            {
                return Settings.FontSize;
            }
            set
            {
                if (Settings.FontSize != value)
                {
                    Settings.FontSize = value;
                    NotifyPropertyChanged("FontSize");
                }
            }
        }

        public string UsedByApplicationText
        {
            get
            {
                double mb = Utils.Caching.Image.GetCacheFolderSize().ConvertBytesToMegabytes();
                return String.Format("The cache currently occupies {0:0.00}MB", mb);
            }
        }

        public void CleanCache()
        {
            SparklrWP.Utils.Caching.Image.CleanImageCache();
            NotifyPropertyChanged("UsedByApplicationText");
        }

        public void ClearCache()
        {
            SparklrWP.Utils.Caching.Image.ClearImageCache();
            NotifyPropertyChanged("UsedByApplicationText");
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