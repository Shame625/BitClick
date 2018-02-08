using System;
using Newtonsoft.Json;

[System.Serializable]
public class Item
{
    public ulong GUID;
    public ItemData itemData;


    public struct ItemInfo
    {
        public byte numberOfStats;

        public string name;
        public string quality;
        public string level;
        public string slot;

        public ushort iconID;

        public string itemstats;
    }
}