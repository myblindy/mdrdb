using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrEmployee
    {
        public DrEmployee()
        {
            DrHistory = new HashSet<DrHistory>();
        }

        public short Id { get; set; }
        public string Lname { get; set; }
        public string Fname { get; set; }
        public string Mname { get; set; }
        public string Initials { get; set; }
        public string Username { get; set; }
        public bool? Addhistory { get; set; }
        public bool? Addnewdr { get; set; }
        public bool? Viewinternaldr { get; set; }
        public bool? Isactive { get; set; }
        public bool? Viewewohwtengdr { get; set; }
        public bool? Viewewodr { get; set; }

        public ICollection<DrHistory> DrHistory { get; set; }
        public DrEmployee IdNavigation { get; set; }
        public DrEmployee InverseIdNavigation { get; set; }
    }
}
