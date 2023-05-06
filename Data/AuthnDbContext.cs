using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Authn.Models;

public class AuthnDbContext : DbContext
{
    public AuthnDbContext(DbContextOptions<AuthnDbContext> options)
        : base(options)
    {
    }

    public DbSet<Authn.Models.AppUser> AppUsers { get; set; } = default!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        ModelBuilderExtensions.Seed(modelBuilder);

        //modelBuilder.Entity<AppUser>(entity =>
        //{
        //    entity.HasKey(e => e.Id);
        //    entity.Property(e => e.Id);
        //    entity.Property(e => e.Provider).HasMaxLength(250);
        //    entity.Property(e => e.NameIdentifier).HasMaxLength(500);
        //    entity.Property(e => e.Username).HasMaxLength(250);
        //    entity.Property(e => e.Password).HasMaxLength(250);
        //    entity.Property(e => e.Email).HasMaxLength(250);
        //    entity.Property(e => e.Firstname).HasMaxLength(250);
        //    entity.Property(e => e.Lastname).HasMaxLength(250);
        //    entity.Property(e => e.Mobile).HasMaxLength(250);
        //    entity.Property(e => e.Roles).HasMaxLength(1000);
        //    entity.HasData(new AppUser
        //    {
        //        Provider = "Cookies",
        //        Id = 1,
        //        Email = "samet.krt@gmail.com",
        //        Username = "samet",
        //        Password = "123",
        //        Firstname = "Samet",
        //        Lastname = "Kurt",
        //        Mobile = "123-123-456",
        //        Roles = "Admin"
        //    });
        //});
    }

    public static class ModelBuilderExtensions
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Provider = "Cookies",
                    Id = 1,
                    Email = "samet.krt@gmail.com",
                    Username = "samet",
                    NameIdentifier = "",
                    Password = "123",
                    Firstname = "Samet",
                    Lastname = "Kurt",
                    Mobile = "123-123-456",
                    Roles = "Admin"
                }
                ) ; 
        }
    }

}

