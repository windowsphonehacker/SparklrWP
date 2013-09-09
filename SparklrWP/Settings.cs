using System.IO.IsolatedStorage;

namespace SparklrWP
{
    public static class Settings
    {
        static Settings()
        {
            loadSettings();
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
            }
        }
    }
}
