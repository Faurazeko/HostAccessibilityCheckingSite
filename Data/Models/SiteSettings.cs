using System.ComponentModel.DataAnnotations;

using System;

using System.ComponentModel.DataAnnotations.Schema;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class SiteSettings
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Host { get; set; }
        [Required]
        public int IntervalSeconds { get; set; }
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
