using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameManager gameManager;
    public NetworkHelper networkHelper;

    public GameObject itemUISlotPrefab;
    public GameObject itemUIPrefab;

    public GameObject bagHeader;
    public GameObject bagContents;

    public GameObject equipmentContent;

    public Text infoText;
    public Text statsText;

    List<GameObject> slots = new List<GameObject>();

    List<GameObject> equipmentslots = new List<GameObject>();

    bool isDragged = false;
    private GameObject draggableObject;

    private bool isGenerated = false;

    public void GenerateBag()
    {
        //spawns empty slots
        for (int i = 0; i < gameManager.player.inventory.bagSize; i++)
        {
            GameObject go = Instantiate(itemUISlotPrefab, bagContents.transform, false);

            slots.Add(go);
            slots[i].GetComponent<Slot>().SetID(i, this, (byte)Constants.Container.Bag);
        }

        //load equipment slots
        foreach (Transform tr in equipmentContent.transform)
        {
            equipmentslots.Add(tr.gameObject);

            tr.gameObject.GetComponent<Slot>().SetInventoryManager(this);
        }

        equipmentslots = equipmentslots.OrderBy(o => o.GetComponent<Slot>().slotID).ToList();
    }

    public void HandleBag()
    {
        //warm up
        if (!isGenerated)
        {
            GenerateBag();
            isGenerated = true;
        }

        //proceed to spawn items over slots in bag
        for (int i = 0; i < gameManager.player.inventory.bag.items.Length; i++)
        {
            if (gameManager.player.inventory.bag.items[i] != 0)
            {
                slots[i].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, gameManager.player.inventory.bag.items[i]);
                
            }
        }

        //spawn items in equipment
        for (int i = 0; i < gameManager.player.inventory.equipment.items.Length; i++)
        {
            if (gameManager.player.inventory.equipment.items[i] != 0)
            {
                equipmentslots[i].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, gameManager.player.inventory.equipment.items[i]);
            }
        }

        DisplayInfo();
        DisplayPlayerStats();
    }

    public bool CheckIfSlotNeedsUpdate(int slotID, byte containerId)
    {
        if(containerId == (byte)Constants.Container.Bag)
            return slots[slotID].GetComponent<Slot>().CheckIfUpdateIsNeeded();

        return equipmentslots[slotID].GetComponent<Slot>().CheckIfUpdateIsNeeded();
    }

    public void TryUpdate(int slotID, byte containerId)
    {
        if (containerId == (byte)Constants.Container.Bag)
            slots[slotID].GetComponent<Slot>().UpdateCurrentItem();
        else
        equipmentslots[slotID].GetComponent<Slot>().UpdateCurrentItem();
    }

    private GameObject referenceSlot;

    void Update()
    {
        if (isDragged)
        {
            referenceSlot = FindClosestSlot(Input.mousePosition);
            if (referenceSlot != null)
            {
                draggableObject.transform.position = referenceSlot.transform.position;
                draggableObject.GetComponentInChildren<Image>().rectTransform.sizeDelta = new Vector2(90, 90);
            }
            else
            {
                draggableObject.transform.position = Input.mousePosition;
            }
        }
    }

    private GameObject startingSlot;

    public void DraggingItem(byte containerType, int slotID)
    {
        if (draggableObject == null)
        {
            if(containerType == (byte)Constants.Container.Bag)
                draggableObject = Instantiate(itemUIPrefab, slots[slotID].transform);
            else
            {
                draggableObject = Instantiate(itemUIPrefab, equipmentslots[slotID].transform);
            }
        }

        if (containerType == (byte) Constants.Container.Bag)
        {
            startingSlot = slots[slotID];
            draggableObject.GetComponentInChildren<Image>().sprite = slots[slotID].GetComponent<Slot>().currentItemGO.GetComponentInChildren<Image>().sprite;
            draggableObject.GetComponent<ItemUI>().SetGUID(slots[slotID].GetComponent<Slot>().currentItem,
                -1, containerType, this);
        }
        else
        {
            startingSlot = equipmentslots[slotID];
            draggableObject.GetComponentInChildren<Image>().sprite = equipmentslots[slotID].GetComponent<Slot>().currentItemGO.GetComponentInChildren<Image>().sprite;
            draggableObject.GetComponent<ItemUI>().SetGUID(equipmentslots[slotID].GetComponent<Slot>().currentItem,
                -1, containerType, this);
        }

        draggableObject.transform.SetParent(transform.root);

        draggableObject.GetComponentInChildren<Image>().rectTransform.sizeDelta = new Vector2(120,120);


        isDragged = true;
    }

    public void DroppedItem()
    {
        isDragged = false;
        if (referenceSlot == null)
        {
            Destroy(draggableObject);
        }
        else
        {
            queuedSwap.source1 = startingSlot.GetComponent<Slot>().ContainerType;
            queuedSwap.index1 = startingSlot.GetComponent<Slot>().slotID;
            queuedSwap.source2 = referenceSlot.GetComponent<Slot>().ContainerType;
            queuedSwap.index2 = referenceSlot.GetComponent<Slot>().slotID;

            //attempt to swap
            networkHelper.SwapItems(queuedSwap.source1, queuedSwap.index1, queuedSwap.source2, queuedSwap.index2);
        }
    }

    GameObject FindClosestSlot(Vector2 input)
    {
        GameObject bestTarget = null;

        float closestDistance = 37;
        foreach (GameObject vec in slots)
        {
            float newDistance = Vector2.Distance(vec.transform.position, input);
            if (newDistance < closestDistance)
            {
                bestTarget = vec;
                closestDistance = newDistance;
            }
        }

        foreach (GameObject vec in equipmentslots)
        {
            float newDistance = Vector2.Distance(vec.transform.position, input);
            if (newDistance < closestDistance)
            {
                bestTarget = vec;
                closestDistance = newDistance;
            }
        }

        return bestTarget;
    }

    public struct SwapQueu
    {
        public byte source1;
        public int index1;
        public byte source2;
        public int index2;
    }

    public static SwapQueu queuedSwap;

    public void ProcessWithSwap()
    {
        if (queuedSwap.source1 == (byte) Constants.Container.Bag)
        {
            ulong tempGUID = slots[queuedSwap.index1].GetComponent<Slot>().currentItem;

            //perform the swap
            if (queuedSwap.source2 == (byte) Constants.Container.Bag)
            {
                gameManager.player.inventory.bag.items[queuedSwap.index1] =
                    gameManager.player.inventory.bag.items[queuedSwap.index2];
                gameManager.player.inventory.bag.items[queuedSwap.index2] = tempGUID;

                slots[queuedSwap.index1].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, slots[queuedSwap.index2].GetComponent<Slot>().currentItem);
                slots[queuedSwap.index2].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, tempGUID);
            }
            else if(queuedSwap.source2 == (byte)Constants.Container.Equipment)
            {
                gameManager.player.inventory.bag.items[queuedSwap.index1] =
                    gameManager.player.inventory.equipment.items[queuedSwap.index2];
                gameManager.player.inventory.equipment.items[queuedSwap.index2] = tempGUID;

                slots[queuedSwap.index1].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, equipmentslots[queuedSwap.index2].GetComponent<Slot>().currentItem);
                equipmentslots[queuedSwap.index2].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, tempGUID);

                gameManager.player.CalculateStats();
                DisplayPlayerStats();
            }
            Destroy(draggableObject);
        }
        else if (queuedSwap.source1 == (byte)Constants.Container.Equipment)
        {
            ulong tempGUID = equipmentslots[queuedSwap.index1].GetComponent<Slot>().currentItem;

            //perform the swap
            if (queuedSwap.source2 == (byte)Constants.Container.Bag)
            {
                gameManager.player.inventory.equipment.items[queuedSwap.index1] =
                    gameManager.player.inventory.bag.items[queuedSwap.index2];
                gameManager.player.inventory.bag.items[queuedSwap.index2] = tempGUID;

                equipmentslots[queuedSwap.index1].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, slots[queuedSwap.index2].GetComponent<Slot>().currentItem);
                slots[queuedSwap.index2].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, tempGUID);
            }
            else if (queuedSwap.source2 == (byte)Constants.Container.Equipment)
            {
                gameManager.player.inventory.equipment.items[queuedSwap.index1] =
                    gameManager.player.inventory.equipment.items[queuedSwap.index2];
                gameManager.player.inventory.equipment.items[queuedSwap.index2] = tempGUID;

                equipmentslots[queuedSwap.index1].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, equipmentslots[queuedSwap.index2].GetComponent<Slot>().currentItem);
                equipmentslots[queuedSwap.index2].GetComponent<Slot>().ChangeCurrentItem(ref itemUIPrefab, tempGUID);
            }
            //case of equipment items calls for recalculation!

            gameManager.player.CalculateStats();
            DisplayPlayerStats();

            Destroy(draggableObject);
        }
    }

    void DisplayPlayerStats()
    {
        statsText.text = gameManager.player.stats.ToString();
    }

    void DisplayInfo()
    {
        string temp = "Name: " + GameManager.userName + "\nLevel: " + gameManager.player.experience/43 + "\nExperience: " + gameManager.player.experience + "\nSkill points: " + gameManager.player.skillPoints;
        infoText.text = temp;
    }

    public void SwapFailed()
    {
        Destroy(draggableObject);
    }
}
