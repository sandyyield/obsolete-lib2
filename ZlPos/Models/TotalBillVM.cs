using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class TotalBillVM
    {
        public long totalnum { get; set; }
        public string totalpay { get; set; }
        public List<PayTypeEntity> paytypes { get; set; }

        public class PayTypeEntity
        {
            public string payname { get; set; }
            public string totalnum { get; set; }
            public string totalpay { get; set; }
        }
    }
}
