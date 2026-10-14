// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Models.SignInModel
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using System.ComponentModel.DataAnnotations;

#nullable enable
namespace MyApiNetCore6.Models;

public class SignInModel
{
  [Required]
  public string UserName { get; set; } = (string) null;

  [Required]
  public string Password { get; set; } = (string) null;

  public string? DeviceId { get; set; }
}
