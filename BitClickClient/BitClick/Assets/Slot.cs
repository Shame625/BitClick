using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    private InventoryManager inventoryManager;

    public int slotID;
    public ulong currentItem = 0;

    public GameObject currentItemGO;
    private Image shine;

    bool hasItemData = false;

    public byte ContainerType;

    public void SetID(int ID, InventoryManager inv, byte containerType)
    {
        slotID = ID;
        inventoryManager = inv;
        ContainerType = containerType;
    }

    public void SetInventoryManager(InventoryManager inv)
    {
        inventoryManager = inv;
    }

    public void ChangeCurrentItem(ref GameObject go, ulong GUID)
    {
        if (currentItem == GUID)
            return;

        currentItem = GUID;
        //cache shine
        if (shine == null)
            shine = GetComponentInChildren<Image>();

        //clear slot
        if (GUID == 0)
        {
            if (currentItemGO != null)
                Destroy(currentItemGO);

            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.SlotDefaultColor, out shineColor))
            {
                shineColor.a = 0.055f;
                shine.color = shineColor;
            }

            return;
        }

        ItemData itemData = null;
        //try to fetch data from cacheManager
        try
        {
            itemData = CacheManager.cachedItems[GUID];
            hasItemData = true;
        }
        catch
        {
            //default picture will be used, once upon clicking on item itll update
            hasItemData = false;
        }

        //if there is currentGO, destroy it
        DisposeOfCurrentItem();

        currentItemGO = Instantiate(go, transform, false);
        currentItemGO.GetComponent<ItemUI>().SetGUID(GUID, slotID, ContainerType, inventoryManager);

        //in case itemData did load

        if (itemData != null)
        {
            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities) itemData.Quality),
                out shineColor))
            {
                shineColor.a = 0.75f;
                shine.color = shineColor;
            }

            //set icon
            currentItemGO.GetComponent<ItemUI>().SetIcon(itemData.IconID, (Constants.Slots)itemData.Slot);
        }
    }

    public bool CheckIfUpdateIsNeeded()
    {
        return !hasItemData;
    }

    public void UpdateCurrentItem()
    {
        if (!hasItemData)
        {
            ItemData itemData = null;

            //try to fetch data from cacheManager
            try
            {
                itemData = CacheManager.cachedItems[currentItem];
                hasItemData = true;

            }
            catch
            {
            }

            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities) itemData.Quality),
                out shineColor))
            {
                shineColor.a = 0.75f;
                shine.color = shineColor;
            }

            //set icon
            currentItemGO.GetComponent<ItemUI>().SetIcon(itemData.IconID, (Constants.Slots)itemData.Slot);

            hasItemData = true;
        }
    }

    void DisposeOfCurrentItem()
    {
        try
        {
            Destroy(currentItemGO);
        }
        catch
        {
            
        }
    }
}
