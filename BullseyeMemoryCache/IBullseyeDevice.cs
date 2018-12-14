namespace Baxter.Bullseye.MemoryCache
{
    public interface IBullseyeDevice
    {
        string Id { get; }
        string Payload { get; }

        string GetId();
        string GetDeviceInfo();
    }
    
}