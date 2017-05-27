using System;
using System.Collections.Generic;

namespace mdrdb.Models
{
    public partial class DrDr
    {
        public DrDr()
        {
            DrHistory = new HashSet<DrHistory>();
        }

        public int Id { get; set; }
        public int ProjDr { get; set; }
        public string DateRequired { get; set; }
        public string Priority { get; set; }
        public string TestGuide { get; set; }
        public int Project { get; set; }
        public short Ata { get; set; }
        public short? Assigned { get; set; }
        public int? Discrepancyid { get; set; }
        public string Referencedata { get; set; }
        public string Documentversion { get; set; }
        public int? Relationshipid { get; set; }
        public int? Relationshipto { get; set; }
        public int? Documenttypeid { get; set; }
        public bool? Reproducible { get; set; }
        public string Flightcond { get; set; }
        public string Mano { get; set; }
        public string Load { get; set; }
        public string Descr1 { get; set; }
        public string Descr2 { get; set; }
        public DateTime? DueDate { get; set; }
        public string Custsupportref { get; set; }
        public string Softwaretitle { get; set; }
        public string Dateofrepro { get; set; }
        public string Reprodoneby { get; set; }
        public string Reprodescr { get; set; }
        public string Reprotrainingdevice { get; set; }
        public string Reprotechnicalanalysis { get; set; }
        public string Softwarelog { get; set; }
        public string Softwarelogoriginalname { get; set; }
        public string Tags { get; set; }

        public ICollection<DrHistory> DrHistory { get; set; }
        public DrAta AtaNavigation { get; set; }
        public DrProj ProjectNavigation { get; set; }
    }
}
