using System;
using System.Collections.Generic;
using BullseyeCacheLibrary;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {

            BullseyeDeviceHelper helper = new BullseyeDeviceHelper();
            BullseyeCache TestCache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);

            Console.WriteLine("Starting cache size: " + TestCache.BullseyeCacheCount());


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


            List<IBullseyeDevice> DeviceList = new List<IBullseyeDevice>();
            DeviceList.Add(dev05);
            DeviceList.Add(dev06);
            DeviceList.Add(dev07);


            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");

            TestCache.AddMultipleObjects(DeviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + TestCache.BullseyeCacheCount());

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Is Device05 in cache: " + TestCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + TestCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + TestCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for one second: " + TestCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device05 in cache: " + TestCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + TestCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + TestCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for five seconds: " + TestCache.BullseyeCacheCount());
            TestCache.RemoveAllObjects();
            Console.WriteLine("New cache size after clearing the list of devices : " + TestCache.BullseyeCacheCount());
            Console.WriteLine("Final cache size: " + TestCache.BullseyeCacheCount());
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");


            Console.WriteLine(dev01.GetId() + " has been created.");
            Console.WriteLine(dev01.GetDeviceInfo());

            //add a bunch of devices to the cache and then clear it out

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Add a bunch of devices to the cache and clear it out");
            Console.WriteLine("----------------------------------------------");
            TestCache.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + TestCache.BullseyeCacheCount());

            TestCache.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all of the devices : " + TestCache.BullseyeCacheCount());

            Console.WriteLine("Final cache size: " + TestCache.BullseyeCacheCount());
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");

            //start adding devices to a fresh cache
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Add four devices to the empty cache and then remove one device");
            Console.WriteLine("----------------------------------------------");
            TestCache.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + TestCache.BullseyeCacheCount());
            TestCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + TestCache.BullseyeCacheCount());
            

            TestCache.RemoveObject(dev04);
            Console.WriteLine("New cache size after removing the fourth device : " + TestCache.BullseyeCacheCount());

            Console.WriteLine("Current cache size: " + TestCache.BullseyeCacheCount());

            
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Test that devices drop out of the cache in correctly timed intervals: Devices will fall off in 1,11,16 seconds");
            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("Current cache size after inserting timed devices: " + TestCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device01 in cache: " + TestCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + TestCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + TestCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for five seconds: " + TestCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Is Device01 in cache: " + TestCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + TestCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + TestCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for ten seconds: " + TestCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(15000);
            Console.WriteLine("Is Device01 in cache: " + TestCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + TestCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + TestCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for fifteen seconds: " + TestCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("Is Device01 in cache: " + TestCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + TestCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + TestCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for twenty seconds: " + TestCache.BullseyeCacheCount());

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Cache should now be empty");
            Console.WriteLine("");


            Console.WriteLine("Final cache size: " + TestCache.BullseyeCacheCount());

            Console.Read();



        }
    }
}
