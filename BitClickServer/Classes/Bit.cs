using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BitClickServer
{
    public class Bit
    {
        [JsonProperty(PropertyName = "hp")]
        public ulong _health;

        [JsonIgnore]
        public bool isAlive;

        //client id, and damage
        [JsonIgnore]
        Dictionary<int, ulong> BitDamagers;

        public Bit(ulong health = 100)
        {
            _health = health;
            BitDamagers = new Dictionary<int, ulong>();

            isAlive = true;
        }

        public void ResetBit(ulong health = 100)
        {
            _health = health;
            BitDamagers = new Dictionary<int, ulong>();

            isAlive = true;
        }

        public (ulong, bool, List<(int,ulong)>) TakeDamage(int clientId, ulong dmg)
        {
            if (BitDamagers.ContainsKey(clientId))
                BitDamagers[clientId] += dmg;
            else
                BitDamagers[clientId] = dmg;

            List<(int, ulong)> topDamagers = null;

            if ((int)(_health - dmg) <= 0)
            {
                _health = 0;
                isAlive = false;

                //Bit destroyed, get top 3 damagers

                topDamagers = new List<(int, ulong)>();
                foreach(var v in BitDamagers)
                {
                    topDamagers.Add((v.Key, v.Value));
                }

                topDamagers = topDamagers.OrderByDescending(o=>o.Item2).ToList();
            }
            else
            {
                _health -= dmg;
            }

            return (_health, isAlive, topDamagers);
        }
    }
}
