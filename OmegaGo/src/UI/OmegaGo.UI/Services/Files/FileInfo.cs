namespace OmegaGo.UI.Services.Files
{
    /// <summary>
    /// Basic information about a file
    /// </summary>
    public class FileInfo
    {
        public string Name { get; }
        public string Contents { get; }
        public FileInfo(string name, string contents)
        {
            Name = name;
            Contents = contents;
        }
    }
}