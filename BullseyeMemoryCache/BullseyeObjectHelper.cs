using System;

namespace Baxter.Bullseye.MemoryCache
{
    public class BullseyeObjectHelper : IBullseyeObjectHelper
    {
        public Action<IBullseyeObject> StartUpAction => Startup;

        public Action<IBullseyeObject> UpdateAction => Update;
        public Action<IBullseyeObject> EvictionAction => Evict;

        public void Startup(IBullseyeObject @object)
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the start up helper for @object: " + @object.Id + " ||||||||||||||||||||||||||||| ");
        }

        public void Update(IBullseyeObject @object)
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the update helper for @object: " + @object.Id + " ||||||||||||||||||||||||||||| ");
        }

        public void Evict(IBullseyeObject @object)
        { 
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the eviction helper for @object: " + @object.Id + " ||||||||||||||||||||||||||||| ");
        }
    }
}