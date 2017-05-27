using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace mdrdb.Models
{
    public partial class DrdbContext : DbContext
    {
        public virtual DbSet<DrAta> DrAta { get; set; }
        public virtual DbSet<DrDr> DrDr { get; set; }
        public virtual DbSet<DrEmployee> DrEmployee { get; set; }
        public virtual DbSet<DrHistory> DrHistory { get; set; }
        public virtual DbSet<DrProj> DrProj { get; set; }
        public virtual DbSet<DrStatus> DrStatus { get; set; }

        private ILoggerFactory LoggerFactory;

        public DrdbContext(DbContextOptions<DrdbContext> options, ILoggerFactory LoggerFactory) : base(options)
        {
            this.LoggerFactory = LoggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLoggerFactory(LoggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DrAta>(entity =>
            {
                entity.ToTable("dr_ata");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descr)
                    .IsRequired()
                    .HasColumnName("descr")
                    .HasColumnType("char(100)");
            });

            modelBuilder.Entity<DrDr>(entity =>
            {
                entity.ToTable("dr_dr");

                entity.HasIndex(e => new { e.Id, e.ProjDr, e.Project, e.DueDate })
                    .HasName("ix_dr_dr3");

                entity.HasIndex(e => new { e.Id, e.DateRequired, e.TestGuide, e.Assigned, e.Referencedata, e.Documentversion, e.Relationshipid, e.Relationshipto, e.Documenttypeid, e.Reproducible, e.Flightcond, e.Mano, e.Load, e.Descr1, e.Descr2, e.DueDate, e.Custsupportref, e.Softwaretitle, e.Dateofrepro, e.Reprodoneby, e.Reprodescr, e.Reprotrainingdevice, e.Reprotechnicalanalysis, e.Softwarelog, e.Softwarelogoriginalname, e.Project, e.Ata, e.ProjDr, e.Priority, e.Discrepancyid })
                    .HasName("ix_dr_dr_priority_project");

                entity.HasIndex(e => new { e.Id, e.DateRequired, e.TestGuide, e.Ata, e.Assigned, e.Referencedata, e.Documentversion, e.Relationshipid, e.Relationshipto, e.Documenttypeid, e.Reproducible, e.Flightcond, e.Mano, e.Load, e.Descr1, e.Descr2, e.DueDate, e.Custsupportref, e.Softwaretitle, e.Dateofrepro, e.Reprodoneby, e.Reprodescr, e.Reprotrainingdevice, e.Reprotechnicalanalysis, e.Softwarelog, e.Softwarelogoriginalname, e.ProjDr, e.Discrepancyid, e.Project, e.Priority })
                    .HasName("ix_dr_dr");

                entity.HasIndex(e => new { e.Id, e.DateRequired, e.TestGuide, e.Ata, e.Assigned, e.Referencedata, e.Documentversion, e.Relationshipid, e.Relationshipto, e.Documenttypeid, e.Reproducible, e.Flightcond, e.Mano, e.Load, e.Descr1, e.Descr2, e.DueDate, e.Custsupportref, e.Softwaretitle, e.Dateofrepro, e.Reprodoneby, e.Reprodescr, e.Reprotrainingdevice, e.Reprotechnicalanalysis, e.Softwarelog, e.Softwarelogoriginalname, e.Project, e.ProjDr, e.Priority, e.Discrepancyid })
                    .HasName("ix_dr_dr2");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedNever();

                entity.Property(e => e.Assigned).HasColumnName("assigned");

                entity.Property(e => e.Ata)
                    .HasColumnName("ata")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Custsupportref)
                    .HasColumnName("custsupportref")
                    .HasColumnType("nchar(30)");

                entity.Property(e => e.DateRequired)
                    .HasColumnName("date_required")
                    .HasColumnType("char(12)");

                entity.Property(e => e.Dateofrepro)
                    .HasColumnName("dateofrepro")
                    .HasColumnType("nchar(12)");

                entity.Property(e => e.Descr1)
                    .HasColumnName("descr1")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Descr2)
                    .HasColumnName("descr2")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Discrepancyid)
                    .HasColumnName("discrepancyid")
                    .HasDefaultValueSql("((2))");

                entity.Property(e => e.Documenttypeid).HasColumnName("documenttypeid");

                entity.Property(e => e.Documentversion)
                    .HasColumnName("documentversion")
                    .HasColumnType("nchar(10)");

                entity.Property(e => e.DueDate)
                    .HasColumnName("due_date")
                    .HasColumnType("date");

                entity.Property(e => e.Flightcond)
                    .HasColumnName("flightcond")
                    .HasColumnType("nchar(40)");

                entity.Property(e => e.Load)
                    .HasColumnName("load")
                    .HasColumnType("nchar(25)");

                entity.Property(e => e.Mano)
                    .HasColumnName("mano")
                    .HasColumnType("nchar(40)");

                entity.Property(e => e.Priority)
                    .IsRequired()
                    .HasColumnName("priority")
                    .HasColumnType("char(20)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ProjDr).HasColumnName("proj_dr");

                entity.Property(e => e.Project).HasColumnName("project");

                entity.Property(e => e.Referencedata)
                    .HasColumnName("referencedata")
                    .HasColumnType("nchar(200)");

                entity.Property(e => e.Relationshipid).HasColumnName("relationshipid");

                entity.Property(e => e.Relationshipto).HasColumnName("relationshipto");

                entity.Property(e => e.Reprodescr)
                    .HasColumnName("reprodescr")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Reprodoneby)
                    .HasColumnName("reprodoneby")
                    .HasColumnType("nchar(30)");

                entity.Property(e => e.Reproducible).HasColumnName("reproducible");

                entity.Property(e => e.Reprotechnicalanalysis)
                    .HasColumnName("reprotechnicalanalysis")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Reprotrainingdevice)
                    .HasColumnName("reprotrainingdevice")
                    .HasColumnType("nchar(40)");

                entity.Property(e => e.Softwarelog)
                    .HasColumnName("softwarelog")
                    .HasColumnType("nchar(50)");

                entity.Property(e => e.Softwarelogoriginalname)
                    .HasColumnName("softwarelogoriginalname")
                    .HasColumnType("nchar(100)");

                entity.Property(e => e.Softwaretitle)
                    .HasColumnName("softwaretitle")
                    .HasColumnType("nchar(50)");

                entity.Property(e => e.Tags)
                    .HasColumnName("tags")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.TestGuide)
                    .HasColumnName("test_guide")
                    .HasColumnType("char(50)");

                entity.HasOne(d => d.AtaNavigation)
                    .WithMany(p => p.DrDr)
                    .HasForeignKey(d => d.Ata)
                    .HasConstraintName("FK_dr_dr_dr_ata");

                entity.HasOne(d => d.ProjectNavigation)
                    .WithMany(p => p.DrDr)
                    .HasForeignKey(d => d.Project)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dr_dr_dr_proj");
            });

            modelBuilder.Entity<DrEmployee>(entity =>
            {
                entity.ToTable("dr_employee");

                entity.HasIndex(e => new { e.Id, e.Mname, e.Initials, e.Addhistory, e.Addnewdr, e.Viewinternaldr, e.Isactive, e.Viewewohwtengdr, e.Viewewodr, e.Username, e.Lname, e.Fname })
                    .HasName("ix_dr_employee_username")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Addhistory)
                    .HasColumnName("addhistory")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Addnewdr)
                    .HasColumnName("addnewdr")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Fname)
                    .IsRequired()
                    .HasColumnName("fname")
                    .HasColumnType("char(50)");

                entity.Property(e => e.Initials)
                    .HasColumnName("initials")
                    .HasColumnType("char(5)");

                entity.Property(e => e.Isactive)
                    .HasColumnName("isactive")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.Lname)
                    .IsRequired()
                    .HasColumnName("lname")
                    .HasColumnType("char(50)");

                entity.Property(e => e.Mname)
                    .HasColumnName("mname")
                    .HasColumnType("char(20)");

                entity.Property(e => e.Username)
                    .HasColumnName("username")
                    .HasColumnType("char(20)");

                entity.Property(e => e.Viewewodr)
                    .HasColumnName("viewewodr")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Viewewohwtengdr)
                    .HasColumnName("viewewohwtengdr")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Viewinternaldr)
                    .HasColumnName("viewinternaldr")
                    .HasDefaultValueSql("((0))");

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.InverseIdNavigation)
                    .HasForeignKey<DrEmployee>(d => d.Id)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dr_employee_dr_employee");
            });

            modelBuilder.Entity<DrHistory>(entity =>
            {
                entity.HasKey(e => new { e.DrNum, e.HistNum })
                    .HasName("PK_dr_history_1");

                entity.ToTable("dr_history");

                entity.HasIndex(e => new { e.DrNum, e.HistNum, e.Status })
                    .HasName("pk_dr_history")
                    .IsUnique();

                entity.HasIndex(e => new { e.HistNum, e.DrNum, e.AttachmentId })
                    .HasName("ix_dr_history_attachments");

                entity.HasIndex(e => new { e.Date, e.Version, e.Author, e.Descr, e.Assigned, e.Enteredby, e.Fixedfilesgroup, e.Addedfilesgroup, e.Deletedfilesgroup, e.AttachmentId, e.Internal, e.LogUserId, e.Status, e.DrNum, e.HistNum })
                    .HasName("ix_dr_history_drnum_histnum")
                    .IsUnique();

                entity.Property(e => e.DrNum).HasColumnName("dr_num");

                entity.Property(e => e.HistNum).HasColumnName("hist_num");

                entity.Property(e => e.Addedfilesgroup).HasColumnName("addedfilesgroup");

                entity.Property(e => e.Assigned).HasColumnName("assigned");

                entity.Property(e => e.AttachmentId).HasColumnName("attachmentID");

                entity.Property(e => e.Author)
                    .HasColumnName("author")
                    .HasColumnType("char(30)");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Deletedfilesgroup).HasColumnName("deletedfilesgroup");

                entity.Property(e => e.Descr)
                    .HasColumnName("descr")
                    .HasColumnType("varchar(max)");

                entity.Property(e => e.Enteredby)
                    .HasColumnName("enteredby")
                    .HasColumnType("char(30)");

                entity.Property(e => e.Fixedfilesgroup).HasColumnName("fixedfilesgroup");

                entity.Property(e => e.Internal).HasColumnName("internal");

                entity.Property(e => e.LogUserId).HasColumnName("log_user_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.Version)
                    .HasColumnName("version")
                    .HasColumnType("char(50)");

                entity.HasOne(d => d.AssignedNavigation)
                    .WithMany(p => p.DrHistory)
                    .HasForeignKey(d => d.Assigned)
                    .HasConstraintName("FK_dr_history_dr_employee");

                entity.HasOne(d => d.DrNumNavigation)
                    .WithMany(p => p.DrHistory)
                    .HasForeignKey(d => d.DrNum)
                    .HasConstraintName("FK_dr_history_dr_dr");

                entity.HasOne(d => d.StatusNavigation)
                    .WithMany(p => p.DrHistory)
                    .HasForeignKey(d => d.Status)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dr_history_dr_status");

                entity.HasOne(d => d.DrHistoryNavigation)
                    .WithOne(p => p.InverseDrHistoryNavigation)
                    .HasForeignKey<DrHistory>(d => new { d.DrNum, d.HistNum })
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_dr_history_dr_history");
            });

            modelBuilder.Entity<DrProj>(entity =>
            {
                entity.ToTable("dr_proj");

                entity.HasIndex(e => new { e.Id, e.Name, e.AcName, e.Customer, e.Nn })
                    .HasName("ix_dr_proj_nn");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AcName)
                    .HasColumnName("ac_name")
                    .HasColumnType("char(50)");

                entity.Property(e => e.Customer)
                    .IsRequired()
                    .HasColumnName("customer")
                    .HasColumnType("char(20)");

                entity.Property(e => e.LastChange)
                    .HasColumnName("last_change")
                    .HasColumnType("char(10)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasColumnType("char(50)");

                entity.Property(e => e.Nn)
                    .HasColumnName("nn")
                    .HasColumnType("varchar(10)");
            });

            modelBuilder.Entity<DrStatus>(entity =>
            {
                entity.ToTable("dr_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Descr)
                    .HasColumnName("descr")
                    .HasColumnType("char(100)");

                entity.Property(e => e.Sortid).HasColumnName("sortid");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasColumnName("status")
                    .HasColumnType("char(20)");
            });
        }
    }
}