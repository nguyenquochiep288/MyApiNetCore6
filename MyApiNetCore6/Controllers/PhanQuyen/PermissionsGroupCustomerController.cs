  void RemoveError(string column, int row)
        {
            if (Errors == null)
                return;
            string key = column + row.ToString();
            if (Errors.Contains(key))
                Errors.Remove(key);
        }
        void SetError(string column, int row, string msg)
        {
            try
            {
                if (Errors == null)
                    Errors = new Hashtable();
                ErrorCell ex = new ErrorCell(row, column, msg);
                string key = column + row.ToString();
                if (Errors.Contains(key))
                    Errors.Remove(key);
                Errors.Add(key, ex);
            }
            catch (Exception e)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.Message);
                throw;
            }
        }
        #endregion

        private void slkHangHoa_EditValueChanged(object sender, System.EventArgs e)
        {
            hh_trungbay_view hh = (hh_trungbay_view)gridView.GetFocusedRow();
            if (hh != null)
            {
                if (sender.Equals(slkHangHoa))
                {
                    if (slkHangHoa.EditValue != null && !string.IsNullOrEmpty(slkHangHoa.EditValue.ToString()))
                    {
                        dm_hanghoa_view dmhh = (dm_hanghoa_view)slkHangHoa.Properties.View.GetFocusedRow();

                        hh.GUID_HANGHOA = slkHangHoa.EditValue.ToString();
                        if (dmhh != null)
                        {
                            hh.TENHANGHOA = txtTENHANGHOA.Text = dmhh.TEN;

                            try
                            {
                                hh.MADVT = new dm_dvt_view().GetByGuid<dm_dvt_view>(dmhh.GUID_DVT).MA;
                                slkDVT.EditValue = hh.MADVT;
                            }
                            catch (Exception)
                            {
                                hh.MADVT = "";
                                slkDVT.EditValue = null;
                            }
                            hh.DONGIABAN = dmhh.DGBAN;
                            txtDONGIABAN.EditValue = dmhh.DGBAN;

                        }

                        gridView.Focus();
                    }
                }
                if (sender.Equals(slkDVT))
                {
                    if (slkDVT.EditValue != null && !string.IsNullOrEmpty(slkDVT.EditValue.ToString()))
                    {
                        hh.MADVT = slkDVT.EditValue.ToString();
                        gridView.Focus();
                    }
                }
            }
        }

        private void rps_colID_LOAIHH_EditValueChanged(object sender, System.EventArgs e)
        {
            //GridLookUpEdit grLookup = (GridLookUpEdit)sender;
            //dm_hanghoa obj = (dm_hanghoa)grLookup.Properties.View.GetFocusedRow();
            //if (obj != null)
            //slkHangHoa.EditValue = obj.GUID;
        }

        private void rps_colID_DVT_EditGridLookUp_EditValueChanged(object sender, System.EventArgs e)
        {
            GridLookUpEdit grLookup = (GridLookUpEdit)sender;
            dm_dvt_view obj = (dm_dvt_view)grLookup.Properties.View.GetFocusedRow();
            if(obj!=null)
            slkDVT.EditValue = obj.MA;
        }

        private void btnAddPicVideo_Click(object sender, System.EventArgs e)
        {
            try
            {
                
                  SimpleButton btn = (SimpleButton)sender;
                  if (btn.Equals(btnAddPic))
                  {
                      hh_trungbay_view hh = (hh_trungbay_view)gridView.GetFocusedRow();
                      Interface_Hinh Hinhanh = new Interface_Hinh();
                      openFileDialog1 = new OpenFileDialog();
                      openFileDialog1.Filter = "JPG,BMP,JPEG,GIF,PNG|*.jpg;*.bmp;*.jpeg;*.gif;*.png";
                      openFileDialog1.ShowDialog();
                      if (!string.IsNullOrEmpty(openFileDialog1.FileName))
                      {
                        FileStream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read);
                      //  Image myImg = Image.FromFile(openFileDialog1.FileName);
                      
                          string strSourceFile = openFileDialog1.FileName;

                          Hinhanh.TENFILE = openFileDialog1.SafeFileName;
                          Hinhanh.DUONGDAN = openFileDialog1.FileName;
                          Hinhanh.image = Image.FromStream(stream);
                        listHinhAnh.Add(Hinhanh);
                          bindingImage.DataSource = listHinhAnh;
                          MainGridHH.DataSource = bindingImage;
                          MainGridHH.RefreshDataSource();
                        stream.Close();
                    }
                  }
                  else if (btn.Equals(btnAddVideo))
                  {
                      hh_trungbay_view hh = (hh_trungbay_view)gridView.GetFocusedRow();
                      Interface_Video video = new Interface_Video();
                      openFileDialog1 = new OpenFileDialog();
                      openFileDialog1.Filter = "MP4,AVI,WMV|*.mp4;*.avi;*.wmv";
                      openFileDialog1.ShowDialog();
                      if (!string.IsNullOrEmpty(openFileDialog1.FileName))
                      {
                          //Image myImg = Image.FromFile(openFileDialog1.FileName);
                          string strSourceFile = openFileDialog1.FileName;

                          //  hh.HINHANH = Convert.ToBase64String(File.ReadAllBytes(strSourceFile));//ImageToByte(strSourceFile);
                          //  Hinhanh.hinhanh = Convert.ToBase64String(Common.ConvertResizeImage(strSourceFile));
                          video.TENFILE = openFileDialog1.SafeFileName;
                          video.DUONGDAN = openFileDialog1.FileName;

                          listVideo.Add(video);
                          bindingVideo.DataSource = listVideo;
                          MainGridVideo.DataSource = bindingVideo;
                          MainGridVideo.RefreshDataSource();
                      }
                  }
            }
            catch (Exception ex)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Message);
            }


        }

        private void btnDelPicVideo_Click(object sender, System.EventArgs e)
        {
            SimpleButton btn = (SimpleButton)sender;
            if (btn.Equals(btnDelPic))
            {
                Interface_Hinh ha = (Interface_Hinh)cardViewHH.GetFocusedRow();
                if (ha == null) return;
                listDelHinhAnh.Add(ha);
                listHinhAnh.Remove(ha);
                bindingImage.DataSource = listHinhAnh;
                MainGridHH.DataSource = bindingImage;
                MainGridHH.RefreshDataSource();
            }
            else if (btn.Equals(btnDelVideo))
            {
                Interface_Video ha = (Interface_Video)cardVideo.GetFocusedRow();
                if (ha == null) return;
                listDelVideo.Add(ha);
                listVideo.Remove(ha);
                bindingVideo.DataSource = listVideo;
                MainGridVideo.DataSource = bindingVideo;
                MainGridVideo.RefreshDataSource();
            }
        }

        private void btnNaptuExcel_Click(object sender, System.EventArgs e)
        {
            if(btnSua.Enabled==false)
              fncImport();
        }

        private void gridView_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "STT")
                if (e.ListSourceRowIndex >= 0)
                    e.DisplayText = (e.ListSourceRowIndex + 1).ToString();
        }

        private void cardVideo_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Interface_Video ha = (Interface_Video)cardVideo.GetFocusedRow();
                if (ha == null) return;
                if (File.Exists(ha.DUONGDAN))
                    System.Diagnostics.Process.Start(ha.DUONGDAN);
                else
                    XtraMessageBox.Show("Video không tồn tại, vui lòng chọn video khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { }
            
        }

        private void cardViewHH_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                Interface_Hinh ha = (Interface_Hinh)cardViewHH.GetFocusedRow();
                if (ha == null) return;
                if (File.Exists(ha.DUONGDAN))
                    System.Diagnostics.Process.Start(ha.DUONGDAN);
                else
                    XtraMessageBox.Show("Hình ảnh không tồn tại, vui lòng chọn hình khác!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch { }
     
        }

      