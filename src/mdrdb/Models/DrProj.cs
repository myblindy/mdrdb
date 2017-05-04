using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrProj
    {
        public DrProj()
        {
            DrDr = new HashSet<DrDr>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string AcName { get; set; }
        public string Customer { get; set; }
        public string LastChange { get; set; }
        public string Nn { get; set; }

        public ICollection<DrDr> DrDr { get; set; }
    }
}
