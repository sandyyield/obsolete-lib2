using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class UpdateCommodityParamsEntity
    {
        public string commoditycode { get; set; }
        public string memberprice { get; set; }
        public string saleprice { get; set; }
        public string spucode { get; set; }
    }
}
