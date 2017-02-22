using System;
using System.Linq;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.State;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Phases.Finished
{
    internal class FinishedPhase : GamePhaseBase, IFinishedPhase
    {
        public FinishedPhase(GameController gameController) : base(gameController)
        {

        }

        /// <summary>
        /// Finished phase
        /// </summary>
        public override GamePhaseType Type => GamePhaseType.Finished;

        public override void StartPhase()
        {
            ScoreIt();
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
                var deadPositions = Controller.PreviousPhases.OfType<ILifeAndDeathPhase>().Last().DeadPositions;
                GameBoard boardAfterRemovalOfDeadStones =
                    this.Controller.GameTree.LastNode.BoardState.BoardWithoutTheseStones(deadPositions);
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
    }
}
