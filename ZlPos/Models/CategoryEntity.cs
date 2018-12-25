using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CategoryEntity
    {
        [SugarColumn(IsPrimaryKey = true, IsNullable = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categoryname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ordernum { get; set; }
        [SugarColumn(IsNullable = true)]
        public string pid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string required { get; set; }
        [SugarColumn(IsNullable = true)]
        public string level { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }
        // TODO: 2017/10/17
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }

        //ADD 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updateuser { get; set; }

        //add 2018年12月21日
        [SugarColumn(IsNullable = true)]
        public string parentcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string isshow { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4name { get; set; }
    }
}
