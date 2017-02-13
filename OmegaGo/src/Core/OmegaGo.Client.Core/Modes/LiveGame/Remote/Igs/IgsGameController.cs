﻿using System.Linq;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    internal class IgsGameController : RemoteGameController
    {
        /// <summary>
        /// IGS Connector
        /// </summary>
        internal readonly IgsConnector IgsConnector = null;

        /// <summary>
        /// Creates IGS game controller
        /// </summary>
        /// <param name="gameInfo">Game info</param>
        /// <param name="ruleset">Ruleset</param>
        /// <param name="players">Players</param>
        /// <param name="serverConnection">Connection to IGS server</param>
        public IgsGameController(IgsGameInfo gameInfo, IRuleset ruleset, PlayerPair players, IgsConnection serverConnection) : base(gameInfo, ruleset, players, serverConnection)
        {
            Info = gameInfo;

            //create and register connector
            IgsConnector = new IgsConnector(serverConnection, gameInfo, players);
            RegisterConnector(IgsConnector);
            InitializeServer(serverConnection);
        }

        /// <summary>
        /// Initializes server
        /// </summary>
        private void InitializeServer(IgsConnection serverConnection)
        {
            serverConnection.RegisterConnector(IgsConnector);
            //TODO: THIS IS NOT IMPLEMENTED!
            // TODO: (after refactoring) < move to Life/death
            // TODO: Temporary: The following lines will be moved to the common constructor when life/death begins to work
            // for KGS.
            //serverConnection.Events.TimeControlAdjustment += Events_TimeControlAdjustment;
            //serverConnection.IncomingResignation += IncomingResignation;
            //serverConnection.StoneRemoval += StoneRemoval;
            //serverConnection.Events.EnterLifeDeath += Events_EnterLifeDeath;
            //serverConnection.GameScoredAndCompleted += GameScoredAndCompleted;
        }

        /// <summary>
        /// IGS game info
        /// </summary>
        internal new IgsGameInfo Info { get; }

        //private void GameScoredAndCompleted(object sender, GameScoreEventArgs e)
        //{
        //TODO: Implement this
        // TODO this may not be our game (after refactor update)
        //    //((thPhase as LifeAndDeathPhase)).ScoreIt(new Scores()
        //    //{
        //    //    WhiteScore = e.WhiteScore,
        //    //    BlackScore = e.BlackScore
        //    //});
        //}

        //private void StoneRemoval(object sender, StoneRemovalEventArgs e)
        //{
        // TODO may not be our game
        //    //LifeDeath_MarkGroupDead(e.DeadPosition);
        //}

        //private void IncomingResignation(object sender, GamePlayerEventArgs e)
        //{
        //    if (this.Players.Contains(e.Player))
        //    {
        //        Resign(e.Player);
        //    }
        //}

        protected override IGameControllerPhaseFactory PhaseFactory { get; } =
            new GenericPhaseFactory<InitializationPhase, IgsHandicapPlacementPhase, MainPhase, LifeAndDeathPhase, FinishedPhase>();
    }
}
