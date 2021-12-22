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
        static private List<Thread> threadList = new List<Thread>();
        public HostChecker()
        {
            using (var db = new AppDbContext())
                foreach (var item in db.Sites)
                    AddThread(item);
        }
        
        public static void AddThread(SiteSettings siteSettings)
        {
            PrintThreadInfo(siteSettings);

            var newThread = CreateThread(siteSettings);
            newThread.Start();

            threadList.Add(newThread);
        }

        private static Thread CreateThread(SiteSettings siteSettings)
        {
            Thread thread = new Thread(() =>
            {
                while (true)
                {
                    ApiController.CheckHostLocal(siteSettings.Host, siteSettings.Id);

                    var nextCheckTime = DateTime.Now.AddSeconds(siteSettings.IntervalSeconds);

                    using (var db = new AppDbContext())
                    {
                        var item = db.Sites.FirstOrDefault(s => s.Host == siteSettings.Host && s.IntervalSeconds == siteSettings.IntervalSeconds);

                        if (item != null)
                            item.NextCheckingTime = nextCheckTime;

                        db.SaveChanges();
                    }

                    Thread.Sleep(nextCheckTime - DateTime.Now);
                }
            });

            return thread;
        }

        private static void PrintThreadInfo(SiteSettings siteSettings)
        {
            string str = $"ID: {siteSettings.Id} Host: {siteSettings.Host} Interval:{siteSettings.IntervalSeconds}";

            Console.WriteLine($"Added thread: {str}");
        }
    }
}
