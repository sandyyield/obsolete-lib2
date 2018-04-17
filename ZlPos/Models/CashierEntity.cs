using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CashierEntity
    {
        [SugarColumn(IsNullable = true,IsPrimaryKey =true)]
        public String id { get; set; }//isid
        [SugarColumn(IsNullable = true)]
        public String remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public String account { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String fullname { get; set; }


        [SugarColumn(IsNullable = true)]
        public String del { get; set; }
    }
}
