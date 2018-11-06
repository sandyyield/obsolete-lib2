using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class DisCountDetailEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String ticketcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String vouchertype { get; set; }
        [SugarColumn(IsNullable = true)]
        public String voucheramount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String voucherchannelid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String vouchername { get; set; }
        [SugarColumn(IsNullable = true)]
        public String voucherdocno { get; set; }

        //add 2018年11月3日
        [SugarColumn(IsNullable = true)]
        public string saleactivitytypecode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string activitycode { get; set; }
    }
}
