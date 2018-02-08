using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public class BlockRoom
{
    public Block Block { get; set; }

    public ulong _bitHp { get; set; }

    public uint Level { get; set; }

    [JsonIgnore]
    public ulong ColectiveBlockHp { get; set; }
    [JsonIgnore]
    public ulong CurrentBlockHp = 0;

    [JsonIgnore]
    public DateTime activeTill;
}

public class LevelDisplayData
{
    public ulong _bitHp;
    public uint _numberOfBlocks;
    public uint _numberOfDeadBlocks;
    public uint Level;
    public uint numberOfPeople;
    public uint numberOfLevels;
    public DateTime restartingAt;
    public DateTime activeTill;
}

public class Rankings
{
    public List<UserAndDamage> ranks;
}

public class UserAndDamage
{
    //string uname
    public string Username;
    public ulong Dmg;
}