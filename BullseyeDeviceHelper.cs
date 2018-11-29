using System;

namespace BullseyeCacheLibrary
{
    public class BullseyeDeviceHelper : IBullseyeDeviceHelper
    {
        public Action StartUpAction => Startup;

        public Action UpdateAction => Update;
        public Action EvictionAction => Evict;

        public void Startup()
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the start up helper ||||||||||||||||||||||||||||| ");
        }

        public void Update()
        {
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the update helper ||||||||||||||||||||||||||||| ");
        }

        public void Evict()
        { 
            Console.WriteLine(" ||||||||||||||||||||||||||||| This is the eviction helper ||||||||||||||||||||||||||||| ");
        }
    }
}