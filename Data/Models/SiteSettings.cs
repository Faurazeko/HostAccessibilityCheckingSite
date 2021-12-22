using System.ComponentModel.DataAnnotations;

using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class SiteSettings
    {
        [Key]
        public int Id { get; set; }
        public string Host { get; set; }
        public int IntervalSeconds { get; set; } // in seconds
        [Column(TypeName = "datetime2")]
        public DateTime NextCheckingTime { get; set; }

        public SiteSettings(string host, int intervalSeconds, DateTime nextCheckingTime)
        {
            Host = host;
            IntervalSeconds = intervalSeconds;
            NextCheckingTime = nextCheckingTime;
        }

        public SiteSettings(int id)
        {
            Id = id;
        }
    }
}
