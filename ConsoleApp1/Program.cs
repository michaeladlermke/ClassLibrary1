using System;
using System.Collections.Generic;
using Baxter.Bullseye.MemoryCache;
using Microsoft.Extensions.Caching.Memory;
using log4net;
using Microsoft.Extensions.Logging;
using ILogger = log4net.Core.ILogger;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MemoryCache cache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 1024
            });

            ILoggerFactory loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger<BullseyeMemoryCache>();

            logger.LogInformation("Bullseye Memory Cache Test");

            BullseyeObjectHelper helper = new BullseyeObjectHelper();
            try
            {
                BullseyeMemoryCache nullCache = new BullseyeMemoryCache(null, logger);
            }
            catch(Exception e)
            {
                Console.WriteLine("Throws exception: " + e);
            }
            
            var testCache = new BullseyeMemoryCache(cache, logger)
            {
                SetupAction = helper.StartUpAction,
                UpdateAction = helper.UpdateAction,
                EvictionAction = helper.EvictionAction
            };


            Console.WriteLine("Starting cache size: " + testCache.Count);


            BullseyeObject dev01 = new BullseyeObject("@object 01", "{ 01 some @object info; @object info here; }");
            BullseyeObject dev02 = new BullseyeObject("@object 02", "{ 02 some @object info; @object info here; }");
            BullseyeObject dev03 = new BullseyeObject("@object 03", "{ 03 some @object info; @object info here; }");
            BullseyeObject dev04 = new BullseyeObject("@object 04", "{ 04 some @object info; @object info here; }");
            BullseyeObject dev05 = new BullseyeObject("@object 05", "{ 05 some @object info; @object info here; }");
            BullseyeObject dev06 = new BullseyeObject("@object 06", "{ 06 some @object info; @object info here; }");
            BullseyeObject dev07 = new BullseyeObject("@object 07", "{ 07 some @object info; @object info here; }");
            BullseyeObject dev08 = new BullseyeObject("@object 08", "{ 08 some @object info; @object info here; }");
            BullseyeObject dev09 = new BullseyeObject("@object 09", "{ 09 some @object info; @object info here; }");
            BullseyeObject dev10 = new BullseyeObject("@object 10", "{ 10 some @object info; @object info here; }");
            BullseyeObject dev10Copy = new BullseyeObject("@object 10", "{ 10 some @object info; @object info here; }");


            Console.WriteLine("Should be true: " + dev10.Equals(dev10Copy));
            Console.WriteLine("Should be true: " + dev10.Id.Equals(dev10Copy.Id));
            Console.WriteLine("Should be true: " + dev10.Payload.Equals(dev10Copy.Payload));
            //Console.WriteLine("Should be true: " + (dev10 == dev10Copy));
            Console.WriteLine("Should be false: " + dev09.Equals(dev10Copy));
            //Console.WriteLine("Should be false: " + (dev09 == dev10Copy));


            List<IBullseyeObject> deviceList = new List<IBullseyeObject> {dev05, dev06, dev07};


            testCache.AddMultipleObjects(deviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + testCache.Count);

            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all devices : " + testCache.Count);

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");

            testCache.AddMultipleObjects(deviceList, 3);
            Console.WriteLine("New cache size after adding a list of devices : " + testCache.Count);

            System.Threading.Thread.Sleep(1000);
            Console.WriteLine("Is Device05 in cache: " + testCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + testCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + testCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for one second: " + testCache.Count);
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device05 in cache: " + testCache.GetObject(dev05));
            Console.WriteLine("Is Device06 in cache: " + testCache.GetObject(dev06));
            Console.WriteLine("Is Device07 in cache: " + testCache.GetObject(dev07));
            Console.WriteLine("New cache size after waiting for five seconds: " + testCache.Count);
            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after clearing the list of devices : " + testCache.Count);
            Console.WriteLine("Final cache size: " + testCache.Count);
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
            Console.WriteLine("New cache size after adding one @object : " + testCache.Count);
            testCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another @object : " + testCache.Count);
            testCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third @object : " + testCache.Count);
            testCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth @object : " + testCache.Count);

            testCache.RemoveAllObjects();
            Console.WriteLine("New cache size after removing all of the devices : " + testCache.Count);

            Console.WriteLine("Final cache size: " + testCache.Count);
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("");
            Console.WriteLine("");

            //start adding devices to a fresh cache
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Add four devices to the empty cache and then remove one @object");
            Console.WriteLine("----------------------------------------------");
            testCache.AddObject(dev01, 1);
            Console.WriteLine("New cache size after adding one @object : " + testCache.Count);
            testCache.AddObject(dev02, 11);
            Console.WriteLine("New cache size after adding another @object : " + testCache.Count);
            testCache.AddObject(dev03, 16);
            Console.WriteLine("New cache size after adding a third @object : " + testCache.Count);
            testCache.AddObject(dev04, 16);
            Console.WriteLine("New cache size after adding a fourth @object : " + testCache.Count);
            

            testCache.RemoveObject(dev04.Id);
            Console.WriteLine("New cache size after removing the fourth @object : " + testCache.Count);

            Console.WriteLine("Current cache size: " + testCache.Count);

            
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine(" Test that devices drop out of the cache in correctly timed intervals: Devices will fall off in 1,11,16 seconds");
            Console.WriteLine("----------------------------------------------");

            Console.WriteLine("Current cache size after inserting timed devices: " + testCache.Count);
            System.Threading.Thread.Sleep(5000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for five seconds: " + testCache.Count);
            System.Threading.Thread.Sleep(10000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for ten seconds: " + testCache.Count);
            System.Threading.Thread.Sleep(15000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for fifteen seconds: " + testCache.Count);
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("Is Device01 in cache: " + testCache.GetObject(dev01));
            Console.WriteLine("Is Device02 in cache: " + testCache.GetObject(dev02));
            Console.WriteLine("Is Device03 in cache: " + testCache.GetObject(dev03));
            Console.WriteLine("New cache size after waiting for twenty seconds: " + testCache.Count);

            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("----------------------------------------------");
            Console.WriteLine("Cache should now be empty");
            Console.WriteLine("");


            Console.WriteLine("Final cache size: " + testCache.Count);

            Console.Read();



        }
    }
}
