﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath
{
    internal class LifeAndDeathPhase : GamePhaseBase, ILifeAndDeathPhase
    {
        /// <summary>
        /// List of players who confirmed the life and death phase
        /// </summary>
        private readonly List<GamePlayer> _playersDoneWithLifeDeath = new List<GamePlayer>();

        /// <summary>
        /// Dead positions
        /// </summary>
        private List<Position> _deadPositions = new List<Position>();

        /// <summary>
        /// Creates life and death phase
        /// </summary>
        /// <param name="gameController">Game controller</param>
        public LifeAndDeathPhase(GameController gameController) : base(gameController)
        {
        }

        /// <summary>
        /// Indicates that the life and death territory has changed
        /// </summary>
        public event EventHandler<TerritoryMap> LifeDeathTerritoryChanged;

        /// <summary>
        /// Life and death determination phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.LifeDeathDetermination;

        /// <summary>
        /// Dead positions
        /// </summary>
        public IEnumerable<Position> DeadPositions => _deadPositions;

        /// <summary>
        /// Starts life and death phase
        /// </summary>
        public override void StartPhase()
        {
            foreach (var player in Controller.Players)
            {
                player.Clock.StopClock();
            }
            RecalculateTerritories();

            foreach (var connector in Controller.Connectors)
            {
                connector.LifeDeathReturnToMainForced += ConnectorLifeDeathReturnToMainForced;
                connector.LifeDeathDoneRequested += ConnectorLifeDeathDoneRequested;
                connector.LifeDeathUndoDeathMarksRequested += ConnectorLifeDeathUndoDeathMarksRequested;
                connector.LifeDeathKillGroupRequested += ConnectorLifeDeathKillGroupRequested;
            }
        }

        /// <summary>
        /// Ends life and death phase
        /// </summary>
        public override void EndPhase()
        {
            foreach (var connector in Controller.Connectors)
            {
                connector.LifeDeathReturnToMainForced -= ConnectorLifeDeathReturnToMainForced;
                connector.LifeDeathDoneRequested -= ConnectorLifeDeathDoneRequested;
                connector.LifeDeathUndoDeathMarksRequested -= ConnectorLifeDeathUndoDeathMarksRequested;
                connector.LifeDeathKillGroupRequested -= ConnectorLifeDeathKillGroupRequested;
            }
        }

        /// <summary>
        /// Marks a group at position dead
        /// </summary>
        /// <param name="position"></param>
        public void MarkGroupDead(Position position)
        {
            //take the current board
            var board = Controller.CurrentNode.BoardState;
            if (board[position.X, position.Y] == StoneColor.None)
            {
                return;
            }

            //discover group at position
            var group = Controller.Ruleset.DiscoverGroup(position, board);
            foreach (var deadStone in group)
            {
                if (!_deadPositions.Contains(deadStone))
                {
                    _deadPositions.Add(deadStone);
                }
            }

            //reset life and death players state
            _playersDoneWithLifeDeath.Clear();

            RecalculateTerritories();
            Controller.OnDebuggingMessage(position + " marked dead.");
        }

        /// <summary>
        /// Player is done with life and death
        /// </summary>
        /// <param name="player"></param>
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

        /// <summary>
        /// Undoes the life and death phase (but does not resume game)
        /// </summary>
        public void UndoPhase()
        {
            _deadPositions = new List<Position>();
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            Controller.OnDebuggingMessage("Life/death phase undone.");
        }

        /// <summary>
        /// Resumes main phase
        /// </summary>
        public void Resume()
        {
            _deadPositions = new List<Position>();
            GoToPhase(GamePhaseType.Main);
            _playersDoneWithLifeDeath.Clear();
            RecalculateTerritories();
            Controller.OnDebuggingMessage("Life/death phase cancelled. Resuming gameplay...");
        }
        
        private void ConnectorLifeDeathKillGroupRequested(object sender, Position e)
        {
            LifeDeathRequestKillGroup(e);
        }

        protected virtual Task LifeDeathRequestKillGroup(Position groupMember)
        {
            MarkGroupDead(groupMember);
            return Task.FromResult(0);
        }

        private void ConnectorLifeDeathUndoDeathMarksRequested(object sender, EventArgs e)
        {
            LifeDeathRequestUndoDeathMarks();
        }

        protected virtual Task LifeDeathRequestUndoDeathMarks()
        {
            UndoPhase();
            return Task.FromResult(0);
        }

        private void ConnectorLifeDeathDoneRequested(object sender, EventArgs e)
        {
            LifeDeathRequestDone();
        }

        protected virtual Task LifeDeathRequestDone()
        {
            ScoreIt();
            return Task.FromResult(0);
        }

        private void ConnectorLifeDeathReturnToMainForced(object sender, EventArgs e)
        {
            Resume();
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
                var deadPositions = DeadPositions;
                GameBoard boardAfterRemovalOfDeadStones =
                    Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(deadPositions);
                scores = Controller.Ruleset.CountScore(boardAfterRemovalOfDeadStones);
            }
            bool isDraw = Math.Abs(scores.BlackScore - scores.WhiteScore) < 0.2f;
            GamePlayer winner;
            GamePlayer loser;
            if (isDraw)
            {
                winner = Controller.Players.Black;
                loser = Controller.Players.White;
                Controller.OnDebuggingMessage("It's a draw.");
            }
            else if (scores.BlackScore > scores.WhiteScore)
            {
                winner = Controller.Players.Black;
                loser = Controller.Players.White;
            }
            else if (scores.BlackScore < scores.WhiteScore)
            {
                winner = Controller.Players.White;
                loser = Controller.Players.Black;
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
            gameEndInfo = isDraw ?
                GameEndInformation.CreateDraw(Controller.Players, scores) : 
                GameEndInformation.CreateScoredGame(winner, loser, scores);
            Controller.EndGame(gameEndInfo);
        }

        /// <summary>
        /// Recalculates territories
        /// </summary>
        private void RecalculateTerritories()
        {
            GameBoard boardAfterRemovalOfDeadStones =
              Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(
                   _deadPositions);
            Territory[,] territory = Controller.Ruleset.DetermineTerritory(boardAfterRemovalOfDeadStones);
            OnLifeDeathTerritoryChanged(new TerritoryMap(
                territory,
                _deadPositions.ToList()));
        }

        /// <summary>
        /// Fires the life and death event
        /// </summary>
        /// <param name="map">Territory map</param>
        private void OnLifeDeathTerritoryChanged(TerritoryMap map)
        {
            LifeDeathTerritoryChanged?.Invoke(this, map);
        }
    }
}
