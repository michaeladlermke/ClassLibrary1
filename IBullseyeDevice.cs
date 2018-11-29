namespace BullseyeCacheLibrary
{
    public interface IBullseyeDevice
    {
        string Id { get; }
        string Payload { get; }

        string GetId();
        string GetDeviceInfo();
    }
    
}