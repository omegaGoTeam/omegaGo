using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.Core.AI.FuegoSpace
{
    public class Fuego : AIProgramBase
    {
        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);

     

        public override Task<IEnumerable<Position>> GetDeadPositions()
        {
            throw new NotImplementedException("Assistant later.");
            return null;
            return base.GetDeadPositions();
        }

        public override AIDecision GetHint(AiGameInformation gameInformation)
        {
            throw new NotImplementedException("Assistant later.");
            if (FuegoEngine.Instance.CurrentGame == null)
            {
                return AIDecision.Resign("Not yet implemented, but this should work.");
            }
            else
            {
                if (FuegoEngine.Instance.CurrentGame.Info.Equals(gameInformation.GameInfo))
                {
                    return AIDecision.Resign("Not yet implemented, but this should work.");
                }
                else
                {
                    // Fuego is playing elsewhere, we cannot get hints.
                    return null;
                }
            }
        }
        public override AIDecision RequestMove(AiGameInformation gameInformation)
        {
            if (_brokenDueToInvalidLaunch)
            {
                return AIDecision.Resign(
                    "Fuego already plays in another game. Fuego may only play in one game at a time.");

            }
            RequireInitialization(gameInformation);
            return FuegoEngine.Instance.RequestMove(this, gameInformation);
        }
        public override void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            if (_brokenDueToInvalidLaunch) return;
            RequireInitialization(new AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
        }

        private void RequireInitialization(AiGameInformation aiGameInformation)
        {
            if (!_initialized)
            {
                _initialized = true;
                if (_isPrimaryPlayer)
                {
                    FuegoEngine.Instance.Initialize(aiGameInformation);
                }
            }
        }

        public override void MoveUndone()
        {
            if (_brokenDueToInvalidLaunch) return;
        }
        
        private bool _isPrimaryPlayer;
        private bool _initialized;
        private bool _brokenDueToInvalidLaunch; 
        /// <summary>
        /// Indicates whether Fuego is permitted to resign in hopeless situations in non-handicap games.
        /// </summary>
        public bool AllowResign { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of playouts Fuego tries before offering a move.
        /// </summary>
        public int MaxGames { get; set; }

        /// <summary>
        /// Indicates whether Fuego should be thinking during its opponent's turn.
        /// </summary>
        public bool Ponder { get; set; }

        public void Initialize(AiAgent aiAgent)
        {

            var gameState = aiAgent.GameState;

            // Determine primary player
            foreach(var player in gameState.Players)
            {
                if (player.Agent is AiAgent)
                {
                    AiAgent thatAgent = player.Agent as AiAgent;
                    if (thatAgent.AI is Fuego)
                    {
                        if (thatAgent == aiAgent)
                        {
                            _isPrimaryPlayer = true;
                        }
                        else
                        {
                            _isPrimaryPlayer = false;
                        }
                        break;
                    }
                }
            }

            // If you're the primary player, establish the game.
            if (_isPrimaryPlayer)
            {
                if (FuegoEngine.Instance.CurrentGame != null &&
                    FuegoEngine.Instance.CurrentGame.Players.All(pl => pl.Agent != aiAgent))
                {
                    // A game is in progress and it doesn't include this AI.
                    _brokenDueToInvalidLaunch = true;
                    return;
                }
            }

        }
    }
}
