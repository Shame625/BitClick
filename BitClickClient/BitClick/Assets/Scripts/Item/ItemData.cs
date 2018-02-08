using System;
using System.Runtime.Serialization;

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

    public ItemData(string name, int level, Constants.Qualities quality, Constants.Slots slot, ushort iconId, ulong? damage, float? critChance, uint? critStrike,
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

    public ItemData() { }

    public Item.ItemInfo? DisplayInfo()
    {
        //0 = slot, 1 = stats
        Item.ItemInfo itemInfo = new Item.ItemInfo();

        itemInfo.name = Name;
        itemInfo.level = Level.ToString();
        itemInfo.iconID = IconID;

        //decide the quality
        switch ((Constants.Qualities)Quality)
        {
            case Constants.Qualities.NORMAL:
                itemInfo.quality = "Normal";
                break;
            case Constants.Qualities.RARE:
                itemInfo.quality = "Rare";
                break;
            case Constants.Qualities.SUPERIOR:
                itemInfo.quality = "Superior";
                break;
            case Constants.Qualities.EPIC:
                itemInfo.quality = "Epic";
                break;
            case Constants.Qualities.LEGENDARY:
                itemInfo.quality = "Legendary";
                break;
            default:
                return null;
        }

        //decide the slot name
        switch ((Constants.Slots)Slot)
        {
            case Constants.Slots.HAND:
                itemInfo.slot = "Hand";
                break;
            case Constants.Slots.HEAD:
                itemInfo.slot = "Head";
                break;
            case Constants.Slots.MAIN_HAND:
                itemInfo.slot = "Main Hand";
                break;
            case Constants.Slots.NECK:
                itemInfo.slot = "Neck";
                break;
            case Constants.Slots.RING:
                itemInfo.slot = "Ring";
                break;
            case Constants.Slots.TRINKET:
                itemInfo.slot = "Trinket";
                break;
            case Constants.Slots.WAIST:
                itemInfo.slot = "Waist";
                break;
            case Constants.Slots.CHEST:
                itemInfo.slot = "Chest";
                break;
            case Constants.Slots.LEG:
                itemInfo.slot = "Legs";
                break;
            case Constants.Slots.BOOT:
                itemInfo.slot = "Boots";
                break;
        }

        itemInfo.itemstats = "";
        itemInfo.numberOfStats = 0;

        if (Damage != null)
        {
            itemInfo.itemstats += "+ " + Damage + " Damage\n";
            itemInfo.numberOfStats++;
        }

        if (CritChance != null)
        {
        itemInfo.itemstats += "+ " + CritChance + "% Critical Chance\n";
            itemInfo.numberOfStats++;
        }

        if (CritStrike != null)
        {
            itemInfo.itemstats += "+ " + CritStrike + "% Critical Hit Damage\n";
            itemInfo.numberOfStats++;
        }

        if (Stamina != null)
        {
            itemInfo.itemstats += "+ " + Stamina + " Stamina\n";
            itemInfo.numberOfStats++;
        }

        if (StaminaRechargeRate != null)
        {
            itemInfo.itemstats += "+ " + StaminaRechargeRate + " Stamina Per Minute\n";
            itemInfo.numberOfStats++;
        }

        if (StaminaOnHitChance != null)
        {
            itemInfo.itemstats += "+ " + StaminaOnHitChance + "% Chance for Stamina\n";
            itemInfo.numberOfStats++;
        }

        if (ClickMultiplier != null)
        {
            itemInfo.itemstats += "+ " + ClickMultiplier + " x Clicks\n";
            itemInfo.numberOfStats++;
        }

        //removes last \n
        itemInfo.itemstats = itemInfo.itemstats.Remove(itemInfo.itemstats.Length - 1, 1);

        return itemInfo;
    }
}
