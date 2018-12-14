using System;

namespace Baxter.Bullseye.MemoryCache
{
    public interface IBullseyeDeviceHelper
    {
        Action<IBullseyeDevice> StartUpAction { get; }
        Action<IBullseyeDevice> UpdateAction { get; }
        Action<IBullseyeDevice> EvictionAction { get; }
    }
}