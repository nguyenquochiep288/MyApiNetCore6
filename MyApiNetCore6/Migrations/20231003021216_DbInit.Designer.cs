using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace TS24.SM24.DBClass
{
    public partial class clsQl_banan
    {
        public static string Insert(ql_banan obj)
        {
            string[] notGet = new string[] { };// new string[] { "ID" };
            obj.GUID = obj.ID = Guid.NewGuid().ToString().ToUpper();
            obj.GUID_CONGTY =TS24.HD.BaseMethod.BaseParam.ActiveID;
            return DBConnect.DBFunctions.InsertGUID(obj, DBConnect.DBFunctions.TableName.ql_banan, notGet, "ID", obj.GUID);
        }

        public static string Update(ql_banan obj)
        {
            string[] strCondition = new string[] { "ID" };
            if (DBConnect.DBFunctions.Update(obj, DBConnect.DBFunctions.TableName.ql_banan, strCondition))
                return obj.ID;
            return "";
        }

        public static string InsertorUpdate(ql_banan obj)
        {
            try
            {
                if (!string.IsNullOrEmpty(obj.ID))
                {
                    return Update(obj);
                }
                else
                {
                    return Insert(obj);
                } 
            }
            catch
            {
                return "";
            }
        }

        public static bool Delete(ql_banan obj)
        {
            return DBConnect.DBFunctions.Delete(DBConnect.DBFunctions.TableName.ql_banan, "ID", obj.ID.ToString());
        }

        public static ql_banan GetByGuid(string GUID)
        {
            ql_banan obj = new ql_banan();
            try
            {
                DataTable dt = DBConnect.DBFunctions.SelectData("select * from ql_banan where GUID='" + GUID + "'");
                DataRow dr = dt.Rows[0];
                obj.ID=Helper.GetStringValue(dr,"ID");
                obj.GUID = Helper.GetStringValue(dr, "GUID");
                obj.GUID_BANAN = Helper.GetStringValue(dr, "GUID_BANAN");
                obj.TENBAN = Helper.GetStringValue(dr, "TENBAN");
                obj.TENKHUVUC = Helper.GetStringValue(dr, "TENKHUVUC");
               
                obj.NGAYDAT = Helper.GetDateTimeValue(dr, "NGAYDAT");
                obj.GIODAT_BATDAU = Helper.GetDateTimeValue(dr, "GIODAT_BATDAU");
                obj.GIODAT_KETTHUC = Helper.GetDateTimeValue(dr, "GIODAT_KETTHUC");
                obj.NGAYTHUCHIEN = Helper.GetDateTimeValue(dr, "NGAYTHUCHIEN");
                obj.NGUOITHUCHIEN = Helper.GetStringValue(dr, "NGUOITHUCHIEN");
                obj.GUID_CUAHANG = Helper.GetStringValue(dr, "GUID_CUAHANG");
                obj.GUID_DATHANGBAN = Helper.GetStringValue(dr, "GUID_DATHANGBAN");
                obj.GUID_CONGTY = Helper.GetStringValue(dr, "GUID_CONGTY");
                obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                obj.TINHTRANGSD = Helper.GetIntValue(dr, "TINHTRANGSD");
                obj.DANGSD = Helper.GetIntValue(dr, "DANGSD");
                obj.TENKHACHHANG = Helper.GetStringValue(dr, "TENKHACHHANG");
                obj.MOTA = Helper.GetStringValue(dr, "MOTA");

                return obj;
            }
            catch
            {
                return null;
            }
        }

        public static ql_banan GetById(string ID)
        {
            ql_banan obj = new ql_banan();
            try
            {
                DataTable dt = DBConnect.DBFunctions.SelectData("select * from ql_banan where ID='" + ID + "'");
                DataRow dr = dt.Rows[0];
                obj.ID=Helper.GetStringValue(dr,"ID");
                obj.GUID = Helper.GetStringValue(dr, "GUID");
                obj.GUID_BANAN = Helper.GetStringValue(dr, "GUID_BANAN");
                obj.TENBAN = Helper.GetStringValue(dr, "TENBAN");
                obj.TENKHUVUC = Helper.GetStringValue(dr, "TENKHUVUC");
              
                obj.NGAYDAT = Helper.GetDateTimeValue(dr, "NGAYDAT");
                obj.GIODAT_BATDAU = Helper.GetDateTimeValue(dr, "GIODAT_BATDAU");
                obj.GIODAT_KETTHUC = Helper.GetDateTimeValue(dr, "GIODAT_KETTHUC");
                obj.NGAYTHUCHIEN = Helper.GetDateTimeValue(dr, "NGAYTHUCHIEN");
                obj.NGUOITHUCHIEN = Helper.GetStringValue(dr, "NGUOITHUCHIEN");
                obj.GUID_CUAHANG = Helper.GetStringValue(dr, "GUID_CUAHANG");
                obj.GUID_DATHANGBAN = Helper.GetStringValue(dr, "GUID_DATHANGBAN");
                obj.GUID_CONGTY = Helper.GetStringValue(dr, "GUID_CONGTY");
                obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                obj.TINHTRANGSD = Helper.GetIntValue(dr, "TINHTRANGSD");
                obj.DANGSD = Helper.GetIntValue(dr, "DANGSD");
                obj.TENKHACHHANG = Helper.GetStringValue(dr, "TENKHACHHANG");
                obj.MOTA = Helper.GetStringValue(dr, "MOTA");
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public static ql_banan GetByGUID_DATHANGBAN(string GUID_DATHANGBAN)
        {
            ql_banan obj = new ql_banan();
            try
            {
                DataTable dt = DBConnect.DBFunctions.SelectData("select * from ql_banan where GUID_DATHANGBAN='" + GUID_DATHANGBAN + "' AND GUID_CONGTY='" +TS24.HD.BaseMethod.BaseParam.ActiveID + "'");
                DataRow dr = dt.Rows[0];
                obj.ID=Helper.GetStringValue(dr,"ID");
                obj.GUID = Helper.GetStringValue(dr, "GUID");
                obj.GUID_BANAN = Helper.GetStringValue(dr, "GUID_BANAN");
                obj.TENBAN = Helper.GetStringValue(dr, "TENBAN");
                obj.TENKHUVUC = Helper.GetStringValue(dr, "TENKHUVUC");

                obj.NGAYDAT = Helper.GetDateTimeValue(dr, "NGAYDAT");
                obj.GIODAT_BATDAU = Helper.GetDateTimeValue(dr, "GIODAT_BATDAU");
                obj.GIODAT_KETTHUC = Helper.GetDateTimeValue(dr, "GIODAT_KETTHUC");
                obj.NGAYTHUCHIEN = Helper.GetDateTimeValue(dr, "NGAYTHUCHIEN");
                obj.NGUOITHUCHIEN = Helper.GetStringValue(dr, "NGUOITHUCHIEN");
                obj.GUID_CUAHANG = Helper.GetStringValue(dr, "GUID_CUAHANG");
                obj.GUID_DATHANGBAN = Helper.GetStringValue(dr, "GUID_DATHANGBAN");
                obj.GUID_CONGTY = Helper.GetStringValue(dr, "GUID_CONGTY");
                obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                obj.TINHTRANGSD = Helper.GetIntValue(dr, "TINHTRANGSD");
                obj.DANGSD = Helper.GetIntValue(dr, "DANGSD");
                obj.TENKHACHHANG = Helper.GetStringValue(dr, "TENKHACHHANG");
                obj.MOTA = Helper.GetStringValue(dr, "MOTA");
                return obj;
            }
            catch
            {
                return null;
            }
        }

        public static List<ql_banan> GetAll()
        {
            List<ql_banan> lst = new List<ql_banan>();
            try
            {
                DataTable dt = DBConnect.DBFunctions.SelectData("select * from ql_banan where GUID_CONGTY='" +TS24.HD.BaseMethod.BaseParam.ActiveID + "'");
                foreach (DataRow dr in dt.Rows)
                {
                    ql_banan obj = new ql_banan();
                    obj.ID=Helper.GetStringValue(dr,"ID");
                    obj.GUID = Helper.GetStringValue(dr, "GUID");
                    obj.GUID_BANAN = Helper.GetStringValue(dr, "GUID_BANAN");
                    obj.TENBAN = Helper.GetStringValue(dr, "TENBAN");
                    obj.TENKHUVUC = Helper.GetStringValue(dr, "TENKHUVUC");
                    obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                    obj.NGAYDAT = Helper.GetDateTimeValue(dr, "NGAYDAT");
                    obj.GIODAT_BATDAU = Helper.GetDateTimeValue(dr, "GIODAT_BATDAU");
                    obj.GIODAT_KETTHUC = Helper.GetDateTimeValue(dr, "GIODAT_KETTHUC");
                    obj.NGAYTHUCHIEN = Helper.GetDateTimeValue(dr, "NGAYTHUCHIEN");
                    obj.NGUOITHUCHIEN = Helper.GetStringValue(dr, "NGUOITHUCHIEN");
                    obj.GUID_CUAHANG = Helper.GetStringValue(dr, "GUID_CUAHANG");
                    obj.GUID_DATHANGBAN = Helper.GetStringValue(dr, "GUID_DATHANGBAN");
                    obj.GUID_CONGTY = Helper.GetStringValue(dr, "GUID_CONGTY");
                    obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                    obj.TINHTRANGSD = Helper.GetIntValue(dr, "TINHTRANGSD");
                    obj.DANGSD = Helper.GetIntValue(dr, "DANGSD");
                    obj.TENKHACHHANG = Helper.GetStringValue(dr, "TENKHACHHANG");
                    obj.MOTA = Helper.GetStringValue(dr, "MOTA");
                    lst.Add(obj);
                }
                return lst.OrderBy(c => c.ID).ToList();
            }
            catch
            {
                return null;
            }
        }
         public static List<ql_banan> GetAll_()
        {
            List<ql_banan> lst = new List<ql_banan>();
            try
            {
                DataTable dt = DBConnect.DBFunctions.SelectData("select * from ql_banan where GUID_CONGTY='" +TS24.HD.BaseMethod.BaseParam.ActiveID + "'");
                foreach (DataRow dr in dt.Rows)
                {
                    ql_banan obj = new ql_banan();
                    obj.ID=Helper.GetStringValue(dr,"ID");
                    obj.GUID = Helper.GetStringValue(dr, "GUID");
                    obj.GUID_BANAN = Helper.GetStringValue(dr, "GUID_BANAN");
                    obj.TENBAN = Helper.GetStringValue(dr, "TENBAN");
                    obj.TENKHUVUC = Helper.GetStringValue(dr, "TENKHUVUC");
                    obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE");
                    obj.NGAYDAT = Helper.GetDateTimeValue(dr, "NGAYDAT");
                    obj.GIODAT_BATDAU = Helper.GetDateTimeValue(dr, "GIODAT_BATDAU");
                    obj.GIODAT_KETTHUC = Helper.GetDateTimeValue(dr, "GIODAT_KETTHUC");
                    obj.NGAYTHUCHIEN = Helper.GetDateTimeValue(dr, "NGAYTHUCHIEN");
                    obj.NGUOITHUCHIEN = Helper.GetStringValue(dr, "NGUOITHUCHIEN");
                    obj.GUID_CUAHANG = Helper.GetStringValue(dr, "GUID_CUAHANG");
                    obj.GUID_DATHANGBAN = Helper.GetStringValue(dr, "GUID_DATHANGBAN");
                    obj.GUID_CONGTY = Helper.GetStringValue(dr, "GUID_CONGTY");
                    obj.SOLUONGGHE = Helper.GetStringValue(dr, "SOLUONGGHE")+" ng∆∞·ªùi";
                    obj.TINHTRANGSD = Helper.GetIntValue(dr, "TINHTRANGSD");
                    obj.DANGSD = Helper.GetIntValue(dr, "DANGSD");
                    obj.TENKHACHHANG = Helper.GetStringValue(dr, "TENKHACHHANG");
                    obj.MOTA = Helper.GetStringValue(dr, "MOTA");
                    lst.Add(obj);
                }
                return lst.OrderBy(c => c.ID).ToList();
            }
            catch
            {
                return null;
            }
        }
    
    }
   
}            ˆ   à
 ‚     ˇˇ á
ˇ É
    40309æ  à ËÈÍÍÈÍ æ  â  ÍÏÌÔÏÏÏ ~
 â „ á∂@˝ 
 â ‰Í  ~
 â	 ‰Vˆ   â
 ‚     ˇˇ à
ˇ É
    40309æ  â ËÈÍÍÈÍ æ  ä  ÍÏÌÔÏÏÏ ~
 ä „ à∂@˝ 
 ä ‰Î  ~
 ä	 ‰^ˆ   ä
 ‚     ˇˇ â
ˇ É
    40309æ  ä ËÈÍÍÈÍ æ  ã  ÍÏÌÔÏÏÏ ~
 ã „ â∂@˝ 
 ã ‰Ï  ~
 ã	 ‰fˆ   ã
 ‚     ˇˇ ä
ˇ É
    40309æ  ã ËÈÍÍÈÍ æ  å  ÍÏÌÔÏÏÏ ~
 å „ ä∂@˝ 
 å ‰L  ~
 å	 ‰nˆ   å
 ‚     ˇˇ ã
ˇ É
    40309æ  å ËÈÍÍÈÍ æ  ç  ÍÏÌÔÏÏÏ ~
 ç „ ã∂@˝ 
 ç ‰Ì  ~
 ç	 ‰vˆ   ç
 ‚     ˇˇ å
ˇ É