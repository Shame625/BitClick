using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
    public static List<Texture2D> levelThemes = new List<Texture2D>();

    public static Dictionary<ushort, Sprite> RingSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> MainHandSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> HeadSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> TrinketSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> NeckSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> HandSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> WaistSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> ChestSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> LegSprites = new Dictionary<ushort, Sprite>();
    public static Dictionary<ushort, Sprite> BootSprites = new Dictionary<ushort, Sprite>();

    private void Awake()
    {
        LoadAssets();
    }

    void LoadAssets()
    {
        LoadRings();
        LoadMainHand();
        LoadHead();
        LoadTrinket();
        LoadNeck();
        LoadHand();
        LoadWaist();
        LoadChest();
        LoadLeg();
        LoadBoot();

        LoadLevelThemes();
    }

    void LoadLevelThemes()
    {
        object[] temp = Resources.LoadAll("Themes", typeof(Texture2D));

        foreach (object t in temp)
        {
            levelThemes.Add((Texture2D)t);
        }

        Constants.NUMBER_OF_THEMES = (uint)temp.Length;
    }

    void LoadRings()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Ring", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            RingSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadMainHand()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\MainHand", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            MainHandSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadHead()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Head", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            HeadSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadTrinket()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Trinket", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            TrinketSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadNeck()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Neck", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            NeckSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadHand()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Hand", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            HandSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadWaist()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Waist", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            WaistSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadChest()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Chest", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            ChestSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadLeg()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Leg", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            LegSprites[counter] = t as Sprite;
            counter++;
        }
    }

    void LoadBoot()
    {
        object[] temp = Resources.LoadAll("ItemIcons\\Boot", typeof(Sprite));

        ushort counter = 0;

        foreach (object t in temp)
        {
            BootSprites[counter] = t as Sprite;
            counter++;
        }
    }
}