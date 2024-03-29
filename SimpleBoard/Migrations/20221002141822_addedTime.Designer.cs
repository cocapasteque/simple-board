﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleBoard;

#nullable disable

namespace SimpleBoard.Migrations
{
    [DbContext(typeof(BoardContext))]
    [Migration("20221002141822_addedTime")]
    partial class addedTime
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("SimpleBoard.BoardEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<float>("Score")
                        .HasColumnType("float");

                    b.Property<float>("Time")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
