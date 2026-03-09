using System.Data;
using System.Collections;
using Microsoft.VisualBasic;
using System.Diagnostics;
using System;
using MySql.Data.MySqlClient;

namespace  TS24.hd999.Class
{
	public class clsExcute
	{
      
        public static DataSet mDataset = new DataSet();
        public static MySqlDataAdapter mDataAdapter;
        public static MySqlTransaction objTran;
		public static DataView dv;
		public static string strConn;
        public static string strError;
        public static bool ExcuteSql(string strSql)
		{
			bool returnValue;
			
			if (clsConnection.ConnectSQL() == false)
			{
				returnValue = false;
				return returnValue;
			}
			clsExcute.mDataset = new DataSet();

            //clsExcute.mDataset.CaseSensitive = true;
            clsConnection.mCommand = new MySqlCommand();
            clsConnection.mCommand.Connection = clsConnection.mSqlConn;
            clsConnection.mCommand.CommandText = strSql;
            mDataAdapter = new MySqlDataAdapter();
            mDataAdapter.SelectCommand = clsConnection.mCommand;
			try
			{
				mDataAdapter.Fill(mDataset, "TableName");
			}
			catch (Exception ex)
			{
                return false;
			}
			clsConnection.DisconnectSQL();
			returnValue = true;
			
			return returnValue;
		}

        public static int ExcuteMySQL(string strSql, string database=null, string databaseName = null)
        {
            try
            {
                if (!string.IsNullOrEmpty(database) && string.IsNullOrEmpty(databaseName))
                {
                    switch (database)
                    {
                        case "BEGIN":
                            clsConnection._DBIP = BaseParam.IPDatabase_Begin;
                            clsConnection._DBPort = BaseParam.PortDatabase_Begin.ToString();
                            clsConnection._DBName = BaseParam.NameDatabase_Begin;
                            clsConnection._DBUsername = BaseParam.UsernameDatabase_Begin;
                            clsConnection._DBPassword = BaseParam.PasswordDatabase_Begin;
                            break;
                        case "END":
                            clsConnection._DBIP = BaseParam.IPDatabase_End;
                            clsConnection._DBPort = BaseParam.PortDatabase_End.ToString();
                            clsConnection._DBName = string.IsNullOrEmpty(database) ? BaseParam.NameDatabase_End : database;
                            clsConnection._DBUsername = BaseParam.UsernameDatabase_End;
                            clsConnection._DBPassword = BaseParam.PasswordDatabase_End;
                            break;
                    }
                }
                else if (!string.IsNullOrEmpty(databaseName))
                {
                    clsConnection._DBIP = BaseParam.IPDatabase_Begin;
                    clsConnection._DBPort = BaseParam.PortDatabase_Begin.ToString();
                    clsConnection._DBName = databaseName;
                    clsConnection._DBUsername = BaseParam.UsernameDatabase_Begin;
                    clsConnection._DBPassword = BaseParam.PasswordDatabase_Begin;
                }
                if (clsConnection.ConnectSQL() == false)
                {
                    return -1;
                }

                MySqlConnection mSqlConn = new MySqlConnection(strConn);
                MySqlCommand mCommand = new MySqlCommand(strSql, clsConnection.mSqlConn);
                int ikq = mCommand.ExecuteNonQuery();
                clsConnection.DisconnectSQL();

                return ikq;
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return -1;
            }

        }

        public static bool ExcuteSql(string strSql, string Connect, string database)
        {
            strError = string.Empty;
            bool returnValue;
            switch (Connect)
            {
                case "BEGIN":
                    clsConnection._DBIP = BaseParam.IPDatabase_Begin;
                    clsConnection._DBPort = BaseParam.PortDatabase_Begin.ToString();
                    clsConnection._DBName = string.IsNullOrEmpty(database) ? BaseParam.NameDatabase_Begin : database;
                    clsConnection._DBUsername = BaseParam.UsernameDatabase_Begin;
                    clsConnection._DBPassword = BaseParam.PasswordDatabase_Begin;
                    break;
                case "END":
                    clsConnection._DBIP = BaseParam.IPDatabase_End;
                    clsConnection._DBPort = BaseParam.PortDatabase_End.ToString();
                    clsConnection._DBName = string.IsNullOrEmpty(database) ?  BaseParam.NameDatabase_End : database;
                    clsConnection._DBUsername = BaseParam.UsernameDatabase_End;
                    clsConnection._DBPassword = BaseParam.PasswordDatabase_End;
                    break;
            }
            if (clsConnection.ConnectSQL() == false)
            {
                returnValue = false;
                return returnValue;
            }
            clsExcute.mDataset = new DataSet();

            //clsExcute.mDataset.CaseSensitive = true;
            clsConnection.mCommand = new MySqlCommand();
            clsConnection.mCommand.Connection = clsConnection.mSqlConn;
            clsConnection.mCommand.CommandText = strSql;
            mDataAdapter = new MySqlDataAdapter();
            mDataAdapter.SelectCommand = clsConnection.mCommand;
            try
            {
                mDataAdapter.Fill(clsExcute.mDataset, "TableName");
            }
            catch (Exception ex)
            {
                strError = ex.Message;
                return false;
            }
            clsConnection.DisconnectSQL();
            returnValue = true;

            return returnValue;
        }

        public static bool ExcuteProcedure(string strProcedure)
		{
			bool returnValue;
			// Dim dbConnection As New clsConnection 'gọi đến class DbConnection
			bool bCheck = true;
			if (clsConnection.ConnectSQL() == false)
			{
				return false;
			}

            clsConnection.mCommand = new MySqlCommand(strProcedure, clsConnection.mSqlConn);
			clsConnection.StartTransaction();
            clsConnection.mCommand.CommandType = CommandType.Text;
			
			try
			{
                clsConnection.mCommand.ExecuteNonQuery();
				bCheck = true;
				clsConnection.CommitTransaction();
			}
			catch (Exception ex)
			{
				//Tạm thời bỏ qua
				bCheck = false;
				clsConnection.RollbackTransaction();
				//Thêm vào
				clsConnection.DisconnectSQL();
				returnValue = false;
				return returnValue;
				//hết
			}
			clsConnection.DisconnectSQL();
			returnValue = true;
			
			return returnValue;
		}
		public static bool ExcuteBaoCaoSql(string strSql, DataSet ds)
		{
			bool returnValue;
			// Dim dbConnection As New clsConnection 'gọi đến class DbConnection
			if (clsConnection.ConnectSQL() == false)
			{
				returnValue = false;
				return returnValue;
			}
			ds = new DataSet();
			//ds.CaseSensitive = true;
            clsConnection.mCommand = new MySqlCommand();
            clsConnection.mCommand.Connection = clsConnection.mSqlConn;
			clsConnection.mCommand.CommandText = strSql;
            mDataAdapter = new MySqlDataAdapter();
			mDataAdapter.SelectCommand = clsConnection.mCommand;
			try
			{
				mDataAdapter.Fill(ds, "TableName");
			}
			catch (Exception ex)
			{
                //MessageBox.Show(ex.Message, "Thông Báo");
			}
			clsConnection.DisconnectSQL();
			returnValue = true;
			
			return returnValue;
		}
		public static DataTable Select_Where(string sql)
		{
			DataTable returnValue = null;
			if (ExcuteSql(sql) == true)
			{
				returnValue = clsExcute.mDataset.Tables[0];
			}
			return returnValue;
		}

        public static DataTable Select_Where(string sql, string Connect, string database = null)
        {
            DataTable returnValue = null;
            if (ExcuteSql(sql, Connect, database) == true)
            {
                returnValue = clsExcute.mDataset.Tables[0];
            }
            return returnValue;
        }

        public static DataSet Select_Dataset(string sql, string Connect, string database = null)
        {
            DataSet returnValue = null;
            if (ExcuteSql(sql, Connect, database) == true)
            {
                returnValue = clsExcute.mDataset;
            }
            return returnValue;
        }


        public static DataTable SelectKiemTraBaoCao(string sql)
		{
			DataTable returnValue = null;
			if (ExcuteSql(sql) == true)
			{
				returnValue = clsExcute.mDataset.Tables[0];
			}
			return returnValue;
		}

	}
	
}
                                                                                                                                             