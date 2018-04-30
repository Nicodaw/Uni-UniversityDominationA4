using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Unity Bindings

    [SerializeField] AudioSource sfxSource1;
    [SerializeField] AudioSource sfxSource2;
    [SerializeField] AudioSource musicSource;

    [SerializeField] AudioClip unitAttack;
    [SerializeField] AudioClip unitMove;
    [SerializeField] AudioClip unitDie;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip cardIn;
    [SerializeField] AudioClip cardOut;
    [SerializeField] AudioClip enemyEffect;
    [SerializeField] AudioClip friendlyEffect;
    [SerializeField] AudioClip sectorEffect;
    [SerializeField] AudioClip sacrificeEffect;
    [SerializeField] AudioClip coinGain;
    [SerializeField] AudioClip groundHit;
    [SerializeField] AudioClip pipeHit;
    [SerializeField] AudioClip wingFlap;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] AudioClip minigameMusic;

    #endregion

    #region Private Fields

    static SoundManager _instance;
    bool _musicIsPlaying = true;


    #endregion

    #region Private Properties

    #endregion

    #region Public Properties

    /// <summary>
    /// The current sound manager instance.
    /// </summary>
    public static SoundManager Instance => _instance;

    public bool IsMusicOn => _musicIsPlaying;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Helper Methods
    public void PlaySingle(Sound sound)
    {
        switch ((int)sound)
        {
            case 0:
                Play(unitAttack);
                break;
            case 1:
                Play(unitMove);
                break;
            case 2:
                Play(unitDie);
                break;
            case 3:
                Play(buttonClick);
                break;
            case 4:
                Play(cardIn);
                break;
            case 5:
                Play(cardOut);
                break;
            case 6:
                Play(enemyEffect);
                break;
            case 7:
                Play(friendlyEffect);
                break;
            case 8:
                Play(sectorEffect);
                break;
            case 9:
                Play(sacrificeEffect);
                break;
            case 10:
                Play(coinGain);
                break;
            case 11:
                Play(groundHit);
                break;
            case 12:
                Play(pipeHit);
                break;
            case 13:
                Play(wingFlap);
                break;
            default:
                throw new InvalidOperationException("Sound clip doesn't exist. If you're trying to play music, use PlayMusic(Sound s)");
        }
    }

    public void PlayMusic(Sound sound)
    {
        switch (sound)
        {

            case Sound.MainMenuMusic:
                PlayMusic(menuMusic);
                break;
            case Sound.MainGameMusic:
                PlayMusic(gameMusic);
                break;
            case Sound.MiniGameMusic:
                PlayMusic(minigameMusic);
                break;
            default:
                throw new InvalidOperationException("Music clip doesn't exist. If you're trying to play a sound, use PlaySingle(Sound s)");
        }
    }

    public void ToggleMusic()
    {
        if (_musicIsPlaying)
        {
            musicSource.Pause();
            _musicIsPlaying = false;
        }
        else
        {
            musicSource.UnPause();
            _musicIsPlaying = true;
        }
    }

    void Play(AudioClip clip)
    {
        if (sfxSource1.isPlaying)
        {
            sfxSource2.clip = clip;
            sfxSource2.Play();
            StartCoroutine(ClearSoundBuffer(2));
        }
        else
        {
            sfxSource1.clip = clip;
            sfxSource1.Play();
            StartCoroutine(ClearSoundBuffer(1));
        }
    }

    void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    IEnumerator ClearSoundBuffer(int src)
    {
        AudioSource source = (src == 1) ? sfxSource1 : sfxSource2;
        yield return new WaitWhile(() => source.isPlaying);
        source.clip = null;
    }

    #endregion
}
