#if UNITY_EDITOR
using NUnit.Framework;

/*
 * The default values are not checked by these tests, since they are entirely
 * balance dependant, and so don't account for testing.
 */

public class EffectManagerTest : BaseGameTest
{
    /// <summary>
    /// Gets the default <see cref="T:EffectManager"/> object.
    /// </summary>
    /// <remarks>
    /// Players default to no stats and are the easiest to access, which is
    /// why we use the first player's stats for this.
    /// </remarks>
    EffectManager stats => ((Player)statsObj).Stats;

    UnityEngine.Object statsObj => Players[0];

    [SetUp]
    public void EffectManagerTest_SetUp()
    {
        DefaultPlayerInit();
        game.LoadMapObject();
    }

    [Test]
    public void Attack_ValueCorrect()
    {
        Assume.That(stats.EffectCount, Is.Zero);
        int baseAtt = stats.Attack;

        stats.ApplyEffect(new CustomEffect { attackBonus = 2 });
        Assert.That(stats.Attack, Is.EqualTo(baseAtt + 2));
        stats.ApplyEffect(new CustomEffect { attackBonus = 3 });
        Assert.That(stats.Attack, Is.EqualTo(baseAtt + 2 + 3));
        stats.ApplyEffect(new CustomEffect { attackBonus = -1 });
        Assert.That(stats.Attack, Is.EqualTo(baseAtt + 2 + 3 - 1));
    }

    [Test]
    public void Defence_ValueCorrect()
    {
        Assume.That(stats.EffectCount, Is.Zero);
        int baseDef = stats.Defence;

        stats.ApplyEffect(new CustomEffect { defenceBonus = 3 });
        Assert.That(stats.Defence, Is.EqualTo(baseDef + 3));
        stats.ApplyEffect(new CustomEffect { defenceBonus = 5 });
        Assert.That(stats.Defence, Is.EqualTo(baseDef + 3 + 5));
        stats.ApplyEffect(new CustomEffect { defenceBonus = -2 });
        Assert.That(stats.Defence, Is.EqualTo(baseDef + 3 + 5 - 2));
    }

    [Test]
    public void Actions_ValueCorrect()
    {
        Assume.That(stats.EffectCount, Is.Zero);
        int baseAct = stats.Actions;

        stats.ApplyEffect(new CustomEffect { actionBonus = 2 });
        Assert.That(stats.Actions, Is.EqualTo(baseAct + 2));
        stats.ApplyEffect(new CustomEffect { actionBonus = 4 });
        Assert.That(stats.Actions, Is.EqualTo(baseAct + 2 + 4));
        stats.ApplyEffect(new CustomEffect { actionBonus = -3 });
        Assert.That(stats.Actions, Is.EqualTo(baseAct + 2 + 4 - 3));
    }

    [Test]
    public void Traversable_ValueCorrect()
    {
        // traversable can only default to true
        // and if any effects give false, the entire thing becomes false regardless
        Assume.That(stats.EffectCount, Is.Zero);
        Assume.That(stats.Traversable, Is.True);

        stats.ApplyEffect(new CustomEffect { traversable = true });
        Assert.That(stats.Traversable, Is.True);
        stats.ApplyEffect(new CustomEffect { traversable = false });
        Assert.That(stats.Traversable, Is.False);
        stats.ApplyEffect(new CustomEffect { traversable = true });
        Assert.That(stats.Traversable, Is.False);
        stats.ApplyEffect(new CustomEffect { traversable = false });
        Assert.That(stats.Traversable, Is.False);
    }

    [Test]
    public void MoveRange_ValueCorrect()
    {
        Assume.That(stats.EffectCount, Is.Zero);
        int baseMove = stats.MoveRange;

        stats.ApplyEffect(new CustomEffect { moveRangeBonus = 1 });
        Assert.That(stats.MoveRange, Is.EqualTo(baseMove + 1));
        stats.ApplyEffect(new CustomEffect { moveRangeBonus = 4 });
        Assert.That(stats.MoveRange, Is.EqualTo(baseMove + 1 + 4));
        stats.ApplyEffect(new CustomEffect { moveRangeBonus = -2 });
        Assert.That(stats.MoveRange, Is.EqualTo(baseMove + 1 + 4 - 2));
    }

    [Test]
    public void LevelCap_ValueCorrect()
    {
        Assume.That(stats.EffectCount, Is.Zero);
        int baseCap = stats.LevelCap;

        stats.ApplyEffect(new CustomEffect { levelCapBonus = 3 });
        Assert.That(stats.LevelCap, Is.EqualTo(baseCap + 3));
        stats.ApplyEffect(new CustomEffect { levelCapBonus = 2 });
        Assert.That(stats.LevelCap, Is.EqualTo(baseCap + 3 + 2));
        stats.ApplyEffect(new CustomEffect { levelCapBonus = -4 });
        Assert.That(stats.LevelCap, Is.EqualTo(baseCap + 3 + 2 - 4));
    }

    [Test]
    public void ApplyEffect_Applies()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        CustomEffect eff = new CustomEffect();
        stats.ApplyEffect(eff);
        Assert.That(eff.Id, Is.Zero);
        Assert.That(eff.AppliedManager, Is.EqualTo(stats));
        Assert.That(eff.appliedObject, Is.EqualTo(statsObj));
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        eff = new CustomEffect();
        stats.ApplyEffect(eff);
        Assert.That(eff.Id, Is.EqualTo(1));
        Assert.That(eff.AppliedManager, Is.EqualTo(stats));
        Assert.That(eff.appliedObject, Is.EqualTo(statsObj));
        Assert.That(stats.EffectCount, Is.EqualTo(2));
        eff = new CustomEffect();
        stats.ApplyEffect(eff);
        Assert.That(eff.Id, Is.EqualTo(2));
        Assert.That(eff.AppliedManager, Is.EqualTo(stats));
        Assert.That(eff.appliedObject, Is.EqualTo(statsObj));
        Assert.That(stats.EffectCount, Is.EqualTo(3));
    }

    [Test]
    public void HasEffect_Correct()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Assert.That(stats.HasEffect<Effect>(), Is.False);
        Assert.That(stats.HasEffect<CustomEffect>(), Is.False);
        Assert.That(stats.HasEffect<CustomSecondaryEffect>(), Is.False);
        stats.ApplyEffect(new CustomEffect());
        Assert.That(stats.HasEffect<Effect>(), Is.True);
        Assert.That(stats.HasEffect<CustomEffect>(), Is.True);
        Assert.That(stats.HasEffect<CustomSecondaryEffect>(), Is.False);
        stats.ApplyEffect(new CustomEffect());
        Assert.That(stats.HasEffect<Effect>(), Is.True);
        Assert.That(stats.HasEffect<CustomEffect>(), Is.True);
        Assert.That(stats.HasEffect<CustomSecondaryEffect>(), Is.False);
        stats.ApplyEffect(new CustomSecondaryEffect());
        Assert.That(stats.HasEffect<Effect>(), Is.True);
        Assert.That(stats.HasEffect<CustomEffect>(), Is.True);
        Assert.That(stats.HasEffect<CustomSecondaryEffect>(), Is.True);
    }

    void InitEffects(out Effect eff1, out Effect eff2, out Effect eff3)
    {
        eff1 = new CustomEffect();
        eff2 = new CustomEffect();
        eff3 = new CustomSecondaryEffect();
    }

    [Test]
    public void RemoveEffect_Existing()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Effect eff1;
        Effect eff2;
        Effect eff3;

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff1);
        Assert.That(stats.EffectCount, Is.Zero);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect(eff1);
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff2);
        Assert.That(stats.EffectCount, Is.Zero);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveEffect(eff2);
        Assert.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect(eff3);
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff1);
        Assert.That(stats.EffectCount, Is.Zero);
    }

    [Test]
    public void RemoveEffect_NotExisting()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Effect eff1;
        Effect eff2;
        Effect eff3;

        InitEffects(out eff1, out eff2, out eff3);
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);

        stats.ApplyEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff1);
        Assume.That(stats.EffectCount, Is.Zero);
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);
        stats.RemoveEffect(eff2);
        Assume.That(stats.EffectCount, Is.Zero);
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);
        Assert.That(() => stats.RemoveEffect(eff2), Throws.ArgumentException);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);
        stats.RemoveEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);
        Assert.That(() => stats.RemoveEffect(eff3), Throws.ArgumentException);
        stats.RemoveEffect(eff2);
        Assume.That(stats.EffectCount, Is.Zero);
        Assert.That(() => stats.RemoveEffect(eff1), Throws.ArgumentException);
        Assert.That(() => stats.RemoveEffect(eff2), Throws.ArgumentException);
        Assert.That(() => stats.RemoveEffect(eff3), Throws.ArgumentException);
    }

    [Test]
    public void RemoveEffect_TypeExisting()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Effect eff1;
        Effect eff2;
        Effect eff3;

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect<CustomEffect>();
        Assert.That(stats.EffectCount, Is.Zero);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect<CustomEffect>();
        Assert.That(stats.EffectCount, Is.Zero);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveEffect<CustomEffect>();
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff3);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveEffect<CustomSecondaryEffect>();
        Assert.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect(eff1);
        stats.RemoveEffect(eff2);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveEffect<Effect>();
        Assert.That(stats.EffectCount, Is.Zero);
    }

    [Test]
    public void RemoveEffect_TypeNotExisting()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Effect eff1;
        Effect eff2;
        Effect eff3;

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect<CustomSecondaryEffect>();
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff1);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect<CustomSecondaryEffect>();
        Assert.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveEffect(eff1);
        stats.RemoveEffect(eff2);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect<CustomEffect>();
        Assert.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveEffect(eff3);

        Assume.That(stats.EffectCount, Is.Zero);
        stats.RemoveEffect<Effect>();
        Assert.That(stats.EffectCount, Is.Zero);
    }

    [Test]
    public void RemoveAllEffects_AllRemoved()
    {
        Assume.That(stats.EffectCount, Is.Zero);

        Effect eff1;
        Effect eff2;
        Effect eff3;

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        Assume.That(stats.EffectCount, Is.EqualTo(1));
        stats.RemoveAllEffects();
        Assert.That(stats.EffectCount, Is.Zero);

        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveAllEffects();
        Assert.That(stats.EffectCount, Is.Zero);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(2));
        stats.RemoveAllEffects();
        Assert.That(stats.EffectCount, Is.Zero);

        Assume.That(stats.EffectCount, Is.Zero);
        InitEffects(out eff1, out eff2, out eff3);
        stats.ApplyEffect(eff1);
        stats.ApplyEffect(eff2);
        stats.ApplyEffect(eff3);
        Assume.That(stats.EffectCount, Is.EqualTo(3));
        stats.RemoveAllEffects();
        Assert.That(stats.EffectCount, Is.Zero);
    }
}
#endif
