using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OmegaGo.Core.Online.Kgs.Datatypes;
using OmegaGo.Core.Online.Kgs.Downstream.Abstract;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class UserUpdate : KgsInterruptResponse
    {
        public User User { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.EnsureUserExists(User).CopyDataFrom(User);
        }
    }
}
