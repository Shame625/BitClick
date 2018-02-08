using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    //reference to our audioManager
    AudioManager audioManager;
    GameManager gameManager;
    public LootboxManager lootboxManager;

    //Used to pass around email/password inputs
    public struct EmailPassword
    {
        public string email;
        public string password;
    }

    //2 gameobjects, 1 is related to Login/Register thingies other one is GameRealted ONLY!
    public GameObject gameEnterUI;
    public GameObject gameUI;
    public GameObject inGameUI;

    //Login Panel
    public GameObject loginPanel;
    private InputField l_emailInputText;
    private InputField l_passwordInputText;
    private Text l_errorMessageText;

    //Registration Panel
    public GameObject registrationPanel;
    private InputField r_emailInputText;
    private InputField r_passwordInputText;
    private Text r_errorMessageText;

    private Color messageBad = new Color(1, 0.329f, 0.329f, 1);
    private Color messageGood = new Color(0.212f, 0.557f, 0.243f, 1);

    //Choose username Panel
    public GameObject chooseusernamePanel;
    private InputField u_usernameInputText;
    private Text u_errorMessageText;

    //General ui stuff
    public GameObject HeaderButtons;

    //Lootbox stuff
    private GameObject lootBoxButton;
    private GameObject LootBoxPanel;
    private Text lootBoxText;
    private GameObject lootBoxErrorPanel;

    //Skill stuff
    public GameObject skillsButton;
    private GameObject skillsPanel;

    //Options panel
    public GameObject optionsPanel;
    private Slider musicSlider;
    private Slider sfxSlider;
    private Slider networkSfxSlider;

    //LevelSelector Panel
    public GameObject levelSelectorPanel;
    public Text levelSelectorPanelText;
    public Text levelSelectorLevelOutOfLevelText;
    public GameObject levelSelectorRespawnAt;
    public GameObject levelSelectorActiveTill;

    //Inventory Panel
    public GameObject inventoryPanel;
    InventoryManager inventoryManager;

    //RankPanel
    public GameObject rankPanel;
    public GameObject rankContent;
    public GameObject rankElementPrefab;

    //Ingame stuff
    public GameObject levelFinishedPanel;
    public GameObject levelFailedPanel;
    public Text aliveTillTimer;

    public GameObject cameraIngameControll;

    public SlotFeeder itemFeed;

    //keep it as buffer, when called show it
    private List<UserAndDamage> ranks;

    //Hp UI
    public GameObject blockHp;
    public Image hpContent;
    public Text hpText;

    //Item ui thingy
    public GameObject itemDisplayPanel;

    //Error Message Panel
    public GameObject errorMessagePopUp;
    private Text erorrMessagePopUpText;

    void Update()
    {
        // Tab back and forth between input fields
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (loginPanel.activeSelf)
            {
                if (l_emailInputText.isFocused)
                    EventSystem.current.SetSelectedGameObject(l_passwordInputText.gameObject, null);
                else 
                    EventSystem.current.SetSelectedGameObject(l_emailInputText.gameObject, null);
            }

            else if (registrationPanel.activeSelf)
            {
                if (r_emailInputText.isFocused)
                    EventSystem.current.SetSelectedGameObject(r_passwordInputText.gameObject, null);
                else
                    EventSystem.current.SetSelectedGameObject(r_emailInputText.gameObject, null);
            }
        }
    }

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();

        //AudioManager load
        audioManager = GetComponent<AudioManager>();

        //Load LoginPanel
        l_emailInputText = loginPanel.transform.GetChild(0).GetComponent<InputField>();
        l_passwordInputText = loginPanel.transform.GetChild(1).GetComponent<InputField>();
        l_errorMessageText = loginPanel.transform.GetChild(2).GetComponent<Text>();

        //Load RegistrationPanel
        r_emailInputText = registrationPanel.transform.GetChild(0).GetComponent<InputField>();
        r_passwordInputText = registrationPanel.transform.GetChild(1).GetComponent<InputField>();
        r_errorMessageText = registrationPanel.transform.GetChild(2).GetComponent<Text>();

        //Load Choose Username Panel
        u_usernameInputText = chooseusernamePanel.transform.GetChild(1).GetComponent<InputField>();
        u_errorMessageText = chooseusernamePanel.transform.GetChild(3).GetComponent<Text>();

        //Inventory Manager
        inventoryManager = inventoryPanel.GetComponent<InventoryManager>();

        //Load general butons
        lootBoxButton = HeaderButtons.transform.GetChild(0).gameObject;
        skillsButton = HeaderButtons.transform.GetChild(1).gameObject;

        //Load lootbox panel
        LootBoxPanel = gameUI.transform.GetChild(1).gameObject;
        lootBoxText = LootBoxPanel.transform.GetChild(0).GetComponent<Text>();
        lootBoxErrorPanel = LootBoxPanel.transform.GetChild(5).gameObject;

        //Load skills panel
        skillsPanel = gameUI.transform.GetChild(3).gameObject;

        //Load optionspanel
        musicSlider = optionsPanel.transform.GetChild(1).GetChild(0).GetComponent<Slider>();
        sfxSlider = optionsPanel.transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        networkSfxSlider = optionsPanel.transform.GetChild(3).GetChild(0).GetComponent<Slider>();

        //Load Error msg popup 
        erorrMessagePopUpText = errorMessagePopUp.GetComponentInChildren<Text>();

        instance = this;
    }

    //Loading stuff from files
    public void ReflectAudioManagerScrollBars(float m, float s, float ns)
    {
        musicSlider.value = m;
        sfxSlider.value = s;
        networkSfxSlider.value = ns;
    }

    //Methods
    
    //Gets strings from fields Login or Register
    public EmailPassword GetEmailPassword(bool isLogin)
    {
        if(isLogin)
            return new EmailPassword() {email = l_emailInputText.text, password = l_passwordInputText.text};

        return new EmailPassword() { email = r_emailInputText.text, password = r_passwordInputText.text };
    }

    public string GetUsername()
    {
        return u_usernameInputText.text;
    }

    public void DisplayRegistrationPanel()
    {
        registrationPanel.SetActive(true);
    }

    public void DisplayChooseUsernamePanel(bool b)
    {
        chooseusernamePanel.SetActive(b);
    }

    public void DisplayLoginMessage(Constants.LoginCodes loginCode)
    {
        l_errorMessageText.text = Constants.GetLoginMessage(loginCode);
    }

    public void DisplayRegisterMessage(Constants.RegistrationCodes regCode)
    {
        if (regCode == Constants.RegistrationCodes.REGISTRATION_SUCCESSFUL)
            r_errorMessageText.color = messageGood;
        else
            r_errorMessageText.color = messageBad;

        r_errorMessageText.text = Constants.GetRegistrationMessage(regCode);
    }

    public void DisplayChooseUsernameError(Constants.UsernameCodes uCode)
    {
        u_errorMessageText.text = Constants.GetUsernameMessage(uCode);
    }

    //Handles errors raised by server. "Creates popup window that will display the errror code";
    public void DisplayError(Constants.ErrorCodes errCode)
    {
        errorMessagePopUp.SetActive(true);
        erorrMessagePopUpText.text = Constants.GetErrorStringMsg(errCode);

        if (errCode == Constants.ErrorCodes.NOT_IN_ROOM)
            LoggedIn();
    }

    //opens main menu
    public void LoggedIn()
    {
        //Play mainMenu music
        audioManager.PlayMusic(AudioManager.Music.MainMenu);

        gameEnterUI.SetActive(false);
        gameUI.SetActive(true);
        //incase main menu is hidden
        gameUI.transform.GetChild(0).gameObject.SetActive(true);

        levelSelectorPanel.SetActive(false);

        inGameUI.SetActive(false);
    }

    //Remember to restore UI to default state, in case of error or anything, trigger this.
    public void LoggedOut()
    {
        gameEnterUI.SetActive(true);
        gameUI.SetActive(false);

        levelSelectorPanel.SetActive(false);

        inGameUI.SetActive(false);
    }

    //values of music and sfx slider
    public float GetMusicVolume()
    {
        return musicSlider.value;
    }

    public float GetSfxVolume()
    {
        return sfxSlider.value;
    }

    public float GetNetworkSfxVolume()
    {
        return networkSfxSlider.value;
    }


    //Lootbox Stuff
    public void OpenLootBoxPanel()
    {
        //Hides main menu
        if (!LootBoxPanel.gameObject.activeSelf)
        {
            lootboxManager.gameObject.SetActive(true);
            lootboxManager.OpenLootManager();

            LootBoxPanel.SetActive(true);
            lootBoxErrorPanel.SetActive(false);

            gameUI.transform.GetChild(0).gameObject.SetActive(false);

            if (inventoryPanel.activeSelf)
                inventoryPanel.SetActive(false);
            if (skillsPanel.activeSelf)
                skillsPanel.SetActive(false);
        }
        else
        {
            lootboxManager.gameObject.SetActive(false);
            lootboxManager.CloseLootManager();

            LootBoxPanel.SetActive(false);
            gameUI.transform.GetChild(0).gameObject.SetActive(true);

        }
    }

    public void ChangeLootBox(bool direction)
    {
        lootboxManager.ChangeDirection(ref direction);
    }

    public bool UpdateLootBox(Constants.LootboxType type)
    {
        if(type == Constants.LootboxType.Small)
        {
            lootBoxText.text = "Small Lootbox (" + gameManager.player.inventory.lootBoxes.Small + ")";
            if (gameManager.player.inventory.lootBoxes.Small > 0)
                return true;
        }
        else if (type == Constants.LootboxType.Medium)
        {
            lootBoxText.text = "Medium Lootbox (" + gameManager.player.inventory.lootBoxes.Medium + ")";
            if (gameManager.player.inventory.lootBoxes.Medium > 0)
                return true;
        }
        else if (type == Constants.LootboxType.Big)
        {
            lootBoxText.text = "Big Lootbox (" + gameManager.player.inventory.lootBoxes.Big + ")";
            if (gameManager.player.inventory.lootBoxes.Big > 0)
                return true;
        }

        //checking if header shud blink or not
        HandleInventoryUpdate();
        return false;
    }

    public void LootboxCodeHandler(Constants.LootboxCodes code)
    {
        string text = "";

        switch (code)
        {
            
            //display items you got etc
            case Constants.LootboxCodes.SUCCESSFULL:
                break;
            case Constants.LootboxCodes.UNSUCCESSFULL_NOT_ENOUGH_SPACE:
                text = "Not enough space in inventory.";
                break;
            case Constants.LootboxCodes.UNSUCCESSFULL_OUT_OF_BOXES:
                text = "You don't have any lootboxes of that type.";
                break;
            case Constants.LootboxCodes.UNSUCCESSFULL_UNKNOWN_BOX:
                text = "How?!";
                break;
        }
        if(code != Constants.LootboxCodes.SUCCESSFULL)
            lootBoxErrorPanel.SetActive(true);

        lootBoxErrorPanel.GetComponentInChildren<Text>().text = text;
    }

    //Skills panel stuff
    public void OpenSkillsPanel()
    {
        if (!skillsPanel.gameObject.activeSelf)
        {
            skillsPanel.SetActive(true);

            gameUI.transform.GetChild(0).gameObject.SetActive(false);

            if(lootboxManager.gameObject.activeSelf)
            {
                lootboxManager.gameObject.SetActive(false);
                lootboxManager.CloseLootManager();

                LootBoxPanel.SetActive(false);
            }
            if (inventoryPanel.activeSelf)
                inventoryPanel.SetActive(false);
        }
        else
        {
            skillsPanel.SetActive(false);

            gameUI.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void DisplayItemPanel(ulong ItemGUID)
    {
        itemDisplayPanel.SetActive(true);
        itemDisplayPanel.GetComponent<ItemDisplayPanel>().DisplayItemData(ItemGUID);
    }

    struct UpdateSlotContainer
    {
        public int slotID;
        public byte containerID;
    }

    //inventory stuff
    List<UpdateSlotContainer> updateSlotQuery = new List<UpdateSlotContainer>();

    //check if current item has data inside inventory
    public void CheckIfBagNeedsUpdate(int sID, byte cID)
    {
        if (inventoryManager.CheckIfSlotNeedsUpdate(sID, cID))
        {
            updateSlotQuery.Add(new UpdateSlotContainer() { slotID = sID, containerID = cID });
        }
    }

    //update slots
    public void UpdateSlots()
    {
        foreach (UpdateSlotContainer u in updateSlotQuery)
        {
            inventoryManager.TryUpdate(u.slotID, u.containerID);
        }
        updateSlotQuery.Clear();
    }

    //reopen inventory
    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        HandleBag();
    }

    TimeSpan aliveTill;
    public void InLevel(ref DateTime at)
    {
        audioManager.PlayMusic(AudioManager.Music.Level);

        CancelInvoke("Countdown");
        gameEnterUI.SetActive(false);
        gameUI.SetActive(false);

        levelSelectorPanel.SetActive(false);

        inGameUI.SetActive(true);
        blockHp.SetActive(true);

        //this will probably be 1 big container of all ui elements that we will call with function cos having 100000 things everywhere looks dumb
        cameraIngameControll.SetActive(true);

        aliveTill = at.Subtract(DateTime.Now);
        
        InvokeRepeating("Countdown", 0, 1);
    }
    #region GameRelatedUI

    //Reduce every second a second from difference timespan
    void Countdown()
    {
        aliveTill = aliveTill.Subtract(TimeSpan.FromSeconds(1));
        
        string tempMin = "";
        string tempSeconds = "";

        if (aliveTill.Minutes < 10)
            tempMin = "0" + aliveTill.Minutes;
        else
            tempMin = aliveTill.Minutes.ToString();

        if (aliveTill.Seconds < 10)
            tempSeconds = "0" + aliveTill.Seconds;
        else
            tempSeconds = aliveTill.Seconds.ToString();

        if (aliveTill.Days * 24 + aliveTill.Hours > 0)
        {

            aliveTillTimer.text = string.Format("{0}:{1}:{2}", aliveTill.Days * 24 + aliveTill.Hours, tempMin, tempSeconds);
        }
        else if(aliveTill.Minutes > 0)
        {

            aliveTillTimer.text = string.Format("{0}:{1}", tempMin, tempSeconds);
        }
        else
        {
            aliveTillTimer.text = string.Format("{0}", tempSeconds);
        }

        if (aliveTill.TotalSeconds <= 0)
        {
            aliveTillTimer.text = "Closing";
            CancelInvoke("Countdown");
        }

    }

    public void SetInfoLevelSelector(LevelDisplayData level)
    {
        audioManager.PlayMusic(AudioManager.Music.MainMenu);

        if (inGameUI.activeSelf)
            inGameUI.SetActive(false);

        if(levelFinishedPanel.activeSelf)
            levelFinishedPanel.SetActive(false);

        if (levelFailedPanel.activeSelf)
            levelFailedPanel.SetActive(false);

        if (cameraIngameControll.activeSelf)
            cameraIngameControll.SetActive(false);

        if (!levelSelectorPanel.activeSelf)
            levelSelectorPanel.SetActive(true);

        levelSelectorPanelText.text = "Level: " + level.Level + "\nBit Hp: " + level._bitHp + "\nBlock Count: " + (level._numberOfBlocks - level._numberOfDeadBlocks) + " / " + level._numberOfBlocks +
                                      "\nPlayers: " + level.numberOfPeople;

        //lazy check to save 8bits, lol
        if (level.restartingAt != DateTime.MinValue)
        {
            levelSelectorRespawnAt.SetActive(true);
            levelSelectorRespawnAt.GetComponentInChildren<Text>().text = "Respawning at: " + level.restartingAt;
        }
        else
        {
            levelSelectorRespawnAt.SetActive(false);
        }

        if (level.activeTill != DateTime.MinValue)
        {
            levelSelectorActiveTill.SetActive(true);
            levelSelectorActiveTill.GetComponentInChildren<Text>().text = "Active till: " + level.activeTill;
        }
        else
        {
            levelSelectorActiveTill.SetActive(false);
        }

        levelSelectorLevelOutOfLevelText.text = GameManager.CurrentLevel + " / " + GameManager.NumberOfLevels;
    }

    public void HpChange()
    {
            hpContent.fillAmount = (float)GameManager.currentBlockRoom.CurrentBlockHp /
                                            GameManager.currentBlockRoom.ColectiveBlockHp;
            hpText.text = GameManager.currentBlockRoom.CurrentBlockHp + " / " +
                          GameManager.currentBlockRoom.ColectiveBlockHp;
    }

    //true for successfull, false for unsuccessfull
    public void LevelFinished(bool b, List<UserAndDamage> temp)
    {
        blockHp.SetActive(false);
        cameraIngameControll.SetActive(false);

        ranks = temp;

        if(b)
            levelFinishedPanel.SetActive(true);
        else
            levelFailedPanel.SetActive(true);
    }

    public void ReceivedLoot(ref uint currency, ref uint experience, uint lootbox)
    {
        if (currency > 0)
            itemFeed.FeedNewItem(currency, SlotFeeder.RewardType.Currency);

        if (experience > 0)
            itemFeed.FeedNewItem(experience, SlotFeeder.RewardType.Experience);

        if (lootbox > 0)
        {
            itemFeed.FeedNewItem(lootbox, SlotFeeder.RewardType.Lootbox);

            //setting header item to blink
            HandleInventoryUpdate();
        }
    }

    public void HandleInventoryUpdate()
    {
        //set header button to blink the one with lootboxes
        if (gameManager.player.inventory.lootBoxes.Small != 0 ||
        gameManager.player.inventory.lootBoxes.Medium != 0 ||
        gameManager.player.inventory.lootBoxes.Big != 0)
        {
            lootBoxButton.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            lootBoxButton.transform.GetChild(1).gameObject.SetActive(false);
        }
    }

    public void HandleBag()
    {
        inventoryManager.HandleBag();
    }

    public void DisplayRankPanel()
    {
        if (ranks != null)
        {
            //if there are prefabs in rank content already, destroy them
            foreach (Transform tr in rankContent.transform)
            {
                Destroy(tr.gameObject);
            }

            //sort list
            ranks = ranks.OrderByDescending(o => o.Dmg).ToList();

            rankPanel.SetActive(true);

            //spawn new prefabs
            uint i = 1;
            foreach (UserAndDamage obj in ranks)
            {
                GameObject temp = Instantiate(rankElementPrefab, rankContent.transform, false);

                temp.transform.GetChild(0).GetComponent<Text>().text = i + ". " + obj.Username;
                temp.transform.GetChild(1).GetComponent<Text>().text = obj.Dmg.ToString();

                i++;
            }

        }
    }

    #endregion
}
