﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using twitter_baby_birding.Models;

namespace twitter_baby_birding.Migrations
{
    [DbContext(typeof(TwitterBabyBirdingContext))]
    [Migration("20220303014610_first")]
    partial class first
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.5");

            modelBuilder.Entity("twitter_baby_birding.Models.Tweet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Author")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Content")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Date_time")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("UserName")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("UserName");

                    b.ToTable("Tweets");
                });

            modelBuilder.Entity("twitter_baby_birding.Models.User", b =>
                {
                    b.Property<string>("Name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.HasKey("Name");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("twitter_baby_birding.Models.Tweet", b =>
                {
                    b.HasOne("twitter_baby_birding.Models.User", "User")
                        .WithMany("Tweets")
                        .HasForeignKey("UserName");

                    b.Navigation("User");
                });

            modelBuilder.Entity("twitter_baby_birding.Models.User", b =>
                {
                    b.Navigation("Tweets");
                });
#pragma warning restore 612, 618
        }
    }
}
