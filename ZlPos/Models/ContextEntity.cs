using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class ContextEntity
    {
        

        //这里要id默认为1
        private int _id = 1;
        [SugarColumn(IsPrimaryKey = true,IsIdentity = false,IsNullable = false)]
        public int id { get => this._id; set => this._id = value; }

        [SugarColumn(IsNullable = true)]
        public string scale { get; set; }

        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }


        [SugarColumn(IsNullable = true)]
        public string readCard { get; set; }

        [SugarColumn(IsNullable = true)]
        public string serialPort { get; set; }

        [SugarColumn(IsNullable = true)]
        public string gprint { get; set; }
    }
}
