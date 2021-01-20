﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SchoolManagement.Data.Database;

namespace SchoolManagement.Data.Migrations
{
    [DbContext(typeof(SchoolContext))]
    [Migration("20210120010142_AddGroupMembersLimit")]
    partial class AddGroupMembersLimit
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SchoolManagement.Core.SchoolAggregate.Groups.Group", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("FormTutorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte>("Number")
                        .HasColumnName("Number")
                        .HasColumnType("tinyint");

                    b.Property<Guid>("SchoolId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Sign")
                        .IsRequired()
                        .HasColumnName("Sign")
                        .HasColumnType("nvarchar(4)")
                        .HasMaxLength(4);

                    b.HasKey("Id");

                    b.HasIndex("FormTutorId")
                        .IsUnique()
                        .HasFilter("[FormTutorId] IS NOT NULL");

                    b.HasIndex("SchoolId");

                    b.HasIndex("Id", "Number", "Sign")
                        .IsUnique()
                        .HasName("Index_Code");

                    b.ToTable("Groups","management");
                });

            modelBuilder.Entity("SchoolManagement.Core.SchoolAggregate.Members.Member", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnName("Email")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnName("FirstName")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("Gender")
                        .HasColumnName("Gender")
                        .HasColumnType("int");

                    b.Property<long?>("GroupId")
                        .HasColumnType("bigint");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnName("LastName")
                        .HasColumnType("nvarchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("Role")
                        .HasColumnName("Role")
                        .HasColumnType("int");

                    b.Property<Guid>("SchoolId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("GroupId");

                    b.HasIndex("SchoolId");

                    b.ToTable("Members","management");
                });

            modelBuilder.Entity("SchoolManagement.Core.SchoolAggregate.Schools.School", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(3000)")
                        .HasMaxLength(3000);

                    b.Property<int?>("GroupMembersLimit")
                        .HasColumnType("int");

                    b.Property<string>("LogoId")
                        .HasColumnType("nvarchar(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(500)")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.ToTable("Schools","management");
                });

            modelBuilder.Entity("SchoolManagement.Core.SchoolAggregate.Groups.Group", b =>
                {
                    b.HasOne("SchoolManagement.Core.SchoolAggregate.Members.Member", "FormTutor")
                        .WithOne()
                        .HasForeignKey("SchoolManagement.Core.SchoolAggregate.Groups.Group", "FormTutorId");

                    b.HasOne("SchoolManagement.Core.SchoolAggregate.Schools.School", "School")
                        .WithMany("Groups")
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchoolManagement.Core.SchoolAggregate.Members.Member", b =>
                {
                    b.HasOne("SchoolManagement.Core.SchoolAggregate.Groups.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId");

                    b.HasOne("SchoolManagement.Core.SchoolAggregate.Schools.School", "School")
                        .WithMany("Members")
                        .HasForeignKey("SchoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
