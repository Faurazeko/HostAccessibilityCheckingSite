using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class PingResult
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int SiteId { get; set; }
        public SiteSettings Site { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime Time { get; set; }
        [Required]
        public string Status { get; set; }


        public PingResult(DateTime time, string status)
        {
            Time = time;
            Status = status;
        }

        public PingResult(int siteId, DateTime time, string status)
        {
            SiteId = siteId;
            Time = time;
            Status = status;
        }

        public PingResult(int id, int siteId, DateTime time, string status)
        {
            Id = id;
            SiteId = siteId;
            Time = time;
            Status = status;
        }
    }
}
