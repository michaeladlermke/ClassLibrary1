using System;
using System.Collections.Generic;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Baxter.Bullseye.MemoryCache
{
    public class BullseyeMemoryCache : IBullseyeMemoryCache
    {
        private readonly List<string> _cachedDeviceList = new List<string>();
        public long Count => _cachedDeviceList.Count;

        protected readonly Action<IBullseyeDevice> EvictionAction;
        protected readonly Action<IBullseyeDevice> SetupAction;
        protected readonly Action<IBullseyeDevice> UpdateAction;
        protected readonly ILog Logger = LogManager.GetLogger(typeof(IBullseyeMemoryCache));

        public IMemoryCache Cache { get; set; }
        
        //todo
        // might want to split into two classes... 
        // insert update delete
        // maintain the list
        
        #region Constructors
            
        public BullseyeMemoryCache(IMemoryCache cache)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            }
            catch (Exception e)
            {
                Logger.Error("Null IMemoryCache is not allowed.", e);
                Console.WriteLine(e);
                throw;
            }
        }

        public BullseyeMemoryCache(IMemoryCache cache, Action<IBullseyeDevice> preCallback,
            Action<IBullseyeDevice> updateCallback, Action<IBullseyeDevice> evictionCallback)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
                SetupAction = preCallback ?? throw new ArgumentNullException(nameof(preCallback));
                UpdateAction = updateCallback ?? throw new ArgumentNullException(nameof(updateCallback));
                EvictionAction = evictionCallback ?? throw new ArgumentNullException(nameof(evictionCallback));
            }
            catch (Exception e)
            {
                Logger.Error("Null Arguments are not allowed.", e);
                Console.WriteLine(e);
                throw;
            }
        }
        
        #endregion
        
        #region CreateFunctions

        /// <summary>
        ///     Add a Device to the cache
        /// </summary>
        /// <param name="device"> This is a provided IBullseyeDevice </param>
        /// <param name="seconds"> This is the number of seconds the device will remain in the cache </param>
        /// <exception cref="ArgumentNullException"></exception>
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
                    if (SetupAction != null)
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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
            }
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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region ReadFunctions

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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
            }

            return null;
        }

        #endregion

        #region UpdateFunctions

        /// <summary>
        ///     Update a Device in the cache
        /// </summary>
        /// <param name="device"></param>
        /// <param name="seconds"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
                    if (UpdateAction != null)
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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion

        #region DeleteFunctions


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

                    if (EvictionAction != null) RemovedDeviceCallback(device, EvictionReason.Removed);

                    _cachedDeviceList.Remove(key);
                    Cache.Remove(key);
                }
                else
                {
                    Logger.Info(key + " is not in cache and can't be removed.");
                    Console.WriteLine(key + " is not in cache and can't be removed.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
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
                    if (EvictionAction != null) RemovedDeviceCallback(device, EvictionReason.Removed);

                    _cachedDeviceList.Remove(key);
                    Cache.Remove(key);
                    //Logger.LogDebug(device.Id + " has been removed from the cache.");
                }
                else
                {
                    //Logger.LogDebug(device.Id + " is not in the cache and can't be removed.");
                    Console.WriteLine(device.Id + " is not in the cache and can't be removed.");
                }
            }
            catch (Exception e)
            {
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
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


        #endregion

        #region HelperFunctions

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
                Logger.Error("", e);
                Console.WriteLine(e);
                throw;
            }
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
                        if (reason == EvictionReason.Removed && EvictionAction != null)
                        {
                            result = $"'{evictedKey}' was {reason}";
                            RemovedDeviceCallback(device, reason);
                        }

                        if (reason == EvictionReason.Replaced && UpdateAction != null)
                        {
                            result = $"'{evictedKey}' was {reason}";
                            UpdatedDeviceCallback(device, reason);
                        }

                        Logger.Info(result);
                        Console.WriteLine(result);
                    });

            // Save data in cache.

            if (!_cachedDeviceList.Contains(key)) _cachedDeviceList.Add(key);

            Cache.Set(key, info, cacheEntryOptions);
        }

        #endregion

        #region CallbackFunctions

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been added to the cache
        /// </summary>
        /// <param name="device"> This is the supplied device added to the cache </param>
        protected virtual void NewDeviceCallback(IBullseyeDevice device)
        {
            SetupAction.Invoke(device);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been updated in the cache
        /// </summary>
        /// <param name="device"> This is the supplied device updated in the cache </param>
        protected virtual void UpdatedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            if (reason == EvictionReason.Replaced) UpdateAction.Invoke(device);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a device has been removed from the cache
        /// </summary>
        /// <param name="device"> This is the supplied device removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the device was removed from the cache </param>
        protected virtual void RemovedDeviceCallback(IBullseyeDevice device, EvictionReason reason)
        {
            if (reason == EvictionReason.Removed) EvictionAction.Invoke(device);
        }

        #endregion
        
    }
}