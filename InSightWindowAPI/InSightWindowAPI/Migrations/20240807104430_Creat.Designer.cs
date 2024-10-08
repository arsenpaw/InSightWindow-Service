﻿// <auto-generated />
using System;
using InSightWindowAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace InSightWindowAPI.Migrations
{
    [DbContext(typeof(UsersContext))]
    [Migration("20240807104430_Creat")]
    partial class Creat
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("InSightWindowAPI.Models.DeviceModel.Device", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("DeviceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Devices");

                    b.HasDiscriminator<string>("DeviceType").HasValue("Device");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("InSightWindowAPI.Models.FireBaseToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("FireBaseTokens");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.RefreshToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpitedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.DeviceModel.BulbTest", b =>
                {
                    b.HasBaseType("InSightWindowAPI.Models.DeviceModel.Device");

                    b.Property<int>("LightPowered")
                        .HasColumnType("int");

                    b.Property<bool>("isOn")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("BulbTest");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.DeviceModel.Window", b =>
                {
                    b.HasBaseType("InSightWindowAPI.Models.DeviceModel.Device");

                    b.Property<bool>("IsOpen")
                        .HasColumnType("bit");

                    b.Property<bool>("IsProtected")
                        .HasColumnType("bit");

                    b.Property<bool>("isAlarm")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("Window");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.DeviceModel.Device", b =>
                {
                    b.HasOne("InSightWindowAPI.Models.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.FireBaseToken", b =>
                {
                    b.HasOne("InSightWindowAPI.Models.User", "User")
                        .WithOne("FireBaseToken")
                        .HasForeignKey("InSightWindowAPI.Models.FireBaseToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.RefreshToken", b =>
                {
                    b.HasOne("InSightWindowAPI.Models.User", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("InSightWindowAPI.Models.RefreshToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.Role", b =>
                {
                    b.HasOne("InSightWindowAPI.Models.User", "User")
                        .WithOne("Role")
                        .HasForeignKey("InSightWindowAPI.Models.Role", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("InSightWindowAPI.Models.User", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("FireBaseToken")
                        .IsRequired();

                    b.Navigation("RefreshToken")
                        .IsRequired();

                    b.Navigation("Role")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
