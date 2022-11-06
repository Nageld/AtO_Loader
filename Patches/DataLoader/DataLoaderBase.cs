using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.Patches.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using UnityEngine;

namespace AtO_Loader.Patches.DataLoader
{
    /// <summary>
    /// Abstract class that loads all json files into a specific scriptable object.
    /// </summary>
    /// <typeparam name="T">The scriptable object type to load json to, usually the wrapper class for it.</typeparam>
    public abstract class DataLoaderBase<T>
        where T : ScriptableObject, IDataWrapper
    {
        /// <summary>
        /// Gets the sub folder name the json files reside in, relative to the specific plugin's folder.
        /// </summary>
        protected abstract string DirectoryName { get; }

        /// <summary>
        /// Loads data generically.
        /// </summary>
        /// <returns>Returns a dictionary of the custom data. With key as Id and Value as the object.</returns>
        public virtual Dictionary<string, T> LoadData()
        {
            var datas = new Dictionary<string, T>();
            foreach (var dataFileInfo in DirectoryUtils.GetAllPluginSubFoldersByName(this.DirectoryName, "*.json"))
            {
                try
                {
                    var newData = this.LoadDataFromDisk(dataFileInfo);
                    if (this.ValidateData(newData))
                    {
                        datas[newData.GetID] = newData;
                    }
                    else
                    {
                        Plugin.LogError($"Failed to parse {nameof(T)} from json '{dataFileInfo.FullName}'");
                    }
                }
                catch (Exception ex)
                {
                    Plugin.LogError($"Failed to parse {nameof(T)} from json '{dataFileInfo.FullName}'");
                    Plugin.LogError(ex);
                }
            }

            this.PostProcessing(datas);

            return datas;
        }

        /// <summary>
        /// Gets the file from disk and creats a scriptable object from it,
        /// Then validates the new object that is created.
        /// </summary>
        /// <param name="fileInfo">The <see cref="FileInfo"/> of the json to load.</param>
        /// <returns>The new object that has been loaded or null if failed validation.</returns>
        protected virtual T LoadDataFromDisk(FileInfo fileInfo)
        {
            var json = File.ReadAllText(fileInfo.FullName);
            var newItem = ScriptableObject.CreateInstance<T>();
            JsonUtility.FromJsonOverwrite(json, newItem);
            return this.ValidateData(newItem) ? newItem : null;
        }

        /// <summary>
        /// Validates the new data loaded.
        /// </summary>
        /// <param name="data">The new data to validate.</param>
        /// <returns>True if validation passes.</returns>
        protected abstract bool ValidateData(T data);

        /// <summary>
        /// Post Processing Steps.
        /// </summary>
        /// <param name="datas">The Dictionary of custom data.</param>
        protected virtual void PostProcessing(Dictionary<string, T> datas)
        {
        }
    }
}
