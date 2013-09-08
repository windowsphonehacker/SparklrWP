using Microsoft.Phone.Shell;
using System;
using System.Reflection;

namespace SparklrWP.Utils
{
    public static class ShellTileExtensions
    {
        public static void CreateWideTile(Uri uri, ShellTileData tiledata)
        {
            Type shellTileType = Type.GetType("Microsoft.Phone.Shell.ShellTile, Microsoft.Phone");
            MethodInfo createmethod = shellTileType.GetMethod("Create", new Type[] { typeof(Uri), typeof(ShellTileData), typeof(bool) });
            createmethod.Invoke(null, new object[] { uri, tiledata, true });
        }
    }
}
