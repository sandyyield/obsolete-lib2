using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class DisCountDetailEntity
    {

        public int id { get; set; }
        public String ticketcode { get; set; }
        public String shopcode { get; set; }
        public String branchcode { get; set; }
        public String branchname { get; set; }
        public String vouchertype { get; set; }
        public String voucheramount { get; set; }
        public String voucherchannelid { get; set; }
        public String vouchername { get; set; }
        public String voucherdocno { get; set; }
    }
}
