using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CommodityPriceEntity
    {
        public String id { get; set; }
        public String shopcode { get; set; }
        public String branchcode { get; set; }
        public String branchname { get; set; }
        public String commoditycode { get; set; }
        public String commodityname { get; set; }
        public String saleprice { get; set; }
        public String memberprice { get; set; }
        public String dispatchprice { get; set; }
        public String wholesaleprice { get; set; }
        public String buyprice { get; set; }
        public String updateuser { get; set; }
        public String crateuser { get; set; }
        public String createtime { get; set; }
        public String updatetime { get; set; }
    }
}
