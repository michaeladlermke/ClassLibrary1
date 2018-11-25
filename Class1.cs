using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;

namespace ClassLibrary1

{
    public class BullseyeCache
    {
        public MemoryCache Cache { get; set; }
        public List<object> CacheContents = new List<object>();
        
        public BullseyeCache()
        {
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }
        
        /// <summary>
        /// This callback calls the device's Setup callback functions.
        /// </summary>
        /// <param name="device"></param>
        private void newDeviceCallback(BullseyeDevice device)
        {
            device.Setup();
        }
        
        /// <summary>
        /// This callback calls the device's Update callback functions.
        /// </summary>
        /// <param name="device"></param>
        private void updatedDeviceCallback(BullseyeDevice device)
        {
            device.Setup();
        }
        
        /// <summary>
        /// This function is a placeholder for a function call that happens after a device has been removed from the cache
        /// </summary>
        /// <returns> Returns a string message as a placeholder to explain that something would happen here. </returns>
        private void removedDeviceCallback(BullseyeDevice device, EvictionReason reason)
        {
            device.Evicted(reason);
        }

        /// <summary>
        /// This function is just a way to return the number of items contained in the cache
        /// </summary>
        /// <returns>Returns a count of items in the cache</returns>
        public long BullseyeCacheCount()
        {
            return Cache.Count;
        }

        /// <summary>
        /// This function is used to add an object to the cache
        /// 
        /// </summary>
        /// <param name="key"> The object key </param>
        /// <param name="info"> The payload for the object </param>
        /// <param name="seconds"> Number of seconds for the object to remain in the cache </param>
        ///
        /// 
        public void AddObject(BullseyeDevice device, int seconds)
        {
            string key = device.GetId();
            string info = device.GetDeviceInfo();
            
            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                // Key not in cache, so get data.
                cacheEntry = info;
                string _result;

                // This is where a call to do something to prepare for the insertion of a new device into the cache would happen
                newDeviceCallback(device);
                

                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    // Set cache entry size by extension method.
                    .SetSize(1)
                    // Keep in cache for this time, reset time if accessed.
                    .SetSlidingExpiration(TimeSpan.FromSeconds(seconds))
                    // Set the eviction callback
                    .RegisterPostEvictionCallback(
                        (evictedKey, value, reason, substate) =>
                        {
                            removedDeviceCallback(device, reason);
                            _result = $"'{evictedKey}':'{value}' was evicted because: {reason}";
                            Console.WriteLine(_result);
                        });

                // Set cache entry size via property.
                // cacheEntryOptions.Size = 1;

                // Save data in cache.
                CacheContents.Add(key);
                Cache.Set(key, cacheEntry, cacheEntryOptions);
            }
            else
            {
                //this is what happens if the object to be added already exists and needs to be updated
                updatedDeviceCallback(device);
            }
        }

        /// <summary>
        /// This function is used to get an object from the cache
        /// </summary>
        /// <param name="key"> The object key. </param>
        /// <returns> This function returns the object payload. </returns>
        public string GetObject(string key)
        {
            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                return "This object is not in cache.";
            }
            else
            {
                return cacheEntry;
            }
        }

        /// <summary>
        /// This function removes an object from the cache
        /// </summary>
        /// <param name="key"> The object key. </param>
        public void RemoveObject(BullseyeDevice device)
        {
            string key = device.GetId();
            
            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                Console.WriteLine("This object is not in cache.");
            }
            else
            {
                removedDeviceCallback(device, EvictionReason.Removed);
                CacheContents.Remove(key);
                Cache.Remove(key);
            }
        }

        /// <summary>
        /// This function removes all objects from the cache by creating a new fresh cache.
        /// </summary>
        public void RemoveAllObjects()
        {
            CacheContents.Clear();
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        /// <summary>
        /// Prints the contents of the entire cache to console
        /// </summary>
        public void ReturnAllObjectsInCache()
        {
            
            Console.WriteLine("=========================");
            Console.WriteLine("Current Cache Contents: ");
            
            foreach (string key in CacheContents)
            {
                Console.WriteLine(GetObject(key));
            }
            
            Console.WriteLine("=========================");
        }

        /// <summary>
        /// Add multiple devices at once
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        /// <param name="seconds"> This is the desired expiration time for the list of devices </param>
        public void AddMultipleObjects(List<BullseyeDevice> list, int seconds)
        {
            foreach (BullseyeDevice device in list)
            {
                AddObject(device, seconds);
            }
        }
        
        /// <summary>
        /// This function checks the cache for a list of devices and returns if they exist in cache or not
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        public void CheckCacheForMultipleObjects(List<BullseyeDevice> list)
        {
            foreach (BullseyeDevice device in list)
            {
                GetObject(device.GetId());
            }
        }

    }

    // class for a generic device object
    public class BullseyeDevice
    {
        private string bullseyeId;
        private string bullseyeDeviceInfo;

        public BullseyeDevice(string id, string info)
        {
            bullseyeId = id;
            bullseyeDeviceInfo = info;
        }

        public string GetId()
        {
            return bullseyeId;
        }

        public string GetDeviceInfo()
        {
            return bullseyeDeviceInfo;
        }
        
        public string Setup()
        {
            string _result = $"'{bullseyeId}':'{bullseyeDeviceInfo}' is being set up.";
            Console.WriteLine("THINGS ARE BEING DONE BY THE DEVICE " + bullseyeId + " TO SET UP!!!!!");
            
            return _result;
        }

        public string Evicted(EvictionReason reason)
        {
            string _result = $"'{bullseyeId}':'{bullseyeDeviceInfo}' was evicted because: {reason}";
            Console.WriteLine("THINGS ARE BEING DONE BY DEVICE " + bullseyeId + " TO CLOSE IT DOWN!!!!");
            
            return _result;
        }

    }
}