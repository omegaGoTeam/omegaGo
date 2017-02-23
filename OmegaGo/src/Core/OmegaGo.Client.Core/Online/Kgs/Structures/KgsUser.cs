using OmegaGo.Core.Online.Kgs.Datatypes;

namespace OmegaGo.Core.Online.Kgs.Structures
{
    public class KgsUser : User
    {
        public void CopyDataFrom(User user)
        {
            Name = user.Name;
            Flags = user.Flags;
            Rank = user.Rank;
            AuthLevel = user.AuthLevel;
        }
    }
}