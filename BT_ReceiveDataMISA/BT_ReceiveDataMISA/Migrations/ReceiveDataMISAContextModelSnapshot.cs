﻿// <auto-generated />
using BT_ReceiveDataMISA;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BT_ReceiveDataMISA.Migrations
{
    [DbContext(typeof(ReceiveDataMISAContext))]
    partial class ReceiveDataMISAContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BT_ReceiveDataMISA.Entities.CauHinhDongBo", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("varchar(200)")
                        .HasMaxLength(200);

                    b.Property<int>("ChuKyThucHien")
                        .HasColumnType("int");

                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.HasKey("Token");

                    b.ToTable("Tbl_CauHinhDongBo");
                });
#pragma warning restore 612, 618
        }
    }
}
