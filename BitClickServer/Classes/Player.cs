using System;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BitClickServer
{
    public class Player
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public uint Level { get; set; }
        public int InventoryId { get; set; }
        public ulong Experience { get; set; }
        public uint SkillPoints { get; set; }

        public Inventory Inventory { get; set; }

        [NotMapped]
        Stats stats = new Stats();

        [NotMapped]
        class Stats
        {
            public ulong Damage;
            public float CritChance;
            public uint CritStrike;

            public uint Stamina;
            public uint StaminaRechargeRate;
            public float StaminaChanceOnHit;

            public uint ClickMultiplier;

            public void Reset()
            {
                Damage = 0;
                CritChance = 0;
                CritStrike = 0;

                Stamina = 0;
                StaminaRechargeRate = 0;
                StaminaChanceOnHit = 0;

                ClickMultiplier = 0;
            }
        }

        // TODO: Add damage algorithm
        public void CalculateStats()
        {
            stats.Reset();

            //Recalculate
            foreach(Item currentItem in Inventory.equipment.item_Real)
            {

                if (currentItem != null)
                {
                    Console.WriteLine(currentItem.GUID);
                    if (currentItem.itemData.Damage != null)
                        stats.Damage += (ulong)currentItem.itemData.Damage;

                    if (currentItem.itemData.CritChance != null)
                        stats.CritChance += (float)currentItem.itemData.CritChance;

                    if (currentItem.itemData.CritStrike != null)
                        stats.CritStrike += (uint)currentItem.itemData.CritStrike;

                    if (currentItem.itemData.Stamina != null)
                        stats.Stamina += (uint)currentItem.itemData.Stamina;

                    if (currentItem.itemData.StaminaRechargeRate != null)
                        stats.StaminaRechargeRate += (uint)currentItem.itemData.StaminaRechargeRate;

                    if (currentItem.itemData.StaminaOnHitChance != null)
                        stats.StaminaChanceOnHit += (float)currentItem.itemData.StaminaOnHitChance;

                    if (currentItem.itemData.ClickMultiplier != null)
                        stats.ClickMultiplier += (uint)currentItem.itemData.ClickMultiplier;
                }
            }
            DisplayStats();
        }

        // TODO: Initialize player with data from DB
        public Player()
        {
            Level = Constants.LEVEL_START;
            Experience = 0;
            SkillPoints = 0;
        }

        public void DisplayStats()
        {
            string temp = "Damage: " + stats.Damage + "\nCritical Chance: " + stats.CritChance + " %\nCritical Hit Damage: " + stats.CritStrike +
         " % \nStamina: " + stats.Stamina + "\nStamina Per Minute: " + stats.StaminaRechargeRate + "\nChance for Stamina on Hit: " + stats.StaminaChanceOnHit + " %\nClick Multiplier: " + stats.ClickMultiplier;
            Console.WriteLine(temp);
        }
    }
}
