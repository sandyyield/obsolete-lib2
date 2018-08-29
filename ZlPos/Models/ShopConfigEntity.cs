using SqlSugar;

namespace ZlPos.Models
{
    public class ShopConfigEntity
    {
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false)]
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


        //add 2018年8月27日
        [SugarColumn(IsNullable = true)]
        public string industryname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string industryid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string softwaretype { get; set; }
        [SugarColumn(IsNullable = true)]
        public string expirestime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string contactaddress { get; set; }
        [SugarColumn(IsNullable = true)]
        public string isbranchpay { get; set; }
        [SugarColumn(IsNullable = true)]
        public string isrecharge { get; set; }
        [SugarColumn(IsNullable = true)]
        public string payway { get; set; }
        [SugarColumn(IsNullable = true)]
        public string expirestimestr { get; set; }



        

            
        
        
            
            
            
            

    }
}
