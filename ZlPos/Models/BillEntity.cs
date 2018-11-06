using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace ZlPos.Models
{
    public class BillEntity
    {
        [SugarColumn(IsNullable = true)]
        public String holediscount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String waitpay { get; set; }
        [SugarColumn(IsNullable = true)]
        public String consumerecharge { get; set; }
        [SugarColumn(IsNullable = true)]
        public String memberacount { get; set; }

        [SugarColumn(IsNullable = true)]
        public String collection { get; set; }
        [SugarColumn(IsNullable = true,ColumnName = "change")]
        public String changemoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public String ticketstatue { get; set; }

        [SugarColumn(IsNullable = false,IsPrimaryKey = true,IsIdentity = false)]
        public String ticketcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String saletime { get; set; }

        [SugarColumn(IsNullable = true)]
        public long insertTime { get; set; }//数据库存储用于筛选
        [SugarColumn(IsNullable = true)]
        public String tradeid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String tradename { get; set; }
        [SugarColumn(IsNullable = true)]
        public String total { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paytotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public String memberid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String membername { get; set; }
        [SugarColumn(IsNullable = true)]
        public String membercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String returncode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String cashierid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String cashiername { get; set; }
        [SugarColumn(IsNullable = true)]
        public String cashiertime { get; set; }
        [SugarColumn(IsNullable = true)]
        public String assistantid { get; set; }
        [SugarColumn(IsNullable = true)]
        public String assistantname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String assistantcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String ticketprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paytype { get; set; }
        [SugarColumn(IsNullable = true)]
        public String payname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String payprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String paycard { get; set; }


        [SugarColumn(IsNullable = true)]
        public String commissionprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String nationaltaxtotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public String localtaxtotal { get; set; }
        [SugarColumn(IsNullable = true)]
        public String nationaltax { get; set; }
        [SugarColumn(IsNullable = true)]
        public String localtax { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String branchname { get; set; }

        [SugarColumn(IsNullable = true)]
        public String shopname { get; set; }
        [SugarColumn(IsNullable = true)]
        public String remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public String nationaltaxamount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String localtaxamount { get; set; }
        [SugarColumn(IsNullable = true)]
        public String deviceuniquecode { get; set; }


        [SugarColumn(IsNullable = true)]
        public String membercredit { get; set; }
        [SugarColumn(IsNullable = true)]
        public String credit { get; set; }

        //add 2017/7/26
        [SugarColumn(IsNullable = true)]
        public String iscrossbranch { get; set; }
        // TODO: 2017/10/25
        [SugarColumn(IsNullable = true)]
        public String memberno { get; set; }
        // add: 2018/2/27
        [SugarColumn(IsNullable = true)]
        public String membermodel { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<BillCommodityEntity> commoditys { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<PayDetailEntity> paydetails { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<DisCountDetailEntity> discountdetails { get; set; }
        
        //add 2018年10月18日
        [SugarColumn(IsNullable = true)]
        public string whichversion { get; set; }

        //add 2018年11月3日
        [SugarColumn(IsNullable = true)]
        public string molinmoney { get; set; }
    }
}
