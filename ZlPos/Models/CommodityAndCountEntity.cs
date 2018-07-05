using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CommodityAndCountEntity
    {
        public int count { get; set; }
        public List<CommodityEntity> commodityEntities { get; set; }
    }
}
