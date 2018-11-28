using System;

namespace BullseyeCacheLibrary
{
    public interface IBullseyeDeviceHelper
    {
        Action StartUpAction { get; }
        Action UpdateAction { get; }
        Action EvictionAction { get; }
    }
}