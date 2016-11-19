using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Sgf.Properties
{
    public partial class SgfProperty
    {
        /// <summary>
        /// List of valid game info properties in FF4
        /// </summary>
        private static readonly string[] GameInfoProperties = new string[]
        {
            "OT",
            "AN",
            "BT",
            "CP",
            "ON",
            "RU",
            "WT",
            "BR",
            "DT",
            "EV",
            "GC",
            "GN",
            "PB",
            "PC",
            "PW",
            "RE",
            "RO",
            "SO",
            "TM",
            "US",
            "WR",
            "HA",
            "KM",
            "IP",
            "IY",
            "SU",
        };

        /// <summary>
        /// List of valid move properties in FF4
        /// </summary>
        private static readonly string[] MoveProperties = new string[]
        {
            "DO",
            "IT",
            "KO",
            "MN",
            "OB",
            "OW",
            "B",
            "BL",
            "BM",
            "TE",
            "W",
            "WL"
        };

        /// <summary>
        /// List of valid setup properties in FF4
        /// </summary>
        private static readonly string[] SetupProperties = new string[]
        {
            "AB",
            "AE",
            "AW",
            "PL"
        };


        /// <summary>
        /// List of valid root properties in FF4
        /// </summary>
        private static readonly string[] RootProperties = new string[]
        {
            "FF",
            "AP",
            "CA",
            "ST",
            "GM",
            "SZ"
        };

        private static readonly string[] NoTypeProperties = new string[]
        {
            "AR",
            "SQ",
            "CR",
            "DM",
            "HO",
            "LB",
            "LN",
            "MA",
            "TR",
            "UC",
            "C",
            "FG",
            "GB",
            "GW",
            "N",
            "SL",
            "V",
            "DD",
            "AS",
            "PM",
            "SE",
            "TB",
            "TW",
            "VW"
        };

        /// <summary>
        /// List of property identifiers deprecated in FF4
        /// </summary>
        private static readonly string[] DeprecatedProperties = new string[]
        {
            "BS",
            "WS",
            "ID",
            "EL",
            "EX",
            "L",
            "M",
            "LT",
            "OM",
            "OP",
            "SE",
            "SI",
            "OV",
            "CH",
            "SC",
            "TC",
            "RG",
            "CH",
            "SC"
        };
    }
}
