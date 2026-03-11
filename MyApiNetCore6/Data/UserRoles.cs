// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Data.UserRoles
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using System.ComponentModel.DataAnnotations.Schema;

#nullable enable
namespace MyApiNetCore6.Data;

[Table("UserRoles")]
public class UserRoles
{
  public const string User = "User";
  public const string Admin = "Admin";
  public const string UserAdmin = "User,Admin";
}
