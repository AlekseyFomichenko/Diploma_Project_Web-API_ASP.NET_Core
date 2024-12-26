﻿// <auto-generated />
using System;
using Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Diploma_Project_Web_API_ASP.NET_Core.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241226010716_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity.Role", b =>
                {
                    b.Property<int>("RoleType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("RoleType");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("Password");

                    b.Property<int>("RoleType")
                        .HasColumnType("int");

                    b.Property<int>("RoleType1")
                        .HasColumnType("int");

                    b.HasKey("Id")
                        .HasName("UserEntity");

                    b.HasIndex("Login")
                        .IsUnique();

                    b.HasIndex("RoleType1");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity.UserEntity", b =>
                {
                    b.HasOne("Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleType1")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Diploma_Project_Web_API_ASP.NET_Core.DataStore.Entity.Role", b =>
                {
                    b.Navigation("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
