// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Models.BookModel
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using System.ComponentModel.DataAnnotations;

#nullable enable
namespace MyApiNetCore6.Models;

public class BookModel
{
  public int Id { get; set; }

  [MaxLength(100)]
  public string? Title { get; set; }

  public string? Description { get; set; }

  [Range(0.0, 1.7976931348623157E+308)]
  public double Price { get; set; }

  [Range(0, 100)]
  public int Quantity { get; set; }
}
