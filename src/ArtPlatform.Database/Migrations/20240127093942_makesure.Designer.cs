﻿// <auto-generated />
using System;
using System.Collections.Generic;
using ArtPlatform.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ArtPlatform.Database.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240127093942_makesure")]
    partial class makesure
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerProfilePortfolioPiece", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileReference")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SellerProfileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SellerProfileId");

                    b.ToTable("SellerProfilePortfolioPieces");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerProfileRequest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("Accepted")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("AcceptedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("RequestDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("SellerProfileRequests");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerService", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<int>("SellerProfileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SellerProfileId");

                    b.ToTable("SellerServices");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("BuyerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Price")
                        .HasColumnType("double precision");

                    b.Property<int>("SellerId")
                        .HasColumnType("integer");

                    b.Property<int>("SellerServiceId")
                        .HasColumnType("integer");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("TermsAcceptedDate")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BuyerId");

                    b.HasIndex("SellerId");

                    b.HasIndex("SellerServiceId");

                    b.ToTable("SellerServiceOrders");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderMessage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SellerServiceOrderId")
                        .HasColumnType("integer");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("SentAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("SellerServiceOrderId");

                    b.HasIndex("SenderId");

                    b.ToTable("SellerServiceOrderMessages");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderMessageAttachment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("FileReference")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SellerServiceOrderMessageId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SellerServiceOrderMessageId");

                    b.ToTable("SellerServiceOrderMessageAttachments");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderReview", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Rating")
                        .HasColumnType("integer");

                    b.Property<string>("Review")
                        .HasColumnType("text");

                    b.Property<string>("ReviewerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("SellerServiceId")
                        .HasColumnType("integer");

                    b.Property<int>("SellerServiceOrderId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ReviewerId");

                    b.HasIndex("SellerServiceId");

                    b.HasIndex("SellerServiceOrderId");

                    b.ToTable("SellerServiceOrderReviews");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.Property<string>("AddressCity")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AddressCountry")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AddressHouseNumber")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AddressPostalCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AddressRegion")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("AddressStreet")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Biography")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("UserSellerProfileId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.UserSellerProfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("AgeRestricted")
                        .HasColumnType("boolean");

                    b.Property<string>("Biography")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<List<string>>("SocialMediaLinks")
                        .IsRequired()
                        .HasColumnType("text[]");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("UserSellerProfiles");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerProfilePortfolioPiece", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.UserSellerProfile", "SellerProfile")
                        .WithMany("PortfolioPieces")
                        .HasForeignKey("SellerProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SellerProfile");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerProfileRequest", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerService", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.UserSellerProfile", "SellerProfile")
                        .WithMany("SellerServices")
                        .HasForeignKey("SellerProfileId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SellerProfile");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrder", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.User", "Buyer")
                        .WithMany("Orders")
                        .HasForeignKey("BuyerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArtPlatform.Database.Entities.UserSellerProfile", "Seller")
                        .WithMany()
                        .HasForeignKey("SellerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArtPlatform.Database.Entities.SellerService", "SellerService")
                        .WithMany()
                        .HasForeignKey("SellerServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Buyer");

                    b.Navigation("Seller");

                    b.Navigation("SellerService");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderMessage", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.SellerServiceOrder", "SellerServiceOrder")
                        .WithMany("Messages")
                        .HasForeignKey("SellerServiceOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArtPlatform.Database.Entities.User", "Sender")
                        .WithMany()
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SellerServiceOrder");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderMessageAttachment", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.SellerServiceOrderMessage", "SellerServiceOrderMessage")
                        .WithMany()
                        .HasForeignKey("SellerServiceOrderMessageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SellerServiceOrderMessage");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrderReview", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.User", "Reviewer")
                        .WithMany()
                        .HasForeignKey("ReviewerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArtPlatform.Database.Entities.SellerService", "SellerService")
                        .WithMany("Reviews")
                        .HasForeignKey("SellerServiceId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ArtPlatform.Database.Entities.SellerServiceOrder", "SellerServiceOrder")
                        .WithMany("Reviews")
                        .HasForeignKey("SellerServiceOrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Reviewer");

                    b.Navigation("SellerService");

                    b.Navigation("SellerServiceOrder");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.UserSellerProfile", b =>
                {
                    b.HasOne("ArtPlatform.Database.Entities.User", "User")
                        .WithOne("UserSellerProfile")
                        .HasForeignKey("ArtPlatform.Database.Entities.UserSellerProfile", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerService", b =>
                {
                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.SellerServiceOrder", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Reviews");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.User", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("UserSellerProfile");
                });

            modelBuilder.Entity("ArtPlatform.Database.Entities.UserSellerProfile", b =>
                {
                    b.Navigation("PortfolioPieces");

                    b.Navigation("SellerServices");
                });
#pragma warning restore 612, 618
        }
    }
}
