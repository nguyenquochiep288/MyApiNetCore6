using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
 
using Ionic.Zip;
using System.IO;
using TS24.HD.BaseMethod;
using System.Reflection;
using TS24.TO.Commons;

namespace TS24.HD.BKupRestore
{
    public partial class Restore : XtraForm
    {
        public Restore()
        {
            InitializeComponent();
            lnkGoogle.OpenLink += (o, e) => {
                var g = BKupGoogleDrive.Init();
                if (g == null || string.IsNullOrEmpty(g.assemblyauth))
                {
                    Common.ShowMsgBox(MessageBoxButtons.OK, MessageBoxIcon.Warning, "Báº¡n chÆ°a cÃ i Ä‘áº·t vÃ  Ä‘á»“ng bá»™ lÃªn Google Drive nÃªn khÃ´ng thá»ƒ phá»¥c há»“i");
                    return;
                }

                var v = new Google.Core.ListFile
                {
                    Drive = g,
                    FolderGDrive = new BKupGoogleDrive().FolderBakup,
                    AutoCloseAfterDownload = true
                };
                v.EventDownloadCompleted += (f) =>
                {
                    btnBrowser.Text = f.LocalFile;
                };
                v.ShowDialog();
            };
        }
        TS24.MySQLUtilities.BackupRestore bkup = null;
        TS24.MySQLUtilities.BackupRestore bkuppro = null;
        //string FilePath = null;
        bool IsRunning = false;
        public string sNameXHD = "ts24pro";
        public string sPROGRAM = "XHD";
        private void btnBrowser_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            if (sPROGRAM.Equals("IKETO"))
                of.Filter = "ezBooks Backup File (*.ezb)|*.ezb";
            else
                of.Filter = "iXHD Backup File (*.xhd)|*.xhd";
          
            if (of.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                btnBrowser.Text = of.FileName;
            }
        }
        string GetDuongDan()
        {
            string folder = "";
            try
            {
                TS24.TO.HDDB.thamsohethong clsCom;
                clsCom = new TS24.TO.HDDB.thamsohethong();
                clsCom.GUID_CONGTY = BaseParam.ActiveID;
                DataTable dtts = clsCom.GetDataAnd();
                if (dtts != null && dtts.Rows.Count > 0)
                {
                    clsCom.ID = dtts.Rows[0]["ID"].ToString();
                }

                clsCom.GetInfo();
                if (clsCom != null && clsCom.ID != null && clsCom.ID != "")
                {
                    folder = clsCom.DuongDanLuuPDF;
                    hyperLinkEdit1.Text = clsCom.DuongDanLuuPDF;
                }
                else
                {
                    folder = Application.StartupPath + "\\HDDT";
                    hyperLinkEdit1.Text = folder;
                }
                // folder = @"F:\BAKXHD\";
            }
            catch (Exception)
            {


            }

            return folder;
        }

        TS24.MySQLUtilities.ImportInformations AllRestoreData()
        {
            TS24.MySQLUtilities.Methods methods = new TS24.MySQLUtilities.Methods();
            string _encryptionKey = methods.Sha2Hash("TS24NVM");
            string FileName = btnBrowser.Text;
            List<string> files = new List<string>();
            using (ZipFile zip = new ZipFile(FileName))
            {
                if (!string.IsNullOrEmpty(_encryptionKey))
                    zip.Password = _encryptionKey;
                string curPath = Path.GetDirectoryName(FileName);
                //curPath += "p";
                try
                {
                   //curPath = "@" + curPath;
                    if (File.Exists(curPath))
                        File.Delete(curPath);
                }
                catch
                {
                    //throw new Exception("Extract file error.");
                }
                try
                {
                    zip.ExtractAll(curPath, ExtractExistingFileAction.OverwriteSilently);
                }
                catch (Exception ex)
                {
                    TS24.TO.Commons.Log.WriteLog(this, MethodBase.GetCurrentMethod().Name, "Error from backup : " + ex.Message);
                }
              
                foreach (string s in zip.EntryFileNames)
                {
                    files.Add(curPath + "\\" + s);
                    //files.Add(curPath + "\\" + s);
                }
            }
            MySQLUtilities.ImportInformations Result = null;
            foreach (string s in files)
            {
                if (s.Contains("ts24probkup.tmp") || s.Contains("ts24pro.tmp"))
                {
                    dbname = "TS24 Professional";
                    Result = RestoreData(bkuppro, s);
                    if (Result == null || Result.CompleteArg.CompletedType != MySQLUtilities.ImportCompleteArg.CompleteType.Completed)
                        return Result;
                    try
                    {
                        File.Delete(s);
                    }
                    catch { }
                }
                //else if (s.Contains("xhd.tmp")|| s.Contains("sm24.tmp"))
                //{
                //    dbname = sNameXHD;// "SM24";
                //    Result = RestoreData(bkup, s);
                //    if (Result == null || Result.CompleteArg.CompletedType != MySQLUtilities.ImportCompleteArg.CompleteType.Completed)
                //        return Result;
                //    try
                //    {
                //        File.Delete(s);
                //    }
                //    catch { }
                //}
            }

            try
            {
                string folderXHD = GetDuongDan();
                if (!string.IsNullOrEmpty(folderXHD) && !Directory.Exists(folderXHD))
                    try
                    {
                        Directory.CreateDirectory(folderXHD);
                    }
                    catch (Exception)
                    {
                        folderXHD = Application.StartupPath + "\\HDDT";
                        if (!Directory.Exists(folderXHD))
                            Directory.CreateDirectory(folderXHD);
                    }


                if (Directory.Exists(folderXHD))
                {
                    string dirName = "XHDBK";//new DirectoryInfo(folderXHD).Name;
                    //Xu Ly Copy Thu Muc nguon sang thu muc dich
                    string folderSource = "";
                    try
                    {
                        foreach (string s in files)
                        {
                            if (s.Contains(dirName))
                            {
                                int indexDir = s.IndexOf(dirName);
                                folderSource = s.Substring(0, indexDir + dirName.Length);
                                folderSource = folderSource.Replace("/", "\\");
                                break;
                            }
                        }
                        if (Directory.Exists(folderSource))
                        {
                            DirectoryCopy(folderSource, @folderXHD, true);
                            //Microsoft.VisualBasic.FileIO.FileSystem.MoveDirectory(sourceDirName, destDirName);
                        }
                    }
                    catch (Exception ex)
                    {
                        TS24.TO.Commons.Log.WriteLog(this, MethodBase.GetCurrentMethod().Name, "Lá»—i táº¡o Ä‘Æ°á»ng dáº«n 1: " + ex.Message);

                    }

                }
                else
                {
                    TS24.TO.Commons.Log.WriteLog(MethodBase.GetCurrentMethod().Name, "KhÃ´ng thá»ƒ táº¡o Ä‘Æ°á»ng dáº«n: '" + folderXHD + "'");
                }
            }
            catch (Exception ex)
            {
                TS24.TO.Commons.Log.WriteLog(this, MethodBase.GetCurrentMethod().Name, "Lá»—i táº¡o Ä‘Æ°á»ng dáº«n 2: " + ex.Message);

            }
          
           
            return Result;
        }

        private  void DirectoryCopy( string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the source directory does not exist, throw an exception.
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            // If the destination directory does not exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }


            // Get the file contents of the directory to copy.
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {
                // Create the path to the new copy of the file.
                string temppath = Path.Combine(destDirName, file.Name);

                // Copy the file.
                file.CopyTo(temppath, false);
            }

            // If copySubDirs is true, copy the subdirectories.
            if (copySubDirs)
            {

                foreach (DirectoryInfo subdir in dirs)
                {
                    // Create the subdirectory.
                    string temppath = Path.Combine(destDirName, subdir.Name);

                    // Copy the subdirectories.
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        TS24.MySQLUtilities.ImportInformations RestoreData(TS24.MySQLUtilities.BackupRestore bkup, string FilePath)
        {

            IsRunning = true;
            //FilePath = btnBrowser.Text;
            bkup.ImportInfo = new MySQLUtilities.ImportInformations();
            bkup.ImportInfo.FileName = FilePath;
            bkup.ImportInfo.EncryptionKey = "TS24NVM";
            bkup.ImportInfo.AutoCloseConnection = true;
            bkup.ImportInfo.EnableEncryption = false;
            bkup.ImportInfo.DeletAllRow = false;
            bkup.ImportInfo.IsZipFile = true;
            bkup.ImportInfo.AsynchronousMode = false;
            bkup.ImportProgressChanged += new MySQLUtilities.BackupRestore.importProgressChange(bkup_ImportProgressChanged);
            bkup.ImportCompleted += new MySQLUtilities.BackupRestore.importComplete(bkup_ImportCompleted);
            return bkup.Import();
        }

        void bkup_ImportCompleted(object sender, MySQLUtilities.ImportCompleteArg e)
        {
            //throw new NotImplementedException();
        }

        void bkup_ImportProgressChanged(object sender, MySQLUtilities.ImportProgressArg e)
        {
            bgrRestore.ReportProgress(e.PercentageCompleted, e);
            //throw new NotImplementedException();
        }
        void OnRestore()
        {
            if (string.IsNullOrEmpty(btnBrowser.Text))
            {
                btnBrowser.ErrorText = "Vui lÃ²ng chá»n Ä‘Æ°á»ng dáº«n sao lÆ°u";
                return;
            }

            Common.WaitDialog("Äang kiá»ƒm tra dá»¯ liá»‡u, vui lÃ²ng chá»...");
            AutoBackupData(false, false);
            Common.CloseWaitDialog();

            Common.WaitDialog("Äang kiá»ƒm tra káº¿t ná»‘i dá»¯ liá»‡u...");
            TS24.HD.HeThong.Util.common.SaveHistory(string.Format("Restore dá»¯ liá»‡u Path ({0})", btnBrowser.Text));
            bkup = new MySQLUtilities.BackupRestore(null, sNameXHD);//"sm24"
            if (sPROGRAM.Equals("XHD") || sPROGRAM.Equals("IKETO"))
            {
                bkuppro = new MySQLUtilities.BackupRestore(null, "ts24pro");
            }

            Common.CloseWaitDialog();
            lblProcess.Visible =
                        ProcessControl.Visible = true;
            ProcessControl.EditValue = 0;
            bgrRestore.RunWorkerAsync();
        }

        private void wiz_NextClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            if (!CanNext())
            {
                e.Handled = true;
                return;
            }
            ProcessControl.Visible = false;
            if (e.Page == wizWellcome)
            {
                wiz.NextText = "&Báº¯t Ä‘áº§u phá»¥c há»“i >";
            }
            else if (e.Page == wizComplete)
            {
                this.Close();
            }
            else
                wiz.NextText = "&Tiáº¿p tá»¥c >";

            if (e.Page == wizBackup)
            {
                OnRestore();
                e.Handled = true;
                return;
            }
        }

        #region Event backgroundworker
        private void bgrRestore_DoWork(object sender, DoWorkEventArgs e)
        {
            e.Result = AllRestoreData();
        }
        string dbname = null;
        int percent01 = 0;
        int percent02 = 0;
        int current = 0;
        private void bgrRestore_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            MySQLUtilities.ImportProgressArg imp = (MySQLUtilities.ImportProgressArg)e.UserState;
            if (imp != null)
            {
                if (current == 0 && imp.Error == null)
                    percent01 = e.ProgressPercentage;
                else if (current == 1 && imp.Error == null)
                    percent02 = e.ProgressPercentage;

                if (e.ProgressPercentage == 100 && current == 0)
                {
                    TS24.TO.HDDB.thamsohethong clsCom;
                    clsCom = new TS24.TO.HDDB.thamsohethong();
                    DataTable dtts = clsCom.GetDataAnd();
                    if (dtts != null && dtts.Rows.Count > 0)
                    {
                        clsCom.ID = dtts.Rows[0]["ID"].ToString();
                        clsCom.GetInfo();
                        clsCom.DuongDanLuuPDF = hyperLinkEdit1.Text;
                        clsCom.SaveUpdate();
                    }
                    lblProcess.Text = "ÄÃ£ hoÃ n thÃ nh phá»¥c há»“i dá»¯ liá»‡u " + dbname + ", vui lÃ²ng chá» trong giÃ¢y lÃ¡t...";
                    current = 1;
                    percent02 = 0;
                    //percent01 = e.ProgressPercentage;
                }
                else
                    lblProcess.Text = "Äang phá»¥c há»“i dá»¯ liá»‡u " + dbname + "...";

                ProcessControl.EditValue = (percent01 + percent02) / 2;
                ProcessControl.Update();
            }

        }
        public  string GetLocalIPAddress(string GetHostName)
        {
            try
            {
                var host = System.Net.Dns.GetHostEntry(GetHostName);
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                //Log.WriteLog(ex);
                return "";
            }
        }
        private void bgrRestore_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TS24.MySQLUtilities.ImportInformations result = (TS24.MySQLUtilities.ImportInformations)e.Result;
            lblProcess.Visible = ProcessControl.Visible = false;
            IsRunning = false;
            if (result.CompleteArg.CompletedType == MySQLUtilities.ImportCompleteArg.CompleteType.Completed)
            {
                wiz.SelectedPage = wizComplete;
            }
            else
            {
                Common.ShowMsgBox(MessageBoxButtons.OK, MessageBoxIcon.Warning, "QÃºa trÃ¬nh phá»¥c há»“i dá»¯ liá»‡u bá»‹ lá»—i, vui lÃ²ng thá»­ láº¡i sau.");
            }
            TS24.HD.HeThong.Util.common.SaveHistory(string.Format("Restore dá»¯ liá»‡u Path ({0})", BaseParam.TaiKhoanKichHoatEmail +"-" + GetLocalIPAddress(System.Net.Dns.GetHostName()) +"-"+ btnBrowser.Text) + "-" + DateTime.Now);

        }
        #endregion
        bool CanNext()
        {
            if (IsRunning)
            {
                Common.ShowMsgBox(MessageBoxButtons.OK, MessageBoxIcon.Information, "QÃºa trÃ¬nh phá»¥c há»“i dá»¯ liá»‡u Ä‘ang thá»±c thi, vui lÃ²ng Ä‘á»£i trong giÃ¢y lÃ¡t.");
                return false;
            }
            return true;
        }
        private void wiz_FinishClick(object sender, CancelEventArgs e)
        {
            if (wiz.SelectedPage == wizComplete)
                this.Close();
        }

        private void Restore_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !CanNext();
        }

        private void wiz_PrevClick(object sender, DevExpress.XtraWizard.WizardCommandButtonClickEventArgs e)
        {
            if (!CanNext())
            {
                e.Handled = true;
                return;
            }
        }


        #region Backup trÆ°á»›c restore
        string fncCreatePathBackup()
        {
            string sDrive = Application.StartupPath.Substring(0, Application.StartupPath.IndexOf(":")) + @":\BackupXHD\XHD";
            if (!Directory.Exists(sDrive))

                Directory.CreateDirectory(sDrive);
            try
            {
                try
                {
                    sDrive = @"D:\BackupXHD\XHD";
                    if (!Directory.Exists(sDrive))
                        Directory.CreateDirectory(sDrive);
                    return sDrive;
                }
                catch (Exception)
                {
                    sDrive = @"E:\BackupXHD\XHD";
                    if (!Directory.Exists(sDrive))
                        Directory.CreateDirectory(sDrive);
                    return sDrive;
                }
            }
            catch (Exception e)
            {
                Log.WriteLog(this, System.Reflection.MethodBase.GetCurrentMethod().Name, e.ToString());
                sDrive = Application.StartupPath.Substring(0, Application.StartupPath.IndexOf(":")) + @":\BackupXHD\XHD";
                if (!Directory.Exists(sDrive))
                    Directory.CreateDirectory(sDrive);

                return sDrive;
            }
        }
        void AutoBackupData(bool NgayUpdate, bool BackupManual)
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;
                string fileName = "XHDT-" + year + "-" + month + "-" + day + "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".xhd";
                string filePath;
                object pathSaveFile = UtilData.ReadConfig("PathSaveFile");
                if (pathSaveFile == null || string.IsNullOrEmpty(pathSaveFile.ToString()))
                {
                    string strPath = Path.GetTempPath();// fncCreatePathBackup();
                    filePath = Path.Combine(strPath ,fileName);
                }
                else
                {
                    try
                    {
                        if (!Directory.Exists(pathSaveFile.ToString()))
                            Directory.CreateDirectory(pathSaveFile.ToString());
                    }
                    catch (Exception)
                    {
                        pathSaveFile = fncCreatePathBackup();
                    }
                    filePath = pathSaveFile + "\\" + fileName;

                }

                TS24.HD.BKupRestore.Backup frm = new TS24.HD.BKupRestore.Backup();
                frm.sNameXHD = TS24.MySQLLib.Util.DatabaseName;
                frm.btnBrowser.Text = filePath;
                frm.AllBackupData();

            }
            catch (Exception ex)
            {
                Log.WriteLog(this, MethodBase.GetCurrentMethod().Name, ex.Message + ex.StackTrace.ToString());

            }
        }
        bool fncAutoBKup(bool BackupManual)
        {
            try
            {
                bool IsOk = false;

                BackgroundWorker b = new BackgroundWorker { WorkerReportsProgress = true, WorkerSupportsCancellation = true };
                b.DoWork += (o, e) =>
                {
                    AutoBackupData(false, BackupManual);

                    e.Result = (true);// && r2
                };
                b.RunWorkerCompleted += (o, e) =>
                {
                    // Common.CloseWaitDialog();
                    IsOk = Convert.ToBoolean(e.Result);
                };
                b.RunWorkerAsync();
                while (b.IsBusy)
                    Application.DoEvents();
                return IsOk;

                return true;
            }
            catch (Exception e)
            {
                Log.WriteLog(this, MethodBase.GetCurrentMethod().Name, e.Message);
                return false;
            }
        }
        #endregion

        private void wiz_CancelClick(object sender, CancelEventArgs e)
        {
            this.Dispose();
        }

        private void Restore_Load(object sender, EventArgs e)
        {
            switch (sPROGRAM)
            {
                case "IKETO":
                    wizWellcome.Text = "Phá»¥c há»“i dá»¯ liá»‡u ezBooks";
                    wizWellcome.IntroductionText = "ChÆ°Æ¡ng trÃ¬nh há»— trá»£ phá»¥c há»“i dá»¯ liá»‡u tá»« há»‡ thá»‘ng ezBooks";
                    wizBackup.Text = "Há»— trá»£ phá»¥c há»“i dá»¯ liá»‡u ezBooks";
                    break;
            }
            GetDuongDan();
        }

        private void wizComplete_Click(object sender, EventArgs e)
        {

        }

        private void hyperLinkEdit1_OpenLink(object sender, DevExpress.XtraEditors.Controls.OpenLinkEventArgs e)
        {
            TS24.HD.HeThong.FromDialog.frmSettingPath frm = new HeThong.FromDialog.frmSettingPath();
            frm.ShowDialog();

            GetDuongDan();
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                          .½P¶šcÂ3@TEÊLÌ×‡Jtªâã(óW\u^æÏtùH!yÚh3©×ÛŸsÖS[Ù³Ì7‰ò%=nu<.ÍªB«;ß7P	OH–ë”T9ŸâL]`—‹Íª/(ÚîÉÃzLÈG¥ÔJ´¯;7¹˜„&…Û€cÑãÁäa­¥±™O§æX"—UÕWwƒØ.7„ÿ“¸é<ëØn:­’y BÕ “´ß<M7š:s’6Ml*¶^§w÷làô.	®Œ‡nú€ğÚğ£şá’ø=m`‘A0x¤±üe‹EZ‚¯¼c>–‹v!í³ş_pĞ¥°G®Ë—¾ùæã(2ssúHú“‘r$mù_ö¾WŞl¥`$/:ØAy¼®µşÚ¶lGë±Ö {*Âïgò¯‹~¼;S¹EÙˆÂF:øäcISxQ“–h ¢«8İUç4AÑ¤Ä¤Ïë n9iş$ ¥…¿‡Á{BàÕ~zÕÿjx5ÿx5ºqT_AÕ§w¥1ºÇFÒ‡ŞÃ”dƒôø<bkÒxñ†$__Ú<À8ş:’>oOrŠ¦ÁBDÒÿ—án‡àJnÒqy ’>G[yJAÆ’”µÌgMIëàwŸöº›$ınIe¥ôWêxG¯€®úhXJ$} Ô/fê/úÃ‹%ô¢ã{|‘4½O´ïyl—šy%„±C«§ÁH7ŒYiZ‡øS±wqŠ£;ï¤†É¿‘H|ˆ•ğé÷/Â~€*?„RÒŸ‡w<ù®%ùÕó“jsm gŞÔ¨g®eºæYùÅG”Âlæ.‰² ŸBï(dİòxKº¢#só‹ù÷Æ"ÉØ¢Öwµâ¥5¶¬ßJ`~G27N·¡hjı1Â†"/"zº/