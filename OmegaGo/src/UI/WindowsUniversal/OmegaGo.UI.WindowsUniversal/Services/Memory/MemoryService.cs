using OmegaGo.UI.Services.Memory;
using Windows.System;

namespace OmegaGo.UI.WindowsUniversal.Services.Memory
{
    public sealed class MemoryService : IMemoryService
    {
        public ulong MemoryUsageLimit => MemoryManager.AppMemoryUsageLimit;

        public ulong MemoryUsage => MemoryManager.AppMemoryUsage;
    }
}
