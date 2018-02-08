using System;
using Newtonsoft.Json;

public class Bit
{
    [JsonProperty(PropertyName = "hp")]
    public ulong _health;

    public bool isAlive;

    public Bit(ulong health = 100)
    {
        _health = health;
        isAlive = true;
    }

    public void SetHP(ulong newHp)
    {
        _health = newHp;

        if (_health == 0)
            isAlive = false;
    }
}
