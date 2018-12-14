using System;

namespace Baxter.Bullseye.MemoryCache
{
    public class BullseyeDeviceHelper : IBullseyeDeviceHelper
    {
        public Action<IBullseyeDevice> StartUpAction => Startup;

        public Action<IBullseyeDevice> UpdateAction => Update;
        public Action<IBullseyeDevice> EvictionAction => Evict;

        public void Startup(IBullseyeDevice device)
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the start up helper for device: " + device.Id + " ||||||||||||||||||||||||||||| ");
        }

        public void Update(IBullseyeDevice device)
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the update helper for device: " + device.Id + " ||||||||||||||||||||||||||||| ");
        }

        public void Evict(IBullseyeDevice device)
        { 
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the eviction helper for device: " + device.Id + " ||||||||||||||||||||||||||||| ");
        }
    }
}