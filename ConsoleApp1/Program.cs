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
            BullseyeCache testCache = new BullseyeCache(helper.StartUpAction, helper.UpdateAction, helper.EvictionAction);

            Console.WriteLine("Starting cache size: " + testCache.BullseyeCacheCount());


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
            BullseyeDevice dev10Copy = new BullseyeDevice("device 10", "{ 10 some device info; device info here; }");


            Console.WriteLine("Should be true: " + dev10.Equals(dev10Copy));
            Console.WriteLine("Should be true: " + dev10.Id.Equals(dev10Copy.Id));
            Console.WriteLine("Should be true: " + dev10.Payload.Equals(dev10Copy.Payload));
            //Console.WriteLine("Should be true: " + (dev10 == dev10Copy));
            Console.WriteLine("Should be false: " + dev09.Equals(dev10Copy));
            //Console.WriteLine("Should be false: " + (dev09 == dev10Copy));


            List<IBullseyeDevice> deviceList = new List<IBullseyeDevice> {dev05, dev06, dev07};


            testCache.AddMultipleObjects(deviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + testCache.BullseyeCacheCount());

            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all devices : " + testCache.BullseyeCacheCount());

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");

            testCache.AddMultipleObjects(deviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + testCache.BullseyeCacheCount());

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Is Device05 in cache: " + testCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + testCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + testCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for one second: " + testCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device05 in cache: " + testCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + testCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + testCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for five seconds: " + testCache.BullseyeCacheCount());
            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after clearing the list of devices : " + testCache.BullseyeCacheCount());
            Console.WriteLine("Final cache size: " + testCache.BullseyeCacheCount());
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
            testCache.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + testCache.BullseyeCacheCount());

            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all of the devices : " + testCache.BullseyeCacheCount());

            Console.WriteLine("Final cache size: " + testCache.BullseyeCacheCount());
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");

            //start adding devices to a fresh cache
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Add four devices to the empty cache and then remove one device");
            Console.WriteLine("----------------------------------------------");
            testCache.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third device : " + testCache.BullseyeCacheCount());
            testCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth device : " + testCache.BullseyeCacheCount());
            

            testCache.RemoveObject(dev04);
            Console.WriteLine("New cache size after removing the fourth device : " + testCache.BullseyeCacheCount());

            Console.WriteLine("Current cache size: " + testCache.BullseyeCacheCount());

            
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Test that devices drop out of the cache in correctly timed intervals: Devices will fall off in 1,11,16 seconds");
            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("Current cache size after inserting timed devices: " + testCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for five seconds: " + testCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for ten seconds: " + testCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(15000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for fifteen seconds: " + testCache.BullseyeCacheCount());
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for twenty seconds: " + testCache.BullseyeCacheCount());

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Cache should now be empty");
            Console.WriteLine("");


            Console.WriteLine("Final cache size: " + testCache.BullseyeCacheCount());

            Console.Read();



        }
    }
}
