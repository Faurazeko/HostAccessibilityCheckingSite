using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Http;
using System.Linq;

using Microsoft.AspNetCore;

using HostAccessibilityCheckingSite.Data.Models;
using HostAccessibilityCheckingSite.Controllers;

namespace HostAccessibilityCheckingSite
{
    public class HostChecker
    {
        static private List<Thread> threads = new List<Thread>();
        public HostChecker()
        {
            using (var db = new AppDbContext())
                foreach (var item in db.SiteList)
                    AddThread(item);
        }
        
        public static void AddThread(SiteSettings siteSettings)
        {
            var newThread = new Thread( () =>
            {
                string str = $"ID: {siteSettings.Id} Host: {siteSettings.Host} Interval:{siteSettings.Interval}";

                Console.WriteLine($"Added thread: {str}");

                while (true)
                {
                    ApiController.CheckHostLocal(siteSettings.Host, siteSettings.Id);

                    var nextCheckTime = DateTime.Now.AddSeconds(siteSettings.Interval);

                    using (var db = new AppDbContext())
                    {
                        var item = db.SiteList.FirstOrDefault(s => s.Host == siteSettings.Host && s.Interval == siteSettings.Interval);

                        if (item != null)
                            item.NextCheckingTime = nextCheckTime;

                        db.SaveChanges();
                    }
                    Console.WriteLine($"Pinged: {str}");

                    Thread.Sleep(nextCheckTime - DateTime.Now);
                }
            });

            newThread.Start();

            threads.Add(newThread);
        }
    }
}
