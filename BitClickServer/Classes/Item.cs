using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using static BitClickServer.Constants;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace BitClickServer
{
    [System.Serializable]
    public class Item
    {
        public ulong GUID { get; set; }

        [JsonIgnore]
        public string Data { get; set; }

        [NotMapped]
        public ItemData itemData { get; set; }

        private static Random rnd = new Random();

        // TODO: Initialize Item with values from DB
        public Item(string data)
        {
            Data = data;
        }

        // Create item with randomized stats, using level +- deviation to
        // determine item level. Stats are based on item level.
        public static Item GenerateRandomItem(int level, int deviation)
        {
            // Get random name for item
            string name = GenerateItemName();

            // Get a random quality
            Qualities quality = GetRandomQuality();
            (Slots slot, ushort iconId) = GetRandomSlot();

            // Get level based on level and deviation
            rnd = new Random();
            int ilvl = rnd.Next(level - deviation, level + deviation);

            // Item cant have level below 1
            if (ilvl < 1)
                ilvl = 1;

            ulong? damage = null;
            float? critChance = null;
            uint? critStrike = null;

            uint? stamina = null;
            uint? staminaRechargeRate = null;
            float? staminaOnHitChance = null;

            uint? clickMultiplier = null;

            Dictionary<Constants.Stats, uint> stats = CalculateNumberOfStats(ref quality, ref slot);

            foreach(KeyValuePair<Constants.Stats, uint> kv in stats)
            {
                switch(kv.Key)
                {
                    case Constants.Stats.DAMAGE:
                        damage = CalculateDamageStat(ref ilvl, ref quality) * kv.Value;
                        break;
                    case Constants.Stats.CRITICAL_STRIKE_CHANCE:
                        critChance = CalculateCritChanceStat(ref quality);
                        break;
                    case Constants.Stats.CRITICAL_HIT_DAMAGE:
                        critStrike = CalculateCritStrikeStat(ref ilvl, ref quality) * kv.Value;
                        break;
                    case Constants.Stats.STAMINA:
                        stamina = CalculateStaminaStat(ref ilvl, ref quality) * kv.Value;
                        break;
                    case Constants.Stats.STAMINA_RECHARGE_RATE:
                        staminaRechargeRate = CalculateStaminaRechargeRateStat(ref ilvl, ref quality) * kv.Value;
                        break;
                    case Constants.Stats.STAMINA_ON_HIT_CHANCE:
                        staminaOnHitChance = CalculateStaminaChanceOnHitStat(ref ilvl, ref quality);
                        break;
                    case Constants.Stats.CLICK_MULTIPLIER:
                        clickMultiplier = CalculateClickMultiplierState(ref ilvl, ref quality) * kv.Value;
                        break;
                }
            }

            //apply damage to weapon always!
            if (slot == Slots.MAIN_HAND)
            {
                if (damage != null)
                    damage += CalculateDamageStat(ref ilvl, ref quality);
                else
                    damage = CalculateDamageStat(ref ilvl, ref quality);
            }

            ItemData itemData = new ItemData(name, ilvl, quality, slot, iconId, damage, critChance, critStrike, stamina, staminaRechargeRate, staminaOnHitChance, clickMultiplier);
            Item item = new Item();
            item.SetData(itemData);
            return item;
        }

        private static string GenerateItemName()
        {
            return "Test";
        }

        //generate random number of stats on an item
        private static Dictionary<Constants.Stats, uint> CalculateNumberOfStats(ref Qualities quality, ref Slots slot)
        {
            uint statCount = 0;
            Dictionary<Constants.Stats, uint> stats = new Dictionary<Stats, uint>();
            switch (quality)
            {
                case Qualities.LEGENDARY:
                    statCount = (uint)rnd.Next(5, 6);
                    break;
                case Qualities.EPIC:
                    statCount= (uint)rnd.Next(4, 5);
                    break;
                case Qualities.RARE:
                    statCount = (uint)rnd.Next(3, 4);
                    break;
                case Qualities.SUPERIOR:
                    statCount = (uint)rnd.Next(2,3);
                    break;
                case Qualities.NORMAL:
                    statCount = (uint)rnd.Next(1,2);
                    break;
            }

            if(slot != Slots.RING && slot != Slots.TRINKET && slot != Slots.MAIN_HAND)
            for(uint i = 0; i <= statCount; i++)
            {
                double roll = rnd.NextDouble();

                if(roll <= 0.12)
                {
                    if (stats.ContainsKey(Stats.CRITICAL_STRIKE_CHANCE))
                        stats[Stats.CRITICAL_STRIKE_CHANCE]++;
                    else
                        stats[Stats.CRITICAL_STRIKE_CHANCE] = 1;
                }
                else if (roll <= 0.24)
                {
                    if (stats.ContainsKey(Stats.CRITICAL_HIT_DAMAGE))
                        stats[Stats.CRITICAL_HIT_DAMAGE]++;
                    else
                        stats[Stats.CRITICAL_HIT_DAMAGE] = 1;
                }
                else if (roll <= 0.44)
                {
                    if (stats.ContainsKey(Stats.STAMINA))
                        stats[Stats.STAMINA]++;
                    else
                        stats[Stats.STAMINA] = 1;
                }
                else if (roll <= 0.64)
                {
                    if (stats.ContainsKey(Stats.STAMINA_RECHARGE_RATE))
                        stats[Stats.STAMINA_RECHARGE_RATE]++;
                    else
                        stats[Stats.STAMINA_RECHARGE_RATE] = 1;
                }
                else if (roll <= 0.84)
                {
                    if (stats.ContainsKey(Stats.STAMINA_ON_HIT_CHANCE))
                        stats[Stats.STAMINA_ON_HIT_CHANCE]++;
                    else
                        stats[Stats.STAMINA_ON_HIT_CHANCE] = 1;
                }
                else
                {
                    if (stats.ContainsKey(Stats.CLICK_MULTIPLIER))
                        stats[Stats.CLICK_MULTIPLIER]++;
                    else
                        stats[Stats.CLICK_MULTIPLIER] = 1;
                }
            }
            else
                for (uint i = 0; i <= statCount; i++)
                {
                    double roll = rnd.NextDouble();

                    if (roll <= 0.1)
                    {
                        if (stats.ContainsKey(Stats.CRITICAL_STRIKE_CHANCE))
                            stats[Stats.CRITICAL_STRIKE_CHANCE]++;
                        else
                            stats[Stats.CRITICAL_STRIKE_CHANCE] = 1;
                    }
                    else if (roll <= 0.2)
                    {
                        if (stats.ContainsKey(Stats.CRITICAL_HIT_DAMAGE))
                            stats[Stats.CRITICAL_HIT_DAMAGE]++;
                        else
                            stats[Stats.CRITICAL_HIT_DAMAGE] = 1;
                    }
                    else if (roll <= 0.4)
                    {
                        if (stats.ContainsKey(Stats.STAMINA))
                            stats[Stats.STAMINA]++;
                        else
                            stats[Stats.STAMINA] = 1;
                    }
                    else if (roll <= 0.6)
                    {
                        if (stats.ContainsKey(Stats.STAMINA_RECHARGE_RATE))
                            stats[Stats.STAMINA_RECHARGE_RATE]++;
                        else
                            stats[Stats.STAMINA_RECHARGE_RATE] = 1;
                    }
                    else if (roll <= 0.8)
                    {
                        if (stats.ContainsKey(Stats.STAMINA_ON_HIT_CHANCE))
                            stats[Stats.STAMINA_ON_HIT_CHANCE]++;
                        else
                            stats[Stats.STAMINA_ON_HIT_CHANCE] = 1;
                    }
                    else if (roll <= 0.9)
                    {
                        if (stats.ContainsKey(Stats.CLICK_MULTIPLIER))
                            stats[Stats.CLICK_MULTIPLIER]++;
                        else
                            stats[Stats.CLICK_MULTIPLIER] = 1;
                    }
                    else
                    {
                        if (stats.ContainsKey(Stats.DAMAGE))
                            stats[Stats.DAMAGE]++;
                        else
                            stats[Stats.DAMAGE] = 1;
                    }
                }

            return stats;
        }

        private static Qualities GetRandomQuality()
        {
            double roll = rnd.NextDouble();

            // 1% chance
            if (roll <= 0.01)
                return Qualities.LEGENDARY;
            // 5% chance
            else if (roll <= 0.05)
                return Qualities.EPIC;
            // 15% chance
            else if (roll <= 0.15)
                return Qualities.RARE;
            // 25% chance
            else if (roll <= 0.25)
                return Qualities.SUPERIOR;
            else
                return Qualities.NORMAL;
        }

        private static (Slots, ushort iconID) GetRandomSlot()
        {
            double roll = rnd.NextDouble();

            //fix later to return random iconid

            if (roll <= 0.10)
                return (Slots.HAND, 0);
            else if (roll <= 0.20)
                return (Slots.HEAD, (ushort)(rnd.Next(0 , 3)));
            else if (roll <= 0.30)
                return (Slots.MAIN_HAND, 0);
            else if (roll <= 0.40)
                return (Slots.NECK, 0);
            else if (roll <= 0.50)
                return (Slots.RING, 0);
            else if (roll <= 0.60)
                return (Slots.TRINKET, 0);
            else if (roll <= 0.70)
                return (Slots.CHEST, 0);
            else if (roll <= 0.80)
                return (Slots.LEG, 0);
            else if (roll <= 0.90)
                return (Slots.BOOT, 0);
            else
                return (Slots.WAIST, 0);
        }

        private static ulong CalculateDamageStat(ref int ilvl, ref Qualities quality)
        {
            // Get multiplier based on item quality
            float multiplier = GetQualityMultiplier(ref quality);

            // Calculate damage with formula ilvl^log10(ilvl)
            // and multiply with multiplier
            return (ulong)(Math.Pow(ilvl, Math.Log10(ilvl)) * multiplier);
        }

        private static float? CalculateCritChanceStat(ref Qualities quality)
        {
            switch(quality)
            {
                case Qualities.LEGENDARY: return RandomFloatBetweenTwoNumbers(8, 10);
                case Qualities.EPIC: return RandomFloatBetweenTwoNumbers(6, 8);
                case Qualities.RARE: return RandomFloatBetweenTwoNumbers(4, 6);
                case Qualities.SUPERIOR: return RandomFloatBetweenTwoNumbers(2, 4);
                case Qualities.NORMAL: return RandomFloatBetweenTwoNumbers(1, 2);
                default: return null;
            }
        }

        private static uint? CalculateCritStrikeStat(ref int ilvl, ref Qualities quality)
        {
            switch (quality)
            {
                case Qualities.LEGENDARY: return (uint)(rnd.Next(76, 100));
                case Qualities.EPIC: return (uint)(rnd.Next(51, 75));
                case Qualities.RARE: return (uint)(rnd.Next(26, 50));
                case Qualities.SUPERIOR: return (uint)(rnd.Next(11,25));
                case Qualities.NORMAL: return (uint)(rnd.Next(1, 10));
                default: return null;
            }
        }

        private static uint? CalculateStaminaStat(ref int ilvl, ref Qualities quality)
        {
            switch (quality)
            {
                case Qualities.LEGENDARY: return (uint)(100 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.EPIC: return (uint)(75 * ((ilvl / 10 )< 10 ? 1 : 0));
                case Qualities.RARE: return (uint)(50 * ((ilvl / 10 )< 10 ? 1 : 0));
                case Qualities.SUPERIOR: return (uint)(25 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.NORMAL: return (uint)(10 * ((ilvl / 10) < 10 ? 1 : 0));
                default: return null;
            }
        }

        private static uint? CalculateStaminaRechargeRateStat(ref int ilvl, ref Qualities quality)
        {
            switch (quality)
            {
                case Qualities.LEGENDARY: return (uint)(5 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.EPIC: return (uint)(4 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.RARE: return (uint)(3 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.SUPERIOR: return (uint)(2 * ((ilvl / 10) < 10 ? 1 : 0));
                case Qualities.NORMAL: return (uint)(1 * ((ilvl / 10) < 10 ? 1 : 0));
                default: return null;
            }
        }

        private static float? CalculateStaminaChanceOnHitStat(ref int ilvl, ref Qualities quality)
        {
            switch (quality)
            {
                case Qualities.LEGENDARY: return 3;
                case Qualities.EPIC: return 2.5f;
                case Qualities.RARE: return 2;
                case Qualities.SUPERIOR: return 1.5f;
                case Qualities.NORMAL: return 1;
                default: return null;
            }
        }

        private static uint? CalculateClickMultiplierState(ref int ilvl, ref Qualities quality)
        {
            switch (quality)
            {
                case Qualities.LEGENDARY: return 10;
                case Qualities.EPIC: return 6;
                case Qualities.RARE: return 4;
                case Qualities.SUPERIOR: return 2; 
                case Qualities.NORMAL: return 1;
                default: return null;
            }
        }

        private static float GetQualityMultiplier(ref Qualities quality)
        {
            switch(quality)
            {
                case Qualities.NORMAL: return 1.0f;
                case Qualities.SUPERIOR: return 1.2f;
                case Qualities.RARE: return 1.7f;
                case Qualities.EPIC: return 2.3f;
                case Qualities.LEGENDARY: return 3.1f;
                default: return 1.0f;
            }
        }

        static float RandomFloatBetweenTwoNumbers(double minimum, double maximum)
        {
            return (float)Math.Round((rnd.NextDouble() * (maximum - minimum) + minimum) * 2) / 2;
        }

        public bool SetData(ItemData itemdata)
        {
        
        Data = JsonConvert.SerializeObject(itemdata, Formatting.None, Constants.jsonSettings);
            itemData = itemdata;

        if (Data != null)
            return true;

        return false;
        }

        public ItemData GetData()
        {
            return JsonConvert.DeserializeObject<ItemData>(Data);
        }

        public Item() {}
    }
}
