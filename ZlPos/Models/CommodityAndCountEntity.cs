using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    [Obsolete]
    class CommodityAndCountEntity
    {
        public int count { get; set; }
        public List<SPUEntity> commodityEntities { get; set; }
    }
}
