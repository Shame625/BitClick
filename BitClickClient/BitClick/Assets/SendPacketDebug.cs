using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendPacketDebug : MonoBehaviour
{
    public NetworkHelper networkHelper;

    public InputField param1;
    public InputField param2;
    public InputField param3;
    public InputField param4;


    public void Send()
    {
        networkHelper.SwapItems(byte.Parse(param1.text), int.Parse(param2.text),
            byte.Parse(param3.text), int.Parse(param4.text)
            );
    }
}
