using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class CacheManager
{
    public static Dictionary<ulong, ItemData> cachedItems = new Dictionary<ulong, ItemData>();

    public static void TrySave(Item[] items)
    {
        foreach (Item i in items)
        {
            try
            {
                cachedItems[i.GUID] = i.itemData;
            }
            catch
            {
                return;
            }
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/itemCache.bra");
       
        bf.Serialize(file, cachedItems);
        file.Close();
    }

    public static void LoadCache()
    {
        if (File.Exists(Application.persistentDataPath + "/itemCache.bra"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/itemCache.bra", FileMode.Open);
            cachedItems = (Dictionary<ulong, ItemData>)bf.Deserialize(file);
            file.Close();
        }
    }
}
