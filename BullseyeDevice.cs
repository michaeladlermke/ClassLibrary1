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

        public bool Equals(BullseyeDevice other)
        {
            if (other == null) throw new ArgumentNullException(nameof(other));
            return Id.Equals(other.Id) && Payload.Equals(other.Payload);
        }
        
    }
}