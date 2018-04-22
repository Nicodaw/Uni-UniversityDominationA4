using System;
using System.Linq;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class TemporaryLandmarkEffect : TurnedEffect
    {
        #region Private Fields

        int _playedBy;
        [NonSerialized]
        GameObject _landmarkModel;

        #endregion

        #region Override Properties

        protected override int TurnsLeft { get; set; } = 5;

        protected override Player TurnedPlayer => Game.Instance.Players[_playedBy];

        public override string CardName => "Christian Union Leaflet guy";

        public override string CardDescription => "Put a marker on an unoccupied sector. Owner gets +1/+1 on all units";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.Sector;

        public override CardTier CardTier => CardTier.Tier2;

        #endregion

        #region Public Properties

        public int PlayerAttackBonus => 1;

        public int PlayerDefenceBonus => 1;

        #endregion

        #region Concrete methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Sectors = game.Map.Sectors.Where(s => s.Landmark == null && s.Unit == null && !s.Stats.HasEffect<TemporaryLandmarkEffect>())
        };

        #endregion

        #region Helper Methods

        void PutLandmark()
        {
            //LandmarkEffect tempEffect = new LandmarkEffect(_sector.Id, (int)AttackBonus, (int)DefenceBonus);
            //_sector.Landmark.RegisterPlayerEffect(tempEffect);
            UnityEngine.Object.Instantiate(_landmarkModel, AppliedSector.transform);
        }

        void RemoveLandmark()
        {
            UnityEngine.Object.Destroy(AppliedSector.transform.GetChild(0));
            RemoveSelf();
        }

        protected override void ApplyToSector()
        {
            _playedBy = Game.Instance.CurrentPlayer.Id;
            PutLandmark();
        }

        protected override void RestoreSector() => PutLandmark();

        #endregion
    }
}
