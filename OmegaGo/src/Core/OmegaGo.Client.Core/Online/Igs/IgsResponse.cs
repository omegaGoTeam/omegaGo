using System.Collections.Generic;
using System.Linq;
using OmegaGo.Core.Online.Igs.Structures;

namespace OmegaGo.Core.Online.Igs
{
    internal class IgsResponse : List<IgsLine>
    {
        /// <summary>
        /// Gets a value indicating whether at least one line in this response has the IGS code 5.
        /// </summary>
        public bool IsError => this.Any(line => line.Code == IgsCode.Error);

        /// <summary>
        /// If the response contains a game heading (with the IGS code 15), the first such heading is returned. Otherwise, null is returned.
        /// </summary>
        public GameHeading GetGameHeading()
        {
            foreach(IgsLine line in this)
            {
                if (line.Code == IgsCode.Move)
                {
                    var heading = (IgsRegex.ParseGameHeading(line));
                    if (heading != null)
                        return heading;
                }
            }
            return null;
        }
    }
}