// Decompiled with JetBrains decompiler
// Type: API_QuanLyTHP.Class.Utility
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll


namespace API_QuanLyTHP.Class;

public class Utility
{
  public static string GetMaPhieu(string MaPhieu)
  {
    if (MaPhieu.StartsWith("PN-") || MaPhieu.StartsWith("PX-") || MaPhieu.StartsWith("PT-") || MaPhieu.StartsWith("PC-") || MaPhieu.StartsWith("PCK-") || MaPhieu.StartsWith("PDH-") || !MaPhieu.StartsWith("PGH-"))
      ;
    return "";
  }
}
