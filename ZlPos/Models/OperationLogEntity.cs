using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class OperationLogEntity
    {
        [SugarColumn(IsNullable = true,IsPrimaryKey = true,IsIdentity = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string deviceuniquecode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string account { get; set; }
        [SugarColumn(IsNullable = true)]
        public string fullname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string operatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string operatecode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string operatenmessage { get; set; }
        [SugarColumn(IsNullable = true)]
        public string remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public string source { get; set; }
        [SugarColumn(IsNullable = true)]
        public bool isUpload { get; set; }

        //add 2018年12月26日
        [SugarColumn(IsNullable = true)]
        public string IP { get; set; }
        [SugarColumn(IsNullable = true)]
        public string operatename { get; set; }

    }
}
