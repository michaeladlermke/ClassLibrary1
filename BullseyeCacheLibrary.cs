using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;

namespace BullseyeCacheLibrary
// make the device an interface called IBullseyeObject with an id as the key

{
    public class BullseyeCacheLibrary
    {
        public MemoryCache Cache { get; set; }
        
        public BullseyeCacheLibrary()
        {
            // NOTES TO FIX:
            // take in two call back a pre and a post and a remove callback 
            // in the setup and evict
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        public BullseyeCacheLibrary(Action preCallback, Action updateCallback, Action EvictionCallback)
        {
            // NOTES TO FIX:
            // take in two call back a pre and a post and a remove callback 
            // in the setup and evict
            Cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });
        }

        /// <summary>
        /// This function is a placeholder for a function call that happens after a device has been added to the cache
        /// </summary>
        /// <param name="device"> This is the supplied device added to the cache </param>
        private void NewDeviceCallback(BullseyeDevice device)
        {
            device.Setup();
        }

        /// <summary>
        /// This function is a placeholder for a function call that happens after a device has been updated in the cache
        /// </summary>
        /// <param name="device"> This is the supplied device updated in the cache </param>
        private void UpdatedDeviceCallback(BullseyeDevice device)
        {
            device.Setup();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"> This is the supplied device removed from the cache </param>
        /// <param name="reason"> This is the supplied reason for why the device was removed from the cache </param>
        private void RemovedDeviceCallback(BullseyeDevice device, EvictionReason reason)
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
                string result;

                // This is where a call to do something to prepare for the insertion of a new device into the cache would happen
                NewDeviceCallback(device);
                

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

                            RemovedDeviceCallback(device, reason);
                            result = $"'{evictedKey}':'{value}' was evicted because: {reason}";
                            Console.WriteLine(result);

                        });

                // Set cache entry size via property.
                // cacheEntryOptions.Size = 1;

                // Save data in cache.
                Cache.Set(key, cacheEntry, cacheEntryOptions); // can get rid of the else and remove this from the if statement
            }
            else //not needed, move the set to outside the if statement
            {
                //this is what happens if the object to be added already exists and needs to be updated
                UpdatedDeviceCallback(device);
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
        /// This function is used to get an object from the cache using a BullseyeDevice object
        /// </summary>
        /// <param name="device"> The object is a BullseyeDevice </param>
        /// <returns> This function returns the object payload. </returns>
        public string GetObject(BullseyeDevice device)
        {
            string key = device.GetId();
            return GetObject(key);
        }

        /// <summary>
        /// This function is used to remove an object from the cache
        /// </summary>
        /// <param name="device"> Supplied device to be removed from the cache </param>
        public void RemoveObject(BullseyeDevice device)
        {
            var key = device.GetId();
            
            if (!Cache.TryGetValue(key, out string cacheEntry))
            {
                Console.WriteLine("This object is not in cache.");
            }
            else
            {
                RemovedDeviceCallback(device, EvictionReason.Removed);  //won't need this
                Cache.Remove(key);
            }
        }

        /// <summary>
        /// This function removes all objects from the cache by creating a new fresh cache.
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
        /// Add multiple devices at once
        /// </summary>
        /// <param name="list"> This is a provided list of BullseyeDevices </param>
        /// <param name="seconds"> This is the desired expiration time for the list of devices </param>
        public void AddMultipleObjects(List<BullseyeDevice> list, int seconds)
        {
            foreach (var device in list)
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
            foreach (var device in list)
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
            var result = $"'{bullseyeId}':'{bullseyeDeviceInfo}' is being set up.";
            Console.WriteLine("THINGS ARE BEING DONE BY THE DEVICE " + bullseyeId + " TO SET UP.");
            
            return result;
        }

        public string Evicted(EvictionReason reason)
        {
            var result = $"'{bullseyeId}':'{bullseyeDeviceInfo}' was evicted because: {reason}";
            Console.WriteLine("THINGS ARE BEING DONE BY DEVICE " + bullseyeId + " TO CLOSE IT DOWN.");
            
            return result;
        }

    }
}