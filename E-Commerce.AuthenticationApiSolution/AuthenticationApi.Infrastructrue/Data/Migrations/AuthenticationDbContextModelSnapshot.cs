﻿// <auto-generated />
using System;
using AuthenticationApi.Infrastructrue.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AuthenticationApi.Infrastructrue.Data.Migrations
{
    [DbContext(typeof(AuthenticationDbContext))]
    partial class AuthenticationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.AppUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("DateRegistered")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsBanned")
                        .HasColumnType("bit");

                    b.Property<bool>("IsVerfiyMail")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("RoleID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TelephoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.ResetPassword", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<string>("ResetToken")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("UsersId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("UsersId");

                    b.ToTable("ResetPassword");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.Role", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.UserAddress", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("apartment")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("building")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("city")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("created_at")
                        .HasColumnType("datetime2");

                    b.Property<string>("floor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("phone_number")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("postal_code")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("state")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("street")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("updated_at")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.AppUser", b =>
                {
                    b.HasOne("AuthenticationApi.Domain.Entites.Role", "Role")
                        .WithMany("RoleUsers")
                        .HasForeignKey("RoleID");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.ResetPassword", b =>
                {
                    b.HasOne("AuthenticationApi.Domain.Entites.AppUser", "Users")
                        .WithMany("ResetPassword")
                        .HasForeignKey("UsersId");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.UserAddress", b =>
                {
                    b.HasOne("AuthenticationApi.Domain.Entites.AppUser", "User")
                        .WithMany("UserAddresses")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.AppUser", b =>
                {
                    b.Navigation("ResetPassword");

                    b.Navigation("UserAddresses");
                });

            modelBuilder.Entity("AuthenticationApi.Domain.Entites.Role", b =>
                {
                    b.Navigation("RoleUsers");
                });
#pragma warning restore 612, 618
        }
    }
}
