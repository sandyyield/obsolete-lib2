using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class AssistantsEntity
    {
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true,IsPrimaryKey = true)]
        public string  id{ get; set; }
        [SugarColumn(IsNullable = true)]
        public string status { get; set; }
        [SugarColumn(IsNullable = true)]
        public string remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public string name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string assistantcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }





    }
}
