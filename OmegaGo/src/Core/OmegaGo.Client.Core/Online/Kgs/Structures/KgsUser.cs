using OmegaGo.Core.Online.Kgs.Downstream;

namespace OmegaGo.Core.Online.Kgs
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