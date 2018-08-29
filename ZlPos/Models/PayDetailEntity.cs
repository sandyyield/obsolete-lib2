using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class PayDetailEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String ticketcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String payname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string payamount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paycardcode { get; set; }
        //2017/12/16新增

        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String businesscode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String countcardnums { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paycredit { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paytime { get; set; }
        // add: 2018/2/27
        [SugarColumn(IsNullable = true)]
        public String ordercode { get; set; }

        // add 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
    }
}
