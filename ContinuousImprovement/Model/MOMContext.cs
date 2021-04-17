﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ContinuousImprovement.Model
{
    public partial class MOMContext : DbContext
    {
        public MOMContext()
        {
        }

        public MOMContext(DbContextOptions<MOMContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CipfComment> CipfComment { get; set; }
        public virtual DbSet<CipfListOfLike> CipfListOfLike { get; set; }
        public virtual DbSet<CipfSuggestion> CipfSuggestion { get; set; }
        public virtual DbSet<CipfUserProfiles> CipfUserProfiles { get; set; }
        public virtual DbSet<EmployeeInfoCrs530> EmployeeInfoCrs530 { get; set; }
        public virtual DbSet<HrweeklyReport> HrweeklyReport { get; set; }
        public virtual DbSet<ProductionDepartment> ProductionDepartment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CipfComment>(entity =>
            {
                entity.ToTable("CIPF_Comment");

                entity.Property(e => e.DateComment).HasColumnType("datetime");

                entity.Property(e => e.SubmitSuggestionDt).HasColumnType("date");

                entity.Property(e => e.SuggestionId).HasMaxLength(50);

                entity.Property(e => e.UserComment).HasMaxLength(50);
            });

            modelBuilder.Entity<CipfListOfLike>(entity =>
            {
                entity.ToTable("CIPF_ListOfLike");

                entity.Property(e => e.SubmitSuggestionDt).HasColumnType("date");

                entity.Property(e => e.UserBeLiked).HasMaxLength(50);

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            modelBuilder.Entity<CipfSuggestion>(entity =>
            {
                entity.ToTable("CIPF_Suggestion");

                entity.Property(e => e.ApproveDt).HasColumnType("datetime");

                entity.Property(e => e.AssignDt).HasColumnType("datetime");

                entity.Property(e => e.Cifno)
                    .HasColumnName("CIFNo")
                    .HasMaxLength(50);

                entity.Property(e => e.CostCenterRecSug).HasMaxLength(50);

                entity.Property(e => e.DeptRecSug).HasMaxLength(50);

                entity.Property(e => e.OwnerCode).HasMaxLength(50);

                entity.Property(e => e.PlanFinishActionDt).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(50);

                entity.Property(e => e.SubmitDate).HasColumnType("datetime");

                entity.Property(e => e.Remark);

                entity.Property(e => e.InputBy);
            });

            modelBuilder.Entity<CipfUserProfiles>(entity =>
            {
                entity.ToTable("CIPF_UserProfiles");

                entity.Property(e => e.CostCenter)
                    .HasColumnName("costCenter")
                    .HasMaxLength(50);

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(50);

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("employeeId")
                    .HasMaxLength(50);

                entity.Property(e => e.FullName).HasColumnName("fullName");
            });

            modelBuilder.Entity<EmployeeInfoCrs530>(entity =>
            {
                entity.ToTable("EmployeeInfoCRS530");

                entity.Property(e => e.CostCenter).HasMaxLength(50);

                entity.Property(e => e.Department).HasMaxLength(50);

                entity.Property(e => e.FullEmployeeId)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.FullName).HasMaxLength(50);

                entity.Property(e => e.Updatetime)
                    .HasColumnName("UPDATETIME")
                    .HasColumnType("datetime");

                entity.Property(e => e.WorkSchedule).HasMaxLength(50);
            });

            modelBuilder.Entity<HrweeklyReport>(entity =>
            {
                entity.ToTable("HRWeeklyReport");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.CostCenter)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DayOfLeave)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Department).IsRequired();

                entity.Property(e => e.EmployeeId).IsRequired();

                entity.Property(e => e.EstOtfirstShift)
                    .HasColumnName("EstOTFirstShift")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.EstOtlastShift)
                    .HasColumnName("EstOTLastShift")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.FullName).IsRequired();

                entity.Property(e => e.JoinDate).HasColumnType("date");

                entity.Property(e => e.OtfirstShift)
                    .HasColumnName("OTFirstShift")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.OtlastShift)
                    .HasColumnName("OTLastShift")
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WorkingDate).HasColumnType("date");

                entity.Property(e => e.WorkingDayStandard)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.WorkingTimeStandard)
                    .HasColumnType("decimal(18, 2)")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ProductionDepartment>(entity =>
            {
                entity.HasKey(e => e.CostCenter);

                entity.Property(e => e.CostCenter)
                    .HasMaxLength(50)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.Department).IsRequired();

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Tpu)
                    .IsRequired()
                    .HasColumnName("TPU");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}