using Abraxas.Players.Managers;
using UnityEngine;

namespace Abraxas.Stones.Targets
{
    /// <summary>
    /// Target_CurrentActivePlayerSO is a stone target that represents the current active player.
    /// </summary>
    [CreateAssetMenu(fileName = "New Target ActivePlayer", menuName = "Abraxas/Data/StoneData/Targets/ActivePlayer")]
    class Target_CurrentActivePlayerSO : TargetSO<Players.Players>
    {
        #region Dependencies
        readonly IPlayerManager _playerManager;
        public Target_CurrentActivePlayerSO(IPlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        #endregion

        #region Properties
        public override Players.Players Target
        {
            get
            {
                return _playerManager.ActivePlayer;
            }
            set
            {
                // do not override the target
            }
        }
        #endregion
    }
}
