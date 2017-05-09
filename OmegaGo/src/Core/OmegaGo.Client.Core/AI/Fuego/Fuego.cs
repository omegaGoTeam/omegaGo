using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Modes.LiveGame.Players.Agents.AI;
using OmegaGo.Core.Modes.LiveGame.State;

namespace OmegaGo.Core.AI.FuegoSpace
{
    /// <summary>
    ///     The Fuego AI is a Monte Carlo advanced Go intelligence.
    ///     Because of localization and because of quests, this class must be named Fuego.
    ///     
    ///     This doesn't actually contain a lot of Fuego's functionality. This represents a Fuego player or a Fuego assistant and is 
    /// a layer or indirection between a game and the Fuego engine. It exists mostly so that it works well with the other AI's - 
    /// they all must implement <see cref="IAIProgram"/>. 
    /// </summary>
    public class Fuego : AIProgramBase // Do not rename.
    {/// <summary>
        /// The primary player, in this context, is the <see cref="Fuego"/> instance that comes the first in <see cref="GameController.Players"/>. Some actions are only done by the primary player, to avoid duplication. Who is the primary player
        /// is arbitrary and unimportant. What's important is that there is only one.  
        /// </summary>
        private bool _isPrimaryPlayer;
        /// <summary>
        /// This ensures that <see cref="RequireInitialization(AiGameInformation)"/> only takes effect once. 
        /// </summary>
        private bool _initialized;
        /// <summary>
        /// In some edge case scenarios, a game might be opened where Fuego is not permitted to play. This variable prevents it 
        /// from trying since that might crash the app.
        /// </summary>
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

        public override AICapabilities Capabilities => new AICapabilities(false, true, 2, 19, true);
     
        public override async Task<IEnumerable<Position>> GetDeadPositions(IGameController gameController)
        {
            if (FuegoSingleton.Instance.CurrentGame == null)
            {
                return await FuegoSingleton.Instance.GetIsolatedDeadPositions(this, gameController as GameController);
            }
            else
            {
                if (FuegoSingleton.Instance.CurrentGame == gameController)
                {
                    var positions = await FuegoSingleton.Instance.GetDeadPositions(this);
                    return positions;
                }
                else
                {
                    // Fuego is playing elsewhere, we cannot get hints.
                    return new List<Position>();
                }
            }
        }

        public override AIDecision GetHint(AiGameInformation gameInformation)
        {
            if (FuegoSingleton.Instance.CurrentGame == null)
            {
                return FuegoSingleton.Instance.GetIsolatedHint(this, gameInformation);
            }
            else
            {
                if (FuegoSingleton.Instance.CurrentGame.Info.Equals(gameInformation.GameInfo))
                {
                    return FuegoSingleton.Instance.GetHint(this, gameInformation);
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
            return FuegoSingleton.Instance.RequestMove(this, gameInformation);
        }

        public override void MovePerformed(Move move, GameTree gameTree, GamePlayer informedPlayer, GameInfo info)
        {
            if (_brokenDueToInvalidLaunch) return;
            RequireInitialization(new AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
            if (_isPrimaryPlayer)
            {
                FuegoSingleton.Instance.MovePerformed(
                    new AiGameInformation(info, informedPlayer.Info.Color, informedPlayer, gameTree));
            }
        }

        public override void MoveUndone()
        {
            if (_brokenDueToInvalidLaunch) return;
            if (_isPrimaryPlayer)
            {
                // Initialization is guaranteed.
                FuegoSingleton.Instance.MoveUndone();
            }

        }

        public override void YourMoveWasRejected()
        {
            MoveUndone();
        }

        /// <summary>
        /// Taking effect only once per game, this method initializes the <see cref="FuegoSingleton"/> by setting the correct
        /// game rule parameters (such as chinese vs. japanese). 
        /// This is only used by <see cref="MovePerformed(Move, GameTree, GamePlayer, GameInfo)"/> and <see cref="RequestMove(AiGameInformation)"/> which are only used when this is an actual player in a game.  
        /// </summary>
        /// <param name="aiGameInformation">The ai game information.</param>
        private void RequireInitialization(AiGameInformation aiGameInformation)
        {
            if (!_initialized)
            {
                _initialized = true;
                if (_isPrimaryPlayer)
                {
                    FuegoSingleton.Instance.Initialize(aiGameInformation);
                }
            }
        }

        /// <summary>
        /// This runs once for each Fuego player in a game, but only for players, not the assistant. It determines whether
        /// this <see cref="Fuego"/> instance is  
        /// </summary>
        /// <param name="aiAgent">The ai agent.</param>
        public void Initialize(AiAgent aiAgent)
        {

            var gameState = aiAgent.GameState;

            // Determine primary player
            foreach(var player in gameState.Players)
            {
                // ReSharper disable once UseNullPropagation - for clarity
                if (player.Agent is AiAgent)
                {
                    AiAgent thatAgent = player.Agent as AiAgent;
                    if (thatAgent.AI is Fuego)
                    {
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression - for clarity
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
                if (FuegoSingleton.Instance.CurrentGame != null &&
                    FuegoSingleton.Instance.CurrentGame.Players.All(pl => pl.Agent != aiAgent))
                {
                    // A game is in progress and it doesn't include this AI.
                    _brokenDueToInvalidLaunch = true;
                }
            }

        }
    }
}
