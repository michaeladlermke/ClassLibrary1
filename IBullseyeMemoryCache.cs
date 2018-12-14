using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace BullseyeCacheLibrary
{
    public interface IBullseyeMemoryCache
    {
        IMemoryCache Cache { get; set; }

        void AddDevice(IBullseyeDevice device, int seconds);
        void AddMultipleDevices(List<IBullseyeDevice> list, int seconds);
        long Count { get; }

        List<IBullseyeDevice> CheckCacheForMultipleDevices(List<IBullseyeDevice> list);
        IBullseyeDevice GetDevice(IBullseyeDevice device);
        IBullseyeDevice GetDevice(string key);
        void RemoveAllDevices();
        void RemoveDevice(IBullseyeDevice device);
        void RemoveDevice(string key);
        void UpdateDevice(IBullseyeDevice device, int seconds);
    }
}