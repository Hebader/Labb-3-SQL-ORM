using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Labb_3_SQL___ORM.Models
{
    public partial class SchoolContext : DbContext
    {
        public SchoolContext()
        {
        }

        public SchoolContext(DbContextOptions<SchoolContext> options)
            : base(options)
        {
        }

      
        public virtual DbSet<Class> Classes { get; set; } = null!;
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Grade> Grades { get; set; } = null!;
        public virtual DbSet<Position> Positions { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<staff> staff { get; set; } = null!;
        public object Staff { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog=School;Integrated Security=True;");
            }
        }
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.ClassId).HasColumnName("ClassID");

                entity.Property(e => e.ClassName).HasMaxLength(20);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.CourseId)
                    .ValueGeneratedNever()
                    .HasColumnName("CourseID");

                entity.Property(e => e.CourseName).HasMaxLength(20);

                entity.Property(e => e.FkstaffId).HasColumnName("FKStaffID");
            });

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.Property(e => e.GradeId)
                    .ValueGeneratedNever()
                    .HasColumnName("GradeID");

                entity.Property(e => e.Date).HasMaxLength(20);

                entity.Property(e => e.FkcourseId).HasColumnName("FKCourseID");

                entity.Property(e => e.FkstudentId).HasColumnName("FKStudentID");

                entity.Property(e => e.Grade1)
                    .HasMaxLength(5)
                    .HasColumnName("Grade");

                entity.Property(e => e.Teacher).HasMaxLength(30);
            });

            modelBuilder.Entity<Position>(entity =>
            {
                entity.Property(e => e.PositionId)
                    .ValueGeneratedNever()
                    .HasColumnName("PositionID");

                entity.Property(e => e.PositionName).HasMaxLength(50);
            });

            modelBuilder.Entity<Student>(entity =>
            {
                
                entity.Property(e => e.StudentId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("StudentID");

                entity.Property(e => e.BirthDate).HasMaxLength(20);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.FkclassId).HasColumnName("FKClassID");

                entity.Property(e => e.LastName).HasMaxLength(50);
            });

            modelBuilder.Entity<staff>(entity =>
            {
                entity.ToTable("Staff");

                entity.Property(e => e.StaffId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("StaffID");

                entity.Property(e => e.FirstName).HasMaxLength(30);

                entity.Property(e => e.FkpositionId).HasColumnName("FKPositionID");

                entity.Property(e => e.LastName).HasMaxLength(30);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
         
    }


   
}
