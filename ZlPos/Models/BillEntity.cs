using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class BillEntity
    {
        public String holediscount { get; set; }
        public String waitpay { get; set; }
        public String consumerecharge { get; set; }
        public String memberacount { get; set; }

        public String collection { get; set; }
        public String change { get; set; }
        public String ticketstatue { get; set; }
        public String ticketcode { get; set; }
        public String saletime { get; set; }

        public long insertTime { get; set; }//数据库存储用于筛选
        public String tradeid { get; set; }
        public String tradename { get; set; }
        public String total { get; set; }
        public String paytotal { get; set; }
        public String memberid { get; set; }
        public String membername { get; set; }
        public String membercode { get; set; }
        public String returncode { get; set; }
        public String cashierid { get; set; }
        public String cashiername { get; set; }
        public String cashiertime { get; set; }
        public String assistantid { get; set; }
        public String assistantname { get; set; }
        public String assistantcode { get; set; }
        public String ticketprice { get; set; }
        public String paytype { get; set; }
        public String payname { get; set; }
        public String payprice { get; set; }
        public String paycard { get; set; }


        public String commissionprice { get; set; }
        public String shopcode { get; set; }
        public String nationaltaxtotal { get; set; }
        public String localtaxtotal { get; set; }
        public String nationaltax { get; set; }
        public String localtax { get; set; }
        public String branchcode { get; set; }
        public String branchname { get; set; }

        public String shopname { get; set; }
        public String remark { get; set; }
        public String nationaltaxamount { get; set; }
        public String localtaxamount { get; set; }
        public String deviceuniquecode { get; set; }



        public String membercredit { get; set; }
        public String credit { get; set; }

        //add 2017/7/26
        public String iscrossbranch { get; set; }
        // TODO: 2017/10/25
        public String memberno { get; set; }
        // add: 2018/2/27
        public String membermodel { get; set; }

        public List<BillCommodityEntity> commoditys { get; set; }
        public List<PayDetailEntity> paydetails { get; set; }
        public List<DisCountDetailEntity> discountdetails { get; set; }
    }
}
