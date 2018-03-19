using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class MemberEntity
    {
        public String id { get; set; }

        public String shopcode { get; set; }

        public String name { get; set; }

        public String isautoupgrade { get; set; }

        public String expiretime { get; set; }

        public String discount { get; set; }

        public String upgradenumber { get; set; }

        public String createuser { get; set; }

        public String updateuser { get; set; }

        public String createtime { get; set; }

        public String updatetime { get; set; }

        public String remark { get; set; }

        public String isrecharge { get; set; }

        //2018年1月30日 add


        public String branchcode { get; set; }


        public String del { get; set; }
    }
}
