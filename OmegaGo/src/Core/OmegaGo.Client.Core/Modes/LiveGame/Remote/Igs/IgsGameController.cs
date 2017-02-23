﻿using System.Linq;
using OmegaGo.Core.Modes.LiveGame.Connectors.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Modes.LiveGame.Phases.Finished;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement.Igs;
using OmegaGo.Core.Modes.LiveGame.Phases.Initialization;
using OmegaGo.Core.Modes.LiveGame.Phases.LifeAndDeath;
using OmegaGo.Core.Modes.LiveGame.Phases.Main;
using OmegaGo.Core.Modes.LiveGame.Phases.Main.Igs;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.Core.Online.Igs;
using OmegaGo.Core.Online.Igs.Events;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Modes.LiveGame.Remote.Igs
{
    public class IgsGameController : RemoteGameController
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
        public IgsGameController(
            IgsGameInfo gameInfo,
            IRuleset ruleset,
            PlayerPair players,
            IgsConnection serverConnection) :
                base(gameInfo, ruleset, players, serverConnection)
        {
            Info = gameInfo;

            //create and register connector
            IgsConnector = new IgsConnector(this, serverConnection);
            RegisterConnector(IgsConnector);
            InitializeServer(serverConnection);
        }

        /// <summary>
        /// Initializes server
        /// </summary>
        private void InitializeServer(IgsConnection serverConnection)
        {
            serverConnection.RegisterConnector(IgsConnector);
            //TODO Petr : THIS IS NOT IMPLEMENTED!            
            // TODO Petr : Temporary: The following lines will be moved to the common constructor when life/death begins to work
            // for KGS.
            //serverConnection.Events.TimeControlAdjustment += Events_TimeControlAdjustment;
            //serverConnection.StoneRemoval += StoneRemoval;
            //serverConnection.Events.EnterLifeDeath += Events_EnterLifeDeath;
            //serverConnection.GameScoredAndCompleted += GameScoredAndCompleted;
        }

        /// <summary>
        /// IGS game info
        /// </summary>
        internal new IgsGameInfo Info { get; }

        // TODO Petr: where should this be?
        //private void StoneRemoval(object sender, StoneRemovalEventArgs e)
        //{
        //    //LifeDeath_MarkGroupDead(e.DeadPosition);
        //}       

        protected override IGameControllerPhaseFactory PhaseFactory { get; } =
            new GenericPhaseFactory<InitializationPhase, IgsHandicapPlacementPhase, IgsMainPhase, LifeAndDeathPhase, FinishedPhase>();
    }
}