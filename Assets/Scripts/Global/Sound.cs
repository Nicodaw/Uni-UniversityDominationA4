/// <summary>
/// Enumarate all sounds we have in the game
/// </summary>
public enum Sound
{
    //Unit interaction
    UnitAttackSound,
    UnitMoveSound,
    UnitDieSound,

    //UI interaction
    UIButtonClickSound,

    //Cards and effects
    CardInSound,
    CardOutSound,
    EnemyEffectSound,
    FriendlyEffectSound,
    SectorEffect,
    SacrificeSound,
    
    //Minigame
    CoinGainSound,
    GroundHitSound,
    PipeHitSound,
    WingFlapSound,

    //Music
    MainMenuMusic, //FIND APPROPRIATE HEARTHSTONE BANGER
    MainGameMusic,
    MiniGameMusic
}
