using System.IO.IsolatedStorage;

namespace SparklrWP
{
    /// <summary>
    /// Provide bindable properties for Silverlight.
    /// </summary>
    public class BindableSettings
    {
        public double FontSize
        {
            get
            {
                return Settings.FontSize;
            }
        }
    }

    public static class Settings
    {
        static Settings()
        {
            loadSettings();
        }

        private static double fontSize = 20;
        public static double FontSize
        {
            get
            {
                return fontSize;
            }
            set
            {
                if (fontSize != value && value > 0)
                {
                    fontSize = value;
                    saveAppSeting("fontSize", fontSize);
                }
            }
        }

        private static bool loadGIFsInStream = false;
        public static bool LoadGIFsInStream
        {
            get
            {
                return loadGIFsInStream;
            }
            set
            {
                if (loadGIFsInStream != value)
                {
                    loadGIFsInStream = value;
                    saveAppSeting("loadGIFsInStream", loadGIFsInStream);
                }
            }
        }

        private static void saveAppSeting(string key, object value)
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove(key);
                }
                IsolatedStorageSettings.ApplicationSettings.Add(key, value);
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
        }


        private static void loadSettings()
        {
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<bool>("loadGIFsInStream", out loadGIFsInStream);
                IsolatedStorageSettings.ApplicationSettings.TryGetValue<double>("fontSize", out fontSize);

                if (fontSize <= 0)
                    fontSize = 20;
            }
        }
    }
}
