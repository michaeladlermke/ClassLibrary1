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
        protected readonly ILogger<IBullseyeMemoryCache> Logger;

        public IMemoryCache Cache { get; set; }
        
        #region Constructors
            
        public BullseyeMemoryCache(IMemoryCache cache, ILogger<IBullseyeMemoryCache> logger)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
                Logger = logger;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogError("Null IMemoryCache is not allowed.");
                throw;
            }
        }

        public BullseyeMemoryCache(IMemoryCache cache, Action<IBullseyeDevice> preCallback,
            Action<IBullseyeDevice> updateCallback, Action<IBullseyeDevice> evictionCallback, ILogger<IBullseyeMemoryCache> logger)
        {
            try
            {
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
                SetupAction = preCallback ?? throw new ArgumentNullException(nameof(preCallback));
                UpdateAction = updateCallback ?? throw new ArgumentNullException(nameof(updateCallback));
                EvictionAction = evictionCallback ?? throw new ArgumentNullException(nameof(evictionCallback));
                Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Logger.LogError("Null arguments are not allowed.");
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
            catch (Exception e) when (e.InnerException is ArgumentNullException)
            {
                Logger.LogError("Null device is not allowed.");
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e) when (e.InnerException is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
            catch (Exception e) when (e.InnerException is ArgumentNullException)
            {
                Logger.LogError("Null list is not allowed.");
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e) when (e.InnerException is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
            if (key == null)
            {
                return null;
            }

            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                return null;
            }

            var device = new BullseyeDevice(key, cacheEntry);
            return device;
        }

        /// <summary>
        ///     This function is used to get a Device from the cache using a BullseyeDevice Device
        /// </summary>
        /// <param name="device"> The Device is a BullseyeDevice </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns> This function returns a IBullseyeDevice Device. </returns>
        public IBullseyeDevice GetDevice(IBullseyeDevice device)
        {
            if (device == null)
            {
                return null;
            }

            var key = device.GetId();
            return GetDevice(key);
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
                    InsertDevice(device, seconds);
                }
                else
                {
                    AddDevice(device, seconds);
                }
            }
            catch (Exception e) when (e.InnerException is ArgumentNullException)
            {
                Logger.LogError("Null device is not allowed.");
                Console.WriteLine(e);
                throw;
            }
            catch (Exception e) when (e.InnerException is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
                    Logger.LogInformation(key + " is not in cache and can't be removed. ");
                    Console.WriteLine(key + " is not in cache and can't be removed.");
                }
            }
            catch (Exception e) when (e.InnerException is ArgumentNullException)
            {
                Logger.LogError("Null key is not allowed.");
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
                    Logger.LogInformation(device.Id + "has been removed from the cache. ");
                }
                else
                {
                    Logger.LogInformation(device.Id + " is not in the cache and can't be removed. ");
                    Console.WriteLine(device.Id + " is not in the cache and can't be removed.");
                }
            }
            catch (Exception e) when (e.InnerException is ArgumentNullException)
            {
                Logger.LogError("Null device is not allowed.");
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

            if (list == null)
            {
                return foundDevices;
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
                        
                        Logger.LogInformation(result);
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