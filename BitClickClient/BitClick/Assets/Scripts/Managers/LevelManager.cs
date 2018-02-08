using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //this object holds all "subobjects"
    private UIManager uiManager;
    private AudioManager audioManager;
    private GameManager gameManager;

    public GameObject mainObject;

    public GameObject bitPrefab;
    public GameObject hitPrefab;
    public GameObject hitCritPrefab;

    public GameObject scrollingCombatText;

    private Dictionary<uint, GameObject> objectManager;

    
    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        uiManager = GetComponent<UIManager>();
        audioManager = GetComponent<AudioManager>();
    }

    public void GenerateLevel(ref BlockRoom bl)
    {
        objectManager = new Dictionary<uint, GameObject>();
        mainObject.transform.rotation = Quaternion.identity;

        SetLevelTheme(bl.Level);

        //Reset bit ID counter
        uint IdCounter = 0;
        //If there is anything in mainObject, delete it.
        foreach (Transform go in mainObject.transform)
        {
            Destroy(go.gameObject);
        }

        //get block size
        uint blockSize = Constants.GET_BLOCK_SIZE(bl.Level);

        //calculate hp
        GameManager.currentBlockRoom._bitHp = Constants.GET_BIT_HP(bl.Level);
        bl.ColectiveBlockHp = (blockSize * blockSize * blockSize) * Constants.GET_BIT_HP(bl.Level);

        mainObject.transform.position = new Vector3((float)blockSize/2 -0.5f, (float)blockSize/2 - 0.5f, (float)blockSize/2 -0.5f);

        Camera.main.transform.position = new Vector3(mainObject.transform.position.x, mainObject.transform.position.y, -15);
        Camera.main.transform.LookAt(mainObject.transform);

        mainObject.GetComponent<BoxCollider>().size = new Vector3(blockSize + 0.2f, blockSize +0.2f, blockSize + 0.2f);
        //mainObject.GetComponent<BoxCollider>().center = new Vector3(-0.5f, 0, -0.5f);

        for (int x = 0; x < blockSize; x++)
        {
            for (int y = 0; y < blockSize; y++)
            {
                for (int z = 0; z < blockSize; z++)
                {
                    //Dont spawn block if its dead

                    if (bl.Block._block[IdCounter]._health > 0)
                    {
                        bl.CurrentBlockHp += bl.Block._block[IdCounter]._health;

                        GameObject temp = Instantiate(bitPrefab, new Vector3(x, y, z), Quaternion.identity);
                        temp.transform.SetParent(mainObject.transform);

                        //sets id of block, and sets theirs audioclips
                        temp.GetComponent<BitObj>().SetData(ref IdCounter, audioManager.anotherplayerHit, audioManager.anotherplayerCrit);

                        if (bl.Block._block[IdCounter]._health != bl._bitHp)
                            ObjectTookDamageEffect(bl.Block._block[IdCounter], temp);

                            objectManager.Add(IdCounter, temp);
                    }
                    IdCounter++;
                }
            }
        }
        uiManager.HpChange();
    }

    public void HandleBitHpChange(ref uint Id, ref ulong newHp, ref ulong Damage, ref bool isCrit, ref int clientId)
    {
        //using try, cos if by some reason ID is out of bounds we dont get shit
        try
        {
            //Handle ghost bit
            if(newHp == 0 && Damage == 0)
            {
                //play animation
                objectManager[Id].GetComponent<BitObj>().DeathSequence();
                //destroy object afterwards
                Destroy(objectManager[Id].gameObject, objectManager[Id].gameObject.GetComponentInChildren<Animation>().clip.length + 0.01f);
                return;
            }
            Bit currentBit = GameManager.currentBlockRoom.Block._block[Id];

            //if its my client, dispaly scrolling combat text and call my unique audio source to play my hit
            if (clientId == GameManager.clientId)
            {
                GameObject damageObj = Instantiate(scrollingCombatText, objectManager[Id].transform.position, scrollingCombatText.transform.rotation);
                damageObj.GetComponent<ScrollingDamageText>().SetDamage(Damage, isCrit);

                audioManager.MyPlayerHit(isCrit);
            }
            else
            {
                //else play another player hit
                objectManager[Id].GetComponent<BitObj>().TakeDamageSound(isCrit);
            }

            ulong Difference = currentBit._health - newHp;
            GameManager.currentBlockRoom.CurrentBlockHp -= Difference;

            currentBit.SetHP(newHp);

            BitTookHit(objectManager[Id].transform.position, isCrit);
            ObjectTookDamageEffect(currentBit, objectManager[Id].gameObject);

            uiManager.HpChange();

            //change with animation or 3d animation sequence
            if (!currentBit.isAlive)
            {
                //play animation
                objectManager[Id].GetComponent<BitObj>().DeathSequence();
                //destroy object afterwards
                Destroy(objectManager[Id].gameObject, objectManager[Id].gameObject.GetComponentInChildren<Animation>().clip.length + 0.01f);
            }
        }
        catch
        {
            //no key in dictionary
        }
    }

    void BitTookHit(Vector3 location, bool isCrit)
    {
        GameObject temp = Instantiate(!isCrit ? hitPrefab : hitCritPrefab, location, Quaternion.identity);

        Destroy(temp, 0.71f);
    }

    public void ObjectTookDamageEffect(Bit bit, GameObject go)
    {
        //Handle some animation in here 
        float scaleDiff = (float) bit._health / GameManager.currentBlockRoom._bitHp;

        if (scaleDiff < 0.2f)
            scaleDiff = 0.2f;

        go.GetComponent<BitObj>().ReduceSize(scaleDiff);
    }

    public void Dispose()
    {
        objectManager = null;

        foreach (Transform go in mainObject.transform)
        {
            Destroy(go.gameObject);
        }
    }

    void SetLevelTheme(uint level)
    {
        gameManager.bgController.SetTexture(AssetManager.levelThemes[(int)Constants.GET_LEVEL_THEME(level)]);
    }

    //modify later to contain block


}
