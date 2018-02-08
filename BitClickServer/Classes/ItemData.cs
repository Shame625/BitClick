using System;
using System.Runtime.Serialization;
using static BitClickServer.Constants;

namespace BitClickServer
{
    [DataContract]
    [System.Serializable]
    public class ItemData
    {
        [DataMember(Name = "0")]
        public string Name { get; set; }

        [DataMember(Name = "1")]
        public int Level { get; set; }

        [DataMember(Name = "2")]
        public byte Quality { get; set; }

        [DataMember(Name = "3")]
        public byte Slot { get; set; }

        [DataMember(Name = "4")]
        public ushort IconID { get; set; }

        [DataMember(Name = "5")]
        public ulong? Damage { get; set; }

        [DataMember(Name = "6")]
        public float? CritChance { get; set; }

        [DataMember(Name = "7")]
        public uint? CritStrike { get; set; }

        [DataMember(Name = "8")]
        public uint? Stamina { get; set; }

        [DataMember(Name = "9")]
        public uint? StaminaRechargeRate { get; set; }

        [DataMember(Name = "10")]
        public float? StaminaOnHitChance { get; set; }

        [DataMember(Name = "11")]
        public uint? ClickMultiplier { get; set; }


        public ItemData(string name, int level, Qualities quality, Slots slot, ushort iconId, ulong? damage, float? critChance, uint? critStrike,
            uint? stamina, uint? staminaRechargeRate, float? staminaOnHitChance, uint? clickMulitplier)
        {
            Name = name;
            Level = level;
            Quality = (byte)quality;
            Slot = (byte)slot;
            IconID = iconId;

            Damage = damage;
            CritChance = critChance;
            CritStrike = critStrike;

            Stamina = stamina;
            StaminaRechargeRate = staminaRechargeRate;
            StaminaOnHitChance = staminaOnHitChance;

            ClickMultiplier = clickMulitplier;
        }

        public ItemData() {}
    }
}
