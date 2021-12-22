using System;
using System.Linq;
using System.Collections.Generic;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class History
    {
        public List<PingResult> Notes { get; set; }
        public SiteSettings Site { get; set; }

        public History(List<PingResult> notes, SiteSettings site)
        {
            Notes = notes;
            Site = site;
        }
    }
}
