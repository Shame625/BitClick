using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoreInfoDebug : MonoBehaviour
{

    public Text SentOrRec;
    public Text timeStamp;
    public Text PacketType;
    public Text HexDump;
    public Text JsonDump;

    public void ShowInfo(bool SoR, string time, string pType, string hexDump, ref byte[] arr)
    {
        transform.gameObject.SetActive(true);
        //SoR == true ? SentOrRec.text = "Sent" : SentOrRec.text = "Rec";
        timeStamp.text = time;
        PacketType.text = pType;
        HexDump.text = hexDump;
        JsonDump.text = System.Text.Encoding.ASCII.GetString(arr);
    }
}
