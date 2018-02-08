using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace BitClickServer
{
    public static class Constants
    {
        #region Options
        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings(){NullValueHandling = NullValueHandling.Ignore};
        #endregion

        #region Registration/Login Constants

        //responses
        public enum RegistrationCodes : byte
        {
            REGISTRATION_EMAIL_BAD = 0x00,
            REGISTRATION_PASSWORD_BAD = 0x01,
            REGISTRATION_SUCCESSFUL = 0x02,
        }

        public enum LoginCodes : byte
        {
            LOGIN_BAD = 0x00,
            LOGIN_SUCCESSFUL = 0x01,
            LOGIN_SUCCESSFUL_MISSING_USERNAME = 0x02
        }

        public enum UsernameCodes : byte
        {
            USERNAME_BAD = 0x00,
            USERNAME_SUCCESSFUL = 0x01,
        }

        public const uint PASSWORD_MIN_LEN = 6;
        public const uint PASSWORD_MAX_LEN = 32;

        public const uint USERNAME_MIN_LEN = 3;
        public const uint USERNAME_MAX_LEN = 16;

        #endregion

        #region Error Consants

        public enum ErrorCodes : byte
        {
            NOT_LOGGED_IN,
            ALREADY_LOGGED_IN,
            LOGGED_IN_FROM_ANOTHER_LOCATION,
            LEVEL_DOES_NOT_EXIST,
            NOT_IN_ROOM,
            BIT_DOES_NOT_EXIST,
            LEVEL_FINISHED,
            ITEM_DOES_NOT_EXIST
        }
        #endregion

        #region Server Specific Constants
        public const uint BUFFER_SIZE = 32768;
        public const uint LEVEL_CHECK_TIME_INTERVAL_MS = 1000;
        #endregion

        #region  LevelConstants
        public const uint BLOCK_BASE_SIZE = 4;
        private const int BASE_RESPAWN_SECONDS = 10;

        public static uint GET_BLOCK_SIZE(uint level)
        {
            return BLOCK_BASE_SIZE + (uint)(Math.Ceiling(level / 50f));
        }

        public static uint GET_BIT_HP(uint level)
        {
            return (uint)Math.Pow(BLOCK_BASE_SIZE + level, 3);
        }

        public static int GET_RESPAWN_SECONDS(uint level)
        {
            return (int)level * BASE_RESPAWN_SECONDS;
        }

        public static int BLOCK_ALIVE_SECONDS(uint level)
        {
            return (int)(level * 10) * 2;
        }
        #endregion

        #region Game Constants
        public const byte CHANCE_BIT_SMALL_LOOTBOX = 6;
        public const byte CHANCE_BIT_MEDIUM_LOOTBOX = 1;

        public enum Container : byte
        {
            Bag,
            Equipment
        }

        public enum ContainerCodes : byte
        {
            UNKNOWN_CONTAINER,
            UNKNOWN_INDEX,
            WRONG_TYPE,
            UNKNOWN_ERROR,
            CANNOT_MOVE_EMPTY,
            SUCCESS
        }

        public enum Qualities : byte
        {
            NORMAL,
            SUPERIOR,
            RARE,
            EPIC,
            LEGENDARY
        }

        public static Dictionary<int, Slots> equipmentSlots = new Dictionary<int, Slots>()
        {
            {0, Slots.MAIN_HAND},
            {1, Slots.HEAD},
            {2, Slots.HAND},
            {3, Slots.WAIST},
            {4, Slots.RING},
            {5, Slots.RING},
            {6, Slots.NECK},
            {7, Slots.TRINKET},
            {8, Slots.TRINKET},
            {9, Slots.CHEST},
            {10, Slots.LEG},
            {11, Slots.BOOT}
        };

        public enum Slots : byte
        {       
            MAIN_HAND,
            HEAD,
            HAND,
            WAIST,
            RING,
            NECK,
            TRINKET,
            CHEST,
            LEG,
            BOOT
        };

        public enum Stats : byte
        {
            DAMAGE,

            STAMINA,
            STAMINA_RECHARGE_RATE,
            STAMINA_ON_HIT_CHANCE,

            CRITICAL_STRIKE_CHANCE,
            CRITICAL_HIT_DAMAGE,
            
            CLICK_MULTIPLIER  
        }

        public enum Skills : byte
        {
            STAMINA_PERCENT,
            DAMAGE_PERCENT,
            CLICK_MULTIPLIER,
            STAMINA_RECHARGE_PERCENT,
            CRITICAL_HIT_DAMAGE_PERCENT
        }

        public static Dictionary<Skills, uint> SkillCaps = new Dictionary<Skills, uint>()
        {
            { Skills.STAMINA_PERCENT, 25 },
            { Skills.DAMAGE_PERCENT, 25 },
            { Skills.CLICK_MULTIPLIER, 10 },
            { Skills.STAMINA_RECHARGE_PERCENT, 10 },
            { Skills.CRITICAL_HIT_DAMAGE_PERCENT, 30 }
        };

        #endregion

        #region Lootbox Constants
        public enum LootboxCodes
        {
            UNSUCCESSFULL_OUT_OF_BOXES,
            UNSUCCESSFULL_UNKNOWN_BOX,
            UNSUCCESSFULL_NOT_ENOUGH_SPACE,
            SUCCESSFULL,
        }

        public enum LootboxType
        {
            Small = 1,
            Medium = 2,
            Big = 3
        }

        public static uint LootboxMaxItem(LootboxType t)
        {
            if(t == LootboxType.Small)
                return 1;
            else if (t == LootboxType.Medium)
                 return 2;
            else if (t == LootboxType.Big)
                return 3;
            return 0;
        }

        public static Item[] GenerateLootboxItems(uint level, Constants.LootboxType type)
        {
            Item[] items = null;
            
            if (type == Constants.LootboxType.Small)
            {
                items = new Item[1];
            }
            else if (type == Constants.LootboxType.Medium)
            {
                items = new Item[2];
            }
            else if (type == Constants.LootboxType.Big)
            {
                items = new Item[3];
            }

            for (int i = 0; i < items.Length; i++)
            {
                items[i] = Item.GenerateRandomItem((int)level, 5);
            }

            return items;
        }
        #endregion

        #region AccountConstants
        public const uint BAG_START_SIZE = 30;
        public const uint LEVEL_START = 1;
        #endregion
    }
}
