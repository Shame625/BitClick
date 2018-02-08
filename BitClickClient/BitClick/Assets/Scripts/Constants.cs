using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
    {
        #region Options
        public static JsonSerializerSettings jsonSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        #endregion

        #region Registration/Login Constants

        //responses
        public enum RegistrationCodes : byte
        {
            REGISTRATION_EMAIL_BAD = 0x00,
            REGISTRATION_PASSWORD_BAD = 0x01,
            REGISTRATION_SUCCESSFUL = 0x02
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

    public static string GetRegistrationMessage(RegistrationCodes regCode)
        {
            string temp = "";

            switch (regCode)
            {
                case RegistrationCodes.REGISTRATION_EMAIL_BAD:
                    temp += "Wrong email format, or its already in use!";
                    break;
                case RegistrationCodes.REGISTRATION_PASSWORD_BAD:
                    temp += "Password must be between 6 and 32 characters long!";
                    break;
                case RegistrationCodes.REGISTRATION_SUCCESSFUL:
                    temp += "Account created!";
                break;
            }
            return temp;
        }

        public static string GetLoginMessage(LoginCodes loginCode)
        {
            string temp = "";

            switch (loginCode)
            {
                case LoginCodes.LOGIN_BAD:
                    temp += "Wrong email or password!";
                    break;
            }
            return temp;
        }

        public static string GetUsernameMessage(UsernameCodes usernameCode)
        {
        string temp = "";

            switch (usernameCode)
            {
                case UsernameCodes.USERNAME_BAD:
                    temp += "User name must be between 3-16 characters long and cannot contain special characters.";
                    break;
            }
            return temp;
    }

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

    //strings
    public static string GetErrorStringMsg(ErrorCodes errCode)
        {
        string temp = "Error: " + errCode + " ";

        switch (errCode)
        {
            case ErrorCodes.NOT_LOGGED_IN:
                temp += "You have to be logged in.";
                break;
            case ErrorCodes.ALREADY_LOGGED_IN:
                temp += "You are already logged in.";
                break;
            case ErrorCodes.LOGGED_IN_FROM_ANOTHER_LOCATION:
                temp += "You logged in from another location.";
                break;
            case ErrorCodes.LEVEL_DOES_NOT_EXIST:
                temp += "Requested level does not exist.";
                break;
            case ErrorCodes.NOT_IN_ROOM:
                temp += "You are not in a room.";
                break;
            case ErrorCodes.BIT_DOES_NOT_EXIST:
                temp += "Bit does not exist.";
                break;
            case ErrorCodes.LEVEL_FINISHED:
                temp += "Level is finished.";
                break;
            case ErrorCodes.ITEM_DOES_NOT_EXIST:
                temp += "Item does not exist.";
                break;
        }
        
        return temp;
        }
        #endregion

        #region Server Specific Constants
        public const uint BUFFER_SIZE = 10000;

        #endregion

        #region  LevelConstants
        public const uint BLOCK_BASE_SIZE = 4;
        public static uint NUMBER_OF_THEMES;

        public static uint GET_BLOCK_SIZE(uint level)
        {
            return BLOCK_BASE_SIZE + (uint)(Math.Ceiling(level / 50f));
        }

        public static uint GET_BIT_HP(uint level)
        {
            return (uint)Math.Pow(BLOCK_BASE_SIZE + level, 3);
        }
        
        public static uint GET_LEVEL_THEME(uint level)
        {
        return (level / 10 ) % NUMBER_OF_THEMES;
        }

    #endregion

        #region Game Constants

        public static string CurrencyName = "Scrap";
        public static string SlotDefaultColor = "#FFFFFF41";

        public enum Container : byte
        {
            Bag,
            Equipment
        };

        public enum ContainerCodes : byte
        {
            UNKNOWN_CONTAINER,
            UNKNOWN_INDEX,
            WRONG_TYPE,
            UNKNOWN_ERROR,
            CANNOT_MOVE_EMPTY,
            SUCCESS
        };

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

        public enum Qualities : byte
        {
            NORMAL,
            SUPERIOR,
            RARE,
            EPIC,
            LEGENDARY
        };

        public enum Stats : byte
        {
            DAMAGE,
            STAMINA,
            CRITICAL_STRIKE_CHANCE,
            CRITICAL_HIT_DAMAGE,
            STAMINA_RECHARGE_RATE,
            CLICK_MULTIPLIER,
            STAMINA_ON_HIT_CHANCE
        };

        public enum Skills : byte
        {
            STAMINA_PERCENT,
            DAMAGE_PERCENT,
            CLICK_MULTIPLIER,
            STAMINA_RECHARGE_PERCENT,
            CRITICAL_HIT_DAMAGE_PERCENT
        };

        public static Dictionary<Skills, uint> SkillCaps = new Dictionary<Skills, uint>()
        {
            { Skills.STAMINA_PERCENT, 25 },
            { Skills.DAMAGE_PERCENT, 25 },
            { Skills.CLICK_MULTIPLIER, 10 },
            { Skills.STAMINA_RECHARGE_PERCENT, 10 },
            { Skills.CRITICAL_HIT_DAMAGE_PERCENT, 30 }
        };

        public static string GetQualityColor(Qualities quality)
        {
            string temp = "";

            if(quality == Qualities.NORMAL)
            {
                temp = "#FFFFFFFF";
            }
            else if (quality == Qualities.SUPERIOR)
            {
                temp = "#008000FF";
            }
            else if (quality == Qualities.RARE)
            {
                temp = "#508AA8FF";
            }
            else if (quality == Qualities.EPIC)
            {
                temp = "#800080FF";
            }
            else if (quality == Qualities.LEGENDARY)
            {
                temp = "#FFA500FF";
            }

            return temp;
        }

        public static Sprite GetItemIcon(Slots slot, ushort iconID)
        {
        Sprite iconSprite = null;

            try
            {
                switch (slot)
                {
                    case Slots.HEAD:
                        iconSprite = AssetManager.HeadSprites[iconID];
                        break;
                    case Slots.MAIN_HAND:
                        iconSprite = AssetManager.MainHandSprites[iconID];
                        break;
                    case Slots.NECK:
                        iconSprite = AssetManager.NeckSprites[iconID];
                        break;
                    case Slots.RING:
                        iconSprite = AssetManager.RingSprites[iconID];
                        break;
                    case Slots.TRINKET:
                        iconSprite = AssetManager.TrinketSprites[iconID];
                        break;
                    case Slots.HAND:
                        iconSprite = AssetManager.HandSprites[iconID];
                    break;
                    case Slots.WAIST:
                        iconSprite = AssetManager.WaistSprites[iconID];
                    break;
                    case Slots.CHEST:
                        iconSprite = AssetManager.ChestSprites[iconID];
                        break;
                    case Slots.LEG:
                        iconSprite = AssetManager.LegSprites[iconID];
                        break;
                    case Slots.BOOT:
                        iconSprite = AssetManager.BootSprites[iconID];
                        break;
            }
        }
        catch 
        {
        }

        return iconSprite;
        }

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
            if (t == LootboxType.Small)
                return 1;
            else if (t == LootboxType.Medium)
                return 2;
            else if (t == LootboxType.Big)
                return 3;
            return 0;
        }

    #endregion

        #region PlayerPrefs
    public static Dictionary<string, string> Options = new Dictionary<string, string>()
        {
            {"musicVolume", "musicVolume" },
            {"sfxVolume", "sfxVolume" },
            {"networkSfxVolume", "networkSfxVolume" }
        };

         #endregion
}