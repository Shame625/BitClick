using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BitObj : MonoBehaviour
{
    public uint Id;

    private Transform childTransform;
    private Vector3 newScale;

    AudioSource audioSource;
    AudioClip Hit;
    AudioClip Crit;

    private bool doLerp = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        childTransform = transform.GetChild(0);
    }

    private void Update()
    {
        if (doLerp)
        {
            childTransform.localScale = Vector3.Lerp(childTransform.localScale, newScale, 0.1f);

            doLerp = (childTransform.localScale != newScale);
        }
    }

    public void SetData(ref uint id, AudioClip h, AudioClip c)
    {
        Id = id;
        Hit = h;
        Crit = c;
    }

    public void ReduceSize(float scale)
    {
        newScale = new Vector3(scale, scale, scale);
        doLerp = true;
    }

    public void DeathSequence()
    {
        doLerp = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponentInChildren<Animation>().Play();
    }

    public void TakeDamageSound(bool isCrit)
    {
        audioSource.volume = AudioManager.networkSfxVolume;
        if(isCrit)
        {
            audioSource.pitch = Random.Range(1f, 1.3f);
            audioSource.clip = Crit;
        }
        else
        {
            audioSource.pitch = Random.Range(0.5f, 0.7f);
            audioSource.clip = Hit;
        }
        audioSource.Play();
    }
}
