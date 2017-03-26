using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OmegaGo.Core.AI;
using OmegaGo.Core.AI.FuegoSpace;
using OmegaGo.Core.AI.Joker23.Players;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Players;
using OmegaGo.UI.Services.Settings;

namespace OmegaGo.UI.ViewModels
{
    public class Assistant
    {
        private IGameSettings gameSettings;
        private readonly bool _isOnlineGame;
        private IAIProgram Program;

        public bool ProvidesHints => Program.Capabilities.ProvidesHints
                                     && (gameSettings.Assistant.EnableInOnlineGames || !_isOnlineGame);

        public bool ProvidesFinalEvaluation => Program.Capabilities.ProvidesFinalEvaluation &&
                                               !_isOnlineGame;

        public Assistant(IGameSettings gameSettings, bool isOnlineGame)
        {
            this.gameSettings = gameSettings;
            this._isOnlineGame = isOnlineGame;
            foreach(var program in AISystems.AIPrograms)
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
                Fuego fuego = Program as Fuego;
                fuego.AllowResign = gameSettings.Assistant.FuegoAllowResign;
                fuego.Ponder = gameSettings.Assistant.FuegoPonder;
                fuego.MaxGames = gameSettings.Assistant.FuegoMaxGames;
            }
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
    }
}