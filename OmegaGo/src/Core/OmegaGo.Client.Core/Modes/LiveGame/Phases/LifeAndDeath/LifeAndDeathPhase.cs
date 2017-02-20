using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class LifeAndDeathPhase : GamePhaseBase, ILifeAndDeathPhase
    {
        private List<GamePlayer> _playersDoneWithLifeDeath = new List<GamePlayer>();

        private List<Position> _deadPositions = new List<Position>();

        public LifeAndDeathPhase(GameController gameController) : base(gameController)
        {
        }

        public override GamePhaseType Type => GamePhaseType.LifeDeathDetermination;

        /// <summary>
        /// Dead positions
        /// </summary>
        public IEnumerable<Position> DeadPositions => _deadPositions;

        void RecalculateTerritories()
        {
            GameBoard boardAfterRemovalOfDeadStones =
              this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                   _deadPositions);
            Territory[,] territory = this.Controller.Ruleset.DetermineTerritory(boardAfterRemovalOfDeadStones);
            //TODO: Implement
            //this.Controller.OnLifeDeathTerritoryChanged(new Game.TerritoryMap(territory, this.Controller.Info.BoardSize));
        }

        public void MarkGroupDead(Position position)
        {
            var board = Controller.GameTree.LastNode.BoardState;
            if (board[position.X, position.Y] == StoneColor.None)
            {
                return;
            }
            var group = Controller.Ruleset.DiscoverGroup(position, board);
            foreach (var deadStone in group)
            {
                if (!_deadPositions.Contains(deadStone))
                {
                    _deadPositions.Add(deadStone);
                }
            }
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            Controller.OnDebuggingMessage(position + " marked dead.");
        }
        public void Done(GamePlayer player)
        {
            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            Controller.OnDebuggingMessage(player + " has completed his part of the Life/Death determination phase.");
            if (_playersDoneWithLifeDeath.Count == 2)
            {
                GoToPhase(GamePhaseType.Finished);
            }
        }       

        public void UndoPhase()
        {
            _deadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            Controller.OnDebuggingMessage("Life/death phase undone.");
        }

        public void Resume()
        {
            _deadPositions = new List<Position>();
            GoToPhase(GamePhaseType.Main);
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            Controller.OnDebuggingMessage("Life/death phase cancelled. Resuming gameplay...");
        }

        public override void StartPhase()
        {
            foreach (var player in Controller.Players)
            {
                player.Clock.StopClock();
            }
            RecalculateTerritories();
        }


        //TODO: Implement
        //public virtual void OnLifeDeathTerritoryChanged(TerritoryMap map)
        //{
        //    LifeDeathTerritoryChanged?.Invoke(this, map);
        //    BoardMustBeRefreshed?.Invoke(this, EventArgs.Empty);
        //}
    }
}
