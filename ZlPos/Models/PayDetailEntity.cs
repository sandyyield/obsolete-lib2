using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class PayDetailEntity
    {
        public int id { get; set; }
        public String ticketcode { get; set; }
        public String paycode { get; set; }
        public String payname { get; set; }
        public String shopcode { get; set; }
        public String payamount { get; set; }
        public String paycardcode { get; set; }

        //2017/12/16新增
        public String branchcode { get; set; }
        public String branchname { get; set; }
        public String businesscode { get; set; }
        public String countcardnums { get; set; }
        public String paycredit { get; set; }
        public String paytime { get; set; }
        // add: 2018/2/27
        public String ordercode { get; set; }
    }
}
