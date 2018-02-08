using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotFeeder : MonoBehaviour
{
    public GameObject itemElementPrefab;

    public Sprite currencySprite;
    public Sprite experienceSprite;
    public Sprite lootboxSprite;

    List<Slot> slots = new List<Slot>();

    public static Color itemColor = new Color(0.024f, 0.675f, 1, 1);
    public static Color currencyColor = new Color(0.949f, 0.651f, 0.204f);
    public static Color experienceColor = new Color(0.208f, 0.706f, 0.114f);

    private void Awake()
    {
        //Load slots
        int i = 0;
        foreach(Transform tr in transform.GetChild(1))
        {
            slots.Add(new Slot(i, tr.GetComponent<RectTransform>()));
            i++;
        }
    }

    //change input that it gets, should be Item
    public void FeedNewItem(uint number, RewardType type)
    {
        //move all items down by 1
        for(int i = 4; i > 0; i--)
        {
            if(i == 4)
            {
                if(slots[4].objectOnIt != null)
                {
                    Destroy(slots[4].objectOnIt);
                }
            }

            slots[i].objectOnIt = slots[i - 1].objectOnIt;

            if (slots[i].objectOnIt != null)
            {
                slots[i].objectOnIt.transform.SetParent(slots[i].location);
                slots[i].objectOnIt.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
            }

        }

        //feed new item on top
        GameObject go = Instantiate(itemElementPrefab, slots[0].location, false);

        if (type == RewardType.Currency)
        {
            go.GetComponent<Image>().color = currencyColor;
            go.transform.GetChild(0).GetComponent<Text>().text = number + " " + Constants.CurrencyName;
            go.transform.GetChild(1).GetComponent<Image>().sprite = currencySprite;
        }
        else if(type == RewardType.Experience)
        {
            go.GetComponent<Image>().color = experienceColor;
            go.transform.GetChild(0).GetComponent<Text>().text = number + " Experience";
            go.transform.GetChild(1).GetComponent<Image>().sprite = experienceSprite;
        }
        else if (type == RewardType.Lootbox)
        {
            go.GetComponent<Image>().color = itemColor;
            go.transform.GetChild(0).GetComponent<Text>().text = number + " LootBox";
            go.transform.GetChild(1).GetComponent<Image>().sprite = lootboxSprite;
        }

        slots[0].objectOnIt = go;
    }

    class Slot
    {
        public int Id;

        public RectTransform location;

        public GameObject objectOnIt;

        public Slot(int i, RectTransform l)
        {
            Id = i;
            location = l;
        }
    }

    public enum RewardType
    {
        Currency,
        Experience,
        Lootbox
    }
}
