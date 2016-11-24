using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrHistory
    {
        public int DrNum { get; set; }
        public int HistNum { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string Author { get; set; }
        public string Descr { get; set; }
        public bool Internal { get; set; }
        public int? LogUserId { get; set; }
        public int Status { get; set; }
        public short? Assigned { get; set; }
        public string Enteredby { get; set; }
        public int? Fixedfilesgroup { get; set; }
        public int? Addedfilesgroup { get; set; }
        public int? Deletedfilesgroup { get; set; }
        public int? AttachmentId { get; set; }

        public virtual DrEmployee AssignedNavigation { get; set; }
        public virtual DrDr DrNumNavigation { get; set; }
        public virtual DrStatus StatusNavigation { get; set; }
        public virtual DrHistory DrHistoryNavigation { get; set; }
        public virtual DrHistory InverseDrHistoryNavigation { get; set; }
    }
}
