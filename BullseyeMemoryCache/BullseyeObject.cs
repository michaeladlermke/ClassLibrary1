using System;

namespace Baxter.Bullseye.MemoryCache
{

    // class for a generic @object Device
    public class BullseyeObject : IBullseyeObject
    {
        public string Id { get; }
        public string Payload { get; }

        public BullseyeObject(string id, string info)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Payload = info ?? throw new ArgumentNullException(nameof(info));
        }

        public string GetId()
        {
            return Id;
        }

        public string GetDeviceInfo()
        {
            return Payload;
        }

        public bool Equals(BullseyeObject other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return Id.Equals(other.Id) && Payload.Equals(other.Payload);
        }
        
    }
}