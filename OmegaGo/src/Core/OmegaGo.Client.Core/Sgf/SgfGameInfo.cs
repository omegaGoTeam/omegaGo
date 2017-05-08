using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Game;
using OmegaGo.Core.Sgf.Properties;

namespace OmegaGo.Core.Sgf
{
    /// <summary>
    /// SGF Game Info
    /// </summary>
    public class SgfGameInfo
    {
        /// <summary>
        /// Stored game info properties
        /// </summary>
        private readonly IDictionary<string, SgfProperty> _gameInfoProperties;

        /// <summary>
        /// Creates SGF game info
        /// </summary>
        /// <param name="gameInfoProperties">Array of Gameinfo properties</param>
        public SgfGameInfo(params SgfProperty[] gameInfoProperties)
        {
            if (gameInfoProperties == null) throw new ArgumentNullException(nameof(gameInfoProperties));
            _gameInfoProperties = gameInfoProperties.ToDictionary(p => p.Identifier, p => p);
        }

        public SgfProperty Annotation => this["AN"];

        public SgfProperty BlackRank => this["BR"];

        public SgfProperty BlackTeam => this["BT"];

        public SgfProperty Copyright => this["CP"];

        public SgfProperty Date => this["DT"];

        public SgfProperty Event => this["EV"];

        public SgfProperty GameComment => this["GC"];

        public SgfProperty GameName => this["GN"];

        public SgfProperty Handicap => this["HA"];

        public SgfProperty Komi => this["KM"];

        public SgfProperty Opening => this["ON"];

        public SgfProperty Overtime => this["OT"];

        public SgfProperty PlayerBlack => this["PB"];

        public SgfProperty PlayerWhite => this["PW"];

        public SgfProperty Result => this["RE"];

        public SgfProperty Round => this["RO"];

        public SgfProperty Rules => this["RU"];

        public SgfProperty Source => this["SO"];

        public SgfProperty Timelimit => this["TM"];

        public SgfProperty User => this["US"];

        public SgfProperty WhiteRank => this["WR"];

        public SgfProperty WhiteTeam => this["WT"];

        /// <summary>
        /// Converts the SGF game info to GameInfo
        /// </summary>
        /// <returns></returns>
        public GameInfo ToGameInfo()
        {

            var whitePlayerInfo = new PlayerInfo(StoneColor.White, PlayerWhite?.Value<string>(),
                WhiteRank?.Value<string>());
            var blackPlayerInfo = new PlayerInfo(StoneColor.Black, PlayerBlack?.Value<string>(),
                BlackRank?.Value<string>());

            var gameInfo = new GameInfo(whitePlayerInfo, blackPlayerInfo);
            gameInfo.NumberOfHandicapStones = Handicap?.Value<int>() ?? 0;
            gameInfo.Komi = (float?)(Komi?.Value<decimal>()) ?? 0.0f;
            gameInfo.Name = GameName?.Value<string>() ?? "";
            gameInfo.Copyright = Copyright?.Value<string>() ?? "";
            gameInfo.Date = Date?.Value<string>() ?? "";
            gameInfo.Comment = GameComment?.Value<string>() ?? "";
            return gameInfo;
        }

        /// <summary>
        /// Retrieves a Game info property by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public SgfProperty this[string identifier]
        {
            get
            {
                SgfProperty property;
                return _gameInfoProperties.TryGetValue(identifier, out property) ? property : null;
            }
        }
    }
}
