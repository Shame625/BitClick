using System.Text;
using Assets;
using UnityEngine;
using UnityEngine.UI;

public class PacketElement : MonoBehaviour
{
    public static Color colorSent = new Color(0.212f, 0.557f, 0.243f, 1);
    public static Color colorRec = new Color(0.537f, 0.204f, 0.204f, 1);

    private bool sentOrRec;
    private Text SentOrRec;
    private Text timeStamp;
    private Text Type;
    private Text bytes;
    private byte[] arr;

    private GameObject popUp;

    private void Awake()
    {
        SentOrRec = transform.GetChild(0).GetComponent<Text>();
        timeStamp = transform.GetChild(1).GetComponent<Text>();
        Type = transform.GetChild(2).GetComponent<Text>();
        bytes = transform.GetChild(3).GetComponent<Text>();
    }

    void Start()
    {
        popUp = GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkDebugger>().popUp;
    }

    public void SetObjectData(bool SoR, ref byte[] data, ref Package packet)
    {
        sentOrRec = SoR;
        timeStamp.text = System.DateTime.Now.ToLongTimeString();

        if (SoR)
        {
            SentOrRec.text = "SENT";
            GetComponent<Image>().color = colorSent;
        }
        else
        {
            SentOrRec.text = "REC";
            GetComponent<Image>().color = colorRec;
        }

        Type.text = packet.Type.ToString();
        arr = data;

        bytes.text = PrintBytes(ref data);
    }

    public void Click()
    {
        popUp.SetActive(true);
        popUp.GetComponent<MoreInfoDebug>().ShowInfo(sentOrRec, timeStamp.text, Type.text, bytes.text, ref arr);
    }

    //Helper function yeey
    public static string PrintBytes(ref byte[] byteArray)
    {
        var sb = new StringBuilder("Packet: { ");
        for (var i = 0; i < byteArray.Length; i++)
        {
            var b = byteArray[i];
            sb.Append(b.ToString("X"));
            if (i < byteArray.Length - 1)
            {
                sb.Append(", ");
            }
        }
        sb.Append(" }");
        return sb.ToString();
    }
}

