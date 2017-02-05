using System;
using System.Collections.Generic;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    class LifeAndDeathPhase : GamePhaseBase, ILifeAndDeathPhase
    {
        private List<GamePlayer> _playersDoneWithLifeDeath = new List<GamePlayer>();

        public LifeAndDeathPhase(GameController gameController) : base(gameController)
        {
        }

        public override GamePhaseType PhaseType => GamePhaseType.LifeDeathDetermination;

        void RecalculateTerritories()
        {
             GameBoard boardAfterRemovalOfDeadStones =
               this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                    this.Controller.DeadPositions);
            Territory[,] territory = this.Controller.Ruleset.DetermineTerritory(boardAfterRemovalOfDeadStones);
            this.Controller.OnLifeDeathTerritoryChanged(new Game.TerritoryMap(territory, this.Controller.Info.BoardSize));
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
                if (!Controller.DeadPositions.Contains(deadStone))
                {
                    Controller.DeadPositions.Add(deadStone);
                }
            }
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
        }
        public void Done(GamePlayer player)
        {
            Controller.OnDebuggingMessage(player + " has completed his part of the Life/Death determination phase.");
            if (!_playersDoneWithLifeDeath.Contains(player))
            {
                _playersDoneWithLifeDeath.Add(player);
            }
            // TODO maybe infinite recursion here?
           //  this._game.Server?.LifeDeath_Done(this._game);
            if (_playersDoneWithLifeDeath.Count == 2) // && this._game.Server == null)
            {
               ScoreIt();
            }
            RecalculateTerritories();
        }

        public void ScoreIt()
        {
            GameBoard boardAfterRemovalOfDeadStones =
                this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                    this.Controller.DeadPositions);
            Scores scores = this.Controller.Ruleset.CountScore(boardAfterRemovalOfDeadStones);
            bool isDraw = Math.Abs(scores.BlackScore - scores.WhiteScore) < 0.2f;
            GamePlayer winner;
            GamePlayer loser;
            if (isDraw)
            {
                winner = this.Controller.Players.Black;
                loser = this.Controller.Players.White;
            }
            else if (scores.BlackScore > scores.WhiteScore)
            {
                winner = this.Controller.Players.Black;
                loser = this.Controller.Players.White;
            }
            else if (scores.BlackScore < scores.WhiteScore)
            {
                winner = this.Controller.Players.White;
                loser = this.Controller.Players.Black;
            }
            else
            {
                throw new Exception("This cannot happen.");
            }
            this.Controller.GoToEnd(GameEndInformation.ScoringComplete(isDraw, winner, loser, scores));
        }

        public void UndoPhase()
        {
            Controller.DeadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            // TODO online
        }

        public void Resume()
        {
            Controller.DeadPositions = new List<Position>();
            GoToPhase(GamePhaseType.Main);
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            // TODO check
        }

        public override void StartPhase()
        {
            RecalculateTerritories();
        }
    }
}
