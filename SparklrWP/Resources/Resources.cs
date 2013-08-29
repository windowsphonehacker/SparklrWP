using System;

namespace SparklrWP.Resources
{
    class Resources
    {
        private readonly static AppResource localizedresources = new AppResource();

        public AppResource LocalizedResources
        {
            get
            {
                return localizedresources;
            }
        }
    }
}
