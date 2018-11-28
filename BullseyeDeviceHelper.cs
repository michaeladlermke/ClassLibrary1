using System;
using Microsoft.AspNetCore.Hosting;

namespace BullseyeCacheLibrary
{
    public class BullseyeDeviceHelper : IBullseyeDeviceHelper
    {
        public Action StartUpAction { get => Startup; }

        public Action UpdateAction { get => Update; }
        public Action EvictionAction { get => Evict; }

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