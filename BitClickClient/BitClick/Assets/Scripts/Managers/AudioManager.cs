using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    UIManager uiManager;

    public enum Music
    {
        MainMenu,
        Level
    }

    AudioSource musicPlayer;
    AudioSource playerAudioPlayer;

    public static float sfxVolume = 1;
    public static float musicVolume = 1;
    public static float networkSfxVolume = 1;

    public AudioClip mainMenuMusic;
    public AudioClip levelMusic;

    public AudioClip playerHit;
    public AudioClip playerCrit;

    public AudioClip anotherplayerHit;
    public AudioClip anotherplayerCrit;

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();

        musicPlayer = GetComponent<AudioSource>();

        playerAudioPlayer = transform.GetChild(0).GetComponent<AudioSource>();
    }

    private void Start()
    {
        LoadAudioSettings();
    }

    public void MyPlayerHit(bool isCrit)
    {
        if (isCrit)
        {
            playerAudioPlayer.pitch = Random.Range(1.9f, 2.1f);
            playerAudioPlayer.clip = playerCrit;
        }
        else
        {
            playerAudioPlayer.pitch = Random.Range(1.2f, 1.4f);
            playerAudioPlayer.clip = playerHit;
        }

        playerAudioPlayer.Play();
    }

    bool isPlaying = false;

    public void PlayMusic(Music m)
    {
        AudioClip newMusic = null;
        switch (m)
        {
            case Music.MainMenu:
                if (musicPlayer.clip != mainMenuMusic)
                    newMusic = mainMenuMusic;
                else
                    return;
                break;

            case Music.Level:
                if (musicPlayer.clip != levelMusic)
                    newMusic = levelMusic;
                else
                    return;
                break;
        }

        if (isPlaying)
        {
            StartCoroutine(FadeOut(musicPlayer, 2f, newMusic));
        }
        else
            StartCoroutine(FadeInPlay(musicPlayer, 1f, newMusic));
    }

    public void ApplyAudioSettings()
    {
        musicPlayer.volume = musicVolume;
        playerAudioPlayer.volume = sfxVolume;
    }

    //Options testing gameobject
    GameObject optionTester = null;
    //lazy ass way of stoping the sound from options being played when starting the game
    byte warmUp = 0;

    public void SaveAudioSettings(int param)
    {
        musicVolume = uiManager.GetMusicVolume();
        sfxVolume = uiManager.GetSfxVolume();
        networkSfxVolume = uiManager.GetNetworkSfxVolume();

        PlayerPrefs.SetFloat(Constants.Options["musicVolume"], musicVolume);
        PlayerPrefs.SetFloat(Constants.Options["sfxVolume"], sfxVolume);
        PlayerPrefs.SetFloat(Constants.Options["networkSfxVolume"], sfxVolume);

        ApplyAudioSettings();

        //test sound
        if (param > 0)
        {
            if (optionTester == null)
            {
                optionTester = new GameObject("OptionTester");
                optionTester.AddComponent<AudioSource>();
                optionTester.GetComponent<AudioSource>().playOnAwake = false;
                Destroy(optionTester, 10.0f);
            }
        }

        if (param > 0)
        {
            switch (param)
            {
                case 1:
                    optionTester.GetComponent<AudioSource>().volume = sfxVolume;
                    optionTester.GetComponent<AudioSource>().clip = playerHit;
                    break;
                case 2:
                    optionTester.GetComponent<AudioSource>().volume = networkSfxVolume;
                    optionTester.GetComponent<AudioSource>().clip = anotherplayerHit;
                    break;
            }
            if(!optionTester.GetComponent<AudioSource>().isPlaying && warmUp == 2)
                optionTester.GetComponent<AudioSource>().Play();

            if (warmUp < 2)
                warmUp++;
        }
    }

    public void LoadAudioSettings()
    {
        if (PlayerPrefs.HasKey(Constants.Options["musicVolume"]))
            musicVolume = PlayerPrefs.GetFloat(Constants.Options["musicVolume"]);
        if (PlayerPrefs.HasKey(Constants.Options["sfxVolume"]))
            sfxVolume = PlayerPrefs.GetFloat(Constants.Options["sfxVolume"]);
        if (PlayerPrefs.HasKey(Constants.Options["networkSfxVolume"]))
            networkSfxVolume = PlayerPrefs.GetFloat(Constants.Options["networkSfxVolume"]);
        else
        {
            musicVolume = 1;
            sfxVolume = 1;
            networkSfxVolume = 1;
        }

        uiManager.ReflectAudioManagerScrollBars(musicVolume, sfxVolume, networkSfxVolume);
        ApplyAudioSettings();
    }

    public IEnumerator FadeInPlay(AudioSource audioSource, float FadeTime, AudioClip newMusic)
    {
        audioSource.clip = newMusic;
        audioSource.Play();
        audioSource.volume = 0;

        float startVolume = audioSource.volume;

        while (audioSource.volume < musicVolume)
        {
            audioSource.volume += Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.volume = musicVolume;
        isPlaying = true;
    }

    public IEnumerator FadeOut(AudioSource audioSource, float FadeTime, AudioClip newMusic)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        isPlaying = false;
        audioSource.volume = 0;
        StartCoroutine(FadeInPlay(musicPlayer, 1f, newMusic));
    }
}
