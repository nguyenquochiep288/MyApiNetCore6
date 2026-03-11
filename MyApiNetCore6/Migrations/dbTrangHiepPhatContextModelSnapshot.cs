// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Migrations.dbTrangHiepPhatContextModelSnapshot
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyApiNetCore6.Data;
using System;

#nullable enable
namespace MyApiNetCore6.Migrations;

[DbContext(typeof (dbTrangHiepPhatContext))]
internal class dbTrangHiepPhatContextModelSnapshot : ModelSnapshot
{
  protected override void BuildModel(
  #nullable disable
  ModelBuilder modelBuilder)
  {
    modelBuilder.HasAnnotation("ProductVersion", (object) "6.0.10").HasAnnotation("Relational:MaxIdentifierLength", (object) 128 /*0x80*/);
    modelBuilder.UseIdentityColumns();
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<string>("Id").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("ConcurrencyStamp").IsConcurrencyToken(true).HasColumnType<string>("nvarchar(max)");
      b.Property<string>("Name").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.Property<string>("NormalizedName").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.HasKey("Id");
      b.HasIndex("NormalizedName").IsUnique().HasDatabaseName("RoleNameIndex").HasFilter("[NormalizedName] IS NOT NULL");
      b.ToTable("AspNetRoles", (string) null);
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("int");
      b.Property<int>("Id").UseIdentityColumn<int>();
      b.Property<string>("ClaimType").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("ClaimValue").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("RoleId").IsRequired(true).HasColumnType<string>("nvarchar(450)");
      b.HasKey("Id");
      b.HasIndex("RoleId");
      b.ToTable("AspNetRoleClaims", (string) null);
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("int");
      b.Property<int>("Id").UseIdentityColumn<int>();
      b.Property<string>("ClaimType").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("ClaimValue").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("UserId").IsRequired(true).HasColumnType<string>("nvarchar(450)");
      b.HasKey("Id");
      b.HasIndex("UserId");
      b.ToTable("AspNetUserClaims", (string) null);
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<string>("LoginProvider").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("ProviderKey").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("ProviderDisplayName").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("UserId").IsRequired(true).HasColumnType<string>("nvarchar(450)");
      b.HasKey("LoginProvider", "ProviderKey");
      b.HasIndex("UserId");
      b.ToTable("AspNetUserLogins", (string) null);
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<string>("UserId").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("RoleId").HasColumnType<string>("nvarchar(450)");
      b.HasKey("UserId", "RoleId");
      b.HasIndex("RoleId");
      b.ToTable("AspNetUserRoles", (string) null);
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<string>("UserId").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("LoginProvider").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("Name").HasColumnType<string>("nvarchar(450)");
      b.Property<string>("Value").HasColumnType<string>("nvarchar(max)");
      b.HasKey("UserId", "LoginProvider", "Name");
      b.ToTable("AspNetUserTokens", (string) null);
    }));
    modelBuilder.Entity("MyApiNetCore6.Data.ApplicationUser", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<string>("Id").HasColumnType<string>("nvarchar(450)");
      b.Property<int>("AccessFailedCount").HasColumnType<int>("int");
      b.Property<string>("ConcurrencyStamp").IsConcurrencyToken(true).HasColumnType<string>("nvarchar(max)");
      b.Property<string>("Email").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.Property<bool>("EmailConfirmed").HasColumnType<bool>("bit");
      b.Property<string>("FirstName").IsRequired(true).HasColumnType<string>("nvarchar(max)");
      b.Property<string>("LastName").IsRequired(true).HasColumnType<string>("nvarchar(max)");
      b.Property<bool>("LockoutEnabled").HasColumnType<bool>("bit");
      b.Property<DateTimeOffset?>("LockoutEnd").HasColumnType<DateTimeOffset?>("datetimeoffset");
      b.Property<string>("NormalizedEmail").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.Property<string>("NormalizedUserName").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.Property<string>("PasswordHash").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("PasswordDecrypt").HasColumnType<string>("nvarchar(max)");
      b.Property<string>("PhoneNumber").HasColumnType<string>("nvarchar(max)");
      b.Property<bool>("PhoneNumberConfirmed").HasColumnType<bool>("bit");
      b.Property<string>("SecurityStamp").HasColumnType<string>("nvarchar(max)");
      b.Property<bool>("TwoFactorEnabled").HasColumnType<bool>("bit");
      b.Property<string>("UserName").HasMaxLength(256 /*0x0100*/).HasColumnType<string>("nvarchar(256)");
      b.HasKey("Id");
      b.HasIndex("NormalizedEmail").HasDatabaseName("EmailIndex");
      b.HasIndex("NormalizedUserName").IsUnique().HasDatabaseName("UserNameIndex").HasFilter("[NormalizedUserName] IS NOT NULL");
      b.ToTable("AspNetUsers", (string) null);
    }));
    modelBuilder.Entity("MyApiNetCore6.Data.Book", (Action<EntityTypeBuilder>) (b =>
    {
      b.Property<int>("Id").ValueGeneratedOnAdd().HasColumnType<int>("int");
      b.Property<int>("Id").UseIdentityColumn<int>();
      b.Property<string>("Description").HasColumnType<string>("nvarchar(max)");
      b.Property<double>("Price").HasColumnType<double>("float");
      b.Property<int>("Quantity").HasColumnType<int>("int");
      b.Property<string>("Title").IsRequired(true).HasMaxLength(100).HasColumnType<string>("nvarchar(100)");
      b.HasKey("Id");
      b.ToTable("Book");
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", (Action<EntityTypeBuilder>) (b => b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", (string) null).WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade).IsRequired()));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", (Action<EntityTypeBuilder>) (b => b.HasOne("MyApiNetCore6.Data.ApplicationUser", (string) null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired()));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", (Action<EntityTypeBuilder>) (b => b.HasOne("MyApiNetCore6.Data.ApplicationUser", (string) null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired()));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", (Action<EntityTypeBuilder>) (b =>
    {
      b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", (string) null).WithMany().HasForeignKey("RoleId").OnDelete(DeleteBehavior.Cascade).IsRequired();
      b.HasOne("MyApiNetCore6.Data.ApplicationUser", (string) null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired();
    }));
    modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", (Action<EntityTypeBuilder>) (b => b.HasOne("MyApiNetCore6.Data.ApplicationUser", (string) null).WithMany().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade).IsRequired()));
  }
}
