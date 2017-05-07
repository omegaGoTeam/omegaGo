using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.UI.Services.Settings;
using OmegaGo.Core.Modes.LiveGame.Connectors.UI;
using OmegaGo.Core.Modes.LiveGame;
using OmegaGo.Core.Modes.LiveGame.Phases;
using OmegaGo.Core.Online.Common;

namespace OmegaGo.UI.ViewModels
{
    public sealed class Assistant
    {
        private UiConnector _uiConnector;
        private IGameController _gameController;
        private GameInfo _gameInfo;

        private IGameSettings gameSettings;
        private readonly bool _isOnlineGame;
        private IAIProgram Program;

        public bool ProvidesHints => Program.Capabilities.ProvidesHints
                                     && (gameSettings.Assistant.EnableInOnlineGames || !_isOnlineGame);

        public bool ProvidesFinalEvaluation => Program.Capabilities.ProvidesFinalEvaluation &&
                                               !_isOnlineGame;

        public Assistant(IGameSettings gameSettings, UiConnector uiConnector, IGameController gameController, GameInfo gameInfo)
        {
            _uiConnector = uiConnector;
            _gameController = gameController;
            _gameInfo = gameInfo;

            this.gameSettings = gameSettings;
            this._isOnlineGame = gameInfo is RemoteGameInfo;

            foreach (var program in AISystems.AIPrograms)
            {
                if (program.GetType().Name == gameSettings.Assistant.ProgramName)
                {
                    Program = (IAIProgram)Activator.CreateInstance(program.GetType());
                }
            }

            if (Program == null)
            {
                Program = new RandomPlayerWrapper();
            }

            if (Program is Fluffy)
            {
                Fluffy fluffy = Program as Fluffy;
                fluffy.TreeDepth = gameSettings.Assistant.FluffyDepth;
            }

            if (Program is Fuego)
            {
                Fuego oldFuego = Program as Fuego;
                oldFuego.AllowResign = gameSettings.Assistant.FuegoAllowResign;
                oldFuego.Ponder = gameSettings.Assistant.FuegoPonder;
                oldFuego.MaxGames = gameSettings.Assistant.FuegoMaxGames;
            }

            RegisterHandlers();
        }

        public async Task<AIDecision> Hint(GameInfo gameInfo, GamePlayer player, GameTree tree, StoneColor color)
        {
            var aiTask = Task.Run(() => Program.GetHint(new AiGameInformation(gameInfo, color, player, tree)));

            AIDecision decision = await aiTask;
            return decision;
        }

        public void MoveUndone()
        {
            Program.MoveUndone();
        }

        public void MovePerformed(Move move, AiGameInformation aiGameInformation)
        {
            Program.MovePerformed(move, aiGameInformation.GameTree, aiGameInformation.AiPlayer,
                aiGameInformation.GameInfo);
        }

        public async Task<IEnumerable<Position>> GetDeadPositions()
        {
            return await Program.GetDeadPositions();
        }

        private void RegisterHandlers()
        {
            _uiConnector.MoveWasPerformed += Assistant_uiConnector_MoveWasPerformed;
            _gameController.MoveUndone += Assistant_Controller_MoveUndone;
            _gameController.GamePhaseStarted += Assistant_Controller_GamePhaseStarted;
        }

        private async void Assistant_Controller_GamePhaseStarted(object sender, IGamePhase e)
        {
            if (e.Type == GamePhaseType.LifeDeathDetermination)
            {
                if (ProvidesFinalEvaluation && !_isOnlineGame)
                {
                    var deads = await GetDeadPositions();

                    foreach (var dead in deads)
                    {
                        _uiConnector.RequestLifeDeathKillGroup(dead);
                    }
                }
            }
        }

        private void Assistant_uiConnector_MoveWasPerformed(object sender, Move e)
        {
            //MovePerformed(e,
              //  new AiGameInformation(_gameInfo, e.WhoMoves, _gameController.Players[e.WhoMoves],
                //    _gameController.GameTree));
        }

        private void Assistant_Controller_MoveUndone(object sender, EventArgs e)
        {
            //MoveUndone();
        }
    }
}