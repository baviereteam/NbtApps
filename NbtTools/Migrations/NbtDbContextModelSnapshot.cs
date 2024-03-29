﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NbtTools.Database;

#nullable disable

namespace NbtTools.Migrations
{
    [DbContext(typeof(NbtDbContext))]
    partial class NbtDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0");

            modelBuilder.Entity("NbtTools.Items.Item", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<byte>("StackSize")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name");

                    b.ToTable("items", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
