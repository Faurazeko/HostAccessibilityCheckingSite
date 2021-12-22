﻿using System.ComponentModel.DataAnnotations;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class Relation
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int SiteId { get; set; } 
        public SiteSettings Site { get; set; }


        public Relation(int userId, int siteId)
        {
            UserId = userId;
            SiteId = siteId;
        }

    }
}
