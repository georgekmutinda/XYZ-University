using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using XYZUniversityAPI.Domain.Entities;

namespace XYZUniversityAPI.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Client> Clients { get; set; }= null!;
        public DbSet<StudentContact> StudentContacts { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<PaymentChannel> PaymentChannels { get; set; } = null!;
        public DbSet<PaymentType> PaymentTypes { get; set; } = null!;
        


        protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // STUDENT PRIMARY KEY (Admission Number)
        modelBuilder.Entity<Student>()
            .HasKey(s => s.AdmissionNumber);

        // Student selector, must be unique
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentId)
            .IsUnique();

        // Admin → Students (1:M)... 1 admin with many students
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Admin)
            .WithMany(a => a.Students)
            .HasForeignKey(s => s.CreatedBy);

        // Course → Students (1:M)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.Course)
            .WithMany(c => c.Students)
            .HasForeignKey(s => s.CourseId);

        // Student → StudentContacts (1:M)
        modelBuilder.Entity<StudentContact>()
            .HasOne(sc => sc.Student)
            .WithMany(s => s.Contacts)
            .HasForeignKey(sc => sc.AdmissionNumber);
    }

}
}





// how we talk to the db.  ef core  uses the domain entities to create the database tables.
// public DbSet<Student> Students whenever i want to Query the Student table in the database i will use this property Students
// in program.cs we will register this dbcontext with the dependency injection container so that we can use it in our services/repositories