using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootboxManager : MonoBehaviour
{
    public GameObject mainObject;
    public GameObject lootBoxPrefab;

    public Vector3 lootBoxEntryRight;
    public Vector3 lootBoxEntryLeft;

    GameObject currentLootBoxGO;

    GameObject nextLootBoxGO;

    public UIManager uiManager;

    public AnimationClip entryRight;
    public AnimationClip entryLeft;
    public AnimationClip leaveRight;
    public AnimationClip leaveLeft;

    public Constants.LootboxType currentLootBox = Constants.LootboxType.Small;
    sbyte currentIndex = 0;

    Color available = new Color(1, 1, 1, 1);
    Color unAvailable = new Color(0, 0, 0, 1);

    public void OpenLootManager()
    {
        //Look at lootbox
        Camera.main.transform.position = new Vector3(mainObject.transform.position.x, mainObject.transform.position.y + 5, -15);
        Camera.main.transform.LookAt(mainObject.transform);

        if(currentLootBoxGO == null)
        {
            currentLootBoxGO = Instantiate(lootBoxPrefab, transform);
            currentLootBoxGO.transform.SetParent(transform);
        }

        bool isAvailable = uiManager.UpdateLootBox(currentLootBox);

        if (isAvailable)
        {
            currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = available;
        }
        else
        {
            currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = unAvailable;
        }
    }

    public void ChangeDirection(ref bool direction)
    {
        if (nextLootBoxGO == null)
        {
            if (direction)
            {
                if (currentIndex + 1 > 2)
                    return;

                currentIndex++;
                if (currentIndex > 2)
                {
                    currentIndex = 2;
                }

                nextLootBoxGO = Instantiate(lootBoxPrefab, transform);
                nextLootBoxGO.GetComponent<Animation>().clip = entryRight;
                nextLootBoxGO.GetComponent<Animation>().Play();

                currentLootBoxGO.GetComponent<Animation>().clip = leaveLeft;
                currentLootBoxGO.GetComponent<Animation>().Play();

                Destroy(currentLootBoxGO, currentLootBoxGO.GetComponent<Animation>().clip.length);
                currentLootBoxGO = nextLootBoxGO;
            }
            else
            {
                if (currentIndex - 1 < 0)
                    return;
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = 0;

                nextLootBoxGO = Instantiate(lootBoxPrefab, transform);
                nextLootBoxGO.GetComponent<Animation>().clip = entryLeft;
                nextLootBoxGO.GetComponent<Animation>().Play();

                currentLootBoxGO.GetComponent<Animation>().clip = leaveRight;
                currentLootBoxGO.GetComponent<Animation>().Play();

                Destroy(currentLootBoxGO, currentLootBoxGO.GetComponent<Animation>().clip.length);
                currentLootBoxGO = nextLootBoxGO;
            }
            
            Invoke("NullNext", currentLootBoxGO.GetComponent<Animation>().clip.length);
            currentLootBox = (Constants.LootboxType)currentIndex + 1;
            currentLootBoxGO.transform.localScale = GetSize(currentLootBox);

            bool isAvailable = uiManager.UpdateLootBox(currentLootBox);

            if (isAvailable)
            {
                currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = available;
            }
            else
            {
                currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = unAvailable;
            }
        }

    }

    public void UpdateMaterial(bool isAvailable)
    {
        if (isAvailable)
        {
            currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = available;
        }
        else
        {
            currentLootBoxGO.GetComponentInChildren<Renderer>().material.color = unAvailable;
        }
    }

    void NullNext()
    {
        nextLootBoxGO = null;
    }

    public void CloseLootManager()
    {
        gameObject.SetActive(false);
    }

    public Vector3 GetSize(Constants.LootboxType type)
    {
        if (type == Constants.LootboxType.Small)
            return new Vector3(1, 1, 1);
        else if (type == Constants.LootboxType.Medium)
            return new Vector3(1.5f, 1.5f, 1.5f);
        return new Vector3(2, 2, 2);
    }
}
