using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CategoryEntity
    {
        [SugarColumn(IsPrimaryKey = true,IsNullable = true)]
        public String id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String categoryname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String ordernum { get; set; }
        [SugarColumn(IsNullable = true)]
        public String pid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String required { get; set; }
        [SugarColumn(IsNullable = true)]
        public String level { get; set; }
        [SugarColumn(IsNullable = true)]
        public String del { get; set; }
        // TODO: 2017/10/17
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }

    }
}
