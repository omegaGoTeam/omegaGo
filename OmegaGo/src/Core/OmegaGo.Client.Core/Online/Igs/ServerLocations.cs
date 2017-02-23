namespace OmegaGo.Core.Online.Igs
{
    static class ServerLocations
    {
        // The following two hosts are identical.
        public const string IgsPrimary = "igs.joyjoy.net";
        public const string IgsSecondary = "210.155.158.200";
        // The IGS server runs at these ports: they all redirect to the same server application
        public const int IgsPortPrimary = 6969;
        public const int IgsPortSecondary = 7777;
        public const int IgsPortTertiary = 28155;
    }
}
