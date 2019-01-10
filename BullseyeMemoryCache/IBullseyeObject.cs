namespace Baxter.Bullseye.MemoryCache
{
    public interface IBullseyeObject
    {
        string Id { get; }
        string Payload { get; }

        string GetId();
        string GetDeviceInfo();
    }
    
}