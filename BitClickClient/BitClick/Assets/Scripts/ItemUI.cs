using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    private InventoryManager inventoryManager;

    private ulong GUID = 0;

    public byte containerType;
    public int slotID;

    public void Clicked()
    {
        UIManager.instance.DisplayItemPanel(GUID);
        if (slotID != -1)
        {
            UIManager.instance.CheckIfBagNeedsUpdate(slotID, containerType);
        }
    }

    public void SetGUID(ulong g, int sID, byte containerT,InventoryManager inv)
    {
        GUID = g;
        slotID = sID;

        containerType = containerT;
        inventoryManager = inv;
    }

    public void SetGUID(ulong g)
    {
        GUID = g;
    }

    public void SetIcon(ushort iconID, Constants.Slots slot)
    {
        GetComponent<Image>().sprite = Constants.GetItemIcon(slot, iconID);
    }

    public void ItemDragged()
    {
        if(containerType == 0 || containerType == 1)
            inventoryManager.DraggingItem(containerType, slotID);
    }

    public void StopDragging()
    {
        if (containerType == 0 || containerType == 1)
            inventoryManager.DroppedItem();
    }
}
