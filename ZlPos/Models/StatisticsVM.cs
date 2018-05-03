using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class StatisticsVM
    {

        public string starttime { get; set; }
        public string endtime { get; set; }
        public string branchname { get; set; }
        public string cashiername { get; set; }
        public string ticketnums { get; set; }
        public string ticketamount { get; set; }
        public string returnnums { get; set; }
        public string returnamount { get; set; }
        public string rechargeamount { get; set; }
        public string subtotal { get; set; }
        public string cashnums { get; set; }
        public string cashamount { get; set; }
        public string alinums { get; set; }
        public string aliamount { get; set; }
        public string wxnums { get; set; }
        public string wxamount { get; set; }
        public string total { get; set; }
        public List<ZidingyizhifuBean> zidingyizhifu { get; set; }
    }

    public class ZidingyizhifuBean
    {
        /// <summary>
        /// 
        /// </summary>
        public string zidingyiname { get; set; }
        public string zidingyinums { get; set; }
        public string zidingyiamount { get; set; }
    }
}
