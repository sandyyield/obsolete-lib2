using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CategoryEntity
    {
        public String id { get; set; }
        public String categorycode { get; set; }
        public String categoryname { get; set; }
        public String shopcode { get; set; }
        public String ordernum { get; set; }
        public String pid { get; set; }
        public String required { get; set; }
        public String level { get; set; }

        public String del { get; set; }
        // TODO: 2017/10/17
        public String branchcode { get; set; }

    }
}
