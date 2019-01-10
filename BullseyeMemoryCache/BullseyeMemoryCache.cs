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

        //todo IBullseyeObject will be change to a IBullseyeObject, change '@object' to 'object' across the board
        //todo make the actions public with get/setters, get rid of second constructor
        public Action<IBullseyeObject> EvictionAction { get; set; }
        public Action<IBullseyeObject> SetupAction { get; set; }
        public Action<IBullseyeObject> UpdateAction { get; set; }
        public ILogger<IBullseyeMemoryCache> Logger { get; set; }
        public IMemoryCache Cache { get; set; }
        
        #region Constructors
            
        public BullseyeMemoryCache(IMemoryCache cache, ILogger<IBullseyeMemoryCache> logger)
        {
            try
            {
                Logger = logger ?? throw new ArgumentNullException(nameof(logger));
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
            }
            catch (Exception e)
            {
                Logger?.LogError("Null arguments are not allowed.");
                throw;
            }
        }

        /*
        public BullseyeMemoryCache(IMemoryCache cache, Action<IBullseyeObject> preCallback,
            Action<IBullseyeObject> updateCallback, Action<IBullseyeObject> evictionCallback, ILogger<IBullseyeMemoryCache> logger)
        {
            try
            {
                Logger = logger ?? throw new ArgumentNullException(nameof(logger));
                Cache = cache ?? throw new ArgumentNullException(nameof(cache));
                SetupAction = preCallback ?? throw new ArgumentNullException(nameof(preCallback));
                UpdateAction = updateCallback ?? throw new ArgumentNullException(nameof(updateCallback));
                EvictionAction = evictionCallback ?? throw new ArgumentNullException(nameof(evictionCallback));
            }
            catch (Exception e)
            {
                Logger?.LogError("Null arguments are not allowed.");
                throw;
            }
        }
        */
        
        #endregion
        
        #region CreateFunctions

        /// <summary>
        ///     Add a Device to the cache
        /// </summary>
        /// <param name="object"> This is a provided IBullseyeObject </param>
        /// <param name="seconds"> This is the number of seconds the @object will remain in the cache </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void AddObject(IBullseyeObject @object, int seconds)
        {
            try
            {
                if (seconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds));
                }

                if (@object == null)
                {
                    throw new ArgumentNullException(nameof(@object));
                }

                var key = @object.GetId(); // will be @object.id

                if (!_cachedDeviceList.Contains(key))
                {
                    if (SetupAction != null)
                    {
                        _cachedDeviceList.Add(key);
                        NewObjectCallback(@object);
                    }
                }
                else
                {
                    UpdateObject(@object, seconds);
                }

                InsertObject(@object, seconds);
            }
            catch (Exception e) when (e is ArgumentNullException)
            {
                Logger.LogError("Null @object is not allowed.");
                throw;
            }
            catch (Exception e) when (e is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
        public void AddMultipleObjects(List<IBullseyeObject> list, int seconds)
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

                foreach (var device in list)
                {
                    if (device != null)
                    {
                        AddObject(device, seconds);
                    }
                }
            }
            catch (Exception e) when (e is ArgumentNullException)
            {
                Logger.LogError("Null list is not allowed.");
                throw;
            }
            catch (Exception e) when (e is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
        /// <returns> This function returns a IBullseyeObject Device. </returns>
        public IBullseyeObject GetObject(string key)
        {
            if (key == null)
            {
                return null;
            }

            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                return null;
            }

            var device = new BullseyeObject(key, cacheEntry);
            return device;
        }

        /// <summary>
        ///     This function is used to get a Device from the cache using a BullseyeObject Device
        /// </summary>
        /// <param name="object"> The Device is a BullseyeObject </param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns> This function returns a IBullseyeObject Device. </returns>
        public IBullseyeObject GetObject(IBullseyeObject @object)
        {
            if (@object == null)
            {
                return null;
            }

            var key = @object.GetId();
            return GetObject(key);
        }

        #endregion

        #region UpdateFunctions

        /// <summary>
        ///     Update a Device in the cache
        /// </summary>
        /// <param name="object"></param>
        /// <param name="seconds"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void UpdateObject(IBullseyeObject @object, int seconds)
        {
            try
            {
                if (seconds <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds));
                }

                if (@object == null)
                {
                    throw new ArgumentNullException(nameof(@object));
                }

                var key = @object.GetId();

                if (_cachedDeviceList.Contains(key))
                {
                    InsertObject(@object, seconds); //moved to before the update callback
                    if (UpdateAction != null)
                    {
                        UpdatedObjectCallback(@object, EvictionReason.Replaced);
                    }
                }
                else
                {
                    AddObject(@object, seconds);
                }
            }
            catch (Exception e) when (e is ArgumentNullException)
            {
                Logger.LogError("Null @object is not allowed.");
                throw;
            }
            catch (Exception e) when (e is ArgumentOutOfRangeException)
            {
                Logger.LogError("Non positive seconds are not allowed.");
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
        public void RemoveObject(string key)
        {
            try
            {
                if (key == null)
                {
                    throw new ArgumentNullException(nameof(key));
                }

                
                if (_cachedDeviceList.Contains(key)) 
                {
                    var device = GetObject(key);

                    if (EvictionAction != null) RemovedObjectCallback(device, EvictionReason.Removed);

                    _cachedDeviceList.Remove(key);
                    Cache.Remove(key);
                }
                else
                {
                    Logger.LogInformation(key + " is not in cache and can't be removed. ");
                }
            }
            catch (Exception e) when (e is ArgumentNullException)
            {
                Logger.LogError("Null key is not allowed.");
                throw;
            }
        }

        /// <summary>
        ///     This function removes all Devices from the cache by creating a new fresh cache.
        /// </summary>
        public void RemoveAllObjects()
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
        public List<IBullseyeObject> GetMultipleObjects(List<IBullseyeObject> list)
        {

            var foundDevices = new List<IBullseyeObject>();

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
        /// <param name="object"> This is a provided IBullseyeObject </param>
        /// <param name="seconds"> This is the number of seconds the Device will remain in cache </param>
        protected virtual void InsertObject(IBullseyeObject @object, int seconds)
        {
            var key = @object.GetId();
            var info = @object.GetDeviceInfo();
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
                            RemovedObjectCallback(@object, reason);
                        }

                        if (reason == EvictionReason.Replaced && UpdateAction != null)
                        {
                            result = $"'{evictedKey}' was {reason}";
                            UpdatedObjectCallback(@object, reason);
                        }
                        
                        Logger.LogInformation(result);
                    });

            // Save data in cache.

            if (!_cachedDeviceList.Contains(key)) _cachedDeviceList.Add(key);

            Cache.Set(key, info, cacheEntryOptions);
        }

        #endregion

        #region CallbackFunctions

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a @object has been added to the cache
        /// </summary>
        /// <param name="object"> This is the supplied @object added to the cache </param>
        protected virtual void NewObjectCallback(IBullseyeObject @object)
        {
            SetupAction.Invoke(@object);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a @object has been updated in the cache
        /// </summary>
        /// <param name="object"> This is the supplied @object updated in the cache </param>
        protected virtual void UpdatedObjectCallback(IBullseyeObject @object, EvictionReason reason)
        {
            if (reason == EvictionReason.Replaced) UpdateAction.Invoke(@object);
        }

        /// <summary>
        ///     This function is a placeholder for a function call that happens after a @object has been removed from the cache
        /// </summary>
        /// <param name="object"> This is the supplied @object removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the @object was removed from the cache </param>
        protected virtual void RemovedObjectCallback(IBullseyeObject @object, EvictionReason reason)
        {
            if (reason == EvictionReason.Removed) EvictionAction.Invoke(@object);
        }

        #endregion
        
    }
}