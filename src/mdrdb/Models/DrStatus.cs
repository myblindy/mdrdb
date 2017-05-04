using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrStatus
    {
        public DrStatus()
        {
            DrHistory = new HashSet<DrHistory>();
        }

        public int Id { get; set; }
        public string Status { get; set; }
        public string Descr { get; set; }
        public int? Sortid { get; set; }

        public ICollection<DrHistory> DrHistory { get; set; }
    }
}
