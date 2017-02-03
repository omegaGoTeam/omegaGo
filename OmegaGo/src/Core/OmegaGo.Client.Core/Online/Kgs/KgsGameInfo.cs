using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Downstream;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsGameInfo : RemoteGameInfo
    {
        public int ChannelId;

        public static KgsGameInfo FromGameJoin(GameJoin gameJoin, KgsConnection connection)
        {
            var whiteInfo = new PlayerInfo(StoneColor.White,
                gameJoin.GameSummary.Players["white"].Name,
                gameJoin.GameSummary.Players["white"].Rank);
            var blackInfo = new PlayerInfo(StoneColor.Black,
                gameJoin.GameSummary.Players["black"].Name,
                gameJoin.GameSummary.Players["black"].Rank);
            if (!IsSupportedRuleset(gameJoin.Rules.Rules))
            {
                return null;
            }
            var rules = gameJoin.Rules;
            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new Game.GameBoardSize(rules.Size),
                ConvertRuleset(rules.Rules),
                rules.Handicap,
                HandicapPlacementType.Free,
                rules.Komi,
                CountingType.Area,
                connection,
                gameJoin.ChannelId);
            return kgi;
        }

        private static bool IsSupportedRuleset(string rules)
        {
            return rules == RulesDescription.RulesAga ||
                   rules == RulesDescription.RulesChinese ||
                   rules == RulesDescription.RulesJapanese;
        }

        public static KgsGameInfo FromChannel(GameChannel channel, KgsConnection connection)
        {
            if (channel.GameType == GameType.Challenge) return null;
            // TODO this only works for full games in progress so far, I think
            var whiteInfo = new PlayerInfo(StoneColor.White, channel.Players["white"].Name,
                channel.Players["white"].Rank ?? "??");
            var blackInfo = new PlayerInfo(StoneColor.Black, channel.Players["black"].Name,
                channel.Players["black"].Rank ?? "??");
            if (!IsSupportedRuleset(channel.Rules)) return null;
            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new Game.GameBoardSize(channel.Size),
                ConvertRuleset(channel.Rules),
                channel.Handicap,
                HandicapPlacementType.Free,
                channel.Komi,
                CountingType.Area,
                connection,
                channel.ChannelId);
            return kgi;
        }

        private static RulesetType ConvertRuleset(string rules)
        {
            switch (rules)
            {
                case RulesDescription.RulesAga:
                    return RulesetType.AGA;
                case RulesDescription.RulesChinese:
                    return RulesetType.Chinese;
                case RulesDescription.RulesJapanese:
                    return RulesetType.Japanese;
            }
            throw new Exception("This ruleset is not supported in Omega Go.");
        }

        private KgsGameInfo(PlayerInfo whitePlayerInfo, PlayerInfo blackPlayerInfo, GameBoardSize boardSize, RulesetType rulesetType, int numberOfHandicapStones, HandicapPlacementType handicapPlacementType, float komi, CountingType countingType, KgsConnection connection, int channelId) : base(whitePlayerInfo, blackPlayerInfo, boardSize, rulesetType, numberOfHandicapStones, handicapPlacementType, komi, countingType)
        {
            this.ChannelId = channelId;
            this.KgsConnection = connection;
        }
        public KgsConnection KgsConnection { get; }
        public override string ToString()
        {
            return this.White + " vs. " + this.Black + " [" + this.BoardSize + "]";
        }
    }
}