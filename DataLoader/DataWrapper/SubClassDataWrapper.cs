﻿namespace AtO_Loader.DataLoader.DataWrapper;

public class SubClassDataWrapper : SubClassData, IDataWrapper
{
    public int[] cardCounts;
    public string[] cardIds;
    public string trait1ACard;
    public string trait1BCard;
    public string trait3ACard;
    public string trait3BCard;
    public string startingItem;

    /// <summary>
    /// To prevent a null ref in <see cref="SubClassData"/>'s Awake.
    /// </summary>
    private void Awake()
    {
    }

    /// <inheritdoc/>
    public string DataID { get => this.SubClassName; set => this.SubClassName = value; }
}