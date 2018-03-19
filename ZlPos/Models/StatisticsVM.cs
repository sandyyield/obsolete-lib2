using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class StatisticsVM
    {
        public String starttime { get; set; }
        public String endtime { get; set; }
        public String branchname { get; set; }
        public String cashiername { get; set; }
        public String ticketnums { get; set; }
        public String ticketamount { get; set; }
        public String returnnums { get; set; }
        public String returnamount { get; set; }
        public String rechargeamount { get; set; }
        public String subtotal { get; set; }
        public String cashnums { get; set; }
        public String cashamount { get; set; }
        public String alinums { get; set; }
        public String aliamount { get; set; }
        public String wxnums { get; set; }
        public String wxamount { get; set; }
        public String total { get; set; }
        public List<ZidingyizhifuBean> zidingyizhifu { get; set; }
    }

    class ZidingyizhifuBean
    {
        /// <summary>
        /// 
        /// </summary>
        public string zidingyinam { get; set; }
        public string zidingyinums { get; set; }
        public string zidingyiamount { get; set; }
    }
}
