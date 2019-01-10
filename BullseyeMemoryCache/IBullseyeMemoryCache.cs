using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace Baxter.Bullseye.MemoryCache
{
    public interface IBullseyeMemoryCache
    {
        IMemoryCache Cache { get; set; }

        void AddObject(IBullseyeObject @object, int seconds);
        void AddMultipleObjects(List<IBullseyeObject> list, int seconds);
        long Count { get; }

        List<IBullseyeObject> GetMultipleObjects(List<IBullseyeObject> list);
        IBullseyeObject GetObject(IBullseyeObject @object);
        IBullseyeObject GetObject(string key);
        void RemoveAllObjects();
        void RemoveObject(string key);
        void UpdateObject(IBullseyeObject @object, int seconds);
    }
}