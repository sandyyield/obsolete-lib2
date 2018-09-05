using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    /// <summary>
    /// create 2018年9月5日
    /// 离线交易查询参数实体类
    /// </summary>
    class QueryBillEntity
    {
        public string ticketcode { get; set; }
        public string cashierid { get; set; }
        public string starttime { get; set; }
        public string endtime { get; set; }
        public int pageindex { get; set; }
        public int pagesize { get; set; }
    }
}
