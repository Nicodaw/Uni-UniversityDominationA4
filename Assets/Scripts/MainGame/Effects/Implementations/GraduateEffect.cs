using System;

namespace EffectImpl
{
    [Serializable]
    public class GraduateEffect : Effect
    {
        #region Override Properties

        public override string CardName => "Graduate";

        public override string CardDescription => "Upgrade an undergraduate to a postgraduate";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.SelfUnit;

        public override CardTier CardTier => CardTier.Tier1;

        public override int? AttackBonus => 2;

        public override int? DefenceBonus => 2;

        public override int? MoveRangeBonus => 1;

        public override int? LevelCapBonus => 3;

        #endregion

        #region Handlers

        public override void ProcessEffectRemove() => UnGraduateUnit();

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Units = game.CurrentPlayer.Units
        };

        #endregion

        #region Helper Methods

        void GraduateUnit()
        {
            // do graduate add
            AppliedUnit.UsePostGradModel = true;
        }

        void UnGraduateUnit()
        {
            // do graduation remove
            AppliedUnit.UsePostGradModel = false;
        }

        protected override void ApplyToUnit() => GraduateUnit();

        protected override void RestoreUnit() => GraduateUnit();

        #endregion
    }
}
