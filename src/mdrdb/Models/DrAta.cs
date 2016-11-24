using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrAta
    {
        public DrAta()
        {
            DrDr = new HashSet<DrDr>();
        }

        public short Id { get; set; }
        public string Descr { get; set; }

        public virtual ICollection<DrDr> DrDr { get; set; }
    }
}
