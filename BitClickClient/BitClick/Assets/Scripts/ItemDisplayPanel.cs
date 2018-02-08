using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDisplayPanel : MonoBehaviour
{
    public Text Name;
    public Text Level;
    public Text Slot;
    public Image itemIcon;

    public Text ItemData;
    public NetworkHelper networkHelper;


    uint minHeight = 120;
    uint offSetPerStat = 15;

    public void DisplayItemData(ulong itemGUID)
    {
        Item.ItemInfo? itemInfo;
        Name.text = "";
        Level.text = "";
        Slot.text = "";
        ItemData.text = "";
        try
        {
            itemInfo = CacheManager.cachedItems[itemGUID].DisplayInfo();
        }
        catch
        {
            networkHelper.RetrieveItemData(itemGUID);
            Name.text = "<color=#FF0000FF> Retrieving data</color>";
            return;
        }

        GetComponent<RectTransform>().sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x,  minHeight + (offSetPerStat * itemInfo.Value.numberOfStats));

        string itemColor = Constants.GetQualityColor((Constants.Qualities)CacheManager.cachedItems[itemGUID].Quality);

        Name.text = "<color=" + itemColor + ">" + itemInfo.Value.name + "</color>";
        Level.text = "Level: " + itemInfo.Value.level;
        Slot.text = "Slot: " + itemInfo.Value.slot;

        //set shine
        Color shineColor;
        if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)CacheManager.cachedItems[itemGUID].Quality),
            out shineColor))
        {
            shineColor.a = 0.5f;
            itemIcon.transform.parent.GetComponent<Image>().color = shineColor;
        }

        itemIcon.sprite = Constants.GetItemIcon((Constants.Slots)CacheManager.cachedItems[itemGUID].Slot, itemInfo.Value.iconID);

        ItemData.text = "<color=" + itemColor + ">" + itemInfo.Value.itemstats + "</color>";
    }
}
