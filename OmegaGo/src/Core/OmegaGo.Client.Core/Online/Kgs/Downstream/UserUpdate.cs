using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OmegaGo.Core.Online.Kgs.Downstream
{
    class UserUpdate : KgsInterruptResponse
    {
        public User User { get; set; }
        public override void Process(KgsConnection connection)
        {
            connection.Data.Users[User.Name].CopyDataFrom(User);
        }
    }
}
