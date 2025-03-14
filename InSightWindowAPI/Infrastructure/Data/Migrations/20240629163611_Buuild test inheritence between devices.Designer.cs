﻿// <auto-generated />

using Infrastructure.Data;
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
    [Migration("20240629163611_Buuild test inheritence between devices")]
    partial class Buuildtestinheritencebetweendevices
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Api.Models.DeviceModel.Device", b =>
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

                    b.HasDiscriminator<string>("DeviceType").HasValue("BulbTest");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Api.Models.RefreshToken", b =>
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

            modelBuilder.Entity("Api.Models.Role", b =>
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

            modelBuilder.Entity("Api.Models.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

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

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Api.Models.DeviceModel.BulbTest", b =>
                {
                    b.HasBaseType("Api.Models.DeviceModel.Device");

                    b.Property<int>("LightPowered")
                        .HasColumnType("int");

                    b.Property<bool>("isOn")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("BulbTest");

                    b.HasData(
                        new
                        {
                            Id = new Guid("e02814f3-4673-4cbb-b022-b3a7133b1a22"),
                            DeviceType = "BulbTest",
                            LightPowered = 24,
                            isOn = true
                        });
                });

            modelBuilder.Entity("Api.Models.DeviceModel.Window", b =>
                {
                    b.HasBaseType("Api.Models.DeviceModel.Device");

                    b.Property<bool>("isOpen")
                        .HasColumnType("bit");

                    b.HasDiscriminator().HasValue("Window");

                    b.HasData(
                        new
                        {
                            Id = new Guid("92381f13-15eb-4ed4-ad57-19b0ef46b57d"),
                            DeviceType = "Window",
                            isOpen = true
                        });
                });

            modelBuilder.Entity("Api.Models.DeviceModel.Device", b =>
                {
                    b.HasOne("Api.Models.User", "User")
                        .WithMany("Devices")
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Api.Models.RefreshToken", b =>
                {
                    b.HasOne("Api.Models.User", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("Api.Models.RefreshToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Api.Models.Role", b =>
                {
                    b.HasOne("Api.Models.User", "User")
                        .WithOne("Role")
                        .HasForeignKey("Api.Models.Role", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Api.Models.User", b =>
                {
                    b.Navigation("Devices");

                    b.Navigation("RefreshToken")
                        .IsRequired();

                    b.Navigation("Role")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
