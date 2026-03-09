// Decompiled with JetBrains decompiler
// Type: MyApiNetCore6.Controllers.ProductController
// Assembly: API_QuanLyTHP, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: DC050ACB-EFEA-4AC7-80CD-78C98E6478D1
// Assembly location: G:\MyApiNetCore6-03_Authentication_New\Publish_API\API_QuanLyTHP.dll

using DatabaseTHP;
using DatabaseTHP.Class;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using MyApiNetCore6.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

#nullable enable
namespace MyApiNetCore6.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
  private readonly dbTrangHiepPhatContext _context;
  private readonly IConfiguration _configuration;

  public ProductController(dbTrangHiepPhatContext context, IConfiguration configuration)
  {
    this._context = context;
    this._context = context;
    this._configuration = configuration;
  }

  [HttpGet("{LOC_ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(string LOC_ID)
  {
    try
    {
      List<view_dm_HangHoa> lstValue = await this._context.view_dm_HangHoa.Where<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID)).OrderBy<view_dm_HangHoa, string>((Expression<Func<view_dm_HangHoa, string>>) (e => e.MA)).ToListAsync<view_dm_HangHoa>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{Type}/{KeyWhere}/{ValuesSearch}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(
    string LOC_ID,
    int Type,
    string KeyWhere = "",
    string ValuesSearch = "")
  {
    try
    {
      ValuesSearch = ValuesSearch.Replace("%2f", "/");
      List<view_dm_HangHoa> lstValue = await this._context.view_dm_HangHoa.Where<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID)).Where<view_dm_HangHoa>(KeyWhere, (object) ValuesSearch).OrderBy<view_dm_HangHoa, string>((Expression<Func<view_dm_HangHoa, string>>) (e => e.MA)).ToListAsync<view_dm_HangHoa>();
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) lstValue
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpGet("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> GetProduct(string LOC_ID, string ID)
  {
    try
    {
      view_dm_HangHoa Product = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Product == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      this._context.dm_HangHoa_Kho.Any<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = true,
        Message = "Success",
        Data = (object) Product
      });
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPut("{LOC_ID}/{MA}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> PutProduct(string LOC_ID, string MA, [FromBody] v_dm_HangHoa Product)
  {
    try
    {
      if (this.ProductExists((dm_HangHoa) Product))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Product.LOC_ID}-{Product.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (LOC_ID != Product.LOC_ID || Product.MA != MA)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = "Dữ liệu khóa không giống nhau!",
          Data = (object) ""
        });
      if (!this.ProductExistsID(Product.LOC_ID, Product.ID))
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {Product.LOC_ID}-{Product.ID} dữ liệu!",
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (!Product.BAOGOMTHUESUAT)
          Product.ID_THUESUAT = (string) null;
        string loaihanghoa1 = Product.LOAIHANGHOA;
        int num1 = 0;
        string str1 = num1.ToString();
        if (loaihanghoa1 == str1)
        {
          List<dm_HangHoa_Combo> lstdm_HangHoa_Combo = await this._context.dm_HangHoa_Combo.Where<dm_HangHoa_Combo>((Expression<Func<dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == Product.LOC_ID && e.ID_HANGHOA == Product.ID)).ToListAsync<dm_HangHoa_Combo>();
          if (lstdm_HangHoa_Combo != null)
          {
            foreach (dm_HangHoa_Combo itm in lstdm_HangHoa_Combo)
            {
              itm.TYLE_QD = Product.TYLE_QD;
              itm.QTY_TOTAL = itm.TYLE_QD * itm.QTY;
              this._context.Entry<dm_HangHoa_Combo>(itm).State = EntityState.Modified;
            }
          }
          lstdm_HangHoa_Combo = (List<dm_HangHoa_Combo>) null;
        }
        else
        {
          string loaihanghoa2 = Product.LOAIHANGHOA;
          num1 = 2;
          string str2 = num1.ToString();
          if (loaihanghoa2 == str2)
          {
            Product.ISCOMBO = false;
            Product.STATUS_QD = false;
            Product.TYLE_QD = 0.0;
          }
          else
          {
            string loaihanghoa3 = Product.LOAIHANGHOA;
            num1 = 1;
            string str3 = num1.ToString();
            if (loaihanghoa3 == str3)
            {
              Product.ISCOMBO = false;
              Product.STATUS_QD = false;
              Product.TYLE_QD = 1.0;
              List<dm_HangHoa_Combo> lstdm_HangHoa_Combo = await this._context.dm_HangHoa_Combo.Where<dm_HangHoa_Combo>((Expression<Func<dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == Product.LOC_ID && e.ID_HANGHOACOMBO == Product.ID)).ToListAsync<dm_HangHoa_Combo>();
              if (lstdm_HangHoa_Combo != null)
              {
                foreach (dm_HangHoa_Combo dmHangHoaCombo in lstdm_HangHoa_Combo)
                {
                  dm_HangHoa_Combo itm = dmHangHoaCombo;
                  v_dm_HangHoa_Combo checkHangHoa_Combo = Product.lstdm_HangHoa_Combo.Where<v_dm_HangHoa_Combo>((Func<v_dm_HangHoa_Combo, bool>) (e => e.ID_HANGHOA == itm.ID_HANGHOA && e.ID_DVT == itm.ID_DVT)).FirstOrDefault<v_dm_HangHoa_Combo>();
                  if (checkHangHoa_Combo != null)
                  {
                    checkHangHoa_Combo.ISEDIT = true;
                    itm.QTY = checkHangHoa_Combo.QTY;
                    itm.TYLE_QD = checkHangHoa_Combo.TYLE_QD;
                    itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                    itm.ID_NGUOISUA = checkHangHoa_Combo.ID_NGUOITAO;
                    itm.THOIGIANSUA = checkHangHoa_Combo.THOIGIANTHEM;
                  }
                  else
                    this._context.dm_HangHoa_Combo.Remove(itm);
                  checkHangHoa_Combo = (v_dm_HangHoa_Combo) null;
                }
              }
              if (Product.lstdm_HangHoa_Combo != null)
              {
                foreach (dm_HangHoa_Combo itm in Product.lstdm_HangHoa_Combo.Where<v_dm_HangHoa_Combo>((Func<v_dm_HangHoa_Combo, bool>) (e => !e.ISEDIT)))
                {
                  itm.ID = Guid.NewGuid().ToString();
                  itm.LOC_ID = Product.LOC_ID;
                  itm.ID_HANGHOACOMBO = Product.ID;
                  itm.ISACTIVE = true;
                  itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                  this._context.dm_HangHoa_Combo.Add(itm);
                }
              }
              lstdm_HangHoa_Combo = (List<dm_HangHoa_Combo>) null;
            }
          }
        }
        this._context.Entry<v_dm_HangHoa>(Product).State = EntityState.Modified;
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num2 = await this._context.SaveChangesAsync();
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_HangHoa OKProduct = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == Product.LOC_ID && e.ID == Product.ID));
        if (!string.IsNullOrEmpty(Product.PICTURE) && Product.FILENEW)
        {
          string path = "C:\\FTP\\Images_Upload\\Product";
          if (Product.FILENEW)
          {
            try
            {
              if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
              if (System.IO.File.Exists($"{path}\\{Product.PICTURE}"))
                System.IO.File.Delete($"{path}\\{Product.PICTURE}");
              byte[] tempBytes = Convert.FromBase64String(Product.FILEBASE64);
              System.IO.File.WriteAllBytes($"{path}\\{Product.PICTURE}", tempBytes);
              tempBytes = (byte[]) null;
            }
            catch (Exception ex)
            {
              return (IActionResult) this.Ok((object) new ApiResponse()
              {
                Success = false,
                Message = ex.Message,
                Data = (object) ""
              });
            }
          }
          path = (string) null;
        }
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
    }
    catch (DbUpdateConcurrencyException ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpPost]
  [Authorize(Roles = "User")]
  public async Task<ActionResult<v_dm_HangHoa>> PostProduct([FromBody] v_dm_HangHoa Product)
  {
    try
    {
      if (this.ProductExistsMA(Product.LOC_ID, Product.MA))
        return (ActionResult<v_dm_HangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Đã tồn tại{Product.LOC_ID}-{Product.MA} trong dữ liệu!",
          Data = (object) ""
        });
      if (!string.IsNullOrEmpty(Product.PICTURE) && !string.IsNullOrEmpty(Product.FILEBASE64))
      {
        try
        {
          string path = "C:\\FTP\\Images_Upload\\Product";
          if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
          byte[] tempBytes = Convert.FromBase64String(Product.FILEBASE64);
          System.IO.File.WriteAllBytes($"{path}\\{Product.PICTURE}", tempBytes);
          path = (string) null;
          tempBytes = (byte[]) null;
        }
        catch (Exception ex)
        {
          return (ActionResult<v_dm_HangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
          {
            Success = false,
            Message = ex.Message,
            Data = (object) ""
          });
        }
      }
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        if (!Product.BAOGOMTHUESUAT)
          Product.ID_THUESUAT = (string) null;
        string loaihanghoa1 = Product.LOAIHANGHOA;
        int num1 = 0;
        string str1 = num1.ToString();
        if (!(loaihanghoa1 == str1))
        {
          string loaihanghoa2 = Product.LOAIHANGHOA;
          num1 = 0;
          string str2 = num1.ToString();
          if (loaihanghoa2 == str2)
          {
            Product.ISCOMBO = false;
            Product.STATUS_QD = false;
            Product.TYLE_QD = 0.0;
          }
          else
          {
            string loaihanghoa3 = Product.LOAIHANGHOA;
            num1 = 1;
            string str3 = num1.ToString();
            if (loaihanghoa3 == str3)
            {
              Product.ISCOMBO = false;
              Product.STATUS_QD = false;
              Product.TYLE_QD = 1.0;
              if (Product.lstdm_HangHoa_Combo != null)
              {
                foreach (dm_HangHoa_Combo itm in Product.lstdm_HangHoa_Combo)
                {
                  itm.ID = Guid.NewGuid().ToString();
                  itm.LOC_ID = Product.LOC_ID;
                  itm.ID_HANGHOACOMBO = Product.ID;
                  itm.ISACTIVE = true;
                  itm.QTY_TOTAL = itm.QTY * itm.TYLE_QD;
                  this._context.dm_HangHoa_Combo.Add(itm);
                }
              }
            }
          }
        }
        this._context.dm_HangHoa.Add((dm_HangHoa) Product);
        List<dm_Kho> lstKho = await this._context.dm_Kho.Where<dm_Kho>((Expression<Func<dm_Kho, bool>>) (e => e.LOC_ID == Product.LOC_ID)).ToListAsync<dm_Kho>();
        if (lstKho != null)
        {
          foreach (dm_Kho itm in lstKho)
          {
            dm_HangHoa_Kho dm_HangHoa_Kho = new dm_HangHoa_Kho();
            dm_HangHoa_Kho.ID = Guid.NewGuid().ToString();
            dm_HangHoa_Kho.LOC_ID = Product.LOC_ID;
            dm_HangHoa_Kho.ID_KHO = itm.ID;
            dm_HangHoa_Kho.ID_HANGHOA = Product.ID;
            dm_HangHoa_Kho.QTY = 0.0;
            this._context.dm_HangHoa_Kho.Add(dm_HangHoa_Kho);
            dm_HangHoa_Kho = (dm_HangHoa_Kho) null;
          }
        }
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num2 = await this._context.SaveChangesAsync();
        lstKho = (List<dm_Kho>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        view_dm_HangHoa OKProduct = await this._context.view_dm_HangHoa.FirstOrDefaultAsync<view_dm_HangHoa>((Expression<Func<view_dm_HangHoa, bool>>) (e => e.LOC_ID == Product.LOC_ID && e.ID == Product.ID));
        return (ActionResult<v_dm_HangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) OKProduct
        });
      }
    }
    catch (Exception ex)
    {
      return (ActionResult<v_dm_HangHoa>) (ActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  [HttpDelete("{LOC_ID}/{ID}")]
  [Authorize(Roles = "User")]
  public async Task<IActionResult> DeleteProduct(string LOC_ID, string ID)
  {
    try
    {
      dm_HangHoa Product = await this._context.dm_HangHoa.FirstOrDefaultAsync<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
      if (Product == null)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = $"Không tìm thấy {LOC_ID}-{ID} dữ liệu!",
          Data = (object) ""
        });
      ExecuteStoredProc ExecuteStoredProc = new ExecuteStoredProc(this._context, this._configuration);
      List<dm_HangHoa_Kho> lstdm_HangHoa_Kho = await this._context.dm_HangHoa_Kho.Where<dm_HangHoa_Kho>((Expression<Func<dm_HangHoa_Kho, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOA == ID)).ToListAsync<dm_HangHoa_Kho>();
      ApiResponse apiResponse = new ApiResponse();
      if (lstdm_HangHoa_Kho != null)
      {
        foreach (dm_HangHoa_Kho dmHangHoaKho in lstdm_HangHoa_Kho)
        {
          dm_HangHoa_Kho itm = dmHangHoaKho;
          apiResponse = await ExecuteStoredProc.CheckDelete<dm_HangHoa_Kho>(itm, itm.ID, Product.NAME);
          if (!apiResponse.Success)
            return (IActionResult) this.Ok((object) new ApiResponse()
            {
              Success = false,
              Message = apiResponse.Message,
              Data = (object) ""
            });
          itm = (dm_HangHoa_Kho) null;
        }
      }
      apiResponse = await ExecuteStoredProc.CheckDelete<dm_HangHoa>(Product, Product.ID, Product.NAME);
      if (!apiResponse.Success)
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = false,
          Message = apiResponse.Message,
          Data = (object) ""
        });
      using (IDbContextTransaction transaction = this._context.Database.BeginTransaction())
      {
        List<web_PhanQuyenSanPham> lstweb_PhanQuyenSanPham = await this._context.web_PhanQuyenSanPham.Where<web_PhanQuyenSanPham>((Expression<Func<web_PhanQuyenSanPham, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_SANPHAM == ID)).ToListAsync<web_PhanQuyenSanPham>();
        if (lstweb_PhanQuyenSanPham != null)
        {
          foreach (web_PhanQuyenSanPham itm in lstweb_PhanQuyenSanPham)
            this._context.web_PhanQuyenSanPham.Remove(itm);
        }
        List<dm_HangHoa_Combo> lstdm_HangHoa_Combo = await this._context.dm_HangHoa_Combo.Where<dm_HangHoa_Combo>((Expression<Func<dm_HangHoa_Combo, bool>>) (e => e.LOC_ID == LOC_ID && e.ID_HANGHOACOMBO == ID)).ToListAsync<dm_HangHoa_Combo>();
        if (lstdm_HangHoa_Combo != null)
        {
          foreach (dm_HangHoa_Combo itm in lstdm_HangHoa_Combo)
            this._context.dm_HangHoa_Combo.Remove(itm);
        }
        if (lstdm_HangHoa_Kho != null)
        {
          foreach (dm_HangHoa_Kho itm in lstdm_HangHoa_Kho)
            this._context.dm_HangHoa_Kho.Remove(itm);
        }
        this._context.dm_HangHoa.Remove(Product);
        AuditLogController auditLog = new AuditLogController(this._context, this._configuration);
        auditLog.InserAuditLog();
        int num = await this._context.SaveChangesAsync();
        lstweb_PhanQuyenSanPham = (List<web_PhanQuyenSanPham>) null;
        lstdm_HangHoa_Combo = (List<dm_HangHoa_Combo>) null;
        auditLog = (AuditLogController) null;
        transaction.Commit();
        return (IActionResult) this.Ok((object) new ApiResponse()
        {
          Success = true,
          Message = "Success",
          Data = (object) ""
        });
      }
    }
    catch (Exception ex)
    {
      return (IActionResult) this.Ok((object) new ApiResponse()
      {
        Success = false,
        Message = ex.Message,
        Data = (object) ""
      });
    }
  }

  private bool ProductExistsMA(string LOC_ID, string MA)
  {
    return this._context.dm_HangHoa.Any<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.MA == MA));
  }

  private bool ProductExistsID(string LOC_ID, string ID)
  {
    return this._context.dm_HangHoa.Any<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.LOC_ID == LOC_ID && e.ID == ID));
  }

  private bool ProductExists(dm_HangHoa HangHoa)
  {
    return this._context.dm_HangHoa.Any<dm_HangHoa>((Expression<Func<dm_HangHoa, bool>>) (e => e.LOC_ID == HangHoa.LOC_ID && e.MA == HangHoa.MA && e.ID != HangHoa.ID));
  }
}
