namespace OmegaGo.UI.Services.Memory
{
    public interface IMemoryService
    {
        /// <summary>
        /// Gets the app's memory usage limit.
        /// </summary>
        ulong MemoryUsageLimit { get; }

        /// <summary>
        /// Gets the app's current memory usage.
        /// </summary>
        ulong MemoryUsage { get; }
    }
}
