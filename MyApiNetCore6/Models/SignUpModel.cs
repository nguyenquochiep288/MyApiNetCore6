// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Models.SignUpModel
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

#nullable enable
namespace MyApiNetCore6.Models;

public class SignUpModel
{
  public string? ID { get; set; }

  public string? ID_NHOMQUYEN { get; set; }

  public string? FullName { get; set; }

  public string? UserName { get; set; }

  public string? Password { get; set; }

  public string? ConfirmPassword { get; set; }

  public string? Email { get; set; }

  public string? PhoneNumber { get; set; }

  public string? URL_IMAGE { get; set; }
}
