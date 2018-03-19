using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class CommodityInfoVM
    {
        public int id { get; set; }
        public String shopcode { get; set; }
        public String requesttime { get; set; }
        public List<CommodityEntity> commoditys { get; set; }
        public List<CategoryEntity> categorys { get; set; }
        public List<MemberEntity> memberlevels { get; set; }
        public List<PayTypeEntity> paytypes { get; set; }
        public List<AssistantsEntity> assistants { get; set; }
        public List<CashierEntity> users { get; set; }
        public List<SupplierEntity> suppliers { get; set; }

        // TODO: 2017/8/17
        public String templete58 { get; set; }
        public String templete80 { get; set; }
        // TODO: 2017/10/17
        public String branchcode { get; set; }

        // add: 2018/2/27
        public List<BarCodeEntity> barcodes { get; set; }
        public List<CommodityPriceEntity> commoditypricelist { get; set; }
    }
}
