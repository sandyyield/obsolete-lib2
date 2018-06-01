using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CustomerShowConfigEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(IsNullable = true)]
        public string port { get; set; }
        [SugarColumn(IsNullable = true)]
        public string intBaud { get; set; }
    }
}
