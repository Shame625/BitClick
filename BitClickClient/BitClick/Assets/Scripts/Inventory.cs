using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Inventory
{
    public uint bagSize;

    public Lootboxes lootBoxes;
    public Bag bag;
    public Equipment equipment;
}

public class Lootboxes
{
    public uint Small = 0;
    public uint Medium = 0;
    public uint Big = 0;

    public void Addition(ref Lootboxes lb)
    {
        Small += lb.Small;
        Medium += lb.Medium;
        Big += lb.Big;
    }

    public void Deduce(Constants.LootboxType type, uint amount)
    {
        if(type == Constants.LootboxType.Small)
        {
            if (Small == 0)
                return;
            Small -= amount;
        }
        else if (type == Constants.LootboxType.Medium)
        {
            if (Medium == 0)
                return;
            Medium -= amount;
        }
        else if (type == Constants.LootboxType.Big)
        {
            if (Big == 0)
                return;
            Big -= amount;
        }
    }
}

public class Bag
{
    public ulong[] items;

    public void AddToFirstAvailableSlot(ulong GUID)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == 0)
            {
                items[i] = GUID;
                return;
            }
        }
    }
}

public class Equipment
{
    public ulong[] items;
}