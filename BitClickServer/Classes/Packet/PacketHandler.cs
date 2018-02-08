using System;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Newtonsoft.Json;
using static BitClickServer.Constants;
using System.Linq;

namespace BitClickServer
{
    public static class PacketHandler
    {

        //returning our packet object from array of bytes
        public static Package ParsePacket(ref byte[] data)
        {
            try
            {
                string json = Encoding.ASCII.GetString(data);
                return JsonConvert.DeserializeObject<Package>(json);
            }
            catch
            {
                return null;
            }
           
        }

        //Turning our packet object into array of bytes
        public static byte[] SerializePacket(ref Package packet)
        {
            try
            {
                return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(packet, Constants.jsonSettings));
            }
            catch
            {
                return null;
            }
        }

        //Handle packet that we just deserialized, also remember to pass reference to socket
        public static void HandlePacket(ref Client client, ref Package packet)
        {
            //Check if client is logged in before doing any work with data
            if (!client.LoggedIn())
            {
                //If packets are of type Login/registration let them trough if its not, notify client with an error code
                if (!(packet.Type == PackageType.LOGIN_REQUEST || packet.Type == PackageType.REG_REQUEST))
                {
                    Package newPacket = new Package(PackageType.ERROR_RESPONSE);
                    newPacket.errorRes.SetResponse(Constants.ErrorCodes.NOT_LOGGED_IN);
                    Program.SendPacket(ref client, ref newPacket);

                    return;
                }
            }

            //Else continue with work
            //Fill this packet with data that will go off to client
            Package packetToSend = null;

            switch (packet.Type)
            {
                // Registration request
                case PackageType.REG_REQUEST:
                    packetToSend = new Package(PackageType.REG_RESPONSE);
                    var registerResult = RegisterManager.Register(ref packet.registrationReq.email,
                        ref packet.registrationReq.password);
                    packetToSend.registrationRes.SetResponse(registerResult);
                    
                    break;

                // Login Request
                case PackageType.LOGIN_REQUEST:

                    //Check if client is already logged in on the same socket, look up to dictionary if it exists
                    Client oldClient = Program.GetClientByid(client.GetId());

                    if (oldClient != null)
                    {
                        //check if socket is same as the socket
                        if (oldClient._socket == client._socket)
                        {
                            packetToSend = new Package(PackageType.ERROR_RESPONSE);
                            packetToSend.errorRes.SetResponse(Constants.ErrorCodes.ALREADY_LOGGED_IN);
                            Program.SendPacket(ref client, ref packetToSend);

                            return;
                        }
                    }

                    packetToSend = new Package(PackageType.LOGIN_RESPONSE);

                    (Constants.LoginCodes code, int id, int playerId, string username, Player player) loginResult =
                        LoginManager.CheckCredentials(ref packet.loginReq.email, ref packet.loginReq.password);
                    
                    //If loginResult is LoginCodes.SUCCESSFUL set client loggedIn state to true
                    if (loginResult.code == LoginCodes.LOGIN_SUCCESSFUL || loginResult.code == LoginCodes.LOGIN_SUCCESSFUL_MISSING_USERNAME)
                    {
                        //if this returns data, it means that client is already logged in and should be disconnected
                        oldClient = Program.GetClientByid(loginResult.id);

                        //in case its not null, it means we have to disconnect the client on another socket.
                        if (oldClient != null)
                        {
                            Package newPacket = new Package(PackageType.ERROR_RESPONSE);
                            newPacket.errorRes.SetResponse(Constants.ErrorCodes.LOGGED_IN_FROM_ANOTHER_LOCATION);

                            oldClient.Disconnect();
                            Program.SendPacket(ref oldClient, ref newPacket);
                        }

                        client.LogIn(ref loginResult.id, ref loginResult.playerId, ref loginResult.player);

                        if (loginResult.username != "")
                        {
                            client.SetUserName(ref loginResult.username);
                        }

                        packetToSend.loginRes.SetResponse(loginResult.code, loginResult.username, loginResult.id);

                        Console.WriteLine(client.GetId() + " username: + " + client.GetName() + " has succesfully logged in.");
                    }
                    break;

                case PackageType.SET_USERNAME_REQUEST:
                    packetToSend = new Package(PackageType.SET_USERNAME_RESPONSE);

                    //check username first
                    if(packet.setUsernameReq.Name.Any(ch => !Char.IsLetterOrDigit(ch)) || packet.setUsernameReq.Name.Length < Constants.USERNAME_MIN_LEN || packet.setUsernameReq.Name.Length > Constants.USERNAME_MAX_LEN)
                    {
                        packetToSend.setUsernameRes.SetResponse(UsernameCodes.USERNAME_BAD);
                        break;
                    }
                    // Try to set username
                    try
                    {
                        using (var db = new GameContext())
                        {
                            int pId = client.GetPlayerId();
                            var player = db.Players.SingleOrDefault(o => o.Id == pId);

                            Console.WriteLine("HERE");

                            player.Name = packet.setUsernameReq.Name;
                            client.SetUserName (ref packet.setUsernameReq.Name);
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed to set username");
                        Console.WriteLine(e.Message);

                        // Something went wrong, tell client username could not be changed
                        packetToSend.setUsernameRes.response = (byte)UsernameCodes.USERNAME_BAD;
                        break;
                    }

                    // Send packet telling client username was successfully set
                    packetToSend.setUsernameRes.response = (byte)UsernameCodes.USERNAME_SUCCESSFUL;
                    break;

                // Enter block room request
                case PackageType.ENTER_REQUEST:
                    //Remove player if he is in another room
                    if (client.CurrentRoom != null)
                    {
                        client.CurrentRoom.RemoveClient(client);
                        client.CurrentRoom = null;
                    }

                    if (!Program._blockRooms.ContainsKey(packet.enterReq.request))
                    {
                        packetToSend = new Package(PackageType.ERROR_RESPONSE);
                        packetToSend.errorRes.SetResponse(ErrorCodes.LEVEL_DOES_NOT_EXIST);
                    }

                    else
                    {
                        // Add player to room, but first check if level is active, if it ain't, dont do it
                        var room = Program.AddToBlockRoom(ref client, ref packet.enterReq.request);

                        if (room != null)
                        {
                            client.CurrentRoom = room;

                            packetToSend = new Package(PackageType.ENTER_REPSONSE);
                            packetToSend.enterRes.SetResponse(ref room);
                        }
                        else
                        {
                            packetToSend = new Package(PackageType.ERROR_RESPONSE);
                            packetToSend.errorRes.SetResponse(ErrorCodes.LEVEL_FINISHED);
                        }
                    }
                    break;

                case PackageType.LEAVE_REQUEST:
                    if (client.CurrentRoom != null)
                    {
                        client.CurrentRoom.RemoveClient(client);
                        client.CurrentRoom = null;
                        packetToSend = new Package(PackageType.LEAVE_RESPONSE);
                    }

                    else
                    {
                        packetToSend = new Package(PackageType.ERROR_RESPONSE);
                        packetToSend.errorRes.SetResponse(ErrorCodes.NOT_IN_ROOM);
                    }
                    break;

                //Send levels to client
                case PackageType.RETRIEVE_LEVEL_REQUEST:
                    if (Program._blockRooms.ContainsKey(packet.retrieveLevelReq.request))
                    {
                        packetToSend = new Package(PackageType.RETRIEVE_LEVEL_RESPONSE);
                        packetToSend.retrieveLevelRes.SetData(Program._blockRooms[packet.retrieveLevelReq.request]);
                    }
                    else
                    {
                        packetToSend = new Package(PackageType.ERROR_RESPONSE);
                        packetToSend.errorRes.SetResponse(ErrorCodes.LEVEL_DOES_NOT_EXIST);
                    }
                    break;
                
                    //Hitting block
                case PackageType.BIT_HIT_REQUEST:
                    //check if user is in room that is doing this, if not, send error
                    if (client.CurrentRoom != null)
                    {
                        //try, because index can be out of range, should probably ban those users that do it, since its packet edit..., return here, since the BlockRoom will handle packet sends
                        try
                        {
                            //only forward this if bit is alive
                            if (client.CurrentRoom.Block._block[packet.bitHitReq.BlockId].isAlive)
                            {
                                client.CurrentRoom.BitTakeDamage(ref client, ref packet.bitHitReq.BlockId);
                                return;
                            }
                            //case if client got a ghost bit and didn't snyc in time
                            else
                            {
                                packetToSend = new Package(PackageType.BIT_HIT_RESPONSE);
                                packetToSend.bitHitRes.bitHp = 0;
                                packetToSend.bitHitRes.damage = 0;
                                packetToSend.bitHitRes.clientId = client.GetId();
                                packetToSend.bitHitRes.BlockId = packet.bitHitReq.BlockId;
                            }
                        }
                        catch
                        {
                            //supressed msg
                            return;
                        }
                        
                    }
                    else
                    {
                        packetToSend = new Package(PackageType.ERROR_RESPONSE);
                        packetToSend.errorRes.SetResponse(ErrorCodes.NOT_IN_ROOM);
                    }
                    break;

                //retrieve inventory
                case PackageType.INVENTORY_REQUEST:
                    packetToSend = new Package(PackageType.INVENTORY_RESPONSE);
                    packetToSend.inventoryRes.bag = client.Player.Inventory.bag;
                    packetToSend.inventoryRes.bagSize = client.Player.Inventory.BagSize;

                    packetToSend.inventoryRes.currency = client.Player.Inventory.Currency;      
                    
                    packetToSend.inventoryRes.skillPoints = client.Player.SkillPoints;
                    packetToSend.inventoryRes.experience = client.Player.Experience;

                    packetToSend.inventoryRes.equipment = client.Player.Inventory.equipment;
                    packetToSend.inventoryRes.lootBoxes = client.Player.Inventory.lootBoxes;
                    break;

                //opening lootbox
                case PackageType.LOOTBOX_OPEN_REQUEST:
                    packetToSend = new Package(PackageType.LOOTBOX_OPEN_RESPONSE);
                    packetToSend.lootboxOpenRes.openingSuccessfull = (byte)Constants.LootboxCodes.UNSUCCESSFULL_UNKNOWN_BOX;

                    //Check if valid type of lootbox is being opened
                    if (packet.lootboxOpenReq.request < 1 || packet.lootboxOpenReq.request > 3)
                    {
                        break;
                    }
                    //check if user has enough of it
                    else
                    {
                        LootboxType type = (LootboxType)packet.lootboxOpenReq.request;

                        //generates code if its successfull, also generates items
                        (Constants.LootboxCodes code, ulong currency,Item[] itemArr) result = DatabaseManager.LootboxOpened(client, ref type);
                        packetToSend.lootboxOpenRes.openingSuccessfull = (byte)result.code;

                        //all logic happens inside DatabaseManager.LootboxOpened for the sake of easy database access and instant saving in case its a valid operation
                        if (result.code == LootboxCodes.SUCCESSFULL)
                        {
                            packetToSend.lootboxOpenRes.openingSuccessfull = (byte)Constants.LootboxCodes.SUCCESSFULL;

                            packetToSend.lootboxOpenRes.items = result.itemArr;
                            packetToSend.lootboxOpenRes.currency = result.currency;
                        }
                    }

                    break;

                case PackageType.ITEM_DATA_REQUEST:
                    Item reqItem = DatabaseManager.GetSingleItem(packet.singleItemDataReq.GUID);

                    if (reqItem != null)
                    {
                        packetToSend = new Package(PackageType.ITEM_DATA_RESPONSE);
                        packetToSend.singleItemDataRes.item = reqItem;
                    }
                    else
                    {
                        packetToSend = new Package(PackageType.ERROR_RESPONSE);
                        packetToSend.errorRes.SetResponse(ErrorCodes.ITEM_DOES_NOT_EXIST);
                    }
                    break;
                case PackageType.ITEM_SWAP_REQUEST:
                    packetToSend = new Package(PackageType.ITEM_SWAP_RESPONSE);
                    (Constants.ContainerCodes response, bool recalculate) = DatabaseManager.SwapItems(client, ref packet.swapItemReq.source1, ref packet.swapItemReq.index1,
                        ref packet.swapItemReq.source2, ref packet.swapItemReq.index2);
                    packetToSend.swapItemRes.response = (byte)response;

                    if(recalculate)
                        client.Player.CalculateStats();
                    break;
                default:
                    return;
            }

            Program.SendPacket(ref client, ref packetToSend);
        }
    }
}
