﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NotificationService.Infrastructure;

#nullable disable

namespace NotificationService.Migrations
{
    [DbContext(typeof(NotificationDbContext))]
    [Migration("20250408140711_UpdateNotificationTableToDbo")]
    partial class UpdateNotificationTableToDbo
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Administrator")
                .UseCollation("SQL_Latin1_General_CP1_CI_AS")
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("NotificationService.Domain.Entities.NotificationMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<short>("Channel")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<DateTime?>("LastTriedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Recipient")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.Property<short>("RetryCount")
                        .HasColumnType("smallint");

                    b.Property<DateTime?>("SentAt")
                        .HasColumnType("datetime");

                    b.Property<short>("Status")
                        .HasColumnType("smallint");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(200)");

                    b.HasKey("Id");

                    b.ToTable("NotificationMessages", "dbo");
                });
#pragma warning restore 612, 618
        }
    }
}
