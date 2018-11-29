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

    }
}