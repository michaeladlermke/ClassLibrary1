using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Baxter.Bullseye.MemoryCache
{
    public class BullseyeMemoryCache : IBullseyeMemoryCache
    {
        private readonly List<string> _cachedDeviceList = new List<string>();
        private readonly Action<IBullseyeDevice> _evictionAction;
        private readonly Action<IBullseyeDevice> _setupAction;
        private readonly Action<IBullseyeDevice> _updateAction;
        private readonly ILogger<BullseyeMemoryCache> Logger;

        public BullseyeMemoryCache(IMemoryCache cache)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Null IMemoryCache is not allowed.");
                Console.WriteLine(e);
                //throw;
            }
        }

        public BullseyeMemoryCache(IMemoryCache cache, Action<IBullseyeDevice> preCallback,
            Action<IBullseyeDevice> updateCallback, Action<IBullseyeDevice> evictionCallback)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
                _setupAction = preCallback ?? throw new ArgumentNullException(nameof(preCallback));
                _updateAction = updateCallback ?? throw new ArgumentNullException(nameof(updateCallback));
                _evictionAction = evictionCallback ?? throw new ArgumentNullException(nameof(evictionCallback));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Null Arguments are not allowed.");
                Console.WriteLine(e);
                //throw;
            }

            /*
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
            */
        }

        public long Count => _cachedDeviceList.Count;
        //private static readonly ILog Logger = LogManager.GetLogger(typeof(BullseyeMemoryCache));

        //todo
        // might want to split into two classes... 
        // insert update delete
        // maintain the list

        public IMemoryCache Cache { get; set; }


        /// <summary>
        ///     Add a Device to the cache
        /// </summary>
        /// <param name="device"> This is a provided IBullseyeDevice </param>
        /// <param name="seconds"> This is the number of seconds the device will remain in the cache </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddDevice(IBullseyeDevice device, int seconds)
        {
            try
            {
                if (seconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds));
                }

                if (device == null)
                {
                    throw new ArgumentNullException(nameof(device));
                }

                var key = device.GetId();

                if (!_cachedDeviceList.Contains(key))
                {
                    if (_setupAction != null)
                    {
                        _cachedDeviceList.Add(key);
                        NewDeviceCallback(device);
                    }
                }
                else
                {
                    UpdateDevice(device, seconds);
                }

                InsertDevice(device, seconds);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        ///     Update a Device in the cache
        /// </summary>
        /// <param name="device"></param>
        /// <param name="seconds"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void UpdateDevice(IBullseyeDevice device, int seconds)
        {
            try
            {
                if (seconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds));
                }

                if (device == null)
                {
                    throw new ArgumentNullException(nameof(device));
                }

                var key = device.GetId();

                if (_cachedDeviceList.Contains(key))
                {
                    if (_updateAction != null)
                    {
                        UpdatedDeviceCallback(device, EvictionReason.Replaced);
                    }
                }
                else
                {
                    ;
                    AddDevice(device, seconds);
                }

                InsertDevice(device, seconds);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        ///     This function is used to get a Device from the cache
        /// </summary>
        /// <param name="key"> The Device key. </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns> This function returns a IBullseyeDevice Device. </returns>
        public IBullseyeDevice GetDevice(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (!Cache.TryGetValue(key, out string cacheEntry))
                {
                    return null;
                }

                var device = new BullseyeDevice(key, cacheEntry);
                return device;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }

            return null;
        }

        /// <summary>
        ///     This function is used to get a Device from the cache using a BullseyeDevice Device
        /// </summary>
        /// <param name="device"> The Device is a BullseyeDevice </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns> This function returns a IBullseyeDevice Device. </returns>
        public IBullseyeDevice GetDevice(IBullseyeDevice device)
        {
            try
            {
                if (device == null)
                {
                    throw new ArgumentNullException(nameof(device));
                }

                var key = device.GetId();
                return GetDevice(key);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }

            return null;
        }

        /// <summary>
        ///     This function is used to remove a Device from the cache based on a string ID
        /// </summary>
        /// <param name="key"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveDevice(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                if (_cachedDeviceList.Contains(key))
                {
                    var device = GetDevice(key);

                    if (_evictionAction != null) RemovedDeviceCallback(device, EvictionReason.Removed);

                    _cachedDeviceList.Remove(key);
                    Cache.Remove(key);
                }
                else
                {
                    Logger.LogDebug(key + " is not in cache and can't be removed.");
                    Console.WriteLine(key + " is not in cache and can't be removed.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        ///     This function is used to remove a Device from the cache based on a IBullseyeDevice Device
        /// </summary>
        /// <param name="device"> Supplied device to be removed from the cache </param>
        /// <exception cref="ArgumentNullException"></exception>
        public void RemoveDevice(IBullseyeDevice device)
        {
            try
            {
                if (device == null)
                {
                    throw new ArgumentNullException(nameof(device));
                }

                var key = device.Id;

                if (_cachedDeviceList.Contains(key))
                {
                    if (_evictionAction != null) RemovedDeviceCallback(device, EvictionReason.Removed);

                    _cachedDeviceList.Remove(key);
                    Cache.Remove(key);
                    Logger.LogDebug(device.Id + " has been removed from the cache.");
                }
                else
                {
                    Logger.LogDebug(device.Id + " is not in the cache and can't be removed.");
                    Console.WriteLine(device.Id + " is not in the cache and can't be removed.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        ///     This function removes all Devices from the cache by creating a new fresh cache.
        /// </summary>
        public void RemoveAllDevices()
        {
            foreach (var key in _cachedDeviceList)
                if (key != null)
                {
                    Cache.Remove(key);
                }

            _cachedDeviceList.Clear();
        }

        /// <summary>
        ///     Add multiple devices at once
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        /// <param name="seconds"> This is the desired expiration time for the list of devices </param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddMultipleDevices(List<IBullseyeDevice> list, int seconds)
        {
            try
            {
                if (list == null)
                {
                    throw new ArgumentNullException(nameof(list));
                }

                if (seconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds));
                }

                foreach (var device in list) AddDevice(device, seconds);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }
        }

        /// <summary>
        ///     This function checks the cache for a list of devices and returns a list of those devices that exist in the cache
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        /// <exception cref="ArgumentNullException"></exception>
        public List<IBullseyeDevice> CheckCacheForMultipleDevices(List<IBullseyeDevice> list)
        {

            var foundDevices = new List<IBullseyeDevice>();

            try
            {
                if (list == null)
                {
                    throw new ArgumentNullException(nameof(list));
                }

                foreach (var device in list)
                {
                    var key = device.Id;
                    if (Cache.TryGetValue(key, out string cacheEntry))
                    {
                        foundDevices.Add(device);
                    }
                }

                return foundDevices;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                Console.WriteLine(e);
                //throw;
            }

            return foundDevices;
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been added to the cache
        /// </summary>
        /// <param name="device"> This is the supplied device added to the cache </param>
        protected virtual void NewDeviceCallback(IBullseyeDevice device)
        {
            _setupAction.Invoke(device);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been updated in the cache
        /// </summary>
        /// <param name="device"> This is the supplied device updated in the cache </param>
        protected virtual void UpdatedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            if (reason == EvictionReason.Replaced) _updateAction.Invoke(device);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been removed from the cache
        /// </summary>
        /// <param name="device"> This is the supplied device removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the device was removed from the cache </param>
        protected virtual void RemovedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            if (reason == EvictionReason.Removed) _evictionAction.Invoke(device);
        }

        /// <summary>
        ///     Once a Device is ready to be put in cache, it's inserted with this method
        /// </summary>
        /// <param name="device"> This is a provided IBullseyeDevice </param>
        /// <param name="seconds"> This is the number of seconds the Device will remain in cache </param>
        protected virtual void InsertDevice(IBullseyeDevice device, int seconds)
        {
            var key = device.GetId();
            var info = device.GetDeviceInfo();
            var result = "";

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                // Set cache entry size by extension method.
                .SetSize(1)
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromSeconds(seconds))
                // Set the eviction callback
                .RegisterPostEvictionCallback(
                    (evictedKey, value, reason, substate) =>
                    {
                        if (reason == EvictionReason.Removed && _evictionAction != null)
                        {
                            result = $"'{evictedKey}' was {reason}";
                            RemovedDeviceCallback(device, reason);
                        }

                        if (reason == EvictionReason.Replaced && _updateAction != null)
                        {
                            result = $"'{evictedKey}' was {reason}";
                            UpdatedDeviceCallback(device, reason);
                        }

                        //Logger.Error(e.ToString()); todo
                        Console.WriteLine(result);
                    });

            // Save data in cache.

            if (!_cachedDeviceList.Contains(key)) _cachedDeviceList.Add(key);

            Cache.Set(key, info, cacheEntryOptions);
        }
    }
}