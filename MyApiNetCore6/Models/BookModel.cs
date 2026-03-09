using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Devart.Data.MySql;

namespace TS24.SM24.Danhmuc.DB
{
    public class IDBBase : TS24.MySQLLib.Abstract.IData
    {
        public IDBBase() : base()
        {

        }
        public virtual int OnSave(Devart.Data.MySql.MySqlTransaction trans = null)
        {
            return -1;
        }

        //public override List<T> ToList<T>(MySQLLib