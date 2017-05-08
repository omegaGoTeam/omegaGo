using System;
using OmegaGo.Core.Game;
using OmegaGo.Core.Modes.LiveGame.Phases.HandicapPlacement;
using OmegaGo.Core.Online.Common;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream;
using OmegaGo.Core.Rules;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsGameInfo : RemoteGameInfo
    {
        public readonly int ChannelId;

        public static KgsGameInfo FromGameJoin(KgsGameJoin kgsGameJoin)
        {
            var whiteInfo = new PlayerInfo(StoneColor.White,
                kgsGameJoin.GameSummary.Players["white"].Name,
                kgsGameJoin.GameSummary.Players["white"].Rank);
            var blackInfo = new PlayerInfo(StoneColor.Black,
                kgsGameJoin.GameSummary.Players["black"].Name,
                kgsGameJoin.GameSummary.Players["black"].Rank);
            if (!KgsHelpers.IsSupportedRuleset(kgsGameJoin.Rules.Rules))
            {
                return null;
            }
            var rules = kgsGameJoin.Rules;
            var kgi = new KgsGameInfo(
                whiteInfo,
                blackInfo,
                new Game.GameBoardSize(rules.Size),
                KgsHelpers.ConvertRuleset(rules.Rules),
                rules.Handicap,
                HandicapPlacementType.Free,
                rules.Komi,
                CountingType.Area,
                kgsGameJoin.ChannelId);
            return kgi;
        }

       


        public KgsGameInfo(PlayerInfo whitePlayerInfo, PlayerInfo blackPlayerInfo, GameBoardSize boardSize, RulesetType rulesetType, int numberOfHandicapStones, HandicapPlacementType handicapPlacementType, float komi, CountingType countingType, int channelId) : base(whitePlayerInfo, blackPlayerInfo, boardSize, rulesetType, numberOfHandicapStones, handicapPlacementType, komi, countingType)
        {
            this.ChannelId = channelId;
        }

        public override string ToString()
        {
            return this.White + " vs. " + this.Black + " [" + this.BoardSize + "]";
        }

        public override bool Equals(object obj)
        {
            return obj is KgsGameInfo && this.Equals(obj as KgsGameInfo);
        }

        protected bool Equals(KgsGameInfo other)
        {
            return ChannelId == other.ChannelId;
        }

        public override int GetHashCode()
        {
            return ChannelId;
        }
    }
}