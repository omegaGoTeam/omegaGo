namespace OmegaGo.Core
{
    /// <summary>
    /// Represents basic information about the application that is using the library
    /// </summary>
    public class ApplicationInfo
    {
        /// <summary>
        /// Information about the application
        /// </summary>
        /// <param name="name">Name</param>
        /// <param name="version">Version</param>        
        public ApplicationInfo( string name, string version )
        {
            Name = name;
            Version = version;
        }

        /// <summary>
        /// Application's name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Application's version
        /// </summary>
        public string Version { get; }
    }
}
