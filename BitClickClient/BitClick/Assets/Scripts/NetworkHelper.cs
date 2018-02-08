using System.Security.Cryptography;
using Assets;
using UnityEngine;

public class NetworkHelper : MonoBehaviour
{
    private UIManager uiManager;
    private NetworkManager networkManager;
    public LootboxManager lootboxManager;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        networkManager = GetComponent<NetworkManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SwapItems(0, 0, 0, 5);
        }
    }

    public void LogIn()
    {
        UIManager.EmailPassword email_password = uiManager.GetEmailPassword(true);

        Package packet = new Package(PackageType.LOGIN_REQUEST);
        packet.loginReq.SetRequest(email_password.email, email_password.password);

        networkManager.SendPacket(ref packet);
    }

    public void Register()
    {
        //when its false, it gets info from registration panel
        UIManager.EmailPassword email_password = uiManager.GetEmailPassword(false);

        Package packet = new Package(PackageType.REG_REQUEST);
        packet.registrationReq.SetRequest(email_password.email, email_password.password);

        networkManager.SendPacket(ref packet);
    }

    public void ChooseUserName()
    {
        string userName = uiManager.GetUsername();

        Package packet = new Package(PackageType.SET_USERNAME_REQUEST);
        packet.setUsernameReq.SetRequest(userName);

        networkManager.SendPacket(ref packet);
    }

    public void OpenLevelSelector()
    {
        Package packet = new Package(PackageType.RETRIEVE_LEVEL_REQUEST);
        packet.retrieveLevelReq.SetRequest(GameManager.CurrentLevel);

        networkManager.SendPacket(ref packet);
    }

    public void EnterLevel()
    {
        Package packet = new Package(PackageType.ENTER_REQUEST);
        packet.enterReq.SetRequest(GameManager.CurrentLevel);

        networkManager.SendPacket(ref packet);
    }

    public void LeaveLevel()
    {
        Package packet = new Package(PackageType.LEAVE_REQUEST);

        networkManager.SendPacket(ref packet);
    }

    public void RetrieveLevel(bool LoR)
    {
        if (!LoR)
        {
            if (((int) GameManager.CurrentLevel - 1) <= 0)
            {
                return;
            }
            else
            {
                GameManager.CurrentLevel--;
            }
        }
        else
        {
            //100 is placeholder, we will have to request level max size when logging in, we will just return it
            if ((GameManager.CurrentLevel + 1) > GameManager.NumberOfLevels)
            {
                return;
            }
            else
            {
                GameManager.CurrentLevel++;
            }
        }

        Package packet = new Package(PackageType.RETRIEVE_LEVEL_REQUEST);
        packet.retrieveLevelReq.SetRequest(GameManager.CurrentLevel);

        networkManager.SendPacket(ref packet);
    }

    public void RetrieveLevel10(bool LoR)
    {
        uint temp = GameManager.CurrentLevel;

        if (!LoR)
        {
            if (((int)GameManager.CurrentLevel - 10) <= 0)
            {
                GameManager.CurrentLevel = 1;
            }
            else
            {
                GameManager.CurrentLevel-=10;
            }
        }
        else
        {
            //100 is placeholder, we will have to request level max size when logging in, we will just return it
            if ((GameManager.CurrentLevel + 10) > GameManager.NumberOfLevels)
            {
                GameManager.CurrentLevel = GameManager.NumberOfLevels;
            }
            else
            {
                GameManager.CurrentLevel+=10;
            }
        }

        //no need to request data.
        if (temp == GameManager.CurrentLevel)
            return;

        Package packet = new Package(PackageType.RETRIEVE_LEVEL_REQUEST);
        packet.retrieveLevelReq.SetRequest(GameManager.CurrentLevel);

        networkManager.SendPacket(ref packet);
    }

    public void DoDamage(ref uint blockId, Vector3 point)
    {
        Package packet = new Package(PackageType.BIT_HIT_REQUEST);
        packet.bitHitReq.SetRequest(blockId);

        networkManager.SendPacket(ref packet);
    }

    public void RetrieveItems()
    {
        Package packet = new Package(PackageType.INVENTORY_REQUEST);

        networkManager.SendPacket(ref packet);
    }

    public void OpenLootBox()
    {
        Package packet = new Package(PackageType.LOOTBOX_OPEN_REQUEST);
        packet.lootboxOpenReq.request = (byte)lootboxManager.currentLootBox;

        networkManager.SendPacket(ref packet);
    }

    public void RetrieveItemData(ulong guid)
    {
        //check if its cached first
        if (CacheManager.cachedItems.ContainsKey(guid))
            return;

        Package packet = new Package(PackageType.ITEM_DATA_REQUEST);
        packet.singleItemDataReq.GUID = guid;

        networkManager.SendPacket(ref packet);
    }

    public void SwapItems(byte source1, int index1, byte source2, int index2)
    {
        Package packet = new Package(PackageType.ITEM_SWAP_REQUEST);
        packet.swapItemReq.SetRequest(source1, index1, source2, index2);

        //QUEU IT! swapping must be queued

        networkManager.SendPacket(ref packet);
    }

}
