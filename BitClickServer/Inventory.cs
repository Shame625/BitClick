using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitClickServer
{
    public class Inventory
    {
        public int Id { get; set; }

        public ulong Currency { get; set; }
        public uint BagSize { get; set; }

        public string BagString { get; set; }
        public string EquipmentString { get; set; }
        public string LootboxesString { get; set; }

        public Lootboxes lootBoxes;
        public Bag bag;
        public Equipment equipment;

        //called when registering
        public Inventory()
        {
            BagSize = Constants.BAG_START_SIZE;

            LootboxesString = JsonConvert.SerializeObject(new Lootboxes { Small = 0, Medium = 0, Big = 0 }, Constants.jsonSettings);

            bag = new Bag();
            bag.items = new ulong[Constants.BAG_START_SIZE];
            BagString = JsonConvert.SerializeObject(bag, Constants.jsonSettings);

            equipment = new Equipment();
            equipment.items = new ulong[Constants.equipmentSlots.Count];
            EquipmentString = JsonConvert.SerializeObject(equipment, Constants.jsonSettings);
        }

        //called upon login
        public void DeserializeData()
        {
            //deserialize lootBoxes stirng to actual lootbox object
            try
            {
                lootBoxes = JsonConvert.DeserializeObject<Lootboxes>(LootboxesString);
                bag = JsonConvert.DeserializeObject<Bag>(BagString);
                equipment = JsonConvert.DeserializeObject<Equipment>(EquipmentString);
            }
            catch
            {
                Console.WriteLine("Failed to deserialize inventory data!");
            }
        }

        public bool CheckValidity()
        {
            bool needsSave = false;

            if (string.IsNullOrEmpty(BagString))
            {
                bag.items = new ulong[BagSize];
                SerializeBag();
                needsSave = true;
            }

            if (string.IsNullOrEmpty(EquipmentString))
            {
                equipment.items = new ulong[Constants.equipmentSlots.Count];
                SerializeEquipment();
                needsSave = true;
            }

            return needsSave;
        }

        public void SerializeLootboxes()
        {
            LootboxesString = JsonConvert.SerializeObject(lootBoxes, Constants.jsonSettings);
        }

        public string SerializeBag()
        {
            return BagString = JsonConvert.SerializeObject(bag, Constants.jsonSettings);
        }

        public string SerializeEquipment()
        {
            return EquipmentString = JsonConvert.SerializeObject(equipment, Constants.jsonSettings);
        }
    }

    public class Lootboxes
    {
        public uint Small;
        public uint Medium;
        public uint Big;

        public uint GetCount(Constants.LootboxType type)
        {
            if (type == Constants.LootboxType.Small)
            {
                return Small;
            }
            else if (type == Constants.LootboxType.Medium)
            {
                return Medium;
            }
            else if (type == Constants.LootboxType.Big)
            {
                return Big;
            }

            return 0;
        }

        public void Add(Constants.LootboxType type, uint amount)
        {
            if (type == Constants.LootboxType.Small)
            {
                Small += amount;
            }
            else if (type == Constants.LootboxType.Medium)
            {
                Medium += amount;
            }
            else if (type == Constants.LootboxType.Big)
            {
                Big += amount;
            }
        }

        public void Deduce(Constants.LootboxType type, uint amount)
        {
            if (type == Constants.LootboxType.Small)
            {
                Small -= amount;
            }
            else if (type == Constants.LootboxType.Medium)
            {
                Medium -= amount;
            }
            else if (type == Constants.LootboxType.Big)
            {
                Big -= amount;
            }
        }
    }

    public class Bag
    {
        public ulong[] items;

        [JsonIgnore]
        public Item[] item_Real;

        public uint GetFreeSlots()
        {
            uint free = 0;

            foreach(ulong u in items)
            {
                if (u == 0)
                    free++;
            }

            return free;
        }

        public uint[] GetFreeIndexes(uint requiredSlots)
        {
            uint[] freeIndexes = new uint[requiredSlots];

            byte currentIndex = 0;

            for(uint i = 0; i < items.Length; i++)
            {
                if(items[i] == 0)
                {
                    freeIndexes[currentIndex] = i;
                    currentIndex++;
                    if (currentIndex == requiredSlots)
                        return freeIndexes;
                }
            }
            return freeIndexes;
        }
    }

    public class Equipment
    {
        //must be 9 items, to follow current model
        public ulong[] items;

        [JsonIgnore]
        public Item[] item_Real;
    }
}
