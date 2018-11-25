using System;
using System.Collections.Generic;
using ClassLibrary1;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BullseyeCache thing = new BullseyeCache();

            Console.WriteLine("Starting cache size: " + thing.BullseyeCacheCount());


            BullseyeDevice dev01 = new BullseyeDevice("device 01", "{ 01 some device info; device info here; }");
            BullseyeDevice dev02 = new BullseyeDevice("device 02", "{ 02 some device info; device info here; }");
            BullseyeDevice dev03 = new BullseyeDevice("device 03", "{ 03 some device info; device info here; }");
            BullseyeDevice dev04 = new BullseyeDevice("device 04", "{ 04 some device info; device info here; }");
            BullseyeDevice dev05 = new BullseyeDevice("device 05", "{ 05 some device info; device info here; }");
            BullseyeDevice dev06 = new BullseyeDevice("device 06", "{ 06 some device info; device info here; }");
            BullseyeDevice dev07 = new BullseyeDevice("device 07", "{ 07 some device info; device info here; }");
            BullseyeDevice dev08 = new BullseyeDevice("device 08", "{ 08 some device info; device info here; }");
            BullseyeDevice dev09 = new BullseyeDevice("device 09", "{ 09 some device info; device info here; }");
            BullseyeDevice dev10 = new BullseyeDevice("device 10", "{ 10 some device info; device info here; }");

            List<BullseyeDevice> DeviceList = new List<BullseyeDevice>();
            DeviceList.Add(dev05);
            DeviceList.Add(dev06);
            DeviceList.Add(dev07);
            
            thing.AddMultipleObjects(DeviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + thing.BullseyeCacheCount());
            thing.ReturnAllObjectsInCache();
            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Is Device05 in cache: " + thing.GetObject(dev05.GetId()));
            Console.WriteLine("Is Device06 in cache: " + thing.GetObject(dev06.GetId()));
            Console.WriteLine("Is Device07 in cache: " + thing.GetObject(dev07.GetId()));
            Console.WriteLine("New cache size after waiting for one second: " + thing.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device05 in cache: " + thing.GetObject(dev05.GetId()));
            Console.WriteLine("Is Device06 in cache: " + thing.GetObject(dev06.GetId()));
            Console.WriteLine("Is Device07 in cache: " + thing.GetObject(dev07.GetId()));
            Console.WriteLine("New cache size after waiting for five seconds: " + thing.BullseyeCacheCount());
            thing.RemoveAllObjects();
            Console.WriteLine("New cache size after clearing the list of devices : " + thing.BullseyeCacheCount());
            

            Console.WriteLine(dev01.GetId() + " has been created.");
            Console.WriteLine(dev01.GetDeviceInfo());
            
            //add a bunch of devices to the cache and then clear it out
            thing.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + thing.BullseyeCacheCount());
            thing.ReturnAllObjectsInCache();
            thing.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all of the devices : " + thing.BullseyeCacheCount());
            thing.ReturnAllObjectsInCache();
            
            //start adding devices to a fresh cache
            thing.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + thing.BullseyeCacheCount());
            thing.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + thing.BullseyeCacheCount());
            
            thing.ReturnAllObjectsInCache();
            thing.RemoveObject(dev04);
            Console.WriteLine("New cache size after removing the fourth device : " + thing.BullseyeCacheCount());
            thing.ReturnAllObjectsInCache();
            
            Console.WriteLine("Starting cache size after inserting all devices: " + thing.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device01 in cache: " + thing.GetObject(dev01.GetId()));
            Console.WriteLine("Is Device02 in cache: " + thing.GetObject(dev02.GetId()));
            Console.WriteLine("Is Device03 in cache: " + thing.GetObject(dev03.GetId()));
            Console.WriteLine("New cache size after waiting for five seconds: " + thing.BullseyeCacheCount());
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Is Device01 in cache: " + thing.GetObject(dev01.GetId()));
            Console.WriteLine("Is Device02 in cache: " + thing.GetObject(dev02.GetId()));
            Console.WriteLine("Is Device03 in cache: " + thing.GetObject(dev03.GetId()));
            Console.WriteLine("New cache size after waiting for ten seconds: " + thing.BullseyeCacheCount());
            System.Threading.Thread.Sleep(15000);
            Console.WriteLine("Is Device01 in cache: " + thing.GetObject(dev01.GetId()));
            Console.WriteLine("Is Device02 in cache: " + thing.GetObject(dev02.GetId()));
            Console.WriteLine("Is Device03 in cache: " + thing.GetObject(dev03.GetId()));
            Console.WriteLine("New cache size after waiting for fifteen seconds: " + thing.BullseyeCacheCount());
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("Is Device01 in cache: " + thing.GetObject(dev01.GetId()));
            Console.WriteLine("Is Device02 in cache: " + thing.GetObject(dev02.GetId()));
            Console.WriteLine("Is Device03 in cache: " + thing.GetObject(dev03.GetId()));
            Console.WriteLine("New cache size after waiting for twenty seconds: " + thing.BullseyeCacheCount());
            
            
            Console.WriteLine("Final cache size: " + thing.BullseyeCacheCount());





        }
    }
}
