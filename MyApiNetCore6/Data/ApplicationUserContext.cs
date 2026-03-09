using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TS24.SM24.Danhmuc.DB
{
    public partial class hh_yeucaumuahang : IDBBase
    {
        public hh_yeucaumuahang()
            : base()
        {
            TableName = "hh_yeucaumuahang";            
        }
         public override int OnSave(Devart.Data.MySql.MySqlTransaction trans = null)
    