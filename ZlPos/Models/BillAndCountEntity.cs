using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    /// <summary>
    /// Creat 2018年9月5日
    /// </summary>
    [Obsolete]
    class BillAndCountEntity
    {
        public long count { get; set; }
        public List<BillEntity> billEntityList { get; set; }
    }
}
