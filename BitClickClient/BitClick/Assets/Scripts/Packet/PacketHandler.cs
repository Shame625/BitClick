using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using Assets;
using Newtonsoft.Json;
using UnityEngine;


public class PacketHandler : MonoBehaviour
{
    private GameManager gameManager;
    private NetworkHelper networkHelper;
    
    public ItemDisplayPanel itemDisplayPanel;
    public InventoryManager inventoryManager;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        networkHelper = GetComponent<NetworkHelper>();
    }

    //returning our packet object from array of bytes
    public static Package ParsePacket(ref byte[] data)
    {
        string json = Encoding.ASCII.GetString(data);
        Console.WriteLine(json);
        return JsonConvert.DeserializeObject<Package>(json);
    }

    //Turning our packet object into array of bytes
    public static byte[] SerializePacket(ref Package packet)
    {
        return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(packet, Constants.jsonSettings));     
    }

    //Handle packet that we just deserialized note client only handles Responses
    public void HandlePacket(Package packet)
        {
            UnityThreadHelper.Dispatcher.Dispatch(() =>
            {
                switch (packet.Type)
                {
                    case PackageType.REG_RESPONSE:
                        UIManager.instance.DisplayRegisterMessage((Constants.RegistrationCodes)packet.registrationRes.response);
                        break;

                    case PackageType.LOGIN_RESPONSE:
                        if((Constants.LoginCodes)packet.loginRes.resposne == Constants.LoginCodes.LOGIN_BAD)
                            UIManager.instance.DisplayLoginMessage((Constants.LoginCodes)packet.loginRes.resposne);

                        else if((Constants.LoginCodes)packet.loginRes.resposne == Constants.LoginCodes.LOGIN_SUCCESSFUL)
                        {
                            //Handle successful login
                            GameManager.clientId = packet.loginRes.Id;
                            GameManager.userName = packet.loginRes.userName;
                            GetComponent<NetworkHelper>().RetrieveItems();

                            UIManager.instance.LoggedIn();
                        }
                        //case if there is no username
                        else if((Constants.LoginCodes)packet.loginRes.resposne == Constants.LoginCodes.LOGIN_SUCCESSFUL_MISSING_USERNAME)
                        {
                            GameManager.clientId = packet.loginRes.Id;
                            GetComponent<NetworkHelper>().RetrieveItems();

                            UIManager.instance.LoggedIn();
                            UIManager.instance.DisplayChooseUsernamePanel(true);
                        }
                        break;

                    case PackageType.SET_USERNAME_RESPONSE:
                        if(Constants.UsernameCodes.USERNAME_SUCCESSFUL == (Constants.UsernameCodes)packet.setUsernameRes.response)
                        {
                            GameManager.userName = UIManager.instance.GetUsername();
                            UIManager.instance.DisplayChooseUsernamePanel(false);
                        }
                        else
                        {
                            UIManager.instance.DisplayChooseUsernameError((Constants.UsernameCodes)packet.setUsernameRes.response);
                        }

                        break;
                    
                    case PackageType.RETRIEVE_LEVEL_RESPONSE:
                        GameManager.NumberOfLevels = packet.retrieveLevelRes.levelData.numberOfLevels;

                        GameManager.activeTill = packet.retrieveLevelRes.levelData.activeTill;
                        UIManager.instance.SetInfoLevelSelector(packet.retrieveLevelRes.levelData);
                        break;

                    case PackageType.ENTER_REPSONSE:
                        //spawn level
                        BlockRoom br = JsonConvert.DeserializeObject<BlockRoom>(StringCompressor.DecompressString(packet.enterRes.compressedRoom));
                        br.activeTill = GameManager.activeTill;

                        gameManager.EnterLevel(ref br);

                        //set ui
                        UIManager.instance.InLevel(ref br.activeTill);
                        break;

                    case PackageType.LEAVE_RESPONSE:
                        networkHelper.OpenLevelSelector();

                        gameManager.LeaveLevel();
                        break;

                        //handle hit response, it carries ID + new HP
                    case PackageType.BIT_HIT_RESPONSE:
                        gameManager.levelGenerator.HandleBitHpChange(ref packet.bitHitRes.BlockId, ref packet.bitHitRes.bitHp, ref packet.bitHitRes.damage, ref packet.bitHitRes.isCrit, ref packet.bitHitRes.clientId);
                        break;

                        //Block mined succesffully
                    case PackageType.BLOCK_MINED_RESPONSE:
                        //pass list with ranks
                        UIManager.instance.LevelFinished(true, packet.blockMinedRes.ranks.ranks);


                        //unloads level from background
                        gameManager.LeaveLevel();
                        break;

                        //failed to mine block in time
                    case PackageType.BLOCK_MINED_OUT_OF_TIME_RESPONSE:
                        //pass list with ranks
                        
                        UIManager.instance.LevelFinished(false, packet.blockMinedFailedRes.ranks.ranks);

                        gameManager.LeaveLevel();
                        break;

                    //packet on reward
                    case PackageType.REWARD_RESPONSE:
                        uint lootBoxes = 0;

                        if(packet.rewardRes.lootBoxes != null)
                        {
                            lootBoxes = (uint)(packet.rewardRes.lootBoxes.Small + packet.rewardRes.lootBoxes.Medium + packet.rewardRes.lootBoxes.Big);
                            gameManager.player.inventory.lootBoxes.Addition(ref packet.rewardRes.lootBoxes);
                        }

                        gameManager.player.currency += packet.rewardRes.currency;
                        gameManager.player.experience += packet.rewardRes.experience;
                        
                        UIManager.instance.ReceivedLoot(ref packet.rewardRes.currency, ref packet.rewardRes.experience, lootBoxes);
                        break;

                    case PackageType.INVENTORY_RESPONSE:
                        gameManager.player.inventory.bagSize = packet.inventoryRes.bagSize;
                        gameManager.player.inventory.bag = packet.inventoryRes.bag;

                        gameManager.player.currency = packet.inventoryRes.currency;

                        gameManager.player.skillPoints = packet.inventoryRes.skillPoints;
                        gameManager.player.experience = packet.inventoryRes.experience;

                        gameManager.player.inventory.equipment = packet.inventoryRes.equipment;
                        gameManager.player.inventory.lootBoxes = packet.inventoryRes.lootBoxes;

                        gameManager.player.CalculateStats();

                        UIManager.instance.HandleInventoryUpdate();
                        //handles bag
                        UIManager.instance.HandleBag();
                        break;

                    case PackageType.LOOTBOX_OPEN_RESPONSE:
                        UIManager.instance.LootboxCodeHandler((Constants.LootboxCodes)packet.lootboxOpenRes.openingSuccessfull);

                        if ((Constants.LootboxCodes)packet.lootboxOpenRes.openingSuccessfull == Constants.LootboxCodes.SUCCESSFULL)
                        {
                            gameManager.player.inventory.lootBoxes.Deduce(networkHelper.lootboxManager.currentLootBox, 1);
                            UIManager.instance.HandleInventoryUpdate();

                            //cache recieved items, so we dont have to query for them
                            CacheManager.TrySave(packet.lootboxOpenRes.items);

                            //Add to inventory
                            foreach (Item i in packet.lootboxOpenRes.items)
                            {
                                gameManager.player.inventory.bag.AddToFirstAvailableSlot(i.GUID);
                            }

                            //send items to lootbox thingy
                            gameManager.DisplayRewardsLootbox(packet.lootboxOpenRes.items);
                            
                            networkHelper.lootboxManager.UpdateMaterial(UIManager.instance.UpdateLootBox(networkHelper.lootboxManager.currentLootBox));
                        }
                        else
                        {
                            //nothing
                        }
                        break;
                    case PackageType.ITEM_DATA_RESPONSE:
                        CacheManager.TrySave(new Item[] { packet.singleItemDataRes.item });

                        itemDisplayPanel.DisplayItemData(packet.singleItemDataRes.item.GUID);
                        UIManager.instance.UpdateSlots();
                        break;

                    case PackageType.ITEM_SWAP_RESPONSE:
                        if ((Constants.ContainerCodes)packet.swapItemRes.response == Constants.ContainerCodes.SUCCESS)
                        {
                            //dump queue
                            inventoryManager.ProcessWithSwap();
                        }
                        else
                        {
                            inventoryManager.SwapFailed();
                        }
                        break;

                    case PackageType.ERROR_RESPONSE:
                        UIManager.instance.DisplayError((Constants.ErrorCodes) packet.errorRes.response);
                        if ((Constants.ErrorCodes) packet.errorRes.response ==
                            Constants.ErrorCodes.LOGGED_IN_FROM_ANOTHER_LOCATION)
                        {
                            GameManager.clientId = null;
                            UIManager.instance.LoggedOut();
                        }
                        //if requested level does not exist, set requesting level back to 1
                        if ((Constants.ErrorCodes)packet.errorRes.response == Constants.ErrorCodes.LEVEL_DOES_NOT_EXIST)
                            GameManager.CurrentLevel = 1;
                        break;

                    default:
                        break;
                }
            });
        }
}
