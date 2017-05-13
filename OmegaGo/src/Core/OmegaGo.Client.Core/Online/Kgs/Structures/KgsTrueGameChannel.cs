using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    /// <summary>
    /// Represents a game channel of which we are certain that it is a game of Go, not a challenge.
    /// </summary>
    public class KgsTrueGameChannel : KgsGameChannel
    {
        private KgsGameInfo _gameInfo;

        private KgsTrueGameChannel(int channelId, KgsGameInfo gameInfo) : base(channelId)
        {
            this._gameInfo = gameInfo;
        }

        public KgsGameInfo GameInfo => _gameInfo;
        
        public override string ToString()
        {
            return _gameInfo.ToString();
        }

        public override void UpdateFrom(GameChannel gameChannel)
        {
            _gameInfo = CreateGameInfo(gameChannel);
        }

        public static KgsTrueGameChannel FromChannel(GameChannel channel)
        {
            var gameInfo = CreateGameInfo(channel);
            if (gameInfo != null)
            {
                return new KgsTrueGameChannel(channel.ChannelId, gameInfo);
            }
            return null;
        }

        private static KgsGameInfo CreateGameInfo(GameChannel channel)
        {
            if (channel.GameType != GameType.Free &&
                channel.GameType != GameType.Ranked) return null;
            
            var whiteInfo = new PlayerInfo(StoneColor.White, channel.Players["white"].Name,
                channel.Players["white"].Rank ?? "??");
            var blackInfo = new PlayerInfo(StoneColor.Black, channel.Players["black"].Name,
                channel.Players["black"].Rank ?? "??");
            string ruleset = channel.Rules ?? RulesDescription.RulesJapanese;
            if (!KgsHelpers.IsSupportedRuleset(ruleset)) return null;
            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new GameBoardSize(channel.Size),
                KgsHelpers.ConvertRuleset(ruleset),
                channel.Handicap,
                KgsGameInfo.GetHandicapPlacementType(KgsHelpers.ConvertRuleset(ruleset)),
                channel.Komi,
                CountingType.Area,
                channel.ChannelId);
            return kgi;
        }

        public static KgsTrueGameChannel FromGameInfo(KgsGameInfo info, int channelId)
        {
            return new Structures.KgsTrueGameChannel(channelId, info);
        }
    }
}
