using Abraxas.Cards.Controllers;
using Abraxas.Cells.Controllers;
using Abraxas.Games.Managers;
using Abraxas.GameStates;
using Abraxas.Health.Managers;
using Abraxas.Network.Managers;
using Abraxas.Players.Managers;
using Abraxas.Zones.Fields.Managers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

using Player = Abraxas.Players.Players;

namespace Abraxas.AI.Managers
{
    class AIManager : NetworkedManager, IAIManager
    {
        #region Dependencies
        IGameManager _gameManager;
        IPlayerManager _playerManager;
        IFieldManager _fieldManager;
        IPlayerHealthManager _playerHealthManager;
        IGameStateManager _gameStateManager;

        [Inject]
        public void Construct(IGameManager gameManager, IPlayerManager playerManager, IFieldManager fieldManager, IPlayerHealthManager playerHealthManager, IGameStateManager gameStateManager)
        {
            _gameManager = gameManager;
            _playerManager = playerManager;
            _fieldManager = fieldManager;
            _playerHealthManager = playerHealthManager;
            _gameStateManager = gameStateManager;
        }
        #endregion

        #region Fields
        [SerializeField]
        bool _isPlayer1AI, _isPlayer2AI;
        #endregion

        #region Properties
        public bool IsPlayer1AI { get => _isPlayer1AI; set => _isPlayer1AI = value; }
        public bool IsPlayer2AI { get => _isPlayer2AI; set => _isPlayer2AI = value; }
        #endregion

        #region Methods
        public IEnumerator DeterminePlay()
        {
            if (!IsServer) yield break;
            if (!(IsPlayer1AI && _playerManager.ActivePlayer == Player.Player1) &&
                !(IsPlayer2AI && _playerManager.ActivePlayer == Player.Player2))
            {
                yield break;
            }

            Debug.Log($"[AIManager] Starting AI turn for player {_playerManager.ActivePlayer}");

            var playableCards = _gameManager.GetAllPlayableCards();
            var availableCells = _gameManager.GetAvailableCells();

            while (playableCards.Count > 0 && availableCells.Length > 0)
            {
                PlayBestCard(playableCards, availableCells);
                yield return new WaitForSeconds(1f);
                playableCards = _gameManager.GetAllPlayableCards();
                availableCells = _gameManager.GetAvailableCells();
            }

            Debug.Log($"[AIManager] AI turn for player {_playerManager.ActivePlayer} ended.");
            yield return _gameStateManager.BeginNextGameState();
        }

        private void PlayBestCard(List<ICardController> playableCards, ICellController[] availableCells)
        {
            float bestAdvantage = float.MinValue;

            // We'll keep track of *all* (card, cell) pairs that tie for bestAdvantage
            var bestMoves = new List<(ICardController card, ICellController cell)>();

            foreach (var card in playableCards)
            {
                foreach (var cell in availableCells)
                {
                    float advantage = EvaluateAdvantage(card, cell);
                    Debug.Log($"[AIManager] Evaluated advantage for card '{card.Title}' at cell ({cell.FieldPosition.X}, {cell.FieldPosition.Y}): {advantage}");

                    // If we found a new strictly greater advantage, reset the list
                    if (advantage > bestAdvantage)
                    {
                        bestAdvantage = advantage;
                        bestMoves.Clear();
                        bestMoves.Add((card, cell));
                    }
                    else if (Mathf.Approximately(advantage, bestAdvantage))
                    {
                        // If advantage ties with our current bestAdvantage, add it to the “tie” list
                        bestMoves.Add((card, cell));
                    }
                }
            }

            if (bestMoves.Count > 0)
            {
                // Randomly pick among all bestMoves (the “tie” list).
                int choiceIndex = UnityEngine.Random.Range(0, bestMoves.Count);
                var (bestCard, bestCell) = bestMoves[choiceIndex];

                Debug.Log($"[AIManager] Among {bestMoves.Count} tied best moves, AI chose to play card '{bestCard.Title}' "
                        + $"at cell ({bestCell.FieldPosition.X}, {bestCell.FieldPosition.Y}) with advantage {bestAdvantage}");
                _gameManager.PurchaseCardAndMoveFromHandToCell(bestCard, bestCell.FieldPosition);
            }
            else
            {
                Debug.Log("[AIManager] No advantageous move found. Playing random card.");
                PlayRandom();
            }
        }

        private void PlayRandom()
        {
            var playableCards = _gameManager.GetAllPlayableCards();
            var availableCells = _gameManager.GetAvailableCells();

            if (playableCards.Count > 0 && availableCells.Length > 0)
            {
                int randomCardIndex = UnityEngine.Random.Range(0, playableCards.Count);
                int randomCellIndex = UnityEngine.Random.Range(0, availableCells.Length);

                var randomCard = playableCards[randomCardIndex];
                var randomCell = availableCells[randomCellIndex];

                Debug.Log($"[AIManager] Randomly playing card '{randomCard.Title}' at cell ({randomCell.FieldPosition.X}, {randomCell.FieldPosition.Y})");

                _gameManager.PurchaseCardAndMoveFromHandToCell(randomCard, randomCell.FieldPosition);
            }
        }

        private float EvaluateAdvantage(ICardController card, ICellController cell)
        {
            Debug.Log($"[AIManager] Evaluating advantage for '{card.Title}' in cell ({cell.FieldPosition.X}, {cell.FieldPosition.Y})");
            float totalAdvantage = 0f;

            // Our unit's position
            int ourPositionY = cell.FieldPosition.Y;

            // Check for lethal threats
            bool counteredLethal = false;
            var enemyUnits = GetEnemyUnitsInRow(ourPositionY);
            foreach (var enemyCard in enemyUnits)
            {
                if (IsLethalThreat(enemyCard))
                {
                    // Check if placing our card here can counter the threat
                    float engagementAdvantage = EvaluateEngagement(card, cell, enemyCard);

                    totalAdvantage += engagementAdvantage + 1000f; // High priority for countering lethal
                    Debug.Log($"[AIManager] Countering lethal threat from '{enemyCard.Title}' with card '{card.Title}'.");
                    counteredLethal = true;
                }
                else if (IsDangerousImmediateThreat(enemyCard))
                {
                    // Check if placing our card here can counter the threat
                    float engagementAdvantage = EvaluateEngagement(card, cell, enemyCard);

                    totalAdvantage += engagementAdvantage + 10f; // High priority for countering dangerous threats
                    Debug.Log($"[AIManager] Countering dangerous threat from '{enemyCard.Title}' with card '{card.Title}'.");
                    counteredLethal = true;
                }
            }

            if (!counteredLethal)
            {
                // Normal engagement logic for non-lethal threats
                foreach (var enemyCard in enemyUnits)
                {
                    float engagementAdvantage = EvaluateEngagement(card, cell, enemyCard);
                    totalAdvantage += engagementAdvantage;

                    Debug.Log($"[AIManager] Evaluated engagement advantage between card '{card.Title}' and enemy card '{enemyCard.Title}': {engagementAdvantage}");
                }

                // Consider reaching the opponent's home row
                float homeRowAdvantage = EvaluateHomeRowAdvantage(card, cell);
                totalAdvantage += homeRowAdvantage;

                Debug.Log($"[AIManager] Evaluated home row advantage for card '{card.Title}' at cell ({cell.FieldPosition.X}, {cell.FieldPosition.Y}): {homeRowAdvantage}");
            }

            // Adjust advantage based on card cost
            int manaCost = card.TotalCosts.Values.Sum();
            float costPenalty = manaCost * 0.1f; // Adjust weight as needed
            totalAdvantage -= costPenalty;

            Debug.Log($"[AIManager] Total advantage for card '{card.Title}' at cell ({cell.FieldPosition.X}, {cell.FieldPosition.Y}): {totalAdvantage} (Cost penalty: {costPenalty})");

            return totalAdvantage;
        }

        private bool IsLethalThreat(ICardController enemyCard)
        {
            int distanceToHomeRow = CalculateDistanceToHomeRow(enemyCard.Cell.FieldPosition.X, enemyCard.Owner);
            if (distanceToHomeRow <= enemyCard.StatBlock.Stats.SPD)
            {
                int playerHP = _playerHealthManager.GetPlayerHealth(_playerManager.ActivePlayer).HP;
                if (enemyCard.StatBlock.Stats.ATK >= playerHP)
                {
                    Debug.Log($"[AIManager] Enemy card '{enemyCard.Title}' is a lethal threat with {enemyCard.StatBlock.Stats.ATK} ATK and {distanceToHomeRow} distance to home row.");
                    return true;
                }
            }
            return false;
        }

        private bool IsDangerousImmediateThreat(ICardController enemyCard)
        {
            int distanceToHomeRow = CalculateDistanceToHomeRow(enemyCard.Cell.FieldPosition.X, enemyCard.Owner);
            if (distanceToHomeRow <= enemyCard.StatBlock.Stats.SPD)
            {
                int playerHP = _playerHealthManager.GetPlayerHealth(_playerManager.ActivePlayer).HP;
                if (enemyCard.StatBlock.Stats.ATK * 5 >= playerHP)
                {
                    Debug.Log($"[AIManager] Enemy card '{enemyCard.Title}' is a very dangerous threat with {enemyCard.StatBlock.Stats.ATK} ATK and {distanceToHomeRow} distance to home row.");
                    return true;
                }
            }
            return false;
        }

        private int CalculateDistanceToHomeRow(int unitPositionX, Player unitOwner)
        {
            int fieldWidth = _fieldManager.GetRow(0).Count;
            int homeRowX = (unitOwner == Player.Player1) ? fieldWidth - 1 : 0;
            return Mathf.Abs(homeRowX - unitPositionX);
        }



        private float EvaluateEngagement(ICardController ourCard, ICellController ourCell, ICardController enemyCard)
        {
            // Extract stats
            var ourStats = ourCard.StatBlock.Stats;
            var enemyStats = enemyCard.StatBlock.Stats;

            int ourATK = ourStats.ATK;
            int ourDEF = ourStats.DEF;
            int ourSPD = ourStats.SPD;
            int ourRNG = ourStats.RNG;

            int enemyATK = enemyStats.ATK;
            int enemyDEF = enemyStats.DEF;
            int enemySPD = enemyStats.SPD;
            int enemyRNG = enemyStats.RNG;

            int ourPositionX = ourCell.FieldPosition.X;
            int enemyPositionX = enemyCard.Cell.FieldPosition.X;

            bool ourTurn = true; // It's our turn when we evaluate this

            // Determine engagement type
            var engagement = SimulateCombatStep(ourPositionX, enemyPositionX, ourSPD, enemySPD, ourRNG, enemyRNG, !ourTurn);

            float advantage = engagement switch
            {
                EngagementType.OurMelee => ResolveMeleeAttack(ourATK, ourDEF, enemyATK, enemyDEF, ourCard, enemyCard),
                EngagementType.OurRanged => ResolveRangedAttack(ourATK, enemyDEF, enemyCard, attackerIsOurs: true),
                EngagementType.EnemyMelee => ResolveMeleeAttack(ourATK, ourDEF, enemyATK, enemyDEF, ourCard, enemyCard),
                EngagementType.EnemyRanged => ResolveRangedAttack(enemyATK, ourDEF, ourCard, attackerIsOurs: false),   
                _ => -999f,// Avoid combats that result in stalemates
            };
            return advantage;
        }


        /// <summary>
        /// Resolves a ranged attack scenario. The attacker deals damage without immediate retaliation.
        /// If the defender is destroyed, no counterattack occurs.
        /// If the defender survives, advantage is based on damage dealt.
        /// </summary>
        private float ResolveRangedAttack(int attackerATK, int defenderDEF, ICardController defenderCard, bool attackerIsOurs)
        {
            float advantage = 0f;

            bool defenderDestroyed = attackerATK >= defenderDEF;

            if (defenderDestroyed)
            {
                // We destroyed the enemy at range before they could retaliate
                advantage += EvaluateUnitValue(defenderCard);
                Debug.Log($"[AIManager] Ranged attack: Defender '{defenderCard.Title}' destroyed before retaliation. Advantage: {advantage}");
            }
            else
            {
                // Defender survives. Advantage could be partial, based on damage dealt
                advantage += attackerATK * 0.5f;
                Debug.Log($"[AIManager] Ranged attack: Defender '{defenderCard.Title}' survives. Partial advantage: {attackerATK * 0.5f}");
            }

            // If we are the attackers
            if (!attackerIsOurs)
            {
                // Enemy performed a successful ranged attack on us
                advantage = -advantage;
            }

            return advantage;
        }

        /// <summary>
        /// Resolves a melee attack scenario. Both units deal damage simultaneously.
        /// Even if one unit is destroyed, it still deals its damage for this attack exchange.
        /// </summary>
        private float ResolveMeleeAttack(int ourATK, int ourDEF, int enemyATK, int enemyDEF, ICardController ourCard, ICardController enemyCard)
        {
            float advantage = 0f;

            bool enemyDestroyed = ourATK >= enemyDEF;
            bool ourUnitDestroyed = enemyATK >= ourDEF;

            // In melee, damage is simultaneous
            // Even if we destroy them, they hit us back this round

            if (enemyDestroyed && ourUnitDestroyed)
            {
                // Both units destroy each other
                advantage += EvaluateUnitValue(enemyCard);
                advantage -= EvaluateUnitValue(ourCard);
                Debug.Log($"[AIManager] Melee: Both units destroyed. Advantage: {advantage}");
            }
            else if (enemyDestroyed && !ourUnitDestroyed)
            {
                // We kill enemy, we survive
                advantage += EvaluateUnitValue(enemyCard);
                advantage -= enemyATK * 0.2f; // Some minor penalty for taking damage
                Debug.Log($"[AIManager] Melee: We destroy enemy and survive. Advantage: {advantage}");
            }
            else if (!enemyDestroyed && ourUnitDestroyed)
            {
                // Enemy survives, we die
                advantage -= EvaluateUnitValue(ourCard);
                advantage += ourATK * 0.2f; // Some minor consolation for dealing damage
                Debug.Log($"[AIManager] Melee: We die, enemy survives. Advantage: {advantage}");
            }
            else
            {
                // Both survive
                // Partial advantage could be based on net damage dealt
                float netDamage = (ourATK - enemyATK) * 0.5f;
                advantage += netDamage;
                Debug.Log($"[AIManager] Melee: Both survive, net damage advantage: {netDamage}");
            }

            return advantage;
        }

        private enum EngagementType
        {
            None,
            OurMelee,
            OurRanged,
            EnemyMelee,
            EnemyRanged
        }

        private EngagementType SimulateCombatStep(int ourPos, int enemyPos, int ourSpd, int enemySpd, int ourRng, int enemyRng, bool ourTurn)
        {
            Debug.Log($"[AIManager] '{ourPos}', '{enemyPos}', '{ourSpd}', '{enemySpd}', '{ourRng}', '{enemyRng}', '{ourTurn}'");
            if (ourSpd <= 0 && enemySpd <= 0)
            {
                return EngagementType.None;
            }
            int distance = Mathf.Abs(ourPos - enemyPos);

            if (ourTurn)
            {
                if (distance <= ourSpd)
                {
                    return EngagementType.OurMelee;
                }
                else if (distance <= ourSpd + ourRng)
                {
                    return EngagementType.OurRanged;
                }
                else
                {
                    int newOurPos = ourPos - ourSpd;
                    return SimulateCombatStep(newOurPos, enemyPos, ourSpd, enemySpd, ourRng, enemyRng, !ourTurn);
                }
            }
            else
            {
                if (distance <= enemySpd)
                {
                    return EngagementType.EnemyMelee;
                }
                else if (distance <= enemySpd + enemyRng)
                {
                    return EngagementType.EnemyRanged;
                }
                else
                {
                    int newEnemyPos = enemyPos + enemySpd;
                    return SimulateCombatStep(ourPos, newEnemyPos, ourSpd, enemySpd, ourRng, enemyRng, !ourTurn);
                }
            }
        }

        private float EvaluateUnitValue(ICardController card)
        {
            var stats = card.StatBlock.Stats;
            return stats.ATK + stats.DEF + stats.SPD * 2f + stats.RNG * 2f;
        }

        private List<ICardController> GetEnemyUnitsInRow(int row)
        {
            var fieldRow = _fieldManager.GetRow(row);
            var enemyUnits = new List<ICardController>();

            foreach (var cell in fieldRow)
            {
                var card = cell.GetCardAtIndex(0);
                if (card != null && card.Owner != _playerManager.ActivePlayer)
                {
                    enemyUnits.Add(card);
                }
            }

            return enemyUnits;
        }

        private float EvaluateHomeRowAdvantage(ICardController card, ICellController cell)
        {
            // Our unit's stats and position
            var ourStats = card.StatBlock.Stats;
            int ourATK = ourStats.ATK;
            int ourSPD = ourStats.SPD;
            int ourPositionX = cell.FieldPosition.X;

            // Our owner and movement direction
            Player ourOwner = _playerManager.ActivePlayer;
            int ourDirection = (ourOwner == Player.Player1) ? 1 : -1;

            // Field dimensions
            int fieldWidth = _fieldManager.GetRow(0).Count;

            // Correct opponent's home row position
            int opponentHomeRowPositionX = (ourOwner == Player.Player1) ? fieldWidth - 1 : 0;

            // Calculate distance to opponent's home row
            int distanceToHomeRow = (opponentHomeRowPositionX - ourPositionX) * ourDirection;

            if (distanceToHomeRow <= 0)
            {
                // Already at or beyond opponent's home row
                float advantage = ourATK * 2f;
                Debug.Log($"[AIManager] Card '{card.Title}' is already at or beyond opponent's home row. Immediate advantage: {advantage}");
                return advantage;
            }

            // Check if there are enemy units blocking the path
            bool pathBlocked = false;
            var fieldRow = _fieldManager.GetRow(cell.FieldPosition.Y);

            for (int i = ourPositionX + ourDirection; i != opponentHomeRowPositionX + ourDirection; i += ourDirection)
            {
                var cellToCheck = fieldRow[i];
                var cardInCell = cellToCheck.GetCardAtIndex(0);
                if (cardInCell != null && cardInCell.Owner != ourOwner)
                {
                    pathBlocked = true;
                    Debug.Log($"[AIManager] Path to home row is blocked by enemy card '{cardInCell.Title}' at position {i}");
                    break;
                }
            }

            if (!pathBlocked)
            {
                // Calculate time to reach home row
                int turnsToHomeRow = ourSPD > 0 ? Mathf.CeilToInt(distanceToHomeRow / (float)ourSPD) : int.MaxValue;

                if (turnsToHomeRow > 0 && turnsToHomeRow != int.MaxValue)
                {
                    // Units that reach the home row faster are more advantageous
                    float speedFactor = 1f / turnsToHomeRow;
                    float advantage = ourATK * 2f * speedFactor; // Adjust weight as needed

                    Debug.Log($"[AIManager] Card '{card.Title}' can reach opponent's home row in {turnsToHomeRow} turns. Advantage: {advantage}");

                    return advantage;
                }
                else if (turnsToHomeRow == 0)
                {
                    // Immediate advantage if already at the home row
                    float advantage = ourATK * 2f;

                    Debug.Log($"[AIManager] Card '{card.Title}' is at opponent's home row. Immediate advantage: {advantage}");

                    return advantage;
                }
                else
                {
                    // Cannot reach opponent's home row due to zero speed
                    Debug.Log($"[AIManager] Card '{card.Title}' cannot reach opponent's home row due to zero speed.");
                    return 0f;
                }
            }

            return 0f;
        }
        #endregion
    }
}
