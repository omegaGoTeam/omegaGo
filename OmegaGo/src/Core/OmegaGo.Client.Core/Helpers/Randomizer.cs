using System;

namespace OmegaGo.Core.Helpers
{
    /// <summary>
    /// Random values provider. Warning - not thread-safe
    /// </summary>
    internal static class Randomizer
    {
        /// <summary>
        /// Shared <see cref="System.Random" /> instance
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Gets next random non-negative number up to a given maximum (exclusive)
        /// </summary>
        /// <param name="maximum">Maximum value (exclusive)</param>
        /// <returns>Random value</returns>
        public static int Next(int maximum)
        {
            if (maximum <= 0) throw new ArgumentOutOfRangeException(nameof(maximum));
            return Randomizer.random.Next(maximum);
        }
    }
}
