using System;
using System.Collections.Generic;
using System.IO;
using AtO_Loader.DataLoader.DataWrapper;
using AtO_Loader.Utils;
using UnityEngine;
using static Enums;

namespace AtO_Loader.Patches.DataLoader;

/// <summary>
/// Abstract class that loads all json files into a specific scriptable object.
/// </summary>
/// <typeparam name="T1">The IDataWrapper object type to load json to, usually the wrapper class for it.</typeparam>
/// <typeparam name="T2">The Scriptable Object that the game holds the data as.</typeparam>
public abstract class DataLoaderBase<T1, T2>
    where T1 : ScriptableObject, IDataWrapper
    where T2 : ScriptableObject
{
    /// <summary>
    /// Hardcoded list of classes that cardClass -1 will generate for.
    /// </summary>
    private static readonly List<CardClass> CardClasses = new()
    {
        CardClass.Warrior,
        CardClass.Mage,
        CardClass.Healer,
        CardClass.Scout,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DataLoaderBase{T1, T2}"/> class.
    /// </summary>
    /// <param name="dataSource">Data source for this loader.</param>
    public DataLoaderBase(Dictionary<string, T2> dataSource) => this.DataSource = dataSource;

    /// <summary>
    /// Gets the sub folder name the json files reside in, relative to the specific plugin's folder.
    /// </summary>
    protected abstract string DirectoryName { get; }

    /// <summary>
    /// Gets the games data source for this loader, read only purposes.
    /// </summary>
    protected Dictionary<string, T2> DataSource { get; }

    /// <summary>
    /// Loads data generically.
    /// </summary>
    /// <param name="dataSource">The dictionary for the data source.</param>
    /// <returns>Returns a dictionary of the custom data. With key as Id and Value as the object.</returns>
    public virtual Dictionary<string, T1> LoadData()
    {
        var datas = new Dictionary<string, T1>();
        foreach (var dataFileInfo in DirectoryUtils.GetAllPluginSubFoldersByName(this.DirectoryName, "*.json"))
        {
            try
            {
                Plugin.LogInfo($"Reading json from disk {dataFileInfo.FullName}");
                var data = this.LoadDataFromDisk(dataFileInfo);

                Plugin.LogInfo($"Validating data object {dataFileInfo.Name}");
                if (this.ValidateData(data))
                {
                    Plugin.LogInfo($"For Loop Processing {dataFileInfo.Name}");
                    this.ForLoopProcessing(datas, data);
                }
                else
                {
                    Plugin.LogError($"Failed to parse {nameof(T1)} from json '{dataFileInfo.FullName}'");
                }
            }
            catch (Exception ex)
            {
                Plugin.LogError($"Failed to parse {nameof(T1)} from json '{dataFileInfo.FullName}'");
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
    protected virtual T1 LoadDataFromDisk(FileInfo fileInfo)
    {
        var json = File.ReadAllText(fileInfo.FullName);
        var data = ScriptableObject.CreateInstance<T1>();
        data.DataID = data.DataID?.ToLower();
        JsonUtility.FromJsonOverwrite(json, data);
        if (this.ValidateData(data))
        {
            this.PostLoadDataFromDisk(fileInfo, data);
            return data;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// After loading data from disk processing.
    /// </summary>
    /// <param name="fileInfo">FileInfo for data.</param>
    /// <param name="data">New data object.</param>
    protected virtual void PostLoadDataFromDisk(FileInfo fileInfo, T1 data)
    {
    }

    /// <summary>
    /// Validates the new data loaded.
    /// </summary>
    /// <param name="data">The new data to validate.</param>
    /// <returns>True if validation passes.</returns>
    protected abstract bool ValidateData(T1 data);

    /// <summary>
    /// Post Processing Steps.
    /// </summary>
    /// <param name="datas">The Dictionary of custom data.</param>
    protected virtual void PostProcessing(Dictionary<string, T1> datas)
    {
    }

    /// <summary>
    /// Processing steps during the for loop.
    /// </summary>
    /// <param name="datas">The dictionary to add data to.</param>
    /// <param name="data">The new data.</param>
    protected virtual void ForLoopProcessing(Dictionary<string, T1> datas, T1 data)
    {
        Plugin.LogInfo($"Loaded: {data.DataID}");
        datas[data.DataID] = data;
    }
}
