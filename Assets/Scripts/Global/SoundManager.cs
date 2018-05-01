using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    #region Unity Bindings

    //AudioSources part of our buffer for sfx
    [SerializeField] AudioSource sfxSource;
    //AudioSource for music
    [SerializeField] AudioSource musicSource;

    [SerializeField] AudioClip unitAttack;
    [SerializeField] AudioClip unitMove;
    [SerializeField] AudioClip unitDie;
    [SerializeField] AudioClip buttonClick;
    [SerializeField] AudioClip winSound;
    [SerializeField] AudioClip defeatSound;
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
    }
    #endregion

    #region Helper Methods
    public void PlaySingle(Sound sound)
    {
        switch (sound)
        {
            case Sound.UnitAttackSound:
                Play(unitAttack);
                break;
            case Sound.UnitMoveSound:
                Play(unitMove);
                break;
            case Sound.UnitDieSound:
                Play(unitDie);
                break;
            case Sound.UIButtonClickSound:
                Play(buttonClick);
                break;
            case Sound.WinnerSound:
                Play(winSound);
                break;
            case Sound.PlayerDefeatSound:
                Play(defeatSound);
                break;
            case Sound.CardInSound:
                Play(cardIn);
                break;
            case Sound.CardOutSound:
                Play(cardOut);
                break;
            case Sound.EnemyEffectSound:
                Play(enemyEffect);
                break;
            case Sound.FriendlyEffectSound:
                Play(friendlyEffect);
                break;
            case Sound.SectorEffect:
                Play(sectorEffect);
                break;
            case Sound.SacrificeSound:
                Play(sacrificeEffect);
                break;
            case Sound.CoinGainSound:
                Play(coinGain);
                break;
            case Sound.GroundHitSound:
                Play(groundHit);
                break;
            case Sound.PipeHitSound:
                Play(pipeHit);
                break;
            case Sound.WingFlapSound:
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

    void Play(AudioClip clip, float delay = 0)
    {
        sfxSource.PlayOneShot(clip);
    }

    void PlayMusic(AudioClip music)
    {
        musicSource.clip = music;
        musicSource.Play();
    }

    void StopAllMusic()
    {
        musicSource.volume = 0f;
    }
    void StopAllsfx()
    {
        sfxSource.volume = 0f;
    }
    #endregion
}
