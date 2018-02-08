using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
	//Global variables
	public static int? clientId;
	public static string userName;

	public static uint CurrentLevel = 1;
	public static uint NumberOfLevels = 1;

	public static DateTime activeTill;

	public Player player;
	private NetworkManager networkManager;
	public LevelManager levelGenerator;

    public LootboxReward lootboxReward;

	public GameObject background;
	public BackgroundControler bgController;

	//only 1 block room allowed
	public static BlockRoom currentBlockRoom;

	public static bool inGame;

	private void Awake()
	{
		Application.runInBackground = true;

		//loads all the items from the cache
		CacheManager.LoadCache();

		networkManager = GetComponent<NetworkManager>();
		levelGenerator = GetComponent<LevelManager>();
		bgController = background.GetComponent<BackgroundControler>();
		player = GetComponent<Player>();
		inGame = false;
	}

	void Start ()
	{
		networkManager.Connect();
	}

	public void EnterLevel(ref BlockRoom bl)
	{
		inGame = true;

		currentBlockRoom = bl;
		levelGenerator.GenerateLevel(ref bl);
	}

	public void LeaveLevel()
	{
		inGame = false;

		bgController.SetMainMenu();
		currentBlockRoom = null;
		levelGenerator.Dispose();
	}

    public void DisplayRewardsLootbox(Item[] items)
    {
        lootboxReward.gameObject.SetActive(true);
        lootboxReward.LootboxOpened(items);
    }
}
