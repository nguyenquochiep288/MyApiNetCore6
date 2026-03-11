// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Migrations.DbInit
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using MyApiNetCore6.Data;
using System;

#nullable enable
namespace MyApiNetCore6.Migrations;

[DbContext(typeof (dbTrangHiepPhatContext))]
[Migration("20231003021216_DbInit")]
public class DbInit : Migration
{
  protected override void Up(
  #nullable disable
  MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable("AspNetRoles", table =>
    {
      ColumnsBuilder columnsBuilder1 = table;
      bool? unicode1 = new bool?();
      int? maxLength1 = new int?();
      bool? fixedLength1 = new bool?();
      int? precision1 = new int?();
      int? nullable = new int?();
      int? scale1 = nullable;
      bool? stored1 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder1 = columnsBuilder1.Column<string>("nvarchar(450)", unicode1, maxLength1, fixedLength: fixedLength1, precision: precision1, scale: scale1, stored: stored1);
      ColumnsBuilder columnsBuilder2 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode2 = new bool?();
      int? maxLength2 = nullable;
      bool? fixedLength2 = new bool?();
      int? precision2 = new int?();
      int? scale2 = new int?();
      bool? stored2 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder2 = columnsBuilder2.Column<string>("nvarchar(256)", unicode2, maxLength2, nullable: true, fixedLength: fixedLength2, precision: precision2, scale: scale2, stored: stored2);
      ColumnsBuilder columnsBuilder3 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode3 = new bool?();
      int? maxLength3 = nullable;
      bool? fixedLength3 = new bool?();
      int? precision3 = new int?();
      int? scale3 = new int?();
      bool? stored3 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder3 = columnsBuilder3.Column<string>("nvarchar(256)", unicode3, maxLength3, nullable: true, fixedLength: fixedLength3, precision: precision3, scale: scale3, stored: stored3);
      OperationBuilder<AddColumnOperation> operationBuilder4 = table.Column<string>("nvarchar(max)", nullable: true);
      return new
      {
        Id = operationBuilder1,
        Name = operationBuilder2,
        NormalizedName = operationBuilder3,
        ConcurrencyStamp = operationBuilder4
      };
    }, constraints: table => table.PrimaryKey("PK_AspNetRoles", x => x.Id));
    migrationBuilder.CreateTable("AspNetUsers", table =>
    {
      OperationBuilder<AddColumnOperation> operationBuilder5 = table.Column<string>("nvarchar(450)");
      OperationBuilder<AddColumnOperation> operationBuilder6 = table.Column<string>("nvarchar(max)");
      ColumnsBuilder columnsBuilder4 = table;
      bool? unicode4 = new bool?();
      int? maxLength4 = new int?();
      bool? fixedLength4 = new bool?();
      int? precision4 = new int?();
      int? nullable = new int?();
      int? scale4 = nullable;
      bool? stored4 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder7 = columnsBuilder4.Column<string>("nvarchar(max)", unicode4, maxLength4, fixedLength: fixedLength4, precision: precision4, scale: scale4, stored: stored4);
      ColumnsBuilder columnsBuilder5 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode5 = new bool?();
      int? maxLength5 = nullable;
      bool? fixedLength5 = new bool?();
      int? precision5 = new int?();
      int? scale5 = new int?();
      bool? stored5 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder8 = columnsBuilder5.Column<string>("nvarchar(256)", unicode5, maxLength5, nullable: true, fixedLength: fixedLength5, precision: precision5, scale: scale5, stored: stored5);
      ColumnsBuilder columnsBuilder6 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode6 = new bool?();
      int? maxLength6 = nullable;
      bool? fixedLength6 = new bool?();
      int? precision6 = new int?();
      int? scale6 = new int?();
      bool? stored6 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder9 = columnsBuilder6.Column<string>("nvarchar(256)", unicode6, maxLength6, nullable: true, fixedLength: fixedLength6, precision: precision6, scale: scale6, stored: stored6);
      ColumnsBuilder columnsBuilder7 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode7 = new bool?();
      int? maxLength7 = nullable;
      bool? fixedLength7 = new bool?();
      int? precision7 = new int?();
      int? scale7 = new int?();
      bool? stored7 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder10 = columnsBuilder7.Column<string>("nvarchar(256)", unicode7, maxLength7, nullable: true, fixedLength: fixedLength7, precision: precision7, scale: scale7, stored: stored7);
      ColumnsBuilder columnsBuilder8 = table;
      nullable = new int?(256 /*0x0100*/);
      bool? unicode8 = new bool?();
      int? maxLength8 = nullable;
      bool? fixedLength8 = new bool?();
      int? precision8 = new int?();
      int? scale8 = new int?();
      bool? stored8 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder11 = columnsBuilder8.Column<string>("nvarchar(256)", unicode8, maxLength8, nullable: true, fixedLength: fixedLength8, precision: precision8, scale: scale8, stored: stored8);
      OperationBuilder<AddColumnOperation> operationBuilder12 = table.Column<bool>("bit");
      OperationBuilder<AddColumnOperation> operationBuilder13 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder14 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder15 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder16 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder17 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder18 = table.Column<bool>("bit");
      OperationBuilder<AddColumnOperation> operationBuilder19 = table.Column<bool>("bit");
      OperationBuilder<AddColumnOperation> operationBuilder20 = table.Column<DateTimeOffset>("datetimeoffset", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder21 = table.Column<bool>("bit");
      OperationBuilder<AddColumnOperation> operationBuilder22 = table.Column<int>("int");
      return new
      {
        Id = operationBuilder5,
        FirstName = operationBuilder6,
        LastName = operationBuilder7,
        UserName = operationBuilder8,
        NormalizedUserName = operationBuilder9,
        Email = operationBuilder10,
        NormalizedEmail = operationBuilder11,
        EmailConfirmed = operationBuilder12,
        PasswordHash = operationBuilder13,
        PasswordDecrypt = operationBuilder14,
        SecurityStamp = operationBuilder15,
        ConcurrencyStamp = operationBuilder16,
        PhoneNumber = operationBuilder17,
        PhoneNumberConfirmed = operationBuilder18,
        TwoFactorEnabled = operationBuilder19,
        LockoutEnd = operationBuilder20,
        LockoutEnabled = operationBuilder21,
        AccessFailedCount = operationBuilder22
      };
    }, constraints: table => table.PrimaryKey("PK_AspNetUsers", x => x.Id));
    migrationBuilder.CreateTable("Book", table =>
    {
      ColumnsBuilder columnsBuilder9 = table;
      bool? unicode9 = new bool?();
      int? maxLength9 = new int?();
      bool? fixedLength9 = new bool?();
      int? precision9 = new int?();
      int? nullable = new int?();
      int? scale9 = nullable;
      bool? stored9 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder23 = columnsBuilder9.Column<int>("int", unicode9, maxLength9, fixedLength: fixedLength9, precision: precision9, scale: scale9, stored: stored9).Annotation("SqlServer:Identity", (object) "1, 1");
      ColumnsBuilder columnsBuilder10 = table;
      nullable = new int?(100);
      bool? unicode10 = new bool?();
      int? maxLength10 = nullable;
      bool? fixedLength10 = new bool?();
      int? precision10 = new int?();
      int? scale10 = new int?();
      bool? stored10 = new bool?();
      OperationBuilder<AddColumnOperation> operationBuilder24 = columnsBuilder10.Column<string>("nvarchar(100)", unicode10, maxLength10, fixedLength: fixedLength10, precision: precision10, scale: scale10, stored: stored10);
      OperationBuilder<AddColumnOperation> operationBuilder25 = table.Column<string>("nvarchar(max)", nullable: true);
      OperationBuilder<AddColumnOperation> operationBuilder26 = table.Column<double>("float");
      OperationBuilder<AddColumnOperation> operationBuilder27 = table.Column<int>("int");
      return new
      {
        Id = operationBuilder23,
        Title = operationBuilder24,
        Description = operationBuilder25,
        Price = operationBuilder26,
        Quantity = operationBuilder27
      };
    }, constraints: table => table.PrimaryKey("PK_Book", x => x.Id));
    migrationBuilder.CreateTable("AspNetRoleClaims", table => new
    {
      Id = table.Column<int>("int").Annotation("SqlServer:Identity", (object) "1, 1"),
      RoleId = table.Column<string>("nvarchar(450)"),
      ClaimType = table.Column<string>("nvarchar(max)", nullable: true),
      ClaimValue = table.Column<string>("nvarchar(max)", nullable: true)
    }, constraints: table =>
    {
      table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
      table.ForeignKey("FK_AspNetRoleClaims_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
    });
    migrationBuilder.CreateTable("AspNetUserClaims", table => new
    {
      Id = table.Column<int>("int").Annotation("SqlServer:Identity", (object) "1, 1"),
      UserId = table.Column<string>("nvarchar(450)"),
      ClaimType = table.Column<string>("nvarchar(max)", nullable: true),
      ClaimValue = table.Column<string>("nvarchar(max)", nullable: true)
    }, constraints: table =>
    {
      table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
      table.ForeignKey("FK_AspNetUserClaims_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
    });
    migrationBuilder.CreateTable("AspNetUserLogins", table => new
    {
      LoginProvider = table.Column<string>("nvarchar(450)"),
      ProviderKey = table.Column<string>("nvarchar(450)"),
      ProviderDisplayName = table.Column<string>("nvarchar(max)", nullable: true),
      UserId = table.Column<string>("nvarchar(450)")
    }, constraints: table =>
    {
      table.PrimaryKey("PK_AspNetUserLogins", x => new
      {
        LoginProvider = x.LoginProvider,
        ProviderKey = x.ProviderKey
      });
      table.ForeignKey("FK_AspNetUserLogins_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
    });
    migrationBuilder.CreateTable("AspNetUserRoles", table => new
    {
      UserId = table.Column<string>("nvarchar(450)"),
      RoleId = table.Column<string>("nvarchar(450)")
    }, constraints: table =>
    {
      table.PrimaryKey("PK_AspNetUserRoles", x => new
      {
        UserId = x.UserId,
        RoleId = x.RoleId
      });
      table.ForeignKey("FK_AspNetUserRoles_AspNetRoles_RoleId", x => x.RoleId, "AspNetRoles", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
      table.ForeignKey("FK_AspNetUserRoles_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
    });
    migrationBuilder.CreateTable("AspNetUserTokens", table => new
    {
      UserId = table.Column<string>("nvarchar(450)"),
      LoginProvider = table.Column<string>("nvarchar(450)"),
      Name = table.Column<string>("nvarchar(450)"),
      Value = table.Column<string>("nvarchar(max)", nullable: true)
    }, constraints: table =>
    {
      table.PrimaryKey("PK_AspNetUserTokens", x => new
      {
        UserId = x.UserId,
        LoginProvider = x.LoginProvider,
        Name = x.Name
      });
      table.ForeignKey("FK_AspNetUserTokens_AspNetUsers_UserId", x => x.UserId, "AspNetUsers", "Id", (string) null, ReferentialAction.NoAction, ReferentialAction.Cascade);
    });
    migrationBuilder.CreateIndex("IX_AspNetRoleClaims_RoleId", "AspNetRoleClaims", "RoleId");
    migrationBuilder.CreateIndex("RoleNameIndex", "AspNetRoles", "NormalizedName", unique: true, filter: "[NormalizedName] IS NOT NULL");
    migrationBuilder.CreateIndex("IX_AspNetUserClaims_UserId", "AspNetUserClaims", "UserId");
    migrationBuilder.CreateIndex("IX_AspNetUserLogins_UserId", "AspNetUserLogins", "UserId");
    migrationBuilder.CreateIndex("IX_AspNetUserRoles_RoleId", "AspNetUserRoles", "RoleId");
    migrationBuilder.CreateIndex("EmailIndex", "AspNetUsers", "NormalizedEmail");
    migrationBuilder.CreateIndex("UserNameIndex", "AspNetUsers", "NormalizedUserName", unique: true, filter: "[NormalizedUserName] IS NOT NULL");
  }

  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable("AspNetRoleClaims");
    migrationBuilder.DropTable("AspNetUserClaims");
    migrationBuilder.DropTable("AspNetUserLogins");
    migrationBuilder.DropTable("AspNetUserRoles");
    migrationBuilder.DropTable("AspNetUserTokens");
    migrationBuilder.DropTable("Book");
    migrationBuilder.DropTable("AspNetRoles");
    migrationBuilder.DropTable("AspNetUsers");
  }

  protected override void BuildTargetModel(ModelBuilder modelBuilder)
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
