using System.Collections;
using System.Collections.Generic;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public sealed class NetworkDebugger : MonoBehaviour
{
    public GameObject entirePanel;

    public GameObject packetElementPrefab;
    public GameObject packetHistory;
    public Scrollbar packetHistoryScrollbar;

    public GameObject popUp;

    public static bool isEnabled = true;

    public void AddPacketToHistory(bool isSent, ref byte[] bytes, ref Package packet)
    {
        if (isEnabled)
        {
            GameObject temp = Instantiate(packetElementPrefab, packetHistory.transform, false);
            temp.GetComponent<PacketElement>().SetObjectData(isSent, ref bytes, ref packet);

            packetHistoryScrollbar.value = 0.0f;
        }
    }
}
