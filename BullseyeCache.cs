using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace BullseyeCacheLibrary
{
    public class BullseyeCache
    {
        private readonly Action EvictionAction;
        private readonly Action SetupAction;
        private readonly Action UpdateAction;


        public BullseyeCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public BullseyeCache(Action preCallback, Action updateCallback, Action evictionCallback)
        {
            SetupAction = preCallback;
            UpdateAction = updateCallback;
            EvictionAction = evictionCallback;

            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public MemoryCache Cache { get; set; }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been added to the cache
        /// </summary>
        /// <param name="device"> This is the supplied device added to the cache </param>
        private void NewDeviceCallback(IBullseyeDevice device)
        {
            SetupAction.Invoke();
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been updated in the cache
        /// </summary>
        /// <param name="device"> This is the supplied device updated in the cache </param>
        private void UpdatedDeviceCallback(IBullseyeDevice device)
        {
            UpdateAction.Invoke();
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been removed from the cache
        /// </summary>
        /// <param name="device"> This is the supplied device removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the device was removed from the cache </param>
        private void RemovedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            EvictionAction.Invoke();
        }

        /// <summary>
        ///     Return the number of items contained in the cache
        /// </summary>
        /// <returns>Returns a count of items in the cache</returns>
        public long BullseyeCacheCount()
        {
            return Cache.Count;
        }

        /// <summary>
        ///     Add an object to the cache
        /// </summary>
        /// <param name="device"> This is a provided IBullseyeDevice </param>
        /// <param name="seconds"> This is the number of seconds the device will remain in the cache </param>
        public void AddObject(IBullseyeDevice device, int seconds)
        {
            if (seconds <= 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            var key = device.GetId();

            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                if (SetupAction != null)
                    NewDeviceCallback(device);
            }
            else
            {
                UpdateObject(device, seconds);
            }

            InsertObject(device, seconds);
        }

        /// <summary>
        ///     Update an object in the cache
        /// </summary>
        /// <param name="device"></param>
        /// <param name="seconds"></param>
        public void UpdateObject(IBullseyeDevice device, int seconds)
        {
            if (seconds <= 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            var key = device.GetId();

            if (Cache.TryGetValue(key, out string cacheEntry))
            {
                if (UpdateAction != null)
                    UpdatedDeviceCallback(device);
            }
            else
            {
                AddObject(device, seconds);
            }

            InsertObject(device, seconds);
        }

        /// <summary>
        ///     This function is used to get an object from the cache
        /// </summary>
        /// <param name="key"> The object key. </param>
        /// <returns> This function returns a IBullseyeDevice object. </returns>
        public IBullseyeDevice GetObject(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (!Cache.TryGetValue(key, out string cacheEntry)) return null;

            var device = new BullseyeDevice(key, cacheEntry);
            return device;
        }

        /// <summary>
        ///     This function is used to get an object from the cache using a BullseyeDevice object
        /// </summary>
        /// <param name="device"> The object is a BullseyeDevice </param>
        /// <returns> This function returns a IBullseyeDevice object. </returns>
        public IBullseyeDevice GetObject(IBullseyeDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            var key = device.GetId();
            return GetObject(key);
        }

        /// <summary>
        ///     This function is used to remove an object from the cache based on a string ID
        /// </summary>
        /// <param name="key"></param>
        public void RemoveObject(string key)
        {
            if (Cache.TryGetValue(key, out string cacheEntry))
                Cache.Remove(key);
            else
                Console.WriteLine("This object is not in cache.");
        }

        /// <summary>
        ///     This function is used to remove an object from the cache based on a IBullseyeDevice object
        /// </summary>
        /// <param name="device"> Supplied device to be removed from the cache </param>
        public void RemoveObject(IBullseyeDevice device)
        {
            if (device == null) throw new ArgumentNullException(nameof(device));
            var key = device.GetId();
            RemoveObject(key);
        }

        /// <summary>
        ///     This function removes all objects from the cache by creating a new fresh cache.
        /// </summary>
        public void RemoveAllObjects()
        {
            // is there a cache.removeall type function?


            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        /// <summary>
        ///     Once an object is ready to be put in cache, it's inserted with this method
        /// </summary>
        /// <param name="device"> This is a provided IBullseyeDevice </param>
        /// <param name="seconds"> This is the number of seconds the object will remain in cache </param>
        private void InsertObject(IBullseyeDevice device, int seconds)
        {
            var key = device.GetId();
            var info = device.GetDeviceInfo();

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Set cache entry size by extension method.
                .SetSize(1)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(seconds))
                // Set the eviction callback
                .RegisterPostEvictionCallback(
                    (evictedKey, value, reason, substate) =>
                    {
                        //NOTE: NEED TO ADD AN OPTION FOR UPDATING A DEVICE AS A REASON

                        if (EvictionAction != null) RemovedDeviceCallback(device, reason);
                        var result = $"'{evictedKey}':'{value}' was evicted because: {reason}";
                        Console.WriteLine(result);
                    });

            // Save data in cache.
            Cache.Set(key, info, cacheEntryOptions);
        }

        /// <summary>
        ///     Add multiple devices at once
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        /// <param name="seconds"> This is the desired expiration time for the list of devices </param>
        public void AddMultipleObjects(List<IBullseyeDevice> list, int seconds)
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            if (seconds <= 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            foreach (var device in list) AddObject(device, seconds);
        }

        /// <summary>
        ///     This function checks the cache for a list of devices and returns a list of those devices that exist in the cache
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        public List<IBullseyeDevice> CheckCacheForMultipleObjects(List<IBullseyeDevice> list)
        {
            var foundDevices = new List<IBullseyeDevice>();
            if (list == null) throw new ArgumentNullException(nameof(list));
            foreach (var device in list)
            {
                var key = device.Id;
                if (Cache.TryGetValue(key, out string cacheEntry)) foundDevices.Add(device);
            }

            return foundDevices;
        }
    }
}