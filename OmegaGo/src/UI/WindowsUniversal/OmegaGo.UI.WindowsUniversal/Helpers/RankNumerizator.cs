using System.Linq;

namespace OmegaGo.UI.WindowsUniversal.Helpers
{
    internal static class RankNumerizator
    {
        /// <summary>
        /// Converts an IGS rank description to an integer, where a lesser integer means a weaker player. The ranks are, in order, NR, then 30k up to 1k, then 1d up to 9d, then 1p up to 9p. Signs after the rank (+ and ?) are ignored. Any other rank is considered to be less than NR.
        /// </summary>
        /// <param name="rank">The rank, for example NR, 17k, 6d+, 5p?.</param>
        /// <returns></returns>
        public static int ConvertRankToInteger(string rank)
        {
            int value;
            rank = rank.Trim();
            if (rank.Last() == '?') rank = rank.Substring(0, rank.Length - 1);
            if (rank.Last() == '+') rank = rank.Substring(0, rank.Length - 1);
            if (rank.Last() == '*') rank = rank.Substring(0, rank.Length - 1);
            rank = rank.ToUpper();
            if (rank == "NR") return 1;
            if (rank.Last() == 'K')
            {
                if (int.TryParse(rank.Substring(0, rank.Length -1), out value))
                {
                    return 40 - value;
                }
                else
                {
                    return 0;
                }
            }
            if (rank.Last() == 'D')
            {
                if (int.TryParse(rank.Substring(0, rank.Length - 1), out value))
                {
                    return 40 + value;
                }
                else
                {
                    return 0;
                }
            }
            if (rank.Last() == 'P')
            {
                if (int.TryParse(rank.Substring(0, rank.Length - 1), out value))
                {
                    return 50 + value;
                }
                else
                {
                    return 0;
                }
            }
            return 0;
        }
    }
}