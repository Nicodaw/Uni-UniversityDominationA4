using System;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Unity Bindings

    [Header("Sources")]
    [SerializeField] AudioSource m_sfxSource;
    [SerializeField] AudioSource m_musicSource;

    [Header("Clips")]
    [SerializeField] AudioClip m_unitAttack;
    [SerializeField] AudioClip m_unitMove;
    [SerializeField] AudioClip m_unitDie;
    [SerializeField] AudioClip m_buttonClick;
    [SerializeField] AudioClip m_winSound;
    [SerializeField] AudioClip m_defeatSound;
    [SerializeField] AudioClip m_cardIn;
    [SerializeField] AudioClip m_cardOut;
    [SerializeField] AudioClip m_enemyEffect;
    [SerializeField] AudioClip m_friendlyEffect;
    [SerializeField] AudioClip m_sectorEffect;
    [SerializeField] AudioClip m_sacrificeEffect;
    [SerializeField] AudioClip m_coinGain;
    [SerializeField] AudioClip m_groundHit;
    [SerializeField] AudioClip m_pipeHit;
    [SerializeField] AudioClip m_wingFlap;
    [SerializeField] AudioClip m_menuMusic;
    [SerializeField] AudioClip m_gameMusic;
    [SerializeField] AudioClip m_minigameMusic;

    #endregion

    #region Private Fields

    static SoundManager _instance;
    bool _musicPlaying;
    bool _soundEffectsPlaying;

    #endregion

    #region Public Properties

    public bool MusicPlaying
    {
        get { return _musicPlaying; }
        set
        {
            _musicPlaying = value;
            PlayerPrefs.SetInt("_musicPlaying", _musicPlaying ? 1 : 0);
            PlayerPrefs.Save();
            if (_musicPlaying)
                m_musicSource.volume = 1f;
            else
                m_musicSource.volume = 0f;
        }
    }

    public bool SoundEffectsPlaying
    {
        get { return _soundEffectsPlaying; }
        set
        {
            _soundEffectsPlaying = value;
            PlayerPrefs.SetInt("_soundEffectsPlaying", _soundEffectsPlaying ? 1 : 0);
            PlayerPrefs.Save();
            if (_soundEffectsPlaying)
                m_sfxSource.volume = 1f;
            else
                m_sfxSource.volume = 0f;
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// The current sound manager instance.
    /// </summary>
    public static SoundManager Instance => _instance;

    public bool IsMusicOn => _musicPlaying;

    #endregion

    #region MonoBehaviour

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            MusicPlaying = PlayerPrefs.GetInt("_musicPlaying", 1) == 1;
            SoundEffectsPlaying = PlayerPrefs.GetInt("_soundEffectsPlaying", 1) == 1;
        }
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
                Play(m_unitAttack);
                break;
            case Sound.UnitMoveSound:
                Play(m_unitMove);
                break;
            case Sound.UnitDieSound:
                Play(m_unitDie);
                break;
            case Sound.UIButtonClickSound:
                Play(m_buttonClick);
                break;
            case Sound.WinnerSound:
                Play(m_winSound);
                break;
            case Sound.PlayerDefeatSound:
                Play(m_defeatSound);
                break;
            case Sound.CardInSound:
                Play(m_cardIn);
                break;
            case Sound.CardOutSound:
                Play(m_cardOut);
                break;
            case Sound.EnemyEffectSound:
                Play(m_enemyEffect);
                break;
            case Sound.FriendlyEffectSound:
                Play(m_friendlyEffect);
                break;
            case Sound.SectorEffect:
                Play(m_sectorEffect);
                break;
            case Sound.SacrificeSound:
                Play(m_sacrificeEffect);
                break;
            case Sound.CoinGainSound:
                Play(m_coinGain);
                break;
            case Sound.GroundHitSound:
                Play(m_groundHit);
                break;
            case Sound.PipeHitSound:
                Play(m_pipeHit);
                break;
            case Sound.WingFlapSound:
                Play(m_wingFlap);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    public void PlayMusic(Sound sound)
    {
        switch (sound)
        {

            case Sound.MainMenuMusic:
                PlayMusic(m_menuMusic);
                break;
            case Sound.MainGameMusic:
                PlayMusic(m_gameMusic);
                break;
            case Sound.MiniGameMusic:
                PlayMusic(m_minigameMusic);
                break;
            default:
                throw new InvalidOperationException();
        }
    }

    void Play(AudioClip clip)
    {
        if (SoundEffectsPlaying)
        m_sfxSource.PlayOneShot(clip);
    }

    void PlayMusic(AudioClip music)
    {
        m_musicSource.clip = music;
        m_musicSource.Play();
    }

    public void UIClick() => PlaySingle(Sound.UIButtonClickSound);

    #endregion
}
