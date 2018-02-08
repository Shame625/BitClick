using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LootboxReward : MonoBehaviour
{
    public GameObject itemUIPrefab;

    Animation myAnimation;

    public GameObject reward1;
    public GameObject reward2;
    public GameObject reward3;

    private void Awake()
    {
        myAnimation = GetComponent<Animation>();
    }

    public void LootboxOpened(Item[] items)
    {
        if(myAnimation == null)
            myAnimation = GetComponent<Animation>();
        //fades in
        myAnimation.Play();

        if(items.Length == 1)
        {
            reward2.SetActive(false);
            reward3.SetActive(false);
            reward1.SetActive(true);

            reward1.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetGUID(items[0].GUID);
            reward1.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetIcon(items[0].itemData.IconID, (Constants.Slots)items[0].itemData.Slot);

            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[0].itemData.Quality),
                out shineColor))
            {
                shineColor.a = 0.5f;
                reward1.GetComponentInChildren<Image>().color = shineColor;
            }

            Invoke("Reward1Child1", 0.25f);
        }
        else if(items.Length == 2)
        {
            reward1.SetActive(false);
            reward3.SetActive(false);
            reward2.SetActive(true);

            reward2.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetGUID(items[0].GUID);
            reward2.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetIcon(items[0].itemData.IconID, (Constants.Slots)items[0].itemData.Slot);

            reward2.transform.GetChild(1).GetChild(0).GetComponent<ItemUI>().SetGUID(items[1].GUID);
            reward2.transform.GetChild(1).GetChild(0).GetComponent<ItemUI>().SetIcon(items[1].itemData.IconID, (Constants.Slots)items[1].itemData.Slot);

            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[0].itemData.Quality),
                out shineColor))
            {
                shineColor.a = 0.5f;
                reward2.GetComponentInChildren<Image>().color = shineColor;
                reward2.transform.GetChild(1).GetComponentInChildren<Image>().color = shineColor;
            }

            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[1].itemData.Quality),
            out shineColor))
            {
                shineColor.a = 0.5f;
                reward2.transform.GetChild(1).GetComponentInChildren<Image>().color = shineColor;
            }

            Invoke("Reward2Child1", Random.Range(0.15f, 0.30f));
            Invoke("Reward2Child2", Random.Range(0.15f, 0.30f));
        }
        else
        {
            reward1.SetActive(false);
            reward2.SetActive(false);
            reward3.SetActive(true);

            reward3.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetGUID(items[0].GUID);
            reward3.transform.GetChild(0).GetChild(0).GetComponent<ItemUI>().SetIcon(items[0].itemData.IconID, (Constants.Slots)items[0].itemData.Slot);

            reward3.transform.GetChild(1).GetChild(0).GetComponent<ItemUI>().SetGUID(items[1].GUID);
            reward3.transform.GetChild(1).GetChild(0).GetComponent<ItemUI>().SetIcon(items[1].itemData.IconID, (Constants.Slots)items[1].itemData.Slot);

            reward3.transform.GetChild(2).GetChild(0).GetComponent<ItemUI>().SetGUID(items[2].GUID);
            reward3.transform.GetChild(2).GetChild(0).GetComponent<ItemUI>().SetIcon(items[2].itemData.IconID, (Constants.Slots)items[2].itemData.Slot);
            
            
            Color shineColor;
            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[0].itemData.Quality),
                out shineColor))
            {
                shineColor.a = 0.5f;
                reward3.GetComponentInChildren<Image>().color = shineColor;
            }

            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[1].itemData.Quality),
            out shineColor))
            {
                shineColor.a = 0.5f;
                reward3.transform.GetChild(1).GetComponentInChildren<Image>().color = shineColor;
            }

            if (ColorUtility.TryParseHtmlString(Constants.GetQualityColor((Constants.Qualities)items[2].itemData.Quality),
            out shineColor))
            {
                shineColor.a = 0.5f;
                reward3.transform.GetChild(2).GetComponentInChildren<Image>().color = shineColor;
            }

            Invoke("Reward3Child1", Random.Range(0.15f, 0.30f));
            Invoke("Reward3Child2", Random.Range(0.15f, 0.30f));
            Invoke("Reward3Child3", Random.Range(0.15f, 0.30f));
        }
    }

    public void Close()
    {
        CancelInvoke("Reward1Child1");

        CancelInvoke("Reward2Child1");
        CancelInvoke("Reward2Child2");

        CancelInvoke("Reward3Child1");
        CancelInvoke("Reward3Child2");
        CancelInvoke("Reward3Child3");

        reward1.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 600);

        reward2.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(-120, 600);
        reward2.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(120, 600);

        reward3.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 600);
        reward3.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(-220, 600);
        reward3.transform.GetChild(2).GetComponent<RectTransform>().anchoredPosition = new Vector2(220, 600);

        reward1.SetActive(false);
        reward2.SetActive(false);
        reward3.SetActive(false);
        gameObject.SetActive(false);
    }


    void Reward1Child1()
    {
        reward1.transform.GetChild(0).GetComponent<Animation>().Play();
    }

    void Reward2Child1()
    {
        reward2.transform.GetChild(0).GetComponent<Animation>().Play();
    }

    void Reward2Child2()
    {
        reward2.transform.GetChild(1).GetComponent<Animation>().Play();
    }

    void Reward3Child1()
    {
        reward3.transform.GetChild(0).GetComponent<Animation>().Play();
    }
    void Reward3Child2()
    {
        reward3.transform.GetChild(1).GetComponent<Animation>().Play();
    }
    void Reward3Child3()
    {
        reward3.transform.GetChild(2).GetComponent<Animation>().Play();
    }
}
