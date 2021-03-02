﻿// <auto-generated />
using System;
using IDP.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace IDP.Infrastructure.Migrations
{
    [DbContext(typeof(IdentityDbContext))]
    [Migration("20210108132829_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("IDP.Domain.UserAggregate.Entities.Claim", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("ClaimId")
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserSubject")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("UserSubject");

                    b.ToTable("Claims","auth");
                });

            modelBuilder.Entity("IDP.Domain.UserAggregate.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("Email")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("HashedPassword")
                        .HasColumnName("HashedPassword")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("Users","auth");
                });

            modelBuilder.Entity("IDP.Domain.UserAggregate.Entities.Claim", b =>
                {
                    b.HasOne("IDP.Domain.UserAggregate.Entities.User", null)
                        .WithMany("Claims")
                        .HasForeignKey("UserSubject")
                        .HasPrincipalKey("Subject")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("IDP.Domain.UserAggregate.Entities.User", b =>
                {
                    b.OwnsOne("IDP.Domain.UserAggregate.ValueObjects.SecurityCode", "SecurityCode", b1 =>
                        {
                            b1.Property<long>("UserId")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("bigint")
                                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                            b1.Property<DateTime?>("IssuedAt")
                                .HasColumnName("SecurityCodeIssuedAt")
                                .HasColumnType("datetime2");

                            b1.Property<string>("Value")
                                .HasColumnName("SecurityCode")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<DateTime?>("_expirationDate")
                                .HasColumnName("SecurityCodeExpirationDate")
                                .HasColumnType("datetime2");

                            b1.HasKey("UserId");

                            b1.ToTable("Users");

                            b1.WithOwner()
                                .HasForeignKey("UserId");
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
