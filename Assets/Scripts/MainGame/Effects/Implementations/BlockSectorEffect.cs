using System;
using System.Linq;

namespace EffectImpl
{
    [Serializable]
    public class BlockSectorEffect : TurnedEffect
    {
        #region Private Fields

        int _playedBy;

        #endregion

        #region Private Properties

        Player PlayedBy => Game.Instance.Players[_playedBy];

        #endregion

        #region Override Properties

        protected override int TurnsLeft { get; set; } = 2;

        protected override Player TurnedPlayer => PlayedBy;

        public override string CardName => "Industrial action";

        public override string CardDescription => "Target unoccupied sector becomes impassable for 2 turns";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.Sector;

        public override CardTier CardTier => CardTier.Tier1;

        public override bool? Traversable => false;

        #endregion

        #region Concrete Methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Sectors = game.Map.Sectors.Where(s => s.AllowPVC) //reuse of the PVC flag as it selects only non-occupied sectors that don't have a landmark
        };

        public override void ProcessEffectRemove() => UnBlock();

        #endregion

        #region Helper Methods

        void Block() => AppliedSector.BlockPrefabActive = true;

        void UnBlock() => AppliedSector.BlockPrefabActive = false;

        protected override void ApplyToSector()
        {
            _playedBy = Game.Instance.CurrentPlayer.Id;
            Block();
        }

        protected override void RestoreSector() => Block();

        #endregion
    }
}
