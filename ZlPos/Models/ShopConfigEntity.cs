using SqlSugar;

namespace ZlPos.Models
{
    public class ShopConfigEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = true)]
        public int id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string newsstate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string mbcreditrule { get; set; }
        [SugarColumn(IsNullable = true)]
        public string localtax { get; set; }
        [SugarColumn(IsNullable = true)]
        public string deviceflag { get; set; }
        [SugarColumn(IsNullable = true)]
        public string isonmeituanmsg { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mbcreditpay { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mbmsisrecharge { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mbisrecharge { get; set; }
        [SugarColumn(IsNullable = true)]
        public string deviceisrecharge { get; set; }
        [SugarColumn(IsNullable = true)]
        public string nationaltax { get; set; }
        [SugarColumn(IsNullable = true)]
        public string attachurl { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ismeituanmapping { get; set; }
        [SugarColumn(IsNullable = true)]
        public string devicecode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dbneedcheck { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mbcreditrange { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mbcreditrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string rechargepwd { get; set; }


    }
}
