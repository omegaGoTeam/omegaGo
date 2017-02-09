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

        public override GamePhaseType PhaseType => GamePhaseType.LifeDeathDetermination;

        void RecalculateTerritories()
        {
            GameBoard boardAfterRemovalOfDeadStones =
              this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                   _deadPositions);
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
                ScoreIt();
            }
        }

        /// <summary>
        /// Scores the game and moves us to the Finished phase.
        /// </summary>
        /// <param name="e">If this parameter is set, then it overriddes scores that would be determined from life/death determination and ruleset.</param>
        public void ScoreIt(Scores e = null)
        {
            Scores scores = e;
            if (scores == null)
            {
                GameBoard boardAfterRemovalOfDeadStones =
                    this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                        _deadPositions);
                scores = this.Controller.Ruleset.CountScore(boardAfterRemovalOfDeadStones);
            }
            bool isDraw = Math.Abs(scores.BlackScore - scores.WhiteScore) < 0.2f;
            GamePlayer winner;
            GamePlayer loser;
            if (isDraw)
            {
                winner = this.Controller.Players.Black;
                loser = this.Controller.Players.White;
                Controller.OnDebuggingMessage("It's a draw.");
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
            if (!isDraw)
            {
                Controller.OnDebuggingMessage(winner + " wins.");
            }
            Controller.OnDebuggingMessage("Scoring complete! " + scores.AbsoluteScoreDifference);
            GameEndInformation gameEndInfo = null;
            if (isDraw)
            {
                gameEndInfo = GameEndInformation.CreateDraw(Controller.Players, scores);
            }
            else
            {
                gameEndInfo = GameEndInformation.CreateScoredGame(winner, loser, scores);
            }
            this.Controller.EndGame(gameEndInfo);
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
    }
}
