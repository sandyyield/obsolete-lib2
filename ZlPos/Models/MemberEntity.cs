using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class MemberEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsNullable = true)]
        public String id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String name { get; set; }
        [SugarColumn(IsNullable = true)]
        public String isautoupgrade { get; set; }
        [SugarColumn(IsNullable = true)]
        public String expiretime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String discount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String upgradenumber { get; set; }
        [SugarColumn(IsNullable = true)]
        public String createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public String updateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public String createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public String isrecharge { get; set; }

        //2018年1月30日 add

        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }

        [SugarColumn(IsNullable = true)]
        public String del { get; set; }

        // add: 2018/5/15
        [SugarColumn(IsNullable = true)]
        public String membermodel { get; set; }

        //add: 2018年12月25日
        [SugarColumn(IsNullable = true)]
        public string levelcode { get; set; }
    }
}
