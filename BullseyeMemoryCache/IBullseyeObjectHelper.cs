using System;

namespace Baxter.Bullseye.MemoryCache
{
    public interface IBullseyeObjectHelper
    {
        Action<IBullseyeObject> StartUpAction { get; }
        Action<IBullseyeObject> UpdateAction { get; }
        Action<IBullseyeObject> EvictionAction { get; }
    }
}