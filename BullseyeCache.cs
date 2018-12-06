using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace BullseyeCacheLibrary
{
    public class BullseyeCache
    {
        //todo
        // underscore lowercase private 
        // make this class an interface
        // might want to split into two classes... 
        // insert update delete
        // maintain the list

        private readonly Action _setupAction;
        private readonly Action _updateAction;
        private readonly Action _evictionAction;
        private readonly List<string> _cachedObjectList = new List<string>();

        public IMemoryCache Cache { get; set; }

        //todo
        // rename to BullseyeMemoryCache

        public BullseyeCache(IMemoryCache cache)
        {
            Cache = cache;
        }

        public BullseyeCache(Action preCallback, Action updateCallback, Action evictionCallback)
        {
            //todo
            // add the IMemoryCache as a parameter that gets passed into the library

            _setupAction = preCallback;
            _updateAction = updateCallback;
            _evictionAction = evictionCallback;

            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }


        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been added to the cache
        /// </summary>
        /// <param name="device"> This is the supplied device added to the cache </param>
        private void NewDeviceCallback(IBullseyeDevice device)
        {
            _setupAction.Invoke();
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been updated in the cache
        /// </summary>
        /// <param name="device"> This is the supplied device updated in the cache </param>
        private void UpdatedDeviceCallback(IBullseyeDevice device)
        {
            _updateAction.Invoke();
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been removed from the cache
        /// </summary>
        /// <param name="device"> This is the supplied device removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the device was removed from the cache </param>
        private void RemovedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            _evictionAction.Invoke();
        }

        /// <summary>
        ///     Return the number of items contained in the cache
        /// </summary>
        /// <returns>Returns a count of items in the cache</returns>
        public long BullseyeCacheCount()
        {
            return _cachedObjectList.Count;
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
                if (_setupAction != null)
                    NewDeviceCallback(device);
            }
            else
            {
                UpdateObject(device, seconds);
            }

            if (_cachedObjectList != null) _cachedObjectList.Add(key);
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
                if (_updateAction != null)
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
            {
                _cachedObjectList.Remove(key);
                Cache.Remove(key);
            }
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
            foreach (var key in _cachedObjectList)
            {
                if (key != null) Cache.Remove(key);
                //todo
                // use a try to check if it exists and use a try/catch to catch an argument not found exception
            }

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
        protected virtual void InsertObject(IBullseyeDevice device, int seconds)
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

                        if (_evictionAction != null) RemovedDeviceCallback(device, reason);
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