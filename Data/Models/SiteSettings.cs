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
        public int Interval { get; set; } // in seconds
        [Column(TypeName = "datetime2")]
        public DateTime NextCheckingTime { get; set; }

        public SiteSettings(string Host, int Interval, DateTime NextCheckingTime)
        {
            this.Host = Host;
            this.Interval = Interval;
            this.NextCheckingTime = NextCheckingTime;
        }

        public SiteSettings(int Id)
        {
            this.Id = Id;
        }
    }
}
