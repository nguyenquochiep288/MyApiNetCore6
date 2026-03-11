// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Class.QuantityCheckResult
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

#nullable disable
namespace API_QuanLyTHP.Class;

public class QuantityCheckResult
{
  public int ID_SANPHAM { get; set; }

  public int TotalOrdered { get; set; }

  public int TotalReceived { get; set; }

  public int Status { get; set; }
}
