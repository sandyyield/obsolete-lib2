using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CashierEntity
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

        //add 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
    }
}
