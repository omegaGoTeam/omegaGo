using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Ogs
{
    static class OgsConstants
    {
        /// <summary>
        /// The client must send this "ID" whenever it wants to log in a user to OGS. This ID uniquely identifies
        /// our project. If our project somehow goes bad, the administrators of OGS may use this ID to block off
        /// access to OGS from our client.
        /// </summary>
        public const string OmegaGoClientId = "fe9a381fa2dd72d41665";
        /// <summary>
        /// The client must send this "secret" whenever it wants to log in a user to OGS. Yes, it's not really
        /// a secret as anyone can decompile it, but it scares off the least subtle attempts.
        /// </summary>
        public const string OmegaGoClientSecret = "bbd6f707bca1283928110d05d520f77f12b54fde";
    }
}
