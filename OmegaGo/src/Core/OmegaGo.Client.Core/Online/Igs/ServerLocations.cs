namespace OmegaGo.Core.Online.Igs
{
    /// <summary>
    /// Holds addresses and ports of the IGS server. I have never encountered a failure of the IGS server so I think we can safely
    /// just use the primary host and port without offering the user a choice. (some clients offer choice, others don't).
    /// </summary>
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
