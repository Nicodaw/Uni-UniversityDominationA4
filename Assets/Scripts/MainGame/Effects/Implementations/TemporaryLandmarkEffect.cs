using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class TemporaryLandmarkEffect : TurnedEffect
    {
        #region Private Fields

        int _playedBy;

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
            Sectors = game.Map.Sectors.Where(s => s.Landmark == null && s.Unit == null &&
                                             !s.Stats.HasEffect<TemporaryLandmarkEffect>() &&
                                             !s.Stats.HasEffect<BlockSectorEffect>())
        };

        #endregion

        #region Helper Methods

        void PutLandmark() => AppliedSector.LeafletGuyPrefabActive = true;

        void RemoveLandmark() => AppliedSector.LeafletGuyPrefabActive = false;

        protected override void ApplyToSector()
        {
            _playedBy = Game.Instance.CurrentPlayer.Id;
            PutLandmark();
        }

        protected override void RestoreSector() => PutLandmark();

        #endregion
    }
}
