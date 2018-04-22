using System;
using System.Linq;
using UnityEngine;

namespace EffectImpl
{
    [Serializable]
    public class TemporaryLandmarkEffect : Effect
    {
        #region Private Fields
        Player playedBy = Game.Instance.CurrentPlayer;
        int _sector;
        int _turnsLeft;
        GameObject _landmarkModel;

        #endregion

        #region Override Properties

        public override string CardName => "Christian Union Leaflet guy";

        public override string CardDescription => "Put a marker on an unoccupied sector. Owner gets +1/+1 on all units";

        public override CardCornerIcon CardCornerIcon => CardCornerIcon.Sector;

        public override CardTier CardTier => CardTier.Tier2;

        public override int? AttackBonus => 1;

        public override int? DefenceBonus => 1;

        #endregion

        #region Handlers

        #endregion

        #region Concrete methods

        public override EffectAvailableSelection AvailableSelection(Game game) => new EffectAvailableSelection
        {
            Sectors = game.Map.Sectors.Where(s => s.AllowPVC)
        };

        public override void ProcessPlayerTurnStart(object sender, EventArgs e)
        {
            if ((Player)sender == playedBy) //if at the start of the 
            {
                _turnsLeft = _turnsLeft - 1;
                if (_turnsLeft == 0)
                    RemoveLandmark();
            }
        }

        #endregion

        #region Helper Methods

        void PutLandmark(Sector sector)
        {
            _sector = sector.Id;
            LandmarkEffect tempEffect = new LandmarkEffect(_sector, (int)AttackBonus, (int)DefenceBonus);
            Game.Instance.Map.Sectors[_sector].Landmark.RegisterPlayerEffect(tempEffect);
            UnityEngine.Object.Instantiate(_landmarkModel, sector.transform);

        }
        void RemoveLandmark()
        {
            UnityEngine.Object.Destroy(Game.Instance.Map.Sectors[_sector].transform.GetChild(0));
            RemoveSelf();
        }

        protected override void ApplyToSector(Sector sector) => PutLandmark(sector);

        protected override void RestoreSector(Sector sector) => PutLandmark(sector);

        #endregion
    }
}
