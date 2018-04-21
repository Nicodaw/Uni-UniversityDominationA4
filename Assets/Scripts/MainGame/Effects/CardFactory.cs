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
        { CardType.Graduate,              data => new GraduateEffect() },
        { CardType.AdderallSupply,        data => new ActionIncreaseEffect() },
        { CardType.Kuda,                  data => new UnitStatsEffect(CardType.Kuda) },
        { CardType.Breadcrumbs,           data => new UnitStatsEffect(CardType.Breadcrumbs) },
        { CardType.FirstYearInTheLibrary, data => new UnitStatsEffect(CardType.FirstYearInTheLibrary) },
        { CardType.NightBeforeExams,      data => new UnitStatsEffect(CardType.NightBeforeExams) },    //tier 2
        { CardType.KudaWithTheLads,       data => new PlayerStatsEffect(CardType.KudaWithTheLads) } ,  //tier 2
        { CardType.BreadcrumbFactory,     data => new PlayerStatsEffect(CardType.BreadcrumbFactory) }, //tier 2
        { CardType.ArguingOverBars,       data => new PlayerStatsEffect(CardType.ArguingOverBars) },   //tier 2
        { CardType.BadIntentionsSTYC,     data => new PlayerStatsEffect(CardType.BadIntentionsSTYC) }, //tier 2
        { CardType.Resits,                data => new LevelEffect()},
        { CardType.IndustrialAction,      data => new BlockSectorEffect() },
        { CardType.Hangover,              data => new UnitSkipTurnEffect() }
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
                CardType.KudaWithTheLads,
                CardType.BreadcrumbFactory,
                CardType.ArguingOverBars,
                CardType.BadIntentionsSTYC,
                CardType.Resits,
                CardType.IndustrialAction,
                CardType.Hangover,
                CardType.SummerBreak,
                CardType.CopyNotes,
                CardType.ChristianUnionLeaflet,
                CardType.DropOut,
                CardType.StudentDebt
            }
        }
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
