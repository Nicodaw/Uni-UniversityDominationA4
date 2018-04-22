using System;
using System.Collections.Generic;
using EffectImpl;

public static class CardFactory
{
    #region Private Fields

    /// <summary>
    /// Contains a mapping of <see cref="CardType"/> to the constructor function
    /// for the relavent effect.
    /// </summary>
    readonly static Dictionary<CardType, Func<object[], Effect>> _cardBuilder =
        new Dictionary<CardType, Func<object[], Effect>>
    {
        // tier 1
        { CardType.Graduate,              data => new GraduateEffect() },
        { CardType.AdderallSupply,        data => new ActionIncreaseEffect() },
        { CardType.Kuda,                  data => new UnitStatsEffect(CardType.Kuda) },
        { CardType.Breadcrumbs,           data => new UnitStatsEffect(CardType.Breadcrumbs) },
        { CardType.FirstYearInTheLibrary, data => new UnitStatsEffect(CardType.FirstYearInTheLibrary) },
        { CardType.IndustrialAction,      data => new BlockSectorEffect() },
        { CardType.Hangover,              data => new UnitSkipTurnEffect() },
        { CardType.CopyNotes,             data => new TakeCardEffect()},
        { CardType.DropOut,               data => new SacrificeEffect()},
        // tier 2
        { CardType.NightBeforeExams,      data => new UnitStatsEffect(CardType.NightBeforeExams) },
        { CardType.KudaWithTheLads,       data => new PlayerStatsEffect(CardType.KudaWithTheLads) },
        { CardType.BreadcrumbFactory,     data => new PlayerStatsEffect(CardType.BreadcrumbFactory) },
        { CardType.ArguingOverBars,       data => new PlayerStatsEffect(CardType.ArguingOverBars) },
        { CardType.BadIntentionsSTYC,     data => new PlayerStatsEffect(CardType.BadIntentionsSTYC) },
        { CardType.Resits,                data => new LevelEffect() },
        { CardType.SummerBreak,           data => new PlayerSkipTurnEffect()},
        { CardType.ChristianUnionLeaflet, data => new TemporaryLandmarkEffect()},
        // tier 3
        { CardType.StudentDebt,           data => new DestroyUnitsEffect() }
    };

    /// <summary>
    /// A list of all of the cards that can be selected at random.
    /// </summary>
    readonly static Dictionary<CardTier, CardType[]> _availableRandomPool =
        new Dictionary<CardTier, CardType[]>
    {
        { CardTier.Tier1,
            new []
            {
                CardType.Graduate,
                CardType.Kuda,
                CardType.Breadcrumbs,
                CardType.FirstYearInTheLibrary,
                CardType.NightBeforeExams,
                CardType.IndustrialAction,
                CardType.Hangover,
                CardType.CopyNotes,
                CardType.DropOut,
            }
        },
        { CardTier.Tier2,
            new[]
            {
                CardType.KudaWithTheLads,
                CardType.BreadcrumbFactory,
                CardType.ArguingOverBars,
                CardType.BadIntentionsSTYC,
                CardType.Resits,
                CardType.SummerBreak,
                CardType.ChristianUnionLeaflet,
            }
        },
        { CardTier.Tier3,
            new[]
            {
                CardType.StudentDebt
            }
        },
    };

    #endregion

    #region Factory Methods

    /// <summary>
    /// Creates the effect of the given type.
    /// </summary>
    /// <returns>The effect.</returns>
    /// <param name="type">The type of the effect.</param>
    /// <param name="data">The data the effect might need to be created with.</param>
    public static Effect CreateEffect(CardType type, params object[] data) => _cardBuilder[type](data);

    /// <summary>
    /// Creates a random effect from the allowed pool. Data defaults to empty list.
    /// </summary>
    /// <returns>The random effect.</returns>
    public static Effect GetRandomEffect(CardTier tier) => CreateEffect(_availableRandomPool[tier].Random());

    #endregion
}
