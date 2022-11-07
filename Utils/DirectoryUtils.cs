using BepInEx;
using System.Collections.Generic;
using System.IO;

namespace AtO_Loader.Utils
{
    public static class DirectoryUtils
    {
        public static IEnumerable<FileInfo> GetAllPluginSubFoldersByName(string subFolderName, string searchPattern)
        {
            var pluginFolder = new DirectoryInfo(Paths.PluginPath);
            if (!pluginFolder.Exists)
            {
                throw new DirectoryNotFoundException($"Missing the base plugin folder? {pluginFolder.FullName}");
            }

            foreach (var folder in pluginFolder.EnumerateDirectories())
            {
                var path = Path.Combine(folder.FullName, subFolderName);
                var subFolder = new DirectoryInfo(path);
                if (!subFolder.Exists)
                {
                    continue;
                }

                foreach (var file in subFolder.EnumerateFiles(searchPattern, SearchOption.AllDirectories))
                {
                    yield return file;
                }
            }
        }
    }
}
