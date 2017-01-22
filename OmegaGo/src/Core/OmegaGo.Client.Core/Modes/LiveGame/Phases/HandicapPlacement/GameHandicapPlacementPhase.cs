﻿using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;

namespace OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement
{
    class GameHandicapPlacementPhase : IGameHandicapPlacementPhase
    {
        public GamePhaseType PhaseType => GamePhaseType.HandicapPlacement;


        /// <summary>
        /// Called by the IGS connection, this method places fixed handicap stones on the board as a single node in the tree, and
        /// advances the timeline forward. In many respects, this acts as the <see cref="MakeMove(GamePlayer, Move)"/> method, except
        /// that it places multiple stones. 
        /// </summary>
        /// <param name="handicapStones">The number of handicap stones to place.</param>
        /// <exception cref="InvalidOperationException">Handicap stones can't be placed in the middle of a game.</exception>
        public void HandicapPhase_PlaceIgsHandicap(int handicapStones)
        {
            if (_game.NumberOfMovesPlayed != 0)
                throw new InvalidOperationException("Handicap stones can't be placed in the middle of a game.");

            OnDebuggingMessage("Placing " + handicapStones + " handicap stones...");

            _game.NumberOfHandicapStones = handicapStones;
            GameBoard gameBoard = new GameBoard(_game.BoardSize);
            _game.Ruleset.StartHandicapPlacementPhase(ref gameBoard, handicapStones, HandicapPositions.Type.Fixed);
            _game.GameTree.AddMoveToEnd(Move.NoneMove, gameBoard);
            _turnPlayer = _game.White;
            OnTurnPlayerChanged(_turnPlayer);
            OnDebuggingMessage("Asking " + _turnPlayer + " to make a move after handicap placement.");
            _game.NumberOfMovesPlayed++;
            _turnPlayer.Agent.PleaseMakeAMove();
        }

        public void StartPhase()
        {
            
        }
    }
}
