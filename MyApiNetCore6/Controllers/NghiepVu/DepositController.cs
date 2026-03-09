using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
//using Microsoft.Office.Interop;
using System.Globalization;
using System.Reflection;
using TS24.TO.Commons;
using System.Data.OleDb;
using System.IO;
using SmartXLS;
//using TS24.TO.BaseMethod;
using System.Threading;
//using TS24.SM24.BaseMethod;


namespace TS24.SM24.ImportExport
{
    public partial class frmImport : DevExpress.XtraEditors.XtraForm
    {
        public bool bolHienThiXoaDuLieu = true;
        public enum TypeImport
        {
            None,
            Muavao,
            Muavaokhonghoadon,
            Banra,
            DMKhachHang,
            DMBanan,
            DMBoPhan,
            DMBoPhanKhauHao,
            DMChiPhi,
            DMDvt,
            DMPtn,
            DMPtx,
            DMHangHoa,
            DMHangHoaKho,
            DMNguyenVatLieuKho,
            DMHangHoaDonGia,
            DMKho,
            DMCuaHangSM24,
            DMCuaHangVTA,
            DMTramYTe,
            DmVoucher,
            DMLoaiHH,
            DMLoaiKH,
            DMLoaiTS,
            DMLoaiTheKH,
            DMKhuVuc,
            DMNhanVien,
            DMTaiKhoan,
            DMTaiKhoanNganHang,
            DMThueSuat,
            DMTienTe,
            DMTscd,
            PhieuThu,
            PhieuChi,
            ct_tonghop,
            ct_gbn,
            ct_gbc,
            ct_phieuthu,
            ct_phieuchi,
            kt_thanhtoandonhang,
            kt_thanhtoantragop,
            solieudauky,
            PhieuDeNghiMuaHang,
            DonMuaHang,
            PhieuNhap,
            DonDatHang,
            PhieuBaoGia,
            PhieuXuat,
            dm_biendongvat,
            ChiTietHangHoa,
            importPhieuxuat,
            Khuyenmai,
            ChiTietHangHoa02,
            CongNoTaiKhoan,
            CongNoTaiKhoanDoiTuong,
            CongNoPhaiThuTheoChungTu,
            CongNoPhaiTraTheoChungTu,
            DMHinhThucThanhToan,
            ChiTietHangHoa03,
            DanhSachChungTuKhauTruThueTNCN,
            ChiTietHangHoaMTT,
            ChiTietHangHoaDacTrung
        }
        void fncDMHangHoaKho(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                int j = 0;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[j++].ColumnName = "STT";
                dtData.Columns[j++].ColumnName = "GUID_KHO";
                dtData.Columns[j++].ColumnName = "MAVACH";
                dtData.Columns[j++].ColumnName = "MA";
                dtData.Columns[j++].ColumnName = "TEN";
                dtData.Columns[j++].ColumnName = "ID_LOAIHH";
                dtData.Columns[j++].ColumnName = "SOLUONGDAUKY";
                dtData.Columns[j++].ColumnName = "THANHTIENDAUKY";
                dtData.Columns[j++].ColumnName = "THUESUAT";
                dtData.Columns[j++].ColumnName = "DGVON";
                dtData.Columns[j++].ColumnName = "DGBAN";
                dtData.Columns[j++].ColumnName = "GUID_TKKHO";
                dtData.Columns[j++].ColumnName = "GUID_TKCHIPHI";
                dtData.Columns[j++].ColumnName = "GUID_TKDOANHTHU";
                dtData.Columns[j++].ColumnName = "XUATXU";
                dtData.Columns[j++].ColumnName = "SOLUONGCONLAI";
                //dtData.Columns.Add("LOAIKH");
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["GUID_KHO"].ToString().Equals("") || dr["GUID_KHO"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void DMNguyenVatLieuKho(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                int j = 0;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[j++].ColumnName = "STT";
                dtData.Columns[j++].ColumnName = "GUID_KHO";
                dtData.Columns[j++].ColumnName = "MAVACH";
                dtData.Columns[j++].ColumnName = "MA";
                dtData.Columns[j++].ColumnName = "TEN";
                dtData.Columns[j++].ColumnName = "ID_LOAIHH";
                dtData.Columns[j++].ColumnName = "SOLUONGDAUKY";
                dtData.Columns[j++].ColumnName = "THANHTIENDAUKY";
                dtData.Columns[j++].ColumnName = "THUESUAT";
                dtData.Columns[j++].ColumnName = "DGVON";
                dtData.Columns[j++].ColumnName = "DGBAN";
                dtData.Columns[j++].ColumnName = "GUID_TKKHO";
                dtData.Columns[j++].ColumnName = "GUID_TKCHIPHI";
                dtData.Columns[j++].ColumnName = "GUID_TKDOANHTHU";
                dtData.Columns[j++].ColumnName = "XUATXU";
                dtData.Columns[j++].ColumnName = "SOLUONGCONLAI";
                //dtData.Columns.Add("LOAIKH");
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["GUID_KHO"].ToString().Equals("") || dr["GUID_KHO"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        public struct Result
        {
            public enum State
            {
                Total,
                Executing,
                Commit,
                Failed
            }
            int iIndexDel;
            State fState;
            string sError;
            System.Data.DataTable dtSource;
            public Result(string sErr, System.Data.DataTable dtS, State fS, int iIndex)
            {
                sError = sErr;
                dtSource = dtS;
                fState = fS;
                iIndexDel = iIndex;
            }
            public bool Delete
            {
                get
                {
                    if (iIndexDel == 1)
                        return true;
                    else
                        return false;
                }
            }
            public string Error
            {
                get { return sError; }
                set { sError = value; }
            }
            public State ProState
            {
                get { return fState; }
                set { fState = value; }
            }
            public DataTable DataSource
            {
                get { return dtSource; }
                set { dtSource = value; }
            }
        }

        #region Define variable
        DateTimeFormatInfo dateinfo = new DateTimeFormatInfo();
        DateTimeFormatInfo dateinfommyyyy = new DateTimeFormatInfo();
        //bool canClose = true;
        #endregion

        #region Delegate and constructor
        public delegate void EventResult(Result e);
        public EventResult ResEvent;

        public frmImport()
        {
            InitializeComponent();
            dateinfo.ShortDatePattern = "dd/MM/yyyy";
            dateinfo.DateSeparator = "/";
            dateinfommyyyy.ShortDatePattern = "MM/yyyy";
            dateinfommyyyy.DateSeparator = "/";
           // string cty_info_dinhdang = TS24.SM24.BaseMethod.BaseParam.strDinhDang;
        }

        TypeImport tImport = TypeImport.None;

        public TypeImport Import
        {
            get { return tImport; }
            set { tImport = value; }
        }
        #endregion

        #region Event form
        private void ImportExport_Load(object sender, EventArgs e)
        {
            //tImport = TypeImport.KK05;
            if(!bolHienThiXoaDuLieu)
                radChange.Properties.Items.RemoveAt(1);
        }
         
        private void btnDone_Click(object sender, EventArgs e)
        {
            if (!bgrWorker.IsBusy)
                bgrWorker.RunWorkerAsync();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (!bgrWorker.IsBusy)
                bgrWorker.CancelAsync();
            this.DialogResult = DialogResult.Abort;
        }

        private void btnbrowse_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel Workbook |*.xls;*.xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                txtbrowse.Text = ofd.FileName;
                btnDone.Enabled = true;
            }
            ofd.Dispose();
        }
        #endregion

        DataTable fncReadOledb(string sPathRoot)
        {
            CultureInfo cr = Application.CurrentCulture;
            try
            {
                Application.CurrentCulture = new CultureInfo("en-US");
                FileInfo f = new FileInfo(sPathRoot);
                FileStream sf = f.OpenRead();

                WorkBook m_book = new WorkBook();
                if (f.Extension.ToUpper() == ".XLS")
                    m_book.read(sf);
                else
                    m_book.readXLSX(sf);

                sf.Close();
                m_book.Sheet = 0;
                DataTable dtSource = m_book.ExportDataTable();
                if (dtSource != null && dtSource.Rows.Count > 0)
                    dtSource.Rows.RemoveAt(0);
                Application.CurrentCulture = cr;
                return dtSource;
            }
            catch (Exception e)
            {
                Application.CurrentCulture = cr;
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return null;
            }
        }

        #region Process import data
        object doProcess()
        {
            try
            {
                Result res = new Result();
                if (tImport == TypeImport.None)
                {
                    res = new Result(null, null, Result.State.Failed, radChange.SelectedIndex);
                    return res;
                }
                string sPathRoot = txtbrowse.Text;
                DataTable dtSource = null;
                DataTable dtCopy = new DataTable();
                dtCopy = fncReadOledb(sPathRoot);
                dtSource = dtCopy;
                if (dtSource != null)
                {
                    return fncSetSource(dtSource);
                }

                else
                    res = new Result(null, null, Result.State.Failed, radChange.SelectedIndex);
                return res;
            }
            catch (Exception e)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                return null;
            }
        }

        #endregion

        #region Event backgroundworker
        private void bgrWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            //canClose = false;
            e.Result = doProcess();
        }

        private void bgrWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (!bgrWorker.CancellationPending)
            {
                if (e.UserState == null)
                    return;
                switch ((Result.State)e.UserState)
                {
                    case Result.State.Total:
                        fncSetMaxPgr(e.ProgressPercentage);
                        break;

                    case Result.State.Executing:
                        fncUpdateProcess(e.ProgressPercentage);
                        break;

                    case Result.State.Failed:
                        fncShowError("EIM01");
                        break;
                    case Result.State.Commit:

                        break;
                }
            }

        }

        private void bgrWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //canClose = true;
            if (e.Cancelled)
            {
                this.DialogResult = DialogResult.Abort;
                return;
            }
            if (e.Result == null)
            {
                Result resError = new Result("Sai format", null, Result.State.Failed, -1);
                if (ResEvent != null)
                {
                    ResEvent(resError);
                    this.DialogResult = DialogResult.OK;
                }
                this.DialogResult = DialogResult.OK;
                return;
            }
            Result res = (Result)e.Result;
            if (ResEvent != null)
            {
                ResEvent(res);
                this.DialogResult = DialogResult.OK;
            }
        }
        private void frmImport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (bgrWorker.IsBusy)
            {
                bgrWorker.CancelAsync();
            }
        }
        #endregion

        #region Read oledb
        DataTable fncReadOledb(string sPathRoot, int startrow, int startcol)
        {
            try
            {
                WorkBook m_book = new WorkBook();
                m_book.read(sPathRoot);
                DataTable dtSource = m_book.ExportDataTable(startrow, startcol, m_book.LastRow, m_book.LastCol);
                if (dtSource.Rows.Count > 0)
                    dtSource.Rows.RemoveAt(0);
                return dtSource;
            }
            catch (Exception e)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                fncShowError("EIM03");
                return null;
            }
        }
        #endregion

        #region Set data for source
        object fncSetSource(DataTable dtData)
        {
            Result res = new Result();
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                {
                    res = new Result(null, null, Result.State.Failed, radChange.SelectedIndex);
                    return null;
                }
                DataTable dtSource = null;
                switch (tImport)
                {
                    case TypeImport.Muavao:
                        fncMuavao(ref dtSource, dtData);
                        break;
                    case TypeImport.Muavaokhonghoadon:
                        fncMuavaokhonghoadon(ref dtSource, dtData);
                        break;
                    case TypeImport.Banra:
                        fncBanra(ref dtSource, dtData);
                        break;
                    case TypeImport.DMKhachHang:
                        fncDMKhachHang(ref dtSource, dtData);
                        break;
                    //long
                    case TypeImport.DMBoPhan:
                        fncDMBoPhan(ref dtSource, dtData);
                        break;
                    case TypeImport.DMBoPhanKhauHao:
                        fncDMBoPhanKhauHao(ref dtSource, dtData);
                        break;
                    case TypeImport.DMChiPhi:
                        fncDMChiPhi(ref dtSource, dtData);
                        break;
                    case TypeImport.DMBanan:
                        fncDMBanan(ref dtSource, dtData);
                        break;
                    case TypeImport.DMDvt:
                        fncDMDvt(ref dtSource, dtData);
                        break;
                    case TypeImport.DMHangHoa:
                        fncDMHangHoa(ref dtSource, dtData);
                        break;
                    case TypeImport.DMHangHoaKho:
                        fncDMHangHoaKho(ref dtSource, dtData);
                        break;
                    case TypeImport.DMNguyenVatLieuKho:
                        DMNguyenVatLieuKho(ref dtSource, dtData);
                        break;
                    case TypeImport.DMHangHoaDonGia:
                        fncDMHangHoaDonGia(ref dtSource, dtData);
                        break;
                    case TypeImport.DmVoucher:
                        fncDMVoucher(ref dtSource, dtData);
                        break;
                    case TypeImport.DMKho:
                        fncDMKho(ref dtSource, dtData);
                        break;
                    case TypeImport.DMCuaHangSM24:
                        fncDMCuaHangSM24(ref dtSource, dtData);
                        break;
                    case TypeImport.DMCuaHangVTA:
                        fncDMCuaHangVTA(ref dtSource, dtData);
                        break;
                    case TypeImport.DMTramYTe:
                        fncDMTramYTe(ref dtSource, dtData);
                        break;
                    case TypeImport.DMKhuVuc:
                        fncDMKhuVuc(ref dtSource, dtData);
                        break;
                    case TypeImport.DMLoaiHH:
                        fncDMLoaiHH(ref dtSource, dtData);
                        break;
                    case TypeImport.DMLoaiKH:
                        fncDMLoaiKH(ref dtSource, dtData);
                        break;
                    case TypeImport.DMLoaiTheKH:
                        fncDMLoaiTheKH(ref dtSource, dtData);
                        break;
                    case TypeImport.DMPtx:
                        fncDMPtx(ref dtSource, dtData);
                        break;
                    case TypeImport.DMPtn:
                        fncDMPtn(ref dtSource, dtData);
                        break;
                    case TypeImport.DMLoaiTS:
                        fncDMLoaiTS(ref dtSource, dtData);
                        break;
                    case TypeImport.DMNhanVien:
                        fncDMNhanVien(ref dtSource, dtData);
                        break;
                    case TypeImport.DMTaiKhoan:
                        fncDMTaiKhoan(ref dtSource, dtData);
                        break;
                    case TypeImport.DMTaiKhoanNganHang:
                        fncDMTaiKhoanNganHang(ref dtSource, dtData);
                        break;
                    case TypeImport.DMThueSuat:
                        fncDMThueSuat(ref dtSource, dtData);
                        break;
                    case TypeImport.DMTienTe:
                        fncDMTienTe(ref dtSource, dtData);
                        break;
                    case TypeImport.DMTscd:
                        fncDMTscd(ref dtSource, dtData);
                        break;
                    case TypeImport.ct_phieuthu:
                        fncct_phieuthu(ref dtSource, dtData);
                        break;
                    case TypeImport.ct_phieuchi:
                        fncct_phieuchi(ref dtSource, dtData);
                        break;
                    case TypeImport.ct_gbc:
                        fncct_gbc(ref dtSource, dtData);
                        break;
                    case TypeImport.ct_gbn:
                        fncct_gbn(ref dtSource, dtData);
                        break;
                    case TypeImport.ct_tonghop:
                        fncct_tonghop(ref dtSource, dtData);
                        break;
                    case TypeImport.kt_thanhtoandonhang:
                        fncKtthanhtoandonhang(ref dtSource, dtData);
                        break;
                    case TypeImport.kt_thanhtoantragop:
                        fncKtthanhtoantragop(ref dtSource, dtData);
                        break;
                    case TypeImport.solieudauky:
                        fncSolieudauky(ref dtSource, dtData);
                        break;
                    //Mua Hang
                    case TypeImport.PhieuDeNghiMuaHang:
                        fncPhieuDeNghiMuaHang(ref dtSource, dtData);
                        break;
                    case TypeImport.DonMuaHang:
                        fncDonMuaHang(ref dtSource, dtData);
                        break;
                    case TypeImport.PhieuNhap:
                        fncPhieuNhap(ref dtSource, dtData);
                        break;
                    //Ban Hang
                    case TypeImport.DonDatHang:
                        fncDonDatHang(ref dtSource, dtData);
                        break;
                    case TypeImport.PhieuBaoGia:
                        fncPhieuBaoGia(ref dtSource, dtData);
                        break;
                    case TypeImport.PhieuXuat:
                        fncPhieuXuat(ref dtSource, dtData);
                        break;
                    case TypeImport.dm_biendongvat:
                        fncBienDongVAT(ref dtSource, dtData);
                        break;
                    case TypeImport.ChiTietHangHoa:
                        fncChiTietHangHoa(ref dtSource, dtData);
                        break;
                    case TypeImport.ChiTietHangHoaMTT:
                        fncChiTietHangHoaMTT(ref dtSource, dtData);
                        break;
                         
                    case TypeImport.importPhieuxuat:
                        fncImportPhieuXuat(ref dtSource, dtData);
                        break;
                    case TypeImport.Khuyenmai:
                        fncImportKhuyenMai(ref dtSource, dtData);
                        break;
                    case TypeImport.ChiTietHangHoa02:
                        fncChiTietHangHoa02(ref dtSource, dtData);
                        break;
                    case TypeImport.ChiTietHangHoa03:
                        fncChiTietHangHoa03(ref dtSource, dtData);
                        break;
                    case TypeImport.CongNoTaiKhoan:
                        fncCongNoTaiKhoan(ref dtSource, dtData);
                        break;
                    case TypeImport.CongNoTaiKhoanDoiTuong:
                        fncCongNoTaiKhoanDoiTuong(ref dtSource, dtData);
                        break;
                    case TypeImport.CongNoPhaiThuTheoChungTu:
                        fncCongNoPhaiThuTheoChungTu(ref dtSource, dtData);
                        break;
                    case TypeImport.CongNoPhaiTraTheoChungTu:
                        fncCongNoPhaiTraTheoChungTu(ref dtSource, dtData);
                        break;
                    case TypeImport.DanhSachChungTuKhauTruThueTNCN:
                        fncDanhSachChungTuKhauTruThueTNCN(ref dtSource, dtData);
                        break;
                    case TypeImport.ChiTietHangHoaDacTrung:
                        fncChiTietHangHoaDacTrung(ref dtSource, dtData);
                        break;
                }
                if (dtSource != null && dtSource.Rows.Count > 0)
                    res = new Result(null, dtSource, Result.State.Commit, radChange.SelectedIndex);
                else
                    res = new Result(null, null, Result.State.Failed, radChange.SelectedIndex);
                return res;


            }
            catch (Exception e)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                //fncShowError("EIM01");
                res = new Result(null, null, Result.State.Failed, radChange.SelectedIndex);
                return null;
            }
        }

        void fncReturnEve(string sErr, DataTable dtS, Result.State state)
        {
            Result res = new Result(sErr, dtS, state, radChange.SelectedIndex);
            if (ResEvent != null)
                ResEvent(res);
            if (state == Result.State.Commit)
            {
                this.DialogResult = DialogResult.OK;
                //this.Close();
            }
        }

        void fncUpdateProcess(int value)
        {
            pgrCtr.EditValue = value;
            pgrCtr.Update();
        }


        void fncSetMaxPgr(int max)
        {
            pgrCtr.Properties.Maximum = max;
        }

        void fncShowError(string sID)
        {
            //string s = "EIM01";
            Common.CallMsgBox(MessageBoxButtons.OK, sID);
        }

        #endregion

        #region Detail function

        void fncMuavao(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "SERINO";
                dtData.Columns[1].ColumnName = "SOHD";
                dtData.Columns[2].ColumnName = "NGAYPN";
                dtData.Columns[3].ColumnName = "NGAYHD";
                dtData.Columns[4].ColumnName = "MASOTHUEKH";
                dtData.Columns[5].ColumnName = "TENKH";
                dtData.Columns[6].ColumnName = "LYDONHAP";
                dtData.Columns[7].ColumnName = "DS_CHUATHUE";
                dtData.Columns[8].ColumnName = "THUESUAT";
                dtData.Columns[9].ColumnName = "THUEGTGT";
                dtData.Columns[10].ColumnName = "TONGTIEN";
                dtData.Columns[11].ColumnName = "TK_NO";
                dtData.Columns[12].ColumnName = "TK_CO";
                dtData.Columns[13].ColumnName = "LOAIHD";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SERINO"].ToString().Equals("") || dr["SERINO"].ToString().Trim().Equals("")) &&
                        (dr["SOHD"].ToString().Equals("") || dr["SOHD"].ToString().Trim().Equals("")) &&
                        (dr["NGAYPN"].ToString().Equals("") || dr["NGAYPN"].ToString().Trim().Equals("")) &&
                        (dr["NGAYHD"].ToString().Equals("") || dr["NGAYHD"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncMuavaokhonghoadon(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "NGAYPN";
                dtData.Columns[1].ColumnName = "MASOTHUEKH";
                dtData.Columns[2].ColumnName = "TENKH";
                dtData.Columns[3].ColumnName = "LYDONHAP";
                dtData.Columns[4].ColumnName = "DS_CHUATHUE";
                dtData.Columns[5].ColumnName = "THUESUAT";
                dtData.Columns[6].ColumnName = "THUEGTGT";
                dtData.Columns[7].ColumnName = "TONGTIEN";
                dtData.Columns[8].ColumnName = "TK_NO";
                dtData.Columns[9].ColumnName = "TK_CO";
                dtData.Columns[10].ColumnName = "LOAIHD";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["NGAYPN"].ToString().Equals("") || dr["NGAYPN"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncBanra(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "SERINO";
                dtData.Columns[1].ColumnName = "SOHD";
                dtData.Columns[2].ColumnName = "NGAYHD";
                dtData.Columns[3].ColumnName = "MASOTHUEKH";
                dtData.Columns[4].ColumnName = "TENKH";
                dtData.Columns[5].ColumnName = "LYDOXUAT";
                dtData.Columns[6].ColumnName = "DS_CHUATHUE";
                dtData.Columns[7].ColumnName = "THUESUAT";
                dtData.Columns[8].ColumnName = "THUEGTGT";
                dtData.Columns[9].ColumnName = "TONGTIEN";
                dtData.Columns[10].ColumnName = "TK_NO";
                dtData.Columns[11].ColumnName = "TK_CO";
                dtData.Columns[12].ColumnName = "LOAIHD";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SERINO"].ToString().Equals("") || dr["SERINO"].ToString().Trim().Equals("")) &&
                        (dr["SOHD"].ToString().Equals("") || dr["SOHD"].ToString().Trim().Equals("")) &&
                        (dr["NGAYHD"].ToString().Equals("") || dr["NGAYHD"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMKhachHang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "GUID_LOAIKH";
                dtData.Columns[i++].ColumnName = "MA";
                dtData.Columns[i++].ColumnName = "TEN";
                dtData.Columns[i++].ColumnName = "TENCONGTY";
                dtData.Columns[i++].ColumnName = "MASOTHUEKH";
                dtData.Columns[i++].ColumnName = "CCCDAN";
                dtData.Columns[i++].ColumnName = "MDVQHNSACH";
                dtData.Columns[i++].ColumnName = "TKNHAPHANPHOI";
                dtData.Columns[i++].ColumnName = "TINHTRANGTAIKHOAN";
                dtData.Columns[i++].ColumnName = "DIACHI";
                dtData.Columns[i++].ColumnName = "DIACHIGIAOHANG";

                dtData.Columns[i++].ColumnName = "DIACHIXHD";
                dtData.Columns[i++].ColumnName = "DIENTHOAI";
                dtData.Columns[i++].ColumnName = "DIENTHOAIDD";
                dtData.Columns[i++].ColumnName = "EMAIL";
                dtData.Columns[i++].ColumnName = "FAX";
                dtData.Columns[i++].ColumnName = "NGAYTAO";
                dtData.Columns[i++].ColumnName = "GUID_KHUVUC";
                dtData.Columns[i++].ColumnName = "GUID_NGUOIQLY";
                dtData.Columns[i++].ColumnName = "LOAIDONGIA";
                dtData.Columns[i++].ColumnName = "SOTKNH";
                dtData.Columns[i++].ColumnName = "TAINGANHANG";
                dtData.Columns[i++].ColumnName = "GUID_THEKH";
                dtData.Columns[i++].ColumnName = "SOTHEKH";
                dtData.Columns[i++].ColumnName = "DIEMTICHLUY";
                dtData.Columns[i++].ColumnName = "GIOITINH";
                dtData.Columns[i++].ColumnName = "NGAYSINH";
                dtData.Columns[i++].ColumnName = "KHTIEMNANG";
                dtData.Columns[i++].ColumnName = "NHANTINNHAN";
                dtData.Columns[i++].ColumnName = "LIENLAC";
                dtData.Columns[i++].ColumnName = "GHICHU";

                dtData.Columns[i++].ColumnName = "DOITUONGKH";
                //dtData.Columns.Add("LOAIKH");
                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(j);

                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        //long
        void fncDMNhanVien(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                bool stype = false, stypeNC = false;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "NGAYSINH";
                dtData.Columns[3].ColumnName = "NOISINH";
                dtData.Columns[4].ColumnName = "CMND";
                dtData.Columns[5].ColumnName = "NGAYCAP";
                dtData.Columns[6].ColumnName = "NOICAP";
                dtData.Columns[7].ColumnName = "DIACHITT";
                dtData.Columns[10].ColumnName = "EMAIL";
                dtData.Columns[8].ColumnName = "DIACHILL";
                dtData.Columns[9].ColumnName = "DIENTHOAI";
                dtData.Columns[11].ColumnName = "ID_BOPHAN";
                dtData.Columns[12].ColumnName = "CHUCVU";
                dtData.Columns[13].ColumnName = "ID_LUONGCB";
                //dtData.Columns.Add("LOAIKH");
                dtData.AcceptChanges();
                dtSource = dtData.Clone();
                //foreach (DataColumn dc in dtSource.Columns)
                //  dc.DataType = Type.GetType("System.String");
                if (dtData.Columns["NGAYSINH"].DataType == Type.GetType("System.DateTime"))
                    stype = true;
                if (dtData.Columns["NGAYCAP"].DataType == Type.GetType("System.DateTime"))
                    stypeNC = true;
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    else
                    {
                        //ngay sinh
                        if (stype)
                        {

                            if (dr["NGAYSINH"] != DBNull.Value && dr["NGAYSINH"].ToString() != "")
                                dr["NGAYSINH"] = Convert.ToDateTime(dr["NGAYSINH"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer = Common.ConvertDouble(dr["NGAYSINH"]);
                            if (dmer > 0)
                            {
                                try
                                {
                                    dr["NGAYSINH"] = DateTime.FromOADate(dmer).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "dd/MM/yyyy";
                                if (dr["NGAYSINH"] != DBNull.Value && dr["NGAYSINH"].ToString() != "")
                                    dr["NGAYSINH"] = Convert.ToDateTime(dr["NGAYSINH"], dateinfo).ToString("dd/MM/yyyy");
                            }
                        }

                        //ngay cap
                        if (stypeNC)
                        {

                            if (dr["NGAYCAP"] != DBNull.Value && dr["NGAYCAP"].ToString() != "")
                                dr["NGAYCAP"] = Convert.ToDateTime(dr["NGAYCAP"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer1 = Common.ConvertDouble(dr["NGAYCAP"]);
                            if (dmer1 > 0)
                            {
                                try
                                {
                                    dr["NGAYCAP"] = DateTime.FromOADate(dmer1).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "MM/dd/yyyy";
                                if (dr["NGAYCAP"] != DBNull.Value && dr["NGAYCAP"].ToString() != "")
                                    dr["NGAYCAP"] = Convert.ToDateTime(dr["NGAYCAP"], dateinfo).ToString("dd/MM/yyyy");
                            }
                        }
                    }
                    dtSource.ImportRow(dr);
                }

                //dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMTscd(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "LOAITS";
                dtData.Columns[3].ColumnName = "MATK";
                dtData.Columns[4].ColumnName = "NGAYNHAP";
                dtData.Columns[5].ColumnName = "NGUYENGIA";
                dtData.Columns[6].ColumnName = "SOLUONG";
                dtData.Columns[7].ColumnName = "TINHTRANGSD";
                dtData.Columns[10].ColumnName = "TRIGIAKH";
                dtData.Columns[8].ColumnName = "BOPHANSD";
                dtData.Columns[9].ColumnName = "NHACC";
                dtData.Columns[11].ColumnName = "GIATRICONLAI";
                dtData.Columns[12].ColumnName = "THOIGIANSD";
                dtData.Columns[13].ColumnName = "TYLEKH";
                dtData.Columns[14].ColumnName = "TRICHKHTKCO";
                dtData.Columns[15].ColumnName = "TRICHKHTKNO";
                dtData.Columns[16].ColumnName = "NGAYSD";
                dtData.Columns[17].ColumnName = "NGAYGIAM";
                dtData.Columns[18].ColumnName = "LYDOGIAM";
                dtData.Columns[19].ColumnName = "STTTSCD";
                //dtData.Columns.Add("LOAIKH");
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMHangHoa(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "MAVACH";
                dtData.Columns[i++].ColumnName = "MA";
                dtData.Columns[i++].ColumnName = "TEN";
                // dtData.Columns[i++].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[i++].ColumnName = "GUID_LOAIHH";
                dtData.Columns[i++].ColumnName = "GUID_DVT";
                dtData.Columns[i++].ColumnName = "THUESUAT";
                dtData.Columns[i++].ColumnName = "DGVON";
                dtData.Columns[i++].ColumnName = "DGBAN";
                //dtData.Columns[i++].ColumnName = "DGBANC1";
                //dtData.Columns[i++].ColumnName = "DGBANC2";
                //dtData.Columns[i++].ColumnName = "DGBANC3";
                //dtData.Columns[i++].ColumnName = "DGBANSI";
                //dtData.Columns[i++].ColumnName = "DGBIA";
                dtData.Columns[i++].ColumnName = "DINHMUCTON";
                dtData.Columns[i++].ColumnName = "XUATXU";
                dtData.Columns[i++].ColumnName = "HANSUDUNG";
                //  dtData.Columns[i++].ColumnName = "HINHANH";
                dtData.Columns[i++].ColumnName = "MOTA";
                dtData.Columns[i++].ColumnName = "THOIGIANBAOHANH";
                dtData.Columns[i++].ColumnName = "IMEI";
                dtData.Columns[i++].ColumnName = "HANGCOBAOHANH";
                dtData.Columns[i++].ColumnName = "HANGMUANGOAI";
                dtData.Columns[i++].ColumnName = "CHOPHEPXUATAM";
                dtData.Columns[i++].ColumnName = "KHONGDUNGKHO";
                dtData.Columns[i++].ColumnName = "KHONGSUDUNG";
                dtData.Columns[i++].ColumnName = "PATHHINHANH";
                // dtData.Columns[i++].ColumnName = "COMBO";
                //dtData.Columns.Add("LOAIKH");
                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMHangHoaDonGia(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "MAHANGHOA";
                dtData.Columns[i++].ColumnName = "DONGIA";
                dtData.Columns[i++].ColumnName = "MADOITUONG";
                dtData.Columns[i++].ColumnName = "DIEUKIEN";
                dtData.Columns[i++].ColumnName = "LOAIDONGIA";
                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if ((dr["MAHANGHOA"].ToString().Equals("") || dr["MAHANGHOA"].ToString().Trim().Equals(""))) //&&
                        //  (dr["DONGIA"].ToString().Equals("") || dr["DONGIA"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMVoucher(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MAVACH";
                dtData.Columns[2].ColumnName = "MA";
                dtData.Columns[3].ColumnName = "TEN";
                dtData.Columns[4].ColumnName = "GUID_LOAIHH";
                dtData.Columns[5].ColumnName = "NGAYTAO";
                dtData.Columns[6].ColumnName = "GUID_DVT";
                dtData.Columns[7].ColumnName = "THUESUAT";
                dtData.Columns[8].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[9].ColumnName = "DGVON";
                dtData.Columns[10].ColumnName = "SERINO";
                dtData.Columns[11].ColumnName = "COTHOIHAN";
                dtData.Columns[12].ColumnName = "THOIGIANHETHAN";
                dtData.Columns[13].ColumnName = "KHONGSUDUNG";
                dtData.Columns[14].ColumnName = "HANGMUANGOAI";

                //dtData.Columns.Add("LOAIKH");
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMBoPhan(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "ID_TAIKHOAN";
                dtData.Columns[3].ColumnName = "ID_CHA";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMChiPhi(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "GHICHU";
                dtData.Columns[3].ColumnName = "ID_TAIKHOAN";
                dtData.Columns[4].ColumnName = "ID_LOAICP";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMTaiKhoan(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "MATKCHA";
                dtData.Columns[3].ColumnName = "NAMAPDUNG";
                dtData.Columns[4].ColumnName = "GHICHU";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMTaiKhoanNganHang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[1].ColumnName = "SOTK";
                dtData.Columns[3].ColumnName = "LOAITIEN";
                dtData.Columns[4].ColumnName = "KYHIEUCHUNGTU";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMDvt(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMPtn(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[3].ColumnName = "MACDINH";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMPtx(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[3].ColumnName = "MACDINH";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMKhuVuc(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMBanan(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                int j = 0;
                dtData.AcceptChanges();
                dtData.Columns[j++].ColumnName = "STT";
                dtData.Columns[j++].ColumnName = "MA";
                dtData.Columns[j++].ColumnName = "TEN";
                dtData.Columns[j++].ColumnName = "GUID_CHA";
                dtData.Columns[j++].ColumnName = "SOLUONGGHE";
                dtData.Columns[j++].ColumnName = "TENMAYIN";
                dtData.Columns[j++].ColumnName = "KHOGIAY";
                dtData.Columns[j++].ColumnName = "ISBA";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMKho(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[3].ColumnName = "GUID_NGUOIQUANLY";
                dtData.Columns[4].ColumnName = "NGUOILIENHE";
                dtData.Columns[5].ColumnName = "DIACHI";
                dtData.Columns[6].ColumnName = "DIENTHOAI";
                dtData.Columns[7].ColumnName = "GUID_CHA";//danh muc cua hang
                dtData.Columns[8].ColumnName = "MACDINH";
                dtData.Columns[9].ColumnName = "GHICHU";
                dtData.Columns[10].ColumnName = "TINHTRANG";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMCuaHangSM24(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                int a = 0;
                dtData.AcceptChanges();

                dtData.Columns[a++].ColumnName = "STT";
                dtData.Columns[a++].ColumnName = "MA";
                dtData.Columns[a++].ColumnName = "TEN";
                dtData.Columns[a++].ColumnName = "MACDINH";
                dtData.Columns[a++].ColumnName = "GUID_NGUOIQUANLY";
                dtData.Columns[a++].ColumnName = "NGUOILIENHE";
                dtData.Columns[a++].ColumnName = "DIACHI";
                dtData.Columns[a++].ColumnName = "DIENTHOAI";
                dtData.Columns[a++].ColumnName = "GHICHU";
                dtData.Columns[a++].ColumnName = "TINHTRANG";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMTramYTe(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MASOTHUE";
                dtData.Columns[2].ColumnName = "MA";
                dtData.Columns[3].ColumnName = "TEN";
                dtData.Columns[4].ColumnName = "GUID_NGUOIQUANLY";
                dtData.Columns[5].ColumnName = "NGUOILIENHE";
                dtData.Columns[6].ColumnName = "DIACHI";
                dtData.Columns[7].ColumnName = "DIENTHOAI";
                dtData.Columns[8].ColumnName = "GHICHU";

                //dtData.Columns[9].ColumnName = "TINHTRANG";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    if (BaseParamIE.ActiveTax != dr["MASOTHUE"].ToString().Replace("-", "").ToString())
                    {
                        dtData.Rows.RemoveAt(i);
                    }
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMCuaHangVTA(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MASOTHUE";
                dtData.Columns[2].ColumnName = "MA";
                dtData.Columns[3].ColumnName = "TEN";
                //dtData.Columns[3].ColumnName = "MACDINH";
                //dtData.Columns[4].ColumnName = "GUID_NGUOIQUANLY";
                dtData.Columns[4].ColumnName = "NGUOILIENHE";
                dtData.Columns[5].ColumnName = "DIACHI";
                dtData.Columns[6].ColumnName = "DIENTHOAI";
                dtData.Columns[7].ColumnName = "GHICHU";

                //dtData.Columns[9].ColumnName = "TINHTRANG";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    if (BaseParamIE.ActiveTax != dr["MASOTHUE"].ToString().Replace("-", "").ToString())
                    {
                        dtData.Rows.RemoveAt(i);
                    }
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }


        void fncDMThueSuat(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                //dtData.Columns[0].ColumnName = "ID";
                dtData.Columns[0].ColumnName = "THUESUAT";
                dtData.Columns[1].ColumnName = "DIENGIAI";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["THUESUAT"].ToString().Equals("") || dr["THUESUAT"].ToString().Trim().Equals("")))

                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMLoaiHH(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[3].ColumnName = "GUID_CHA";
                dtData.Columns[4].ColumnName = "MOHINHSIEUTHI";
                dtData.Columns[5].ColumnName = "MOHINHNHAHANG";
                dtData.Columns[6].ColumnName = "KHONGSD";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMTienTe(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "TYGIA";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMLoaiKH(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "MA";
                dtData.Columns[2].ColumnName = "TEN";
                dtData.Columns[3].ColumnName = "GUID_CHA";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMLoaiTheKH(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "STT";
                dtData.Columns[1].ColumnName = "LOAITHE";
                dtData.Columns[2].ColumnName = "NGAYCAP";
                dtData.Columns[3].ColumnName = "NGAYHETHAN";
                dtData.Columns[4].ColumnName = "SODIEMCANTLUY";
                dtData.Columns[5].ColumnName = "DONGTIEN";
                dtData.Columns[6].ColumnName = "SOTIENDONG";
                dtData.Columns[7].ColumnName = "TINHTRANGTHE";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["LOAITHE"].ToString().Equals("") || dr["LOAITHE"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMLoaiTS(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "MA";
                dtData.Columns[1].ColumnName = "TEN";
                dtData.Columns[2].ColumnName = "ID_CHA";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                        (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDMBoPhanKhauHao(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();

                dtData.Columns[0].ColumnName = "TEN";
                dtData.Columns[1].ColumnName = "ID_BOPHAN";
                dtData.Columns[2].ColumnName = "GHICHU";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncKtthanhtoandonhang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "DOTTT";
                dtData.Columns[1].ColumnName = "NGAYHENTT";
                dtData.Columns[2].ColumnName = "SOTIENPHAITT";
                dtData.Columns[3].ColumnName = "DATHANHTOAN";
                dtData.Columns[4].ColumnName = "KYHIEUTT";
                dtData.Columns[5].ColumnName = "SOCTTT";
                dtData.Columns[6].ColumnName = "NGAYTT";
                dtData.Columns[7].ColumnName = "NGUOITT";
                dtData.Columns[8].ColumnName = "SOTIENTT";
                dtData.Columns[9].ColumnName = "GHICHU";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["DOTTT"].ToString().Equals("") || dr["DOTTT"].ToString().Trim().Equals("")) &&
                        (dr["NGAYHENTT"].ToString().Equals("") || dr["NGAYHENTT"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncKtthanhtoantragop(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "DOTTT";
                dtData.Columns[1].ColumnName = "NGAYHENTT";
                dtData.Columns[2].ColumnName = "DUNODK";
                dtData.Columns[3].ColumnName = "TRATIENGOC";
                dtData.Columns[4].ColumnName = "TRATIENLAI";
                dtData.Columns[5].ColumnName = "SOTIENPHAITT";
                dtData.Columns[6].ColumnName = "DATHANHTOAN";
                dtData.Columns[7].ColumnName = "KYHIEUTT";
                dtData.Columns[8].ColumnName = "SOCTTT";
                dtData.Columns[9].ColumnName = "NGAYTT";
                dtData.Columns[10].ColumnName = "NGUOITT";
                dtData.Columns[11].ColumnName = "SOTIENTT";
                dtData.Columns[12].ColumnName = "GHICHU";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["DOTTT"].ToString().Equals("") || dr["DOTTT"].ToString().Trim().Equals("")) &&
                        (dr["NGAYHENTT"].ToString().Equals("") || dr["NGAYHENTT"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncSolieudauky(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "GUID_KHO";
                dtData.Columns[1].ColumnName = "GUID_HANGHOA";
                dtData.Columns[2].ColumnName = "TENDVT";
                dtData.Columns[3].ColumnName = "SOLUONG";
                dtData.Columns[4].ColumnName = "DONGIA";
                dtData.Columns[5].ColumnName = "THANHTIEN";
                dtData.Columns[6].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[7].ColumnName = "KYHIEU";
                dtData.Columns[8].ColumnName = "SOCT";
                dtData.Columns[9].ColumnName = "NGAYCT";
                dtData.Columns[10].ColumnName = "GHICHU";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncPhieuThu(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "SOCT";
                dtData.Columns[1].ColumnName = "NGAYCT";
                dtData.Columns[2].ColumnName = "DIENGIAI";
                dtData.Columns[3].ColumnName = "MASOTHUEKH";
                dtData.Columns[4].ColumnName = "TENKH";
                dtData.Columns[5].ColumnName = "DIACHI";
                dtData.Columns[6].ColumnName = "SOTIEN";
                dtData.Columns[7].ColumnName = "TKNO";
                dtData.Columns[8].ColumnName = "TKCO";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];

                    if ((dr["SOCT"].ToString().Equals("") || dr["SOCT"].ToString().Trim().Equals("")) &&
                        (dr["NGAYCT"].ToString().Equals("") || dr["NGAYCT"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);

                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncPhieuChi(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "SOCT";
                dtData.Columns[1].ColumnName = "NGAYCT";
                dtData.Columns[2].ColumnName = "DIENGIAI";
                dtData.Columns[3].ColumnName = "MASOTHUEKH";
                dtData.Columns[4].ColumnName = "TENKH";
                dtData.Columns[5].ColumnName = "DIACHI";
                dtData.Columns[6].ColumnName = "SOTIEN";
                dtData.Columns[7].ColumnName = "TKNO";
                dtData.Columns[8].ColumnName = "TKCO";
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SOCT"].ToString().Equals("") || dr["SOCT"].ToString().Trim().Equals("")) &&
                        (dr["NGAYCT"].ToString().Equals("") || dr["NGAYCT"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    //dtSource.ImportRow(dr);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncct_tonghop(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "KYHIEU";
                dtData.Columns[1].ColumnName = "SO";
                dtData.Columns[2].ColumnName = "NGAY";
                dtData.Columns[3].ColumnName = "DIENGIAI";
                dtData.Columns[4].ColumnName = "TKNO";
                dtData.Columns[5].ColumnName = "TKCO";
                dtData.Columns[6].ColumnName = "SOTIEN";
                dtData.Columns[7].ColumnName = "CHITIETNO";
                dtData.Columns[8].ColumnName = "CHITIETCO";
                dtData.Columns[9].ColumnName = "GHICHU";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SO"].ToString().Equals("") || dr["SO"].ToString().Trim().Equals("")) &&
                        (dr["NGAY"].ToString().Equals("") || dr["NGAY"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    //dtSource.ImportRow(dr);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncct_gbc(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                bool stype = false;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "KYHIEU";
                dtData.Columns[1].ColumnName = "SOUNC";
                dtData.Columns[2].ColumnName = "NGAYLAP";
                dtData.Columns[3].ColumnName = "BENNHAN";
                dtData.Columns[4].ColumnName = "ID_TKNH";
                dtData.Columns[5].ColumnName = "BENNHANNH";
                //dtData.Columns[6].ColumnName = "LOAITIEN";
                dtData.Columns[6].ColumnName = "SOLUONG";
                dtData.Columns[7].ColumnName = "TYGIA";
                dtData.Columns[8].ColumnName = "SOTIEN";
                dtData.Columns[9].ColumnName = "DIENGIAI";
                dtData.Columns[10].ColumnName = "ID_KHACHHANG";
                dtData.Columns[11].ColumnName = "BENCHI";
                dtData.Columns[12].ColumnName = "BENCHISOTK";
                dtData.Columns[13].ColumnName = "BENCHINH";
                dtData.Columns[14].ColumnName = "GHICHU";
                dtData.AcceptChanges();
                dtSource = dtData.Clone();
                //foreach (DataColumn dc in dtSource.Columns)
                //  dc.DataType = Type.GetType("System.String");
                if (dtData.Columns["NGAYLAP"].DataType == Type.GetType("System.DateTime"))
                    stype = true;

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SOUNC"].ToString().Equals("") || dr["SOUNC"].ToString().Trim().Equals("")) &&
                        (dr["NGAYLAP"].ToString().Equals("") || dr["NGAYLAP"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    //dtSource.ImportRow(dr);
                    else
                    {
                        if (stype)
                        {

                            if (dr["NGAYLAP"] != DBNull.Value && dr["NGAYLAP"].ToString() != "")
                                dr["NGAYLAP"] = Convert.ToDateTime(dr["NGAYLAP"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer = Common.ConvertDouble(dr["NGAYLAP"]);
                            if (dmer > 0)
                            {
                                try
                                {
                                    dr["NGAYLAP"] = DateTime.FromOADate(dmer).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "dd/MM/yyyy";
                                if (dr["NGAYLAP"] != DBNull.Value && dr["NGAYLAP"].ToString() != "")
                                    try
                                    {
                                        dr["NGAYLAP"] = Convert.ToDateTime(dr["NGAYLAP"], dateinfo).ToString("dd/MM/yyyy");
                                    }
                                    catch
                                    {
                                        //dr["NGAYLAP"] = dr["NGAYLAP"];
                                    }
                            }
                        }
                        //dtSource.ImportRow(dr);
                        dtData.AcceptChanges();
                    }
                }
                dtSource = dtData.Copy();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncct_gbn(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                bool stype = false;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                dtData.Columns[0].ColumnName = "KYHIEU";
                dtData.Columns[1].ColumnName = "SOUNC";
                dtData.Columns[2].ColumnName = "NGAYLAP";
                dtData.Columns[3].ColumnName = "BENCHI";
                dtData.Columns[4].ColumnName = "ID_TKNH";
                dtData.Columns[5].ColumnName = "BENCHINH";
                dtData.Columns[6].ColumnName = "SOLUONG";
                dtData.Columns[7].ColumnName = "TYGIA";
                dtData.Columns[8].ColumnName = "SOTIEN";
                dtData.Columns[9].ColumnName = "DIENGIAI";
                dtData.Columns[10].ColumnName = "BENNHAN";
                dtData.Columns[11].ColumnName = "BENNHANSOTK";
                dtData.Columns[12].ColumnName = "BENNHANNH";
                dtData.Columns[13].ColumnName = "GHICHU";
                dtData.AcceptChanges();
                dtSource = dtData.Clone();
                //foreach (DataColumn dc in dtSource.Columns)
                //  dc.DataType = Type.GetType("System.String");
                if (dtData.Columns["NGAYLAP"].DataType == Type.GetType("System.DateTime"))
                    stype = true;
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SOUNC"].ToString().Equals("") || dr["SOUNC"].ToString().Trim().Equals("")) &&
                        (dr["NGAYLAP"].ToString().Equals("") || dr["NGAYLAP"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    //dtSource.ImportRow(dr);
                    else
                    {
                        if (stype)
                        {

                            if (dr["NGAYLAP"] != DBNull.Value && dr["NGAYLAP"].ToString() != "")
                                dr["NGAYLAP"] = Convert.ToDateTime(dr["NGAYLAP"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer = Common.ConvertDouble(dr["NGAYLAP"]);
                            if (dmer > 0)
                            {
                                try
                                {
                                    dr["NGAYLAP"] = DateTime.FromOADate(dmer).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "dd/MM/yyyy";
                                if (dr["NGAYLAP"] != DBNull.Value && dr["NGAYLAP"].ToString() != "")
                                    dr["NGAYLAP"] = Convert.ToDateTime(dr["NGAYLAP"], dateinfo).ToString("dd/MM/yyyy");
                            }
                        }
                        //dtSource.ImportRow(dr);
                        dtData.AcceptChanges();

                    }
                }
                dtSource = dtData.Copy();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncct_phieuchi(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                bool stype = false;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;


                dtData.Columns[0].ColumnName = "LOAITIEN";
                dtData.Columns[1].ColumnName = "KYHIEU";
                dtData.Columns[2].ColumnName = "SOPHIEUTC";
                dtData.Columns[3].ColumnName = "NGAYTC";
                dtData.Columns[4].ColumnName = "LYDO";
                dtData.Columns[5].ColumnName = "CHUNGTUKEMTHEO";
                dtData.Columns[6].ColumnName = "DOITUONGTC";
                dtData.Columns[7].ColumnName = "DIACHI";
                dtData.Columns[8].ColumnName = "SOLUONG";
                dtData.Columns[9].ColumnName = "TYGIA";
                dtData.Columns[10].ColumnName = "TONGTIEN";
                dtData.AcceptChanges();
                dtSource = dtData.Clone();
                //foreach (DataColumn dc in dtSource.Columns)
                //  dc.DataType = Type.GetType("System.String");
                if (dtData.Columns["NGAYTC"].DataType == Type.GetType("System.DateTime"))
                    stype = true;
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SOPHIEUTC"].ToString().Equals("") || dr["SOPHIEUTC"].ToString().Trim().Equals("")) &&
                        (dr["NGAYTC"].ToString().Equals("") || dr["NGAYTC"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    else
                    {
                        if (stype)
                        {

                            if (dr["NGAYTC"] != DBNull.Value && dr["NGAYTC"].ToString() != "")
                                dr["NGAYTC"] = Convert.ToDateTime(dr["NGAYTC"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer = Common.ConvertDouble(dr["NGAYTC"]);
                            if (dmer > 0)
                            {
                                try
                                {
                                    dr["NGAYTC"] = DateTime.FromOADate(dmer).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "dd/MM/yyyy";
                                if (dr["NGAYTC"] != DBNull.Value && dr["NGAYTC"].ToString() != "")
                                    dr["NGAYTC"] = Convert.ToDateTime(dr["NGAYTC"], dateinfo).ToString("dd/MM/yyyy");
                            }
                        }
                        //dtSource.ImportRow(dr);
                        dtData.AcceptChanges();
                    }

                }
                dtSource = dtData.Copy();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncct_phieuthu(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                bool stype = false;
                if (dtData == null || dtData.Rows.Count == 0)
                    return;

                dtData.Columns[0].ColumnName = "LOAITIEN";
                dtData.Columns[1].ColumnName = "KYHIEU";
                dtData.Columns[2].ColumnName = "SOPHIEUTC";
                dtData.Columns[3].ColumnName = "NGAYTC";
                dtData.Columns[4].ColumnName = "LYDO";
                dtData.Columns[5].ColumnName = "CHUNGTUKEMTHEO";
                dtData.Columns[6].ColumnName = "DOITUONGTC";
                dtData.Columns[7].ColumnName = "DIACHI";
                dtData.Columns[8].ColumnName = "DIACHIXHD";
                dtData.Columns[9].ColumnName = "SOLUONG";
                dtData.Columns[10].ColumnName = "TYGIA";
                dtData.Columns[11].ColumnName = "TONGTIEN";
                dtData.AcceptChanges();
                dtSource = dtData.Clone();
                //foreach (DataColumn dc in dtSource.Columns)
                //  dc.DataType = Type.GetType("System.String");

                if (dtData.Columns["NGAYTC"].DataType == Type.GetType("System.DateTime"))
                    stype = true;

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["SOPHIEUTC"].ToString().Equals("") || dr["SOPHIEUTC"].ToString().Trim().Equals("")) &&
                        (dr["NGAYTC"].ToString().Equals("") || dr["NGAYTC"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                    else
                    {
                        if (stype)
                        {

                            if (dr["NGAYTC"] != DBNull.Value && dr["NGAYTC"].ToString() != "")
                                dr["NGAYTC"] = Convert.ToDateTime(dr["NGAYTC"]).ToString("dd/MM/yyyy"); //DateTime.ParseExact(dr["NgayPhatHanh"].ToString(), "MM/dd/yyyy", dateinfo);// Convert.ToDateTime(dr["NgayPhatHanh"], dateinfo);
                        }
                        else
                        {
                            double dmer = Common.ConvertDouble(dr["NGAYTC"]);
                            if (dmer > 0)
                            {
                                try
                                {
                                    dr["NGAYTC"] = DateTime.FromOADate(dmer).ToString("dd/MM/yyyy");
                                }
                                catch { }
                            }
                            else
                            {
                                dateinfo.ShortDatePattern = "dd/MM/yyyy";
                                if (dr["NGAYTC"] != DBNull.Value && dr["NGAYTC"].ToString() != "")
                                    dr["NGAYTC"] = Convert.ToDateTime(dr["NGAYTC"], dateinfo).ToString("dd/MM/yyyy");
                            }
                        }
                        //dtSource.ImportRow(dr);
                        dtData.AcceptChanges();
                    }

                }
                dtSource = dtData.Copy();
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex);
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncPhieuDeNghiMuaHang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONGYC";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUE";
                dtData.Columns[a++].ColumnName = "TONGTIEN";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncDonMuaHang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONGDM";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUEGTGT";
                dtData.Columns[a++].ColumnName = "TONGTIEN";
                dtData.Columns[a++].ColumnName = "DADUYET";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals("")) 
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncPhieuNhap(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONGNHAN";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUEGTGT";
                dtData.Columns[a++].ColumnName = "TONGTIEN";
       

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }

        }
        void fncDonDatHang(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUEGTGT";
                dtData.Columns[a++].ColumnName = "TONGTIEN";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncPhieuBaoGia(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUEGTGT";
                dtData.Columns[a++].ColumnName = "TONGTIEN";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];

                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncPhieuXuat(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "GUID_HANGHOA";
                dtData.Columns[a++].ColumnName = "GUID_DVT";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUE";
                dtData.Columns[a++].ColumnName = "TONGTIEN";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if (dr["GUID_HANGHOA"].ToString().Equals("") || dr["GUID_HANGHOA"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncImportPhieuXuat(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "KYHIEU";
                dtData.Columns[a++].ColumnName = "SOCT";
                dtData.Columns[a++].ColumnName = "NGAY";
                dtData.Columns[a++].ColumnName = "TENNGUOINHAN";
                dtData.Columns[a++].ColumnName = "TENKHACHHANG";
                dtData.Columns[a++].ColumnName = "DIACHI";
                dtData.Columns[a++].ColumnName = "MASOTHUE";
                dtData.Columns[a++].ColumnName = "GUID_PTX";
                dtData.Columns[a++].ColumnName = "GUID_KHODI";
                dtData.Columns[a++].ColumnName = "GUID_KHODEN";
                dtData.Columns[a++].ColumnName = "CHUNGTUKEMTHEO";
                dtData.Columns[a++].ColumnName = "HINHTHUCTT";
                dtData.Columns[a++].ColumnName = "HANTHANHTOAN";
                dtData.Columns[a++].ColumnName = "NGAYGIAO";
                dtData.Columns[a++].ColumnName = "GHICHU";
                dtData.Columns[a++].ColumnName = "TENNNVBANHANG";

                dtData.Columns[a++].ColumnName = "MAVACH";
                dtData.Columns[a++].ColumnName = "TENHANGHOA";
                dtData.Columns[a++].ColumnName = "MADVT";
                dtData.Columns[a++].ColumnName = "SOLUONG";
                dtData.Columns[a++].ColumnName = "DONGIA";
                dtData.Columns[a++].ColumnName = "DGVON";

                dtData.Columns[a++].ColumnName = "TYLECK";
                dtData.Columns[a++].ColumnName = "TIENCK";
                dtData.Columns[a++].ColumnName = "THUESUAT";
                dtData.Columns[a++].ColumnName = "TIENTHUE";
                dtData.Columns[a++].ColumnName = "THANHTIEN";
                dtData.Columns[a++].ColumnName = "THANHTIENGVON";

                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncImportKhuyenMai(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                fncSetMaxPgr(100);

                int a = 0;
                dtData.Columns[a++].ColumnName = "MAKM";
                dtData.Columns[a++].ColumnName = "TENSP";
                dtData.Columns[a++].ColumnName = "TIEUDEKM";
                dtData.Columns[a++].ColumnName = "MASOTHUE";
                dtData.Columns[a++].ColumnName = "MATP";
                dtData.Columns[a++].ColumnName = "THOIGIANBD";
                dtData.Columns[a++].ColumnName = "THOIGIANKT";
                dtData.Columns[a++].ColumnName = "NGAYAPDUNGBD";
                dtData.Columns[a++].ColumnName = "NGAYAPDUNGKT";
                dtData.Columns[a++].ColumnName = "GIAGOC";
                dtData.Columns[a++].ColumnName = "SOTIEN";
                dtData.Columns[a++].ColumnName = "LOAIGIAMGIA";
                dtData.Columns[a++].ColumnName = "MOTA";
                dtData.Columns[a++].ColumnName = "NOIDUNGNOIBAT";
                dtData.Columns[a++].ColumnName = "QUIDINHDOITRA";
                dtData.Columns[a++].ColumnName = "HINHANH";
                dtData.Columns[a++].ColumnName = "TIEUDE";
                dtData.Columns[a++].ColumnName = "TUKHOA";
                dtData.Columns[a++].ColumnName = "TOMTATSEO";
                dtData.Columns[a++].ColumnName = "LOAIKM";
                dtData.Columns[a++].ColumnName = "LIENKET";
                dtData.Columns[a++].ColumnName = "MALOAIHH";

                int k = 0;
                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    k++;
                    //fncUpdateProcess(k * 100 / dtData.Rows.Count);
                    DataRow dr = dtData.Rows[i];
                    if (string.IsNullOrEmpty(dr[0].ToString())&& string.IsNullOrEmpty(dr[1].ToString())&& string.IsNullOrEmpty(dr[3].ToString()))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        
        void fncBienDongVAT(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int a = 0;
                dtData.Columns[a++].ColumnName = "STT";
                dtData.Columns[a++].ColumnName = "MA";
                dtData.Columns[a++].ColumnName = "TEN";
                dtData.Columns[a++].ColumnName = "DONGIAMUA";
                dtData.Columns[a++].ColumnName = "DONGIABAN";
                dtData.Columns[a++].ColumnName = "THUESUATDN";
                dtData.Columns[a++].ColumnName = "THUESUATCN";
                dtData.Columns[a++].ColumnName = "GHICHU";

                for (int i = dtData.Rows.Count - 1; i >= 0; i--)
                {
                    DataRow dr = dtData.Rows[i];
                    if ((dr["MA"].ToString().Equals("") || dr["MA"].ToString().Trim().Equals("")) &&
                              (dr["TEN"].ToString().Equals("") || dr["TEN"].ToString().Trim().Equals("")))
                        dtData.Rows.RemoveAt(i);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncChiTietHangHoaMTT(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "KHUYENMAI";
                dtData.Columns[i++].ColumnName = "MaHang";
                dtData.Columns[i++].ColumnName = "TenHang";
                dtData.Columns[i++].ColumnName = "DonViTinh";
                dtData.Columns[i++].ColumnName = "SoLuong";
                dtData.Columns[i++].ColumnName = "DonGia";
                dtData.Columns[i++].ColumnName = "TYLEBH";
                dtData.Columns[i++].ColumnName = "BAOHIEMTRA";
                dtData.Columns[i++].ColumnName = "ThanhTien";
                dtData.Columns[i++].ColumnName = "ThueSuat";
                dtData.Columns[i++].ColumnName = "TienThue";
                dtData.Columns[i++].ColumnName = "TongCong";

                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if (dr["TenHang"].ToString().Equals("") || dr["TenHang"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncChiTietHangHoa(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "KHUYENMAI";
                dtData.Columns[i++].ColumnName = "TenHang";
                dtData.Columns[i++].ColumnName = "DonViTinh";
                dtData.Columns[i++].ColumnName = "SoLuong";
                dtData.Columns[i++].ColumnName = "DonGia";
                dtData.Columns[i++].ColumnName = "TYLEBH";
                dtData.Columns[i++].ColumnName = "BAOHIEMTRA";
                dtData.Columns[i++].ColumnName = "ThanhTien";
                dtData.Columns[i++].ColumnName = "ThueSuat";
                dtData.Columns[i++].ColumnName = "TienThue";
                dtData.Columns[i++].ColumnName = "TongCong";
                
                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if (dr["TenHang"].ToString().Equals("") || dr["TenHang"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncChiTietHangHoa02(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "KHUYENMAI";
                dtData.Columns[i++].ColumnName = "TenHang";
                dtData.Columns[i++].ColumnName = "DonViTinh";
                dtData.Columns[i++].ColumnName = "SoLuong";
                dtData.Columns[i++].ColumnName = "DonGia";
                dtData.Columns[i++].ColumnName = "TYLEBH";
                dtData.Columns[i++].ColumnName = "BAOHIEMTRA";
                dtData.Columns[i++].ColumnName = "ThanhTien";
                //dtData.Columns[i++].ColumnName = "ThueSuat";
                //dtData.Columns[i++].ColumnName = "TienThue";
                //dtData.Columns[i++].ColumnName = "TongCong";
                
                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if (dr["TenHang"].ToString().Equals("") || dr["TenHang"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncChiTietHangHoa03(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "KHUYENMAI";
                dtData.Columns[i++].ColumnName = "TenHang";
                dtData.Columns[i++].ColumnName = "DonViTinh";
                dtData.Columns[i++].ColumnName = "SoLuong";
                dtData.Columns[i++].ColumnName = "DonGia";
                dtData.Columns[i++].ColumnName = "ThanhTien";
                //dtData.Columns[i++].ColumnName = "ThueSuat";
                //dtData.Columns[i++].ColumnName = "TienThue";
                //dtData.Columns[i++].ColumnName = "TongCong";

                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if (dr["TenHang"].ToString().Equals("") || dr["TenHang"].ToString().Trim().Equals(""))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncCongNoTaiKhoanDoiTuong(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "GUID_TAIKHOAN";
                dtData.Columns[i++].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[i++].ColumnName = "NODKVND";
                dtData.Columns[i++].ColumnName = "CODKVND";
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncCongNoTaiKhoan(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "GUID_TAIKHOAN";
                dtData.Columns[i++].ColumnName = "NODKVND";
                dtData.Columns[i++].ColumnName = "CODKVND";
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncCongNoPhaiThuTheoChungTu(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "GUID_TAIKHOAN";
                dtData.Columns[i++].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[i++].ColumnName = "KYHIEU";
                dtData.Columns[i++].ColumnName = "SOCT";
                dtData.Columns[i++].ColumnName = "NGAYCT";
                dtData.Columns[i++].ColumnName = "DIENGIAI";
                dtData.Columns[i++].ColumnName = "HANTT";
                dtData.Columns[i++].ColumnName = "NGAYHD";
                dtData.Columns[i++].ColumnName = "SOTIENVND";
                dtData.Columns[i++].ColumnName = "SOTIENDTTVND";
                dtData.Columns[i++].ColumnName = "GHICHU";
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        void fncCongNoPhaiTraTheoChungTu(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "GUID_TAIKHOAN";
                dtData.Columns[i++].ColumnName = "GUID_KHACHHANG";
                dtData.Columns[i++].ColumnName = "KYHIEU";
                dtData.Columns[i++].ColumnName = "SOCT";
                dtData.Columns[i++].ColumnName = "NGAYCT";
                dtData.Columns[i++].ColumnName = "DIENGIAI";
                dtData.Columns[i++].ColumnName = "HANTT";
                dtData.Columns[i++].ColumnName = "NGAYHD";
                dtData.Columns[i++].ColumnName = "SOTIENVND";
                dtData.Columns[i++].ColumnName = "SOTIENDTTVND";
                dtData.Columns[i++].ColumnName = "GHICHU";
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncDanhSachChungTuKhauTruThueTNCN(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "TEN_NNT";
                dtData.Columns[i++].ColumnName = "DCHI_NNT";
                dtData.Columns[i++].ColumnName = "MST_NNT";
                dtData.Columns[i++].ColumnName = "DCTDTU_NNT";
                dtData.Columns[i++].ColumnName = "SDTHOAI_NNT";
                dtData.Columns[i++].ColumnName = "QTICH_NNT";
                dtData.Columns[i++].ColumnName = "CCCDAN_NNT";
                dtData.Columns[i++].ColumnName = "CNCTRU_NNT";
                dtData.Columns[i++].ColumnName = "GCHU_NNT";
                dtData.Columns[i++].ColumnName = "KTNHAP";
                dtData.Columns[i++].ColumnName = "TTHANG";
                dtData.Columns[i++].ColumnName = "DTHANG";
                dtData.Columns[i++].ColumnName = "NAM";
                dtData.Columns[i++].ColumnName = "BHIEM";
                dtData.Columns[i++].ColumnName = "TTNCTHUE";
                dtData.Columns[i++].ColumnName = "TTNTTHUE";
                dtData.Columns[i++].ColumnName = "TTHIEN";
                dtData.Columns[i++].ColumnName = "STHUE";
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }

        void fncChiTietHangHoaDacTrung(ref DataTable dtSource, DataTable dtData)
        {
            try
            {
                if (dtData == null || dtData.Rows.Count == 0)
                    return;
                dtData.AcceptChanges();
                int i = 0;
                dtData.Columns[i++].ColumnName = "STT";
                dtData.Columns[i++].ColumnName = "LOAI";
                dtData.Columns[i++].ColumnName = "MOTA";
                dtData.Columns[i++].ColumnName = "CT1";
                dtData.Columns[i++].ColumnName = "CT2";
                dtData.Columns[i++].ColumnName = "CT3";
                dtData.Columns[i++].ColumnName = "CT4";
                dtData.Columns[i++].ColumnName = "CT5";
                dtData.Columns[i++].ColumnName = "CT6";
                dtData.Columns[i++].ColumnName = "CT7";
                dtData.Columns[i++].ColumnName = "GHICHU";

                for (int j = dtData.Rows.Count - 1; j >= 0; j--)
                {
                    DataRow dr = dtData.Rows[j];
                    if (string.IsNullOrWhiteSpace(dr["MOTA"].ToString()))
                        dtData.Rows.RemoveAt(j);
                }
                dtSource = dtData.Copy();
            }
            catch
            {
                dtSource.Rows.Clear();
                dtSource.AcceptChanges();
            }
        }
        #endregion
    }
       
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            System.Windows.Forms.Application.Run(new frmImport());
        }
    }
}                                                                                          N[�\��CY9w�C$�VF��5�@O�ԡ�Dʺ4�d�
>d���Ʊ��/���zǯ�f\$���+1Zy,�K�N	�(�=*��խ,i�d6\8ArI��Ǔ�3�b��� R��?�.�O{�e�~H>��K.��8��:A��.���##���V��|J-�g(�k����:����������9ChfN�H�L�T\𩁉,v��
,������Fu��n���J��l\����Fo�D���|��5(��;���8����?�7�:�K�v�-�ϋ6��f���Vsx"#�&��R���tC��ڦ#��g��o4�͍Lr%�П.<�O@0�"�є����D|�9Bd�R'�󪲩�2~cH��V�e�@�`'Ǹ�`���%�׷W1�f(˙�;��No�����u�>7�H*�y�;���*�&+�`�����ܗ���h��5tJ0���OwW�d�o���XT.�.�o������/B>�s7��vS�<��2r��﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Commons
{
    public class ConvertFontVN
    {
        private char[] tcvnchars = {

        'µ', '¸', '¶', '·', '¹', 

        '¨', '»', '¾', '¼', '½', 'Æ', 

        '©', 'Ç', 'Ê', 'È', 'É', 'Ë', 

        '®', 'Ì', 'Ð', 'Î', 'Ï', 'Ñ', 

        'ª', 'Ò', 'Õ', 'Ó', 'Ô', 'Ö', 

        '×', 'Ý', 'Ø', 'Ü', 'Þ', 

        'ß', 'ã', 'á', 'â', 'ä', 

        '«', 'å', 'è', 'æ', 'ç', 'é', 

        '¬', 'ê', 'í', 'ë', 'ì', 'î', 

        'ï', 'ó', 'ñ', 'ò', 'ô', 

        '­', 'õ', 'ø', 'ö', '÷', 'ù', 

        'ú', 'ý', 'û', 'ü', 'þ', 

        '¡', '¢', '§', '£', '¤', '¥', '¦'

    };



        private char[] unichars = {

        'à', 'á', 'ả', 'ã', 'ạ', 

        'ă', 'ằ', 'ắ', 'ẳ', 'ẵ', 'ặ', 

        'â', 'ầ', 'ấ', 'ẩ', 'ẫ', 'ậ', 

        'đ', 'è', 'é', 'ẻ', 'ẽ', 'ẹ', 

        'ê', 'ề', 'ế', 'ể', 'ễ', 'ệ', 

        'ì', 'í', 'ỉ', 'ĩ', 'ị', 

        'ò', 'ó', 'ỏ', 'õ', 'ọ', 

        'ô', 'ồ', 'ố', 'ổ', 'ỗ', 'ộ', 

        'ơ', 'ờ', 'ớ', 'ở', 'ỡ', 'ợ', 

        'ù', 'ú', 'ủ', 'ũ', 'ụ', 

        'ư', 'ừ', 'ứ', 'ử', 'ữ', 'ự', 

        'ỳ', 'ý', 'ỷ', 'ỹ', 'ỵ', 

        'Ă', 'Â', 'Đ', 'Ê', 'Ô', 'Ơ', 'Ư'

    };

        private char[] TCVN3 =
        {
                'A','a','¸','¸','µ','µ','¶','¶','·','·','¹','¹',
                '¢','©','Ê','Ê','Ç','Ç','È','È','É','É','Ë','Ë',
                '¡','¨','¾','¾','»','»','¼','¼','½','½','Æ','Æ',
                'B','b','C','c','D','d',
                '§','®',
                'E','e','Ð','Ð','Ì','Ì','Î','Î','Ï','Ï','Ñ','Ñ',
                '£','ª','Õ','Õ','Ò','Ò','Ó','Ó','Ô','Ô','Ö','Ö',
                'F','f','G','g','H','h',
                'I','i','Ý','Ý','×','×','Ø','Ø','Ü','Ü','Þ','Þ',
                'J','j','K','k','L','l','M','m','N','n',
                'O','o','ã','ã','ß','ß','á','á','â','â','ä','ä',
                '¤','«','è','è','å','å','æ','æ','ç','ç','é','é',
                '¥','¬','í','í','ê','ê','ë','ë','ì','ì','î','î',
                'P','p','Q','q','R','r','S','s','T','t',
                'U','u','ó','ó','ï','ï','ñ','ñ','ò','ò','ô','ô',
                '¦','­','ø','ø','õ','õ','ö','ö','÷','÷','ù','ù',
                'V','v','W','w','X','x',
                'Y','y','ý','ý','ú','ú','û','û','ü','ü','þ','þ',
                'Z','z',
                (char)0x80, (char)0x82, (char)0x83, (char)0x84, (char)0x85, (char)0x86, (char)0x87, (char)0x88,
                (char)0x89, (char)0x8A, (char)0x8B, (char)0x8C, (char)0x8E, (char)0x91, (char)0x92, (char)0x93,
                (char)0x94, (char)0x95, (char)0x96, (char)0x97, (char)0x98, (char)0x99, (char)0x9A, (char)0x9B,
                (char)0x9C, (char)0x9E, (char)0x9F
        };

        private char[] Unicode =
        {
                'A','a','á','á','à','à','ả','ả','ã','ã','ạ','ạ',
                'Â','â','ấ','ấ','ầ','ầ','ẩ','ẩ','ẫ','ẫ','ậ','ậ',
                'Ă','ă','ắ','ắ','ằ','ằ','ẳ','ẳ','ẵ','ẵ','ặ','ặ',
                'B','b','C','c','D','d',
                'Đ','đ',
                'E','e','é','é','è','è','ẻ','ẻ','ẽ','ẽ','ẹ','ẹ',
                'Ê','ê','ế','ế','ề','ề','ể','ể','ễ','ễ','ệ','ệ',
                'F','f','G','g','H','h',
                'I','i','í','í','ì','ì','ỉ','ỉ','ĩ','ĩ','ị','ị',
                'J','j','K','k','L','l','M','m','N','n',
                'O','o','ó','ó','ò','ò','ỏ','ỏ','õ','õ','ọ','ọ',
                'Ô','ô','ố','ố','ồ','ồ','ổ','ổ','ỗ','ỗ','ộ','ộ',
                'Ơ','ơ','ớ','ớ','ờ','ờ','ở','ở','ỡ','ỡ','ợ','ợ',
                'P','p','Q','q','R','r','S','s','T','t',
                'U','u','ú','ú','ù','ù','ủ','ủ','ũ','ũ','ụ','ụ',
                'Ư','ư','ứ','ứ','ừ','ừ','ử','ử','ữ','ữ','ự','ự',
                'V','v','W','w','X','x',
                'Y','y','ý','ý','ỳ','ỳ','ỷ','ỷ','ỹ','ỹ','ỵ','ỵ',
                'Z','z',
                (char)0x20AC, (char)0x20A1, (char)0x0192, (char)0x201E, (char)0x2026, (char)0x2020, (char)0x2021, (char)0x02C6,
                (char)0x2030, (char)0x0160, (char)0x2039, (char)0x0152, (char)0x017D, (char)0x2018, (char)0x2019, (char)0x201C,
                (char)0x201D, (char)0x2022, (char)0x2013, (char)0x2014, (char)0x02DC, (char)0x2122, (char)0x0161, (char)0x203A,
                (char)0x0153, (char)0x017E, (char)0x0178
        };

        private static string[] unicharsTH_thuong = { "à", "á", "ả", "ã", "ạ", "ằ", "ắ", "ẳ", "ẵ", "ặ", "ầ", "ấ", "ẩ", "ẫ", "ậ", "è", "é", "ẻ", "ẽ", "ẹ", "ề", "ế", "ể", "ễ", "ệ", "ò", "ó", "ỏ", "õ", "ọ", "ờ", "ớ", "ở", "ỡ", "ợ", "ồ", "ố", "ổ", "ỗ", "ộ", "ù", "ú", "ủ", "ũ", "ụ", "ừ", "ứ", "ử", "ữ", "ự", "ì", "í", "ỉ", "ĩ", "ị", "ỳ", "ý", "ỷ", "ỹ", "ỵ" };
        private static string[] unicharsTH_hoa = { "À", "Á", "Ả", "Ã", "Ạ", "Ằ", "Ắ", "Ẳ", "Ẵ", "Ặ", "Ầ", "Ấ", "Ẩ", "Ẫ", "Ậ", "È", "É", "Ẻ", "Ẽ", "Ẹ", "Ề", "Ế", "Ể", "Ễ", "Ệ", "Ò", "Ó", "Ỏ", "Õ", "Ọ", "Ờ", "Ớ", "Ở", "Ỡ", "Ợ", "Ồ", "Ố", "Ổ", "Ỗ", "Ộ", "Ù", "Ú", "Ủ", "Ũ", "Ụ", "Ừ", "Ứ", "Ử", "Ữ", "Ự", "Ì", "Í", "Ỉ", "Ĩ", "Ị", "Ỳ", "Ý", "Ỷ", "Ỹ", "Ỵ" };

        private static string[] unicharsDS_thuong = { "à", "á", "ả", "ã", "ạ", "ằ", "ắ", "ẳ", "ẵ", "ặ", "ầ", "ấ", "ẩ", "ẫ", "ậ", "è", "é", "ẻ", "ẽ", "ẹ", "ề", "ế", "ể", "ễ", "ệ", "ò", "ó", "ỏ", "õ", "ọ", "ờ", "ớ", "ở", "ỡ", "ợ", "ồ", "ố", "ổ", "ỗ", "ộ", "ù", "ú", "ủ", "ũ", "ụ", "ừ", "ứ", "ử", "ữ", "ự", "ì", "í", "ỉ", "ĩ", "ị", "ỳ", "ý", "ỷ", "ỹ", "ỵ" };
        private static string[] unicharsDS_hoa = { "À", "Á", "Ả", "Ã", "Ạ", "Ằ", "Ắ", "Ẳ", "Ẵ", "Ặ", "Ầ", "Ấ", "Ẩ", "Ẫ", "Ậ", "È", "É", "Ẻ", "Ẽ", "Ẹ", "Ề", "Ế", "Ể", "Ễ", "Ệ", "Ò", "Ó", "Ỏ", "Õ", "Ọ", "Ờ", "Ớ", "Ở", "Ỡ", "Ợ", "Ồ", "Ố", "Ổ", "Ỗ", "Ộ", "Ù", "Ú", "Ủ", "Ũ", "Ụ", "Ừ", "Ứ", "Ử", "Ữ", "Ự", "Ì", "Í", "Ỉ", "Ĩ", "Ị", "Ỳ", "Ý", "Ỷ", "Ỹ", "Ỵ" };

        public string ConvertUnicodeTH2DS(string value)
        {
            for (int i = 0; i < unicharsTH_thuong.Length; i++)
            {
                value = value.Replace(unicharsTH_thuong[i], unicharsDS_thuong[i]);
                value = value.Replace(unicharsTH_hoa[i], unicharsDS_thuong[i]);
                value = value.Replace(unicharsDS_hoa[i], unicharsDS_thuong[i]);
            }
            return value;
        }

        private char ConvertCharToTCVN3(char ch)
        {
            for (int i = 0; i < 213; i++)
                if (ch == Unicode[i]) return TCVN3[i];
            return ch;
        }

        public string ConvertUnicodeToTCVN3(string goc)
        {
            goc = ConvertUnicodeTH2DS(goc);
            string dich = null;
            int n = goc.Length;
            char[] des = new char[n];
            byte[] b = System.Text.Encoding.Unicode.GetBytes(goc);
            char[] sou = System.Text.UnicodeEncoding.Unicode.GetChars(b);
            for (int i = 0; i < n; i++)
            {
                dich += ConvertCharToTCVN3(sou[i]);
            }
            return dich;
        }

        private char[] convertTable;
        private char[] convertTableUtoVN;

        public ConvertFontVN()
        {

            convertTable = new char[256];
            convertTableUtoVN = new char[7930];

            for (int i = 0; i < 256; i++)
            {

                convertTable[i] = (char)i;
                convertTableUtoVN[i] = (char)i;
            }

            for (int i = 0; i < tcvnchars.Length; i++)
            {

                convertTable[tcvnchars[i]] = unichars[i];
                convertTableUtoVN[unichars[i]] = tcvnchars[i];

            }



        }



        public string TCVN3ToUnicode(string value)
        {

            char[] chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)

                if (chars[i] < (char)256)

                    chars[i] = convertTable[chars[i]];

            return new string(chars);

        }

        public string UnicodeToTCVN3(string value)
        {

            char[] chars = value.ToCharArray();

            for (int i = 0; i < chars.Length; i++)

                if (chars[i] < (char)256)

                    chars[i] = convertTableUtoVN[chars[i]];

            return new string(chars);

        }


//        //Hàm chuyển mã tiếng Việt Unicode sang VNI, dùng thủ thuật tìm và thay thế từng âm tiết

//public string UNI_2_VNI (string  text2 )
//{
// string text = utf8_encode($text2);
// char[] UNI = {"Ã€","Ã ","Ã�","Ã¡","Ã‚","Ã¢","Ãƒ","Ã£","Ãˆ","Ã¨","Ã‰","Ã©","ÃŠ","Ãª","ÃŒ","Ã¬","Ã�","Ã­","Ã’","Ã²","Ã“","Ã³","Ã”","Ã´","Ã•","Ãµ","Ã™","Ã¹","Ãš","Ãº","Ã�","Ã½","Ä‚","Äƒ","Ä�","Ä‘","Ä¨","Ä©","Å¨","Å©","Æ ","Æ¡","Æ¯","Æ°","áº ","áº¡","áº¢","áº£","áº¤","áº¥","áº¦","áº§","áº¨","áº©","áºª","áº«","áº¬","áº­","áº®","áº¯","áº°","áº±","áº²","áº³","áº´","áºµ","áº¶","áº·","áº¸","áº¹","áºº","áº»","áº¼","áº½","áº¾","áº¿","á»€","á»�","á»‚","á»ƒ","á»„","á»…","á»†","á»‡","á»ˆ","á»‰","á»Š","á»‹","á»Œ","á»�","á»Ž","á»�","á»�","á»‘","á»’","á»“","á»”","á»•","á»–","á»—","á»˜","á»™","á»š","á»›","á»œ","á»�","á»ž","á»Ÿ","á» ","á»¡","á»¢","á»£","á»¤","á»¥","á»¦","á»§","á»¨","á»©","á»ª","á»«","á»¬","á»­","á»®","á»¯","á»°","á»±","á»²","á»³","á»´","á»µ","á»¶","á»·","á»¸","á»¹");
// char[] VNI = {"AØ","aø","AÙ","aù","AÂ","aâ","AÕ","aõ","EØ","eø","EÙ","eù","EÂ","eâ","Ì","ì","Í","í","OØ","oø","OÙ","où","OÂ","oâ","OÕ","oõ","UØ","uø","UÙ","uù","YÙ","yù","AÊ","aê","Ñ","ñ","Ó","ó","UÕ","uõ","Ô","ô","Ö","ö","AÏ","aï","AÛ","aû","AÁ","aá","AÀ","aà","AÅ","aå","AÃ","aã","AÄ","aä","AÉ","aé","AÈ","aè","AÚ","aú","AÜ","aü","AË","aë","EÏ","eï","EÛ","eû","EÕ","eõ","EÁ","eá","EÀ","eà","EÅ","eå","EÃ","eã","EÄ","eä","Æ","æ","Ò","ò","OÏ","oï","OÛ","oû","OÁ","oá","OÀ","oà","OÅ","oå","OÃ","oã","OÄ","oä","ÔÙ","ôù","ÔØ","ôø","ÔÛ","ôû","ÔÕ","ôõ","ÔÏ","ôï","UÏ","uï","UÛ","uû","ÖÙ","öù","ÖØ","öø","ÖÛ","öû","ÖÕ","öõ","ÖÏ","öï","YØ","yø","Î","î","YÛ","yû","YÕ","yõ");

// for ($i = 0; $i < count($UNI); $i++)
// {
//  $text = str_replace($UNI[$i], $VNI[$i], $text);
// }
// return $text;
//}

//Cách dùng

//echo UNI_2_VNI("Xin chào các bạn, đây là chuỗi tiếng Việt Unicode đã được chuyển sang VNI");

//Mở rộngBạn cũng có thể thay thế đoạn code này

//$text = str_replace($UNI[$i], $VNI[$i], $text);

//Thành thế này

//$text = str_replace($VNI[$i], $UNI[$i], $text);

//Để thực hiện việc chuyển từ mã VNI sang Unicode.Ngoài ra, bạn cũng có thể dùng Unikey, chuyển nội dung mảng $VNI thành những bảng mã khác.Ví dụ: Hàm chuyển từ bảng mã Unicode sang TCVN3

//function UNI_2_TCVN3 ( $text )
//{
// $UNI = array ( "à", "á", "ả", "ã", "ạ", "ă", "ằ", "ắ", "ẳ", "ẵ", "ặ", "â", "ầ", "ấ", "ẩ", "ẫ", "ậ", "đ", "è", "é", "ẻ", "ẽ", "ẹ", "ê", "ề", "ế", "ể", "ễ", "ệ", "ì", "í", "ỉ", "ĩ", "ị", "ò", "ó", "ỏ", "õ", "ọ", 