using System;
namespace TS24.TO.HDDB
{
    public class tt68_tokhaidangky_tctn : IDDataBase
    {

        #region Define constructor
        public tt68_tokhaidangky_tctn()
        {
            TableName = "tt68_tokhaidangky_tctn";
            Command = new TS24.MySQLLib.MySQLUtilities();
        }
        #endregion


        #region Get or set property
        //ID_Tkhai
        private string _ID_TKHAI;
        /// <summary>
        /// Khóa ngoại
        /// </summary>
        public string ID_TKHAI { get { return _ID_TKHAI; } set { _ID_TKHAI = value; } }

        private int _STT;
        public int STT { get { return _STT; } set { _STT = value; } }

        private string _TTCTN;
        public string TTCTN { get { return _TTCTN; } set { _TTCTN = value; } }

        private string _MSTTCTN;
        public string MSTTCTN { get { return _MSTTCTN; } set { _MSTTCTN = value; } }

        private DateTime _TNGAY;
        public DateTime TNGAY { get { return