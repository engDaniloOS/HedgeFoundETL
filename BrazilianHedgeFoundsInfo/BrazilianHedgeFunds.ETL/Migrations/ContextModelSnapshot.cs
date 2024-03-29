﻿// <auto-generated />
using System;
using BrazilianHedgeFunds.ETL.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BrazilianHedgeFunds.ETL.Migrations
{
    [DbContext(typeof(Context))]
    partial class ContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.16")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("BrazilianHedgeFunds.ETL.Business.Models.HedgeFundRecord", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("ID");

                    b.Property<string>("CNPJ")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("CNPJ_FUNDO");

                    b.Property<double>("DayInvestmentsTotalValue")
                        .HasColumnType("float")
                        .HasColumnName("CAPTC_DIA");

                    b.Property<double>("DayWithdrawalsTotalValue")
                        .HasColumnType("float")
                        .HasColumnName("RESG_DIA");

                    b.Property<int>("InvestorsTotal")
                        .HasColumnType("int")
                        .HasColumnName("NR_COTST");

                    b.Property<double>("PortfolioTotalValue")
                        .HasColumnType("float")
                        .HasColumnName("VL_TOTAL");

                    b.Property<double>("QuotaValue")
                        .HasColumnType("float")
                        .HasColumnName("VL_QUOTA");

                    b.Property<DateTime>("RecordDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("DT_COMPTC");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("TYPE");

                    b.Property<double>("WorthValue")
                        .HasColumnType("float")
                        .HasColumnName("VL_PATRIM_LIQ");

                    b.HasKey("Id");

                    b.HasIndex("CNPJ");

                    b.HasIndex("RecordDate");

                    b.ToTable("HedgeFundRecord");
                });
#pragma warning restore 612, 618
        }
    }
}
