using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection.Metadata;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using System.Linq;

namespace BitClickServer
{
    public class BlockRoom
    {
        public static Random rnd = new Random();
        // The clients / players currently in the BlockRoom
        private Dictionary<int, Client> _clientsInRoom ;

        //all damage done, keep any user, regardless if he disconnects so we can award him by DB push
        private Dictionary<int, UserAndDamage> _totalDamageDone;

        public Block Block { get; set; }
        private uint DeadBlockCount;

        public bool isAlive;

        public bool isRestarting;

        private ulong _bitHp;

        public uint Level { get; set; }

        private DateTime aliveTill;

        private DateTime restartAt;

        // Construct a Block consisting of Bits
        // TODO: Implement scaling algorithm to
        // determine the amount of Bits in the block.
        private Block ConstructBlock(uint size)
        {
            return new Block(new Bit[size]);
        }

        public void AddClient(Client client)
        {
            Console.WriteLine(client.GetId() + " joined the room " + Level);
            _clientsInRoom.Add(client.GetId(), client);

            //add user to total dmg
            if(!_totalDamageDone.ContainsKey(client.GetId()))
                _totalDamageDone.Add(client.GetId(), new UserAndDamage(client.GetName(), 0));
        }

        public void RemoveClient(Client client)
        {
            if (_clientsInRoom.ContainsKey(client.GetId()))
            {
                Console.WriteLine(client.GetId() + " left the room " + Level);
                _clientsInRoom.Remove(client.GetId());
            }
        }

        // Make a new room scaled to the given level
        public BlockRoom(uint level)
        {
            GenerateRoom(level);
        }

        //Done like this so we dont have to destroy object and we can just reuse it
        void GenerateRoom(uint level)
        {
            _clientsInRoom = new Dictionary<int, Client>();
            _totalDamageDone = new Dictionary<int, UserAndDamage>();

            Level = level;
            isRestarting = false;
            DeadBlockCount = 0;
            isAlive = true;

            if(Block == null)
                Block = ConstructBlock(Constants.GET_BLOCK_SIZE(level) * Constants.GET_BLOCK_SIZE(level) * Constants.GET_BLOCK_SIZE(level));

            _bitHp = Constants.GET_BIT_HP(level);

            for (int i = 0; i < Block._block.Length; i++)
            {
                if (Block._block[i] == null)
                    Block._block[i] = new Bit(_bitHp);
                else
                {
                    Block._block[i].ResetBit(_bitHp);
                }
            }

            aliveTill = DateTime.Now.AddSeconds(Constants.BLOCK_ALIVE_SECONDS(level));

            Console.WriteLine("Level " + level + " BlockCount: " + Block._block.Length + " Bit Hp: " + _bitHp + " Alive till: " + aliveTill);
        }

        //Block logic here
        public void BitTakeDamage(ref Client client, ref uint bitId)
        {
            //TODO: need to calculate Damage from requesting client, this is just a placeholder
            bool isCrit = false;

            ulong Damage = 1000; //This will be calculated, it takes in account player stats and some RNG

            if (rnd.Next(1, 100) > 50)
            {
                Damage *= 2;
                isCrit = true;
            }

            if (Damage > Block._block[bitId]._health)
                Damage = Block._block[bitId]._health;

            _totalDamageDone[client.GetId()].Dmg += Damage;

            (ulong newBitHp, bool isAlive, List<(int,ulong)> bitDamagers) bitData = Block._block[bitId].TakeDamage(client.GetId(), Damage);



            //Send packet to everyone about new bit data
            Package packet = new Package(PackageType.BIT_HIT_RESPONSE);
            packet.bitHitRes.SetResponse(ref bitId, ref bitData.newBitHp, ref Damage, ref isCrit, client.GetId());

            foreach (KeyValuePair<int, Client> cl in _clientsInRoom)
            {
                Program.SendPacket(cl.Value, ref packet);
            }

            //if bit died, increase dead block counter
            if (bitData.isAlive == false)
            {
                DeadBlockCount++;

                //reward players that destroyed the bit
                RewardPlayersBit(bitData.bitDamagers);
            }

            //Check if all blocks are dead if it is, also set restartAt
            if (DeadBlockCount == Block._block.Length)
            {
                //Send to all clients packet also do calulcations for rewards etc...
                isAlive = false;

                restartAt = DateTime.Now.AddSeconds(Constants.GET_RESPAWN_SECONDS(Level));

                packet = new Package(PackageType.BLOCK_MINED_RESPONSE);
                packet.blockMinedRes.ranks = new Rankings(_totalDamageDone);

                foreach (KeyValuePair<int, Client> cl in _clientsInRoom)
                {
                    Program.SendPacket(cl.Value, ref packet);
                    cl.Value.CurrentRoom = null;
                }

                _clientsInRoom = null;
                Console.WriteLine(Level + " ROOM has been mined! New one will be up at: " + restartAt);
            }
        }

        void OutOfTime()
        {
            //setting when to next restart the room
            restartAt = DateTime.Now.AddSeconds(Constants.GET_RESPAWN_SECONDS(Level));

            Console.WriteLine("Block Room: " + Level + " has closed!\nRestarting at: " + restartAt);

            Package packet = new Package(PackageType.BLOCK_MINED_OUT_OF_TIME_RESPONSE);
            packet.blockMinedFailedRes.ranks = new Rankings(_totalDamageDone);

            //Handle people, by kicking them out of the room and sending them packets of love (they failed)!
            foreach (KeyValuePair<int, Client> cl in _clientsInRoom)
            {
                Program.SendPacket(cl.Value, ref packet);
                cl.Value.CurrentRoom = null;
            }

            //null clients for good meassure
            _clientsInRoom = null;
            isRestarting = true;
        }

        //checks if room is overdue for being destroyed
        public bool TimeForReset()
        {
            //check if room has ran out of time
            if (isAlive)
            {
                if (DateTime.Now >= aliveTill && !isRestarting)
                {
                    //Room died, out of time
                    isAlive = false;
                    OutOfTime();
                    return true;
                }
            }
            //else if time is right for restart, restart it!
            else
            {
                if (DateTime.Now >= restartAt)
                {
                    GenerateRoom(Level);
                }
            }
            return false;
        }

        void RewardPlayersBit(List<(int, ulong)> list)
        {
            //reward players that destroyed bit
            if (list != null)
            {
                uint counter = 0;
                foreach ((int clientId, ulong Damage) obj in list)
                {
                    //first 3 can get lootbox

                    //Update databse even if user is offline
                    using (var db = new GameContext())
                    {
                        //Find account
                        var account = db.Accounts.SingleOrDefault(o => o.Id == obj.clientId);

                        if (account != null)
                        {
                            //Find player
                            var player = db.Players.SingleOrDefault(o => o.Id == account.PlayerId);

                            if (player != null)
                            {

                                var inventory = db.Inventory.SingleOrDefault(o => o.Id == player.InventoryId);

                                if (inventory != null)
                                {
                                    bool smallBox = false;
                                    bool mediumBox = false;

                                    //Roll randomly for top 3 damagers
                                    if (counter <= 2)
                                    {
                                        if (rnd.Next(0, 100) <= Constants.CHANCE_BIT_MEDIUM_LOOTBOX)
                                            mediumBox = true;
                                        else if (rnd.Next(0, 100) <= Constants.CHANCE_BIT_SMALL_LOOTBOX)
                                            smallBox = true;
                                    }

                                    if (smallBox == true || mediumBox == true)
                                    {
                                        inventory.DeserializeData();
                                        if (smallBox)
                                            inventory.lootBoxes.Small++;
                                        else if (mediumBox)
                                            inventory.lootBoxes.Medium++;

                                        inventory.SerializeLootboxes();
                                    }

                                    //Change those to Constants.Exponentional formula
                                    uint currencyGain = Level;
                                    uint experienceGain = Level;

                                    inventory.Currency += currencyGain;
                                    player.Experience += experienceGain;
                                    //calculate if user got new level here, and if he got extra skill point

                                    //player.LevelUP();

                                    try
                                    {
                                        db.SaveChanges();
                                    }
                                    catch
                                    {
                                        Console.WriteLine("Failed to save data for client id: " + obj.clientId);
                                    }

                                    //try to send packet if client is even connected, and check if player is ingame (room)
                                    Client temp = Program.GetClientByid(obj.clientId);
                                    if (temp != null)
                                    {
                                        Package rewardPacket = new Package(PackageType.REWARD_RESPONSE);
                                        rewardPacket.rewardRes.currency = currencyGain;
                                        rewardPacket.rewardRes.experience = experienceGain;

                                        //if client is still online and in memory make sure to increase his stats on server side
                                        temp.Player.Inventory.Currency += currencyGain;
                                        temp.Player.Experience += experienceGain;


                                        if (smallBox == true || mediumBox == true)
                                            rewardPacket.rewardRes.lootBoxes = new Lootboxes();
                                        else
                                            rewardPacket.rewardRes.lootBoxes = null;

                                        if (smallBox)
                                        {
                                            rewardPacket.rewardRes.lootBoxes.Small = 1;
                                            temp.Player.Inventory.lootBoxes.Small++;
                                        }
                                        else if (mediumBox)
                                        {
                                            rewardPacket.rewardRes.lootBoxes.Medium = 1;
                                            temp.Player.Inventory.lootBoxes.Medium++;
                                        }
                                        Program.SendPacket(ref temp, ref rewardPacket);
                                    }
                                }
                            }
                        }
                    }

                    counter++;
                }
            }
        }

        void RewardPlayersBlock()
        {
            //loop trough all people that participated
            foreach (var v in _totalDamageDone)
            {
                
            }
        }

        public LevelDisplayData GetInfo()
        {
            uint clients_in_room;
            if (_clientsInRoom == null)
                clients_in_room = 0;
            else
            {
                clients_in_room = (uint)_clientsInRoom.Count;
            }
            Console.WriteLine(restartAt);
            if(!isAlive)
                return new LevelDisplayData(_bitHp, (uint)Block._block.Length, DeadBlockCount, Level, clients_in_room, restartAt, DateTime.MinValue);

            return new LevelDisplayData(_bitHp, (uint)Block._block.Length, DeadBlockCount, Level, clients_in_room, DateTime.MinValue, aliveTill);
        }
    }

    //used just for UI displaying
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

        public LevelDisplayData(ulong bh, uint nob, uint nodb, uint l, uint nop, DateTime ra, DateTime at)
        {
            _bitHp = bh;
            _numberOfDeadBlocks = nodb;
            _numberOfBlocks = nob;
            Level = l;
            numberOfPeople = nop;
            restartingAt = ra;
            activeTill = at;

            numberOfLevels = (uint)Program._blockRooms.Count;
        }
    }

    public class Rankings
    {
        public List<UserAndDamage> ranks = new List<UserAndDamage>();

        public Rankings(Dictionary<int, UserAndDamage> dict)
        {
            foreach (KeyValuePair<int, UserAndDamage> var in dict)
            {
                ranks.Add(new UserAndDamage(var.Value.Username, var.Value.Dmg));
            }
        }
    }

    public class UserAndDamage
    {
        //string uname

        public string Username;
        public ulong Dmg;

        public UserAndDamage(string n, ulong dmg)
        {
            Username = n;
            Dmg = dmg;
        }
    }
}
