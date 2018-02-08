using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private NetworkHelper networkHelper;

    public uint skillPoints;
    public ulong currency;
    public ulong experience;

    public Inventory inventory;

    public Stats stats;
    public Skills skills;

    //used for raycasting to ignore other layers but 1
    LayerMask layerMask = 1 << 8;

    private void Awake()
    {
        networkHelper = GetComponent<NetworkHelper>();
        stats = new Stats();
        skills = new Skills();
    }

    void Update()
    {
        if (GameManager.inGame)
        {
            if(Input.GetMouseButtonDown(0))
                ClickBit();
        }
    }

    void ClickBit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            if (hit.transform.tag == "Bit")
            {
                Transform objectHit = hit.transform;

                networkHelper.DoDamage(ref objectHit.gameObject.GetComponent<BitObj>().Id, hit.point);
            }

        }
    }

    public Player()
    {
        inventory = new Inventory();
    }

    public void CalculateStats()
    {
        stats.Reset();

        foreach (ulong u in inventory.equipment.items)
        {

            if (u != 0)
            {
                ItemData currentItem = null;
                try
                {
                    currentItem = CacheManager.cachedItems[u];
                }
                catch
                {
                    return;
                }

                if (currentItem.Damage != null)
                    stats.damage += (ulong)currentItem.Damage;

                if (currentItem.CritChance != null)
                    stats.critChance += (float)currentItem.CritChance;

                if (currentItem.CritStrike != null)
                    stats.critStrike += (uint)currentItem.CritStrike;

                if (currentItem.Stamina != null)
                    stats.stamina += (uint)currentItem.Stamina;

                if (currentItem.StaminaRechargeRate != null)
                    stats.staminaRechargeRate += (uint)currentItem.StaminaRechargeRate;

                if (currentItem.StaminaOnHitChance != null)
                    stats.staminaOnHitChance += (float)currentItem.StaminaOnHitChance;

                if (currentItem.ClickMultiplier != null)
                    stats.clickMultiplier += (uint)currentItem.ClickMultiplier;
            }
        }
    }

    public class Stats
    {
        public ulong damage = 0;
        public float critChance = 0;
        public uint critStrike = 0;

        public uint stamina = 0;
        public uint staminaRechargeRate = 0;
        public float staminaOnHitChance = 0;

        public uint clickMultiplier = 0;

        public void Reset()
        {
            damage = 0;
            critChance = 0;
            critStrike = 0;

            stamina = 0;
            staminaRechargeRate = 0;
            staminaOnHitChance = 0;

            clickMultiplier = 0;
        }

        public override string ToString()
        {
            string temp = "Damage: " + damage + "\nCritical Chance: " + critChance + " %\nCritical Hit Damage: " + critStrike +
                " % \nStamina: " + stamina + "\nStamina Per Minute: " + staminaRechargeRate + "\nChance for Stamina on Hit: " + staminaOnHitChance + " %\nClick Multiplier: " + clickMultiplier;
            return temp;
        }
    }

    //load them
    public class Skills
    {
        public uint Stamina_Percent = 0;
        public uint Damage_Percent = 0;
        public uint Click_Multiplier = 0;
        public uint Stamina_Recharge_Percent = 0;
        public uint Critical_Hit_Damage_Percent = 0;
    }
}
