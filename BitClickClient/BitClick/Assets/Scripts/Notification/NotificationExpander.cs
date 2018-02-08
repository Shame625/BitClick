using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationExpander : MonoBehaviour {

    Text ShowOrHideButtonText;
    Animation myAnimation;

    public AnimationClip expand;
    public AnimationClip minimize;

    bool isEpaneded = false;

    private void Awake()
    {
        myAnimation = GetComponent<Animation>();
        ShowOrHideButtonText = transform.GetChild(0).GetComponentInChildren<Text>();
    }

    public void ShowOrHide()
    {
        if(!isEpaneded)
        {
            isEpaneded = true;
            ShowOrHideButtonText.text = "<";
            myAnimation.clip = expand;
            
        }
        else
        {
            isEpaneded = false;
            ShowOrHideButtonText.text = ">";
            myAnimation.clip = minimize;
        }
        myAnimation.Play();
    }

}
