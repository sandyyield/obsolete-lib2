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

        //add 2018年6月14日
        [SugarColumn(IsNullable = true)]
        public string barcodeStyle { get; set; }

        //add 增加BJQprint 缓存
        [SugarColumn(IsNullable = true)]
        public string BJQprint { get; set; }

        //add 2018年10月25日 增加SPBQTemplet\DDBQTemplet 和 BJQTemplet缓存
        [SugarColumn(IsNullable = true)]
        public string SPBQTemplet { get; set; }

        [SugarColumn(IsNullable = true)]
        public string DDBQTemplet { get; set; }

        [SugarColumn(IsNullable = true)]
        public string BJQTemplet { get; set; }
        
        //add 2018年11月28日 条码秤的缓存
        [SugarColumn(IsNullable = true)]
        public string barcodeScale { get; set; }

        //add 2018年12月6日 保存小票模板
        [SugarColumn(IsNullable = true)]
        public string PrintModelSetting { get; set; }

        [SugarColumn(IsNullable = true)]
        public string AutoLaunchSetting { get; set; }
    }
}
