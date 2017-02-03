using OmegaGo.Core.Game;
using OmegaGo.Core.Online.Kgs.Downstream;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Kgs
{
    public class KgsGameInfo : GameInfo
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

            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new Game.GameBoardSize(gameJoin.GameSummary.Size),
                RulesetType.Japanese,
                0,
                HandicapPlacementType.Fixed,
                0,
                CountingType.Area,
                connection,
                gameJoin.ChannelId); // TODO fix many things here
            return kgi;
        }
        public static KgsGameInfo FromChannel(GameChannel channel, KgsConnection connection)
        {
            if (channel.GameType == GameType.Challenge) return null;
            // TODO this only works for full games in progress so far, I think
            // TODO fix many things here
            var whiteInfo = new PlayerInfo(StoneColor.White, channel.Players["white"].Name,
                channel.Players["white"].Rank ?? "??");
            var blackInfo = new PlayerInfo(StoneColor.Black, channel.Players["black"].Name,
                channel.Players["black"].Rank ?? "??");
            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new Game.GameBoardSize(channel.Size),
                ConvertRuleset(channel.Rules),
                channel.Handicap,
                HandicapPlacementType.Fixed,
                channel.Komi,
                CountingType.Area,
                connection,
                channel.ChannelId);
            return kgi;
        }

        private static RulesetType ConvertRuleset(string rules)
        {
            // TODO
            return RulesetType.Japanese;
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