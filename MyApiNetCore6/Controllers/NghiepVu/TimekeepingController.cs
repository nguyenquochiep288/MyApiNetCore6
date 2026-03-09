using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using TS24.SM24.Danhmuc.DB.Forms;
using TS24.SM24.Danhmuc.DB;
using TS24.SM24.Danhmuc.DB.Forms.Processes;
using TS24.SM24.BaseMethod;

namespace TS24.SM24.Danhmuc.UserControl
{
    public partial class dm_quydoi : DevExpress.XtraEditors.XtraUserControl
    {
        public delegate void CloseForm(bool b);
        public CloseForm fncCloseForm { set; get; }
        public bool formHasChange { set; get; }
        List<dm_dvt1> lsDVT { set; get; }
        List<dm_hanghoa> lsHH { set; get; }
        ViewDmQuyDoi oQuyDoi { set; get; }
        string guid_hh { set; get; }
        string ma_dvt { set; get; }
        string ma_dvtqd { set; get; }

        public dm_quydoi()
        {
            InitializeComponent();
        }

        bool Check()
        {
            bool rs = true;
            if(cbDVT.Text == "")
            {
                dxError.SetError(cbDVT, "Vui lòng nhập đơn vị tính!");
                rs = false;
            }
            else
                dxError.SetError(cbDVT, "");

            if (cbDVTQD.Text == "")
            {
                dxError.SetError(cbDVTQD, "Vui lòng nhập đơn vị tính quy đổi!");
                rs = false;
            }
            else
                dxError.SetError(cbDVTQD, "");

            if (txtSoLuong.Text == "" || Convert.ToDouble(txtSoLuong.Text) <= 0)
            {
                dxError.SetError(txtSoLuong, "Số lượng quy đổi không hợp lệ!");
                rs = false;
            }
            else
                dxError.SetError(txtSoLuong, "");

            return rs;
        }

        void Save()
        {
            if (!Check())
                return;

            ma_dvt = "";
            ma_dvtqd = "";
            dm_dvt1 dvt = null;
            dm_dvt1 dvtqd = null;

            if (!string.IsNullOrEmpty(cbDVT.Text))
            {
                lap:
                dvt = lsDVT.Where(c => c.TEN == cbDVT.Text).FirstOrDefault();
                if (dvt != null)
                    ma_dvt = dvt.MA;
                else
                {
                    PDmQuyDoi.SaveDVT(cbDVT.Text);
                    lsDVT = PDmQuyDoi.GetAllDVT();
                    goto lap;
                }
            }

            if (!string.IsNullOrEmpty(cbDVT.Text))
            {
                lap1:
                dvtqd = lsDVT.Where(c => c.TEN == cbDVTQD.Text).FirstOrDefault();
                if (dvtqd != null)
                    ma_dvtqd = dvtqd.MA;
                else
                {
                    PDmQuyDoi.SaveDVT(cbDVTQD.Text);
                    lsDVT = PDmQuyDoi.GetAllDVT();
                    goto lap1;
                }
            }

            bool insert = true;
            if (oQuyDoi != null && !string.IsNullOrEmpty(oQuyDoi.ID))
                insert = false;

            string id = oQuyDoi != null && !string.IsNullOrEmpty(oQuyDoi.ID) ? oQuyDoi.ID : Guid.NewGuid().ToString();
            if (oQuyDoi != null && guid_hh == oQuyDoi.TENHH) guid_hh = null;

            if (PDmQuyDoi.Save(new ViewDmQuyDoi
            {
                ID = id,
                GUID = oQuyDoi != null && !string.IsNullOrEmpty(oQuyDoi.GUID) ? oQuyDoi.GUID : id,
                DONGIA = Convert.ToDouble(txtDonGia.EditValue == null ? "0" : txtDonGia.EditValue.ToString()),
                GHICHU = txtGhiChu.Text,
                GUID_CONGTY = BaseParam.ActiveID,
                GUID_DVT = dvt != null && !string.IsNullOrEmpty(dvt.ID) ? dvt.GUID : "",
                GUID_DVTQD = dvtqd != null && !string.IsNullOrEmpty(dvtqd.ID) ? dvtqd.GUID : "",
                GUID_HH = guid_hh,
                GUID_KHO = BaseParam.GuidKhoUser,
                MADVT = ma_dvt,
                MADVTQD = ma_dvtqd,
                NGAYTHUCHIEN = DateTime.Now,
                NGUOITHUCHIEN = BaseParam.TenDangNhap,
                SOLUONG = Convert.ToDouble(txtSoLuong.EditValue == null ? "0" : txtSoLuong.EditValue.ToString()),
                TENHH = !String.IsNullOrEmpty(guid_hh) ? null : txtTenHH.Text,
                TINHTRANGSD = chkTTSD.Checked ? "1" : "0",
                XOA = !chkTTSD.Checked ? "1" : "0"
            }, insert))
            {
                XtraMessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Reset();
                LoadData();
                btnXoa.Enabled = btnGhi.Enabled = 
                    panelControl3.Enabled = false;
            }
            else
                XtraMessageBox.Show("Lưu không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void btn_Click(object sender, EventArgs e)
        {
            if (sender.Equals(btnDong))
            {
                if (fncCloseForm != null)
                    fncCloseForm(true);
            }
            else if (sender.Equals(btnThem))
            {
                Reset();
                panelControl3.Enabled = btnGhi.Enabled = btnNgung.Enabled = true;
                btnSua.Enabled = btnXoa.Enabled = false;
            }
            else if (sender.Equals(btnSua))
            {
                panelControl3.Enabled = false;
                gridView_Click(null, null);
            }
            else if (sender.Equals(btnGhi))
            {
                Save();
            }
            else if (sender.Equals(btnNgung))
            {
                Reset();
                panelControl3.Enabled =
                    btnGhi.Enabled = btnXoa.Enabled = false;
                btnSua.Enabled = true;
            }
            else if (sender.Equals(btnXoa))
            {
                Deleted();
                //if (oQuyDoi == null || string.IsNullOrEmpty(oQuyDoi.ID) || oQuyDoi.XOA == "1")
                //    return;
                //if (PDmQuyDoi.Xoa(oQuyDoi.ID))
                //{
                //    XtraMessageBox.Show("Lưu trạng thái thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    panelControl3.Enabled = false;
                //    Reset();
                //    LoadData();
                //}
                //else
                //    XtraMessageBox.Show("Lưu trạng thái không thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dm_quydoi_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        void LoadData()
        {
            lsDVT = PDmQuyDoi.GetAllDVT();
            lkDVT.DataSource = lsDVT;
            cbDVT.Properties.Items.Clear();
            cbDVTQD.Properties.Items.Clear();
            if (lsDVT != null)
            {
                foreach (var i in lsDVT)
                {
                    cbDVT.Properties.Items.Add(i.TEN);
                    cbDVTQD.Properties.Items.Add(i.TEN);
                }
            }
            MainGrid.DataSource = PDmQuyDoi.GetAll();
            dm_hanghoa hh = new dm_hanghoa();
            cbHangHoa.Properties.DataSource =
                lsHH = hh.GetAll<dm_hanghoa>();
            repDMHH.DataSource = lsHH;
        }

        private void gridView_Click(object sender, EventArgs e)
        {
            oQuyDoi = (ViewDmQuyDoi)gridView.GetFocusedRow();
            if (oQuyDoi == null) return;

            guid_hh = oQuyDoi.GUID_HH;
            btnNgung.Enabled =
                btnXoa.Enabled = 
                btnGhi.Enabled = 
                panelControl3.Enabled = true;

            if (!String.IsNullOrEmpty(oQuyDoi.GUID_HH))
            {
                var chk = lsHH.Where(c => c.GUID == oQuyDoi.GUID_HH).FirstOrDefault();
                if (chk != null)
                {
                    txtTenHH.Text = chk.TEN;
                    txtTenHH.Enabled = false;
                }
                else
                {
                    txtTenHH.Text = oQuyDoi.TENHH;
                    txtTenHH.Enabled = true;
                }
            }
            else
                txtTenHH.Text = oQuyDoi.TENHH;
            

            chkDelete.Checked = oQuyDoi.XOA == "1";
            chkTTSD.Checked = oQuyDoi.TINHTRANGSD == "1";

            if (!string.IsNullOrEmpty(oQuyDoi.GUID_DVT))
            {
                var dvt = lsDVT.Where(c => c.GUID == oQuyDoi.GUID_DVT).FirstOrDefault();
                if (dvt != null) cbDVT.Text = dvt.TEN;
                else cbDVT.Text = "";
            }
            else
                cbDVT.Text = "";

            if (!string.IsNullOrEmpty(oQuyDoi.GUID_DVTQD))
            {
                var dvt = lsDVT.Where(c => c.GUID == oQuyDoi.GUID_DVTQD).FirstOrDefault();
                if (dvt != null) cbDVTQD.Text = dvt.TEN;
                else cbDVTQD.Text = "";
            }
            else
                cbDVTQD.Text = "";

            txtSoLuong.EditValue = oQuyDoi.SOLUONG;
            txtDonGia.EditValue = oQuyDoi.DONGIA;
            txtGhiChu.Text = oQuyDoi.GHICHU;
        }

        void Reset()
        {
            oQuyDoi = new ViewDmQuyDoi();
            txtTenHH.Text = "";
            chkDelete.Checked = false;
            chkTTSD.Checked = true;
            cbDVT.Text = "";
            cbDVTQD.Text = "";
            txtSoLuong.EditValue = null;
            txtDonGia.EditValue = null;
            txtGhiChu.Text = "";
            txtTenHH.Enabled = true;
            guid_hh = null;
        }

        private void cbHangHoa_EditValueChanged(object sender, EventArgs e)
        {
            try
            {
                SearchLookUpEdit grLookup = (SearchLookUpEdit)sender;
                dm_hanghoa h = (dm_hanghoa)grLookup.Properties.View.GetFocusedRow();
                if (oQuyDoi == null || String.IsNullOrEmpty(oQuyDoi.ID)) Reset();

                guid_hh = h.ID;
                txtTenHH.Text = h.TEN;

                if (!string.IsNullOrEmpty(h.GUID_DVT))
                {
                    var dvt = lsDVT.Where(c => c.GUID == h.GUID_DVT).FirstOrDefault();
                    if (dvt != null)
                    {
                        cbDVTQD.Text = cbDVT.Text = dvt.TEN;
                    }
                    else
                    {
                        cbDVTQD.Text = cbDVT.Text = "";
                    }
                }

                txtSoLuong.EditValue = 1;
                txtDonGia.EditValue = h.DGBAN;
                txtTenHH.Enabled = false;
            }
            catch(Exception ex)
            {
                XtraMessageBox.Show(ex.Message);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if ((base.ActiveControl is TextEdit) && (keyData == Keys.Return))
            {
                SendKeys.Send("{Tab}");
            }
            if (keyData == Keys.F11)
            {
                Deleted();
            }
            if (keyData == Keys.F5)
            {
                if (btnGhi.Enabled)
                    Save();
            }
            if (keyData == Keys.F4)
            {
                if (btnThem.Enabled)
                    Reset();
            }
            if (keyData == Keys.F12)
            {
                btn_Click(btnDong, null);
            }
            if (keyData == Keys.Escape)
            {
                btn_Click(btnNgung, null);
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Deleted()
        {
            if (oQuyDoi == null || string.IsNullOrEmpty(oQuyDoi.ID))
                return;

            //if(oQuyDoi.TINHTRANGSD == "1" && chkTTSD.Checked)
            //{
            //    XtraMessageBox.Show("Danh mục đang sử dụng không thể xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}

            if (XtraMessageBox.Show("Xác nhận xóa mục/hàng hóa quy đổi?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (PDmQuyDoi.Delete(oQuyDoi.ID))
                    LoadData();
            }
        }

        private void cbHangHoa_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            if (e.Button.Kind == DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
            {
                frm_dmhanghoaNew f = new frm_dmhanghoaNew();
                f.ShowDialog();

                cbHangHoa.Properties.DataSource =
                    lsHH = (new dm_hanghoa()).GetAll<dm_hanghoa>();
                repDMHH.DataSource = lsHH;
            }
        }

        private void gridView_CustomRowCellEdit(object sender, DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventArgs e)
        {
            if (e.Column.FieldName == "GUID_HH")
            {
                var hh = (ViewDmQuyDoi)gridView.GetRow(e.RowHandle);
                if (!String.IsNullOrEmpty(hh.GUID_HH))
                {
                    var chk = lsHH.Where(c => c.ID == hh.GUID_HH).FirstOrDefault();
                    if (chk != null)
                    {
