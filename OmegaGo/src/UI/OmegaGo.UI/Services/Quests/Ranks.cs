﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.UI.Localization;
using OmegaGo.UI.Services.Localization;

namespace OmegaGo.UI.Services.Quests
{
    /// <summary>
    /// Handles omegaGo singleplayer ranks.
    /// Original spec: https://docs.google.com/document/d/1Jo0s3cziPqFTAbYIf1iwyRebLMAwsrvHI3QhQes9sPI/edit#
    /// </summary>
    internal static class Ranks
    {
        private readonly static int[] RankLines = {100, 600, 1400, 2000, 1000000 };

        private static int PointsToRank(int points)
        {
            for (int i = 0; i < Ranks.RankLines.Length; i++)
            {
                if (points < Ranks.RankLines[i])
                {
                    return i;
                }
            }
            return Ranks.RankLines.Length;
        }
        /// <summary>
        /// Determines whether an increase in points from <paramref name="previously"/> to <paramref name="now"/> will increase the player's rank.
        /// </summary>
        /// <param name="previously">The previous points total.</param>
        /// <param name="now">The new points total.</param>
        /// <returns></returns>
        public static bool AdvancedInRank(int previously, int now)
        {
            int prevRank = PointsToRank(previously);
            int nowRank = PointsToRank(now);
            return nowRank > prevRank;
        }

        /// <summary>
        /// Gets the localized name of the omegaGo rank corresponding to a points total (e.g. "Omega Apprentice").
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        /// <param name="points">The player's points total.</param>
        /// <returns></returns>
        public static string GetRankName(Localizer localizer, int points)
        {
            switch (PointsToRank(points))
            {
                case 0:
                    return localizer.Rank1;
                case 1:
                    return localizer.Rank2;
                case 2:
                    return localizer.Rank3;
                case 3:
                    return localizer.Rank4;
                case 4:
                    return localizer.Rank5;
                default:
                    return "Impossible rank";
            }
        }

        /// <summary>
        /// Gets the number of points the user needs (in total) to become the next omegaGo rank.
        /// </summary>
        public static int GetNextRankPoints(int points)
        {
            int yourRank = PointsToRank(points);
            return Ranks.RankLines[yourRank];
        }

       
    }
}
