using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingDamageText : MonoBehaviour
{
    public AnimationClip normal;
    public AnimationClip crit;

    public void SetDamage(ulong Damage, bool isCrit)
    {
        //transform.LookAt(new Vector3(Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z));

        if (isCrit)
        {
            GetComponent<Animation>().clip = crit;
            GetComponentInChildren<TextMesh>().color = Color.red;
        }
        else
        {
            GetComponent<Animation>().clip = normal;
        }
        

        GetComponentInChildren<TextMesh>().text = Damage.ToString();

        GetComponent<Animation>().Play();
        Destroy(gameObject, GetComponent<Animation>().clip.length);
    }
}
