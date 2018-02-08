using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace BitClickServer
{
    public static class DatabaseManager
    {
        //Opening lootbox check and logic inside
        public static (Constants.LootboxCodes code, ulong currency, Item[] itemArr) LootboxOpened(Client client,
            ref Constants.LootboxType type)
        {
            using (var db = new GameContext())
            {
                //Find account
                var account = db.Accounts.SingleOrDefault(o => o.Id == client.GetId());
                Item[] itemArr = null;
                ulong currency = 0;

                if (account != null)
                {
                    //Find player
                    var player = db.Players.SingleOrDefault(o => o.Id == account.PlayerId);

                    if (player != null)
                    {
                        //Find inventory
                        var inventory = db.Inventory.SingleOrDefault(o => o.Id == player.InventoryId);

                        if (inventory != null)
                        {
                            inventory.DeserializeData();

                            if (inventory.lootBoxes.GetCount(type) > 0)
                            {
                                //check if there is inventory space
                                if (inventory.bag.GetFreeSlots() >= Constants.LootboxMaxItem(type))
                                    inventory.lootBoxes.Deduce(type, 1);
                                else
                                    return (Constants.LootboxCodes.UNSUCCESSFULL_NOT_ENOUGH_SPACE, 0, null);
                            }
                            else
                            {
                                return (Constants.LootboxCodes.UNSUCCESSFULL_OUT_OF_BOXES, 0, null);
                            }
                            inventory.SerializeLootboxes();

                            try
                            {
                                //Generate items here
                                itemArr = Constants.GenerateLootboxItems(client.Player.Level + 10, type);
                                currency = client.Player.Level;

                                //add items to database
                                foreach (Item i in itemArr)
                                {
                                    db.Items.Add(i);
                                }

                                db.SaveChanges();

                                //loop trough items and add them to player inventory, get free slots in inventory
                                uint[] freeSlots = inventory.bag.GetFreeIndexes(Constants.LootboxMaxItem(type));

                                byte counter = 0;
                                foreach (Item i in itemArr)
                                {
                                    //database array / abstract
                                    inventory.bag.items[freeSlots[counter]] = i.GUID;

                                    //client in memory
                                    client.Player.Inventory.bag.items[freeSlots[counter]] = i.GUID;

                                    counter++;
                                }

                                inventory.SerializeBag();
                                inventory.Currency += currency;
                                db.SaveChanges();

                                //reflect changes to client data in memory
                                client.Player.Inventory.lootBoxes.Deduce(type, 1);
                            }
                            catch
                            {
                                return (Constants.LootboxCodes.UNSUCCESSFULL_UNKNOWN_BOX, 0, null);
                            }

                            return (Constants.LootboxCodes.SUCCESSFULL, currency, itemArr);
                        }
                    }
                }
            }
            return (Constants.LootboxCodes.UNSUCCESSFULL_UNKNOWN_BOX, 0, null);
        }

        //querying for item data
        public static Item GetSingleItem(ulong reqGUID)
        {
            using (var db = new GameContext())
            {
                var item = db.Items.SingleOrDefault(o => o.GUID == reqGUID);

                if (item != null)
                {
                    Item requestedItem = new Item(item.Data);
                    requestedItem.GUID = reqGUID;
                    requestedItem.itemData = requestedItem.GetData();
                    return requestedItem;
                }
            }
            return null;
        }

        //Swaping items
        public static (Constants.ContainerCodes, bool) SwapItems(Client client, ref byte s1, ref int i1, ref byte s2,
            ref int i2)
        {
            //check if its equal to either 0 or 1
            if (s1 >= 2 || s2 >= 2)
            {
                return (Constants.ContainerCodes.UNKNOWN_CONTAINER, false);
            }

            //check indexes
            //equipment will have indexes between 0 and 8
            if ((Constants.Container) s1 == Constants.Container.Bag)
            {
                if (i1 < 0 || i1 > (client.Player.Inventory.BagSize - 1))
                {
                    return (Constants.ContainerCodes.UNKNOWN_INDEX, false);
                }
            }
            else if ((Constants.Container) s1 == Constants.Container.Equipment)
            {
                //hardcoded atm
                if (i1 < 0 || i1 > Constants.equipmentSlots.Count - 1)
                {
                    return (Constants.ContainerCodes.UNKNOWN_INDEX, false);
                }
            }

            if ((Constants.Container) s2 == Constants.Container.Bag)
            {
                if (i2 < 0 || i2 > (client.Player.Inventory.BagSize - 1))
                {
                    return (Constants.ContainerCodes.UNKNOWN_INDEX, false);
                }
            }
            else if ((Constants.Container) s2 == Constants.Container.Equipment)
            {
                //hardcoded atm
                if (i2 < 0 || i2 > Constants.equipmentSlots.Count - 1)
                {
                    return (Constants.ContainerCodes.UNKNOWN_INDEX, false);
                }
            }

            using (var db = new GameContext())
            {
                var account = db.Accounts.SingleOrDefault(o => o.Id == client.GetId());

                if (account != null)
                {
                    bool recalculateStats = false;
                    //Find player
                    var player = db.Players.SingleOrDefault(o => o.Id == account.PlayerId);

                    if (player != null)
                    {
                        //Find inventory
                        var inventory = db.Inventory.SingleOrDefault(o => o.Id == player.InventoryId);

                        if (inventory != null)
                        {
                            try
                            {
                                inventory.DeserializeData();

                                

                                //do work, check type of transfer
                                if((Constants.Container)s1 == Constants.Container.Bag)
                                    if (inventory.bag.items[i1] == 0)
                                        return (Constants.ContainerCodes.CANNOT_MOVE_EMPTY, false);

                                if ((Constants.Container)s1 == Constants.Container.Equipment)
                                    if (inventory.equipment.items[i1] == 0)
                                        return (Constants.ContainerCodes.CANNOT_MOVE_EMPTY, false);

                                //from bag to bag, or equipment to equipment
                                if (s1 == s2)
                                {
                                    if ((Constants.Container) s1 == Constants.Container.Bag)
                                    {
                                        ulong tempGUID;

                                        tempGUID = inventory.bag.items[i1];
                                        inventory.bag.items[i1] = inventory.bag.items[i2];
                                        inventory.bag.items[i2] = tempGUID;
                                    }
                                    else if ((Constants.Container) s1 == Constants.Container.Equipment)
                                    {
                                        //needs check if item is proper slot, swapping rings or trinkets only

                                        if (Constants.equipmentSlots[i1] == Constants.equipmentSlots[i2])
                                        {
                                            ulong tempGUID;
                                            tempGUID = inventory.equipment.items[i1];
                                            inventory.equipment.items[i1] = inventory.equipment.items[i2];
                                            inventory.equipment.items[i2] = tempGUID;
                                        }
                                        //case empty slot
                                        else if(inventory.equipment.items[i2] == 0)
                                        {
                                            if(Constants.equipmentSlots[i1] == Constants.equipmentSlots[i2])
                                            {
                                                ulong tempGUID;
                                                tempGUID = inventory.equipment.items[i1];
                                                inventory.equipment.items[i1] = inventory.equipment.items[i2];
                                                inventory.equipment.items[i2] = tempGUID;
                                            }
                                            else
                                            {
                                                return (Constants.ContainerCodes.WRONG_TYPE, false);
                                            }
                                        }
                                        else
                                        {
                                            return (Constants.ContainerCodes.WRONG_TYPE, false);
                                        }
                                    }
                                }
                                //swaping from equipment to bag, or from bag to equipment
                                else
                                {
                                    //prioritize from equipment to bag always
                                    //from equipment to bag

                                    if ((Constants.Container) s1 == Constants.Container.Equipment)
                                    {
                                        Item item1 = GetSingleItem(inventory.equipment.items[i1]);
                                        Item item2 = GetSingleItem(inventory.bag.items[i2]);

                                        //if landing spot is 0 in inventory
                                        if (inventory.bag.items[i2] == 0)
                                        {
                                            inventory.bag.items[i2] = inventory.equipment.items[i1];

                                            inventory.equipment.items[i1] = 0;
                                        }
                                        //check if types are same, moving item onto item
                                        else if (item1.itemData.Slot
                                                 == item2.itemData.Slot)
                                        {
                                            ulong tempGUID = inventory.equipment.items[i1];
                                            inventory.equipment.items[i1] = inventory.bag.items[i2];
                                            inventory.bag.items[i2] = tempGUID;
                                        }
                                        else
                                        {
                                            return (Constants.ContainerCodes.WRONG_TYPE, false);
                                        }
                                    }
                                    //from bag to equipment
                                    else
                                    {
                                        Item item1 = GetSingleItem(inventory.bag.items[i1]);
                                        Item item2 = GetSingleItem(inventory.equipment.items[i2]);

                                        //equipment item current == 0
                                        if (inventory.equipment.items[i2] == 0)
                                        {
                                            //check slot
                                            if ((Constants.Slots) item1.itemData.Slot ==
                                                Constants.equipmentSlots[i2])
                                            {
                                                inventory.equipment.items[i2] = inventory.bag.items[i1];
                                                inventory.bag.items[i1] = 0;
                                            }
                                            else
                                            {
                                                return (Constants.ContainerCodes.WRONG_TYPE, false);
                                            }
                                        }
                                        //check if item is going into valid slot
                                        else if ((Constants.Slots)item1.itemData.Slot ==
                                            Constants.equipmentSlots[i2])
                                        {

                                            ulong tempGUID = inventory.equipment.items[i2];
                                            inventory.equipment.items[i2] = inventory.bag.items[i1];
                                            inventory.bag.items[i1] = tempGUID;
                                        }
                                        else
                                        {
                                            return (Constants.ContainerCodes.WRONG_TYPE, false);
                                        }
                                    }
                                }

                                try
                                {
                                    //reflect memory of client, fix RealItemData too
                                    if (s1 == s2)
                                    {
                                        //case equipment swap from equipment THIS DOES NOT WARRANT FOR STAT RECALCULATION!
                                        if (s1 == (byte) Constants.Container.Equipment)
                                        {
                                            ulong tempGUID = client.Player.Inventory.equipment.items[i1];

                                            if(client.Player.Inventory.equipment.items[i2] == 0)
                                            {
                                                client.Player.Inventory.equipment.item_Real[i2] = client.Player.Inventory.equipment.item_Real[i1];
                                                client.Player.Inventory.equipment.item_Real[i1] = null;
                                            }
                                            else
                                            {
                                                Item tempItem = client.Player.Inventory.equipment.item_Real[i1];
                                                client.Player.Inventory.equipment.item_Real[i2] = client.Player.Inventory.equipment.item_Real[i1];
                                                client.Player.Inventory.equipment.item_Real[i1] = tempItem;
                                            }

                                            client.Player.Inventory.equipment.items[i1] =
                                                client.Player.Inventory.equipment.items[i2];
                                            client.Player.Inventory.equipment.items[i2] = tempGUID;
                                        }
                                        //bag to bag
                                        else if (s2 == (byte) Constants.Container.Bag)
                                        {
                                            ulong tempGUID = inventory.bag.items[i1];

                                            client.Player.Inventory.bag.items[i1] =
                                                client.Player.Inventory.bag.items[i2];
                                            client.Player.Inventory.bag.items[i2] = tempGUID;
                                        }
                                    }
                                    else
                                    {
                                        //bag to equipment
                                        if ((Constants.Container) s1 == Constants.Container.Bag)
                                        {
                                            ulong tempGUID = client.Player.Inventory.bag.items[i1];

                                            Item tempItem = GetSingleItem(tempGUID);
                                            client.Player.Inventory.equipment.item_Real[i2] = tempItem;
                                            //client.Player.SwapItem(tempGUID, i2);

                                            //no need to store real data in bag well we can cache it
                                            client.Player.Inventory.bag.item_Real[i1] = null;

                                            client.Player.Inventory.bag.items[i1] =
                                                client.Player.Inventory.equipment.items[i2];
                                            client.Player.Inventory.equipment.items[i2] = tempGUID;

                                            Console.WriteLine("Need recalculation bag -> equipment");
                                            recalculateStats = true;
                                        }
                                        //equipment to bag
                                        else if ((Constants.Container)s1 == Constants.Container.Equipment)
                                        {
                                            ulong tempGUID = client.Player.Inventory.equipment.items[i1];

                                            if (client.Player.Inventory.bag.items[i2] == 0)
                                            {
                                                client.Player.Inventory.equipment.item_Real[i1] = null;
                                            }
                                            else
                                            {
                                                Item tempItem = GetSingleItem(client.Player.Inventory.bag.items[i2]);
                                                client.Player.Inventory.equipment.item_Real[i1] = tempItem;
                                            }
                                            client.Player.Inventory.bag.item_Real[i2] = null;

                                            client.Player.Inventory.equipment.items[i1] =
                                                client.Player.Inventory.bag.items[i2];
                                            client.Player.Inventory.bag.items[i2] = tempGUID;

                                            Console.WriteLine("Need recalculation equipment -> bag");
                                            recalculateStats = true;
                                        }
                                    }
                                    inventory.SerializeBag();
                                    inventory.SerializeEquipment();
                                    
                                    db.SaveChanges();

                                }
                                catch
                                {
                                    Console.WriteLine("Failed to write item swap to db");
                                }
                                    
                                }
                            catch { }

                            return (Constants.ContainerCodes.SUCCESS, recalculateStats);
                        }
                    }
                }
            }
            return (Constants.ContainerCodes.UNKNOWN_ERROR, false);
        }

        public static Item[] GetMultipleItems(ref ulong[] items)
        {
            Item[] realData = new Item[Constants.equipmentSlots.Count];

            using (var db = new GameContext())
            {
                int counter = 0;
                foreach (ulong u in items)
                {
                    if (u != 0)
                    {
                        var item = db.Items.SingleOrDefault(o => o.GUID == u);

                        if (item != null)
                        {
                            Item requestedItem = new Item(item.Data);
                            requestedItem.GUID = u;
                            requestedItem.itemData = requestedItem.GetData();

                            realData[counter] = requestedItem;
                        }
                    }
                    else
                        realData[counter] = null;

                    counter++;
                }
            }

            return realData;
        }
    }
}
