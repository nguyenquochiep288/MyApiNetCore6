s.CHON.Name = "CHON";
            this.CHON.OptionsBand.FixedWidth = true;
            this.CHON.Width = 91;
            // 
            // CHON1
            // 
            this.CHON1.Caption = "Chọn";
            this.CHON1.ColumnEdit = this.rpsCheck;
            this.CHON1.FieldName = "CHKMAHANG";
            this.CHON1.Name = "CHON1";
            this.CHON1.Visible = true;
            this.CHON1.Width = 91;
            // 
            // rpsCheck
            // 
            this.rpsCheck.AutoHeight = false;
            this.rpsCheck.DisplayValueChecked = "1";
            this.rpsCheck.DisplayValueGrayed = "0";
            this.rpsCheck.DisplayValueUnchecked = "0";
            this.rpsCheck.Name = "rpsCheck";
            this.rpsCheck.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
            this.rpsCheck.ValueChecked = 1;
            this.rpsCheck.ValueGrayed = 0;
            this.rpsCheck.ValueUnchecked = 0;
            // 
            // btnInMaVach
            // 
            this.btnInMaVach.Location = new System.Drawing.Point(752, 12);
            this.btnInMaVach.Name = "btnInMaVach";
            this.btnInMaVach.Size = new System.Drawing.Size(75, 23);
            this.btnInMaVach.TabIndex = 0;
            this.btnInMaVach.Text = "In Mã vạch";
            this.btnInMaVach.Click += new System.EventHandler(this.btnInMaVach_Click);
            // 
            // dockManager1
            // 
            this.dockManager1.Form = this;
            this.dockManager1.RootPanels.AddRange(new DevExpress.XtraBars.Docking.DockPanel[] {
            this.dockPanel1});
            this.dockManager1.TopZIndexControls.AddRange(new string[] {
            "DevExpress.XtraBars.BarDockControl",
            "DevExpress.XtraBars.StandaloneBarDockControl",
            "System.Windows.Forms.StatusBar",
            "System.Windows.Forms.MenuStrip",
            "System.Windows.Forms.StatusStrip",
            "DevExpress.XtraBars.Ribbon.RibbonStatusBar",
            "DevExpress.XtraBars.Ribbon.RibbonControl"});
            // 
            // dockPanel1
            // 
            this.dockPanel1.Controls.Add(this.dockPanel1_Container);
            this.dockPanel1.Dock = DevExpress.XtraBars.Docking.DockingStyle.Left;
            this.dockPanel1.ID = new System.Guid("479e2811-eb74-4626-a54d-8fac22884f35");
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.OriginalSize = new System.Drawing.Size(248, 200);
            this.dockPanel1.Size = new System.Drawing.Size(248, 665);
            this.dockPanel1.Text = "Danh sách loại hàng hóa";
            // 
            // dockPanel1_Container
            // 
            this.dockPanel1_Container.Controls.Add(this.treeList1);
            this.dockPanel1_Container.Location = new System.Drawing.Point(4, 23);
            this.dockPanel1_Container.Name = "dockPanel1_Container";
            this.dockPanel1_Container.Size = new System.Drawing.Size(240, 638);
            this.dockPanel1_Container.TabIndex = 0;
            // 
            // treeList1
            // 
            this.treeList1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
            this.treeListColumn1,
            this.treeListColumn2,
            this.treeListColumn3});
            this.treeList1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeList1.KeyFieldName = "";
            this.treeList1.Location = new System.Drawing.Point(0, 0);
            this.treeList1.Name = "treeList1";
            this.treeList1.OptionsBehavior.PopulateServiceColumns = true;
            this.treeList1.ParentFieldName = "";
            this.treeList1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.rBtnPlus});
            this.treeList1.Size = new System.Drawing.Size(240, 638);
            this.treeList1.StateImageList = this.imageCollection1;
            this.treeList1.TabIndex = 11;
            this.treeList1.GetStateImage += new DevExpress.XtraTreeList.GetStateImageEventHandler(this.treeList1_GetStateImage);
            this.treeList1.Click += new System.EventHandler(this.treeList1_Click);
            // 
            // treeListColumn1
            // 
            this.treeListColumn1.Caption = "Mã";
            this.treeListColumn1.FieldName = "MA";
            this.treeListColumn1.MinWidth = 33;
            this.treeListColumn1.Name = "treeListColumn1";
            this.treeListColumn1.OptionsColumn.AllowEdit = false;
            this.treeListColumn1.OptionsColumn.AllowMove = false;
            this.treeListColumn1.OptionsColumn.AllowSort = false;
            // 
            // treeListColumn2
            // 
            this.treeListColumn2.Caption = "Tên";
            this.treeListColumn2.FieldName = "TEN";
            this.treeListColumn2.MinWidth = 33;
            this.treeListColumn2.Name = "treeListColumn2";
            this.treeListColumn2.OptionsColumn.AllowEdit = false;
            this.treeListColumn2.OptionsColumn.AllowMove = false;
            this.treeListColumn2.OptionsColumn.AllowSort = false;
            this.treeListColumn2.Visible = true;
            this.treeListColumn2.VisibleIndex = 0;
            // 
            // treeListColumn3
            // 
            this.treeListColumn3.Caption = "Cap";
            this.treeListColumn3.FieldName = "CAPLOAIHH";
            this.treeListColumn3.Name = "treeListColumn3";
            // 
            // rBtnPlus
            // 
            this.rBtnPlus.AutoHeight = false;
            this.rBtnPlus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.rBtnPlus.Name = "rBtnPlus";
            // 
            // imageCollection1
            // 
            this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
            this.imageCollection1.Images.SetKeyName(0, "Check.png");
            this.imageCollection1.Images.SetKeyName(1, "noCheck.png");
            // 
            // chkUncheckall
            // 
            this.chkUncheckall.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkUncheckall.Location = new System.Drawing.Point(1010, 16);
            this.chkUncheckall.Name = "chkUncheckall";
            this.chkUncheckall.Properties.Caption = "Chọn tất cả";
            this.chkUncheckall.Size = new System.Drawing.Size(101, 19);
            this.chkUncheckall.TabIndex = 243;
            this.chkUncheckall.Visible = false;
            this.chkUncheckall.CheckedChanged += new System.EventHandler(this.chkUncheckall_CheckedChanged);
            // 
            // frmCreateBarcode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1123, 665);
            this.Controls.Add(this.chkUncheckall);
            this.Controls.Add(this.btnInMaVach);
            this.Controls.Add(this.MainGrid);
            this.Controls.Add(this.dockPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "frmCreateBarcode";
            this.Text = "Tạo Barcode";
            this.Load += new System.EventHandler(this.frmCreateBarcode_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MainGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grvHangHoa)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpsCboLoaiDG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpsTextNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rpsCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dockManager1)).EndInit();
            this.dockPanel1.ResumeLayout(false);
            this.dockPanel1_Container.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.treeList1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rBtnPlus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chkUncheckall.Properties)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraGrid.GridControl MainGrid;
        private DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit rpsCheck;
        private DevExpress.XtraGrid.Views.BandedGrid.BandedGridView grvHangHoa;
        private DevExpress.XtraGrid.Views.Band