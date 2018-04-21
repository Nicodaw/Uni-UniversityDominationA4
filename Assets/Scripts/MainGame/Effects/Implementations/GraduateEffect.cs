using System;
using UnityEditor;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class GraduateEffect : Effect
    {
        #region Private Fields

        GameObject _postgradModel = GameObject.FindGameObjectWithTag("Postgrad");
        GameObject _undergradModel = GameObject.FindGameObjectWithTag("Undergrad");
        Unit _target;

        #endregion

        #region Override Properties

        public override string CardName => "Graduate";

        public override string CardDescription => "Upgrade an undergraduate to a postgraduate";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.SelfUnit;

        public override CardTier CardTier => CardTier.Tier1;

        public override int? AttackBonus => 2;

        public override int? DefenceBonus => 2;

        public override int? MoveRangeBonus => 2;

        public override int? LevelCapBonus => 8;

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

        void GraduateUnit(Unit unit)
        {
            // do graduate add
            _target = unit;
            PrefabUtility.ReplacePrefab(_postgradModel, unit.gameObject, ReplacePrefabOptions.ConnectToPrefab);
        }

        void UnGraduateUnit()
        {
            // do graduation remove
            PrefabUtility.ReplacePrefab(_undergradModel, _target.gameObject, ReplacePrefabOptions.ConnectToPrefab);
        }

        protected override void ApplyToUnit(Unit unit) => GraduateUnit(unit);

        protected override void RestoreUnit(Unit unit) => GraduateUnit(unit);

        #endregion
    }
}
