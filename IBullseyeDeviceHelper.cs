using System;

namespace BullseyeCacheLibrary
{
    public interface IBullseyeDeviceHelper
    {
        Action<IBullseyeDevice> StartUpAction { get; }
        Action<IBullseyeDevice> UpdateAction { get; }
        Action<IBullseyeDevice> EvictionAction { get; }
    }
}