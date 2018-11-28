using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System;

namespace BullseyeCacheLibrary
{

    // class for a generic device object
    public class BullseyeDevice : IBullseyeDevice
    {
        public string Id { get; }
        public string Payload { get; }

        public BullseyeDevice(string id, string info)
        {
            Id = id;
            Payload = info;
        }

        public string GetId()
        {
            return Id;
        }

        public string GetDeviceInfo()
        {
            return Payload;
        }

        public string Setup()
        {
            var result = $"'{ Id }':'{ Payload }' is being set up.";
            Console.WriteLine("THINGS ARE BEING DONE BY THE DEVICE " + Id + " TO SET UP.");

            return result;
        }

        public string Evicted(EvictionReason reason)
        {
            var result = $"'{ Id }':'{ Payload }' was evicted because: {reason}";
            Console.WriteLine("THINGS ARE BEING DONE BY DEVICE " + Id + " TO CLOSE IT DOWN.");

            return result;
        }

    }
}