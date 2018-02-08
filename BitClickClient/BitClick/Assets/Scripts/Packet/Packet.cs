using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets
{
    //All opcodes go into this enum
    public enum PackageType : uint
    {
        REG_REQUEST = 0x0001,
        REG_RESPONSE = 0x0002,

        LOGIN_REQUEST = 0x0003,
        LOGIN_RESPONSE = 0x0004,

        ENTER_REQUEST = 0x0005,
        ENTER_REPSONSE = 0x0006,

        LEAVE_REQUEST = 0x0007,
        LEAVE_RESPONSE = 0x0008,

        RETRIEVE_LEVEL_REQUEST = 0x0009,
        RETRIEVE_LEVEL_RESPONSE = 0x000A,

        BIT_HIT_REQUEST = 0x000B,
        BIT_HIT_RESPONSE = 0x000C,

        BLOCK_MINED_RESPONSE = 0x000D,
        BLOCK_MINED_OUT_OF_TIME_RESPONSE = 0x000E,

        ITEM_DATA_REQUEST = 0x0010,
        ITEM_DATA_RESPONSE = 0x0011,

        SET_USERNAME_REQUEST = 0x0012,
        SET_USERNAME_RESPONSE = 0x0013,

        REWARD_RESPONSE = 0x0014,

        INVENTORY_REQUEST = 0x0015,
        INVENTORY_RESPONSE = 0x0016,

        LOOTBOX_OPEN_REQUEST = 0x0017,
        LOOTBOX_OPEN_RESPONSE = 0x0018,

        ITEM_SWAP_REQUEST = 0x0019,
        ITEM_SWAP_RESPONSE = 0x001A,

        //Error handling packet
        ERROR_RESPONSE = 0xFF00
    }

    //main class of any packet
    public class Package
    {
        public ushort Id { get; set; }

        //Default constructor takes argument to set PacketType
        public Package(PackageType PacketType)
        {
            Id = (ushort)PacketType;

            switch (PacketType)
            {
                case PackageType.REG_REQUEST:
                    registrationReq = new RegistrationReq();
                    break;
                case PackageType.LOGIN_REQUEST:
                    loginReq = new LoginReq();
                    break;
                case PackageType.SET_USERNAME_REQUEST:
                    setUsernameReq = new SetUsernameReq();
                    break;
                case PackageType.ENTER_REQUEST:
                    enterReq = new EnterReq();
                    break;
                case PackageType.LEAVE_REQUEST:
                    leaveReq = new LeaveReq();
                    break;
                case PackageType.RETRIEVE_LEVEL_REQUEST:
                    retrieveLevelReq = new RetrieveLevelReq();
                    break;
                case PackageType.BIT_HIT_REQUEST:
                    bitHitReq = new BitHitReq();
                    break;
                case PackageType.INVENTORY_REQUEST:
                    inventoryReq = new InventoryReq();
                    break;
                case PackageType.ITEM_DATA_REQUEST:
                    singleItemDataReq = new SingleItemDataReq();
                    break;
                case PackageType.LOOTBOX_OPEN_REQUEST:
                    lootboxOpenReq = new LootboxOpenReq();
                    break;
                case PackageType.ITEM_SWAP_REQUEST:
                    swapItemReq = new SwapItemReq();
                    break;
                default:
                    //Unknown Packet Type
                    break;
            }
        }

        [JsonProperty(PropertyName = "Reg_Req")]
        public RegistrationReq registrationReq;
        [JsonProperty(PropertyName = "Reg_Res")]
        public RegistrationRes registrationRes;

        [JsonProperty(PropertyName = "Log_Req")]
        public LoginReq loginReq;
        [JsonProperty(PropertyName = "Log_Res")]
        public LoginRes loginRes;

        [JsonProperty(PropertyName = "Set_Username_Req")]
        public SetUsernameReq setUsernameReq;
        [JsonProperty(PropertyName = "Set_Username_Res")]
        public SetUsernameRes setUsernameRes;

        [JsonProperty(PropertyName = "Enter_Req")]
        public EnterReq enterReq;
        [JsonProperty(PropertyName = "Enter_Res")]
        public EnterRes enterRes;

        [JsonProperty(PropertyName = "Leave_Req")]
        public LeaveReq leaveReq;
        [JsonProperty(PropertyName = "Leave_Res")]
        public LeaveRes leaveRes;

        [JsonProperty(PropertyName = "Retrieve_Level_Req")]
        public RetrieveLevelReq retrieveLevelReq;
        [JsonProperty(PropertyName = "Retrieve_Level_Res")]
        public RetrieveLevelRes retrieveLevelRes;

        [JsonProperty(PropertyName = "Bit_Hit_Req")]
        public BitHitReq bitHitReq;
        [JsonProperty(PropertyName = "Bit_Hit_Res")]
        public BitHitRes bitHitRes;

        [JsonProperty(PropertyName = "Block_Mined_Res")]
        public BlockMinedRes blockMinedRes;

        [JsonProperty(PropertyName = "Block_Mined_Failed_Res")]
        public BlockMinedFailedRes blockMinedFailedRes;

        [JsonProperty(PropertyName = "Reward_Res")]
        public RewardRes rewardRes;

        [JsonProperty(PropertyName = "Inventory_Req")]
        public InventoryReq inventoryReq;
        [JsonProperty(PropertyName = "Inventory_Res")]
        public InventoryRes inventoryRes;

        [JsonProperty(PropertyName = "Lootbox_Open_Req")]
        public LootboxOpenReq lootboxOpenReq;
        [JsonProperty(PropertyName = "Lootbox_Open_Res")]
        public LootboxOpenRes lootboxOpenRes;

        [JsonProperty(PropertyName = "Single_Item_Data_Req")]
        public SingleItemDataReq singleItemDataReq;
        [JsonProperty(PropertyName = "Single_Item_Data_Res")]
        public SingleItemDataRes singleItemDataRes;

        [JsonProperty(PropertyName = "Swap_Item_Req")]
        public SwapItemReq swapItemReq;
        [JsonProperty(PropertyName = "Swap_Item_Res")]
        public SwapItemRes swapItemRes;

        [JsonProperty(PropertyName = "Err_Res")]
        public ErrorRes errorRes;

        [JsonIgnore]
        public PackageType Type { get { return (PackageType)Id; } }
    }


    //sub classes, that are included in packet
    public class RegistrationReq
    {
        //TODO fix fields
        public string email;
        public string password;

        public void SetRequest(string e, string p)
        {
            email = e;
            password = p;
        }
    }

    public class RegistrationRes
    {
        public byte response;
    }

    public class LoginReq
    {
        public string email;
        public string password;

        public void SetRequest(string e, string p)
        {
            email = e;
            password = p;
        }
    }

    public class LoginRes
    {
        public byte resposne;
        public string userName;
        public int Id;
    }

    public class EnterReq
    {
        public uint request;

        public void SetRequest(uint r)
        {
            request = r;
        }
    }

    public class EnterRes
    {
        public string compressedRoom;
    }

    public class LeaveReq
    {
    }

    public class LeaveRes
    {
    }

    public class RetrieveLevelReq
    {
        public uint request;

        public void SetRequest(uint levelId)
        {
            request = levelId;
        }
    }

    public class RetrieveLevelRes
    {
        public LevelDisplayData levelData;
    }

    public class BitHitReq
    {
        public uint BlockId;

        public void SetRequest(uint id)
        {
            BlockId = id;
        }
    }

    public class BitHitRes
    {
        public uint BlockId;
        public ulong bitHp;
        public ulong damage;

        public bool isCrit;

        public int clientId;
    }

    public class BlockMinedRes
    {
        public Rankings ranks;

        public uint experience;
        public uint currency;

        public Lootboxes lootBoxes;
    }

    public class BlockMinedFailedRes
    {
        public Rankings ranks;
    }

    public class SetUsernameReq
    {
        public string Name;

        public void SetRequest(string n)
        {
            Name = n;
        }
    }

    public class SetUsernameRes
    {
        public byte response;
    }

    public class RewardRes
    {
        public uint experience;
        public uint currency;

        public Lootboxes lootBoxes;
    }

    public class InventoryReq
    {

    }

    public class InventoryRes
    {
        public ulong experience;
        public uint skillPoints;

        public ulong currency;
        public uint bagSize;

        public Lootboxes lootBoxes;
        public Bag bag;
        public Equipment equipment;
    }

    public class SingleItemDataReq
    {
        public ulong GUID;
    }

    public class SingleItemDataRes
    {
        public Item item;
    }


    public class LootboxOpenReq
    {
        public byte request;
    }

    public class LootboxOpenRes
    {
        public byte openingSuccessfull;

        public ulong? currency;
        public Item[] items;
    }

    public class SwapItemReq
    {
        public byte source1;
        public int index1;

        public byte source2;
        public int index2;

        public void SetRequest(byte s1, int i1, byte s2, int i2)
        {
            source1 = s1;
            index1 = i1;

            source2 = s2;
            index2 = i2;
        }
    }

    public class SwapItemRes
    {
        public byte response;
    }

    public class ErrorRes
    {
        public byte response;
    }
}