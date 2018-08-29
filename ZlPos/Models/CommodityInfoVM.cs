using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Bean;

namespace ZlPos.Models
{
    class CommodityInfoVM : BaseData
    {
        [SugarColumn(IsNullable = true,IsPrimaryKey = true,IsIdentity = true)]
        public int id { get; set; }
        [SugarColumn(IsNullable = true)]
        public String shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public String requesttime { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<CommodityEntity> commoditys { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<CategoryEntity> categorys { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<MemberEntity> memberlevels { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<PayTypeEntity> paytypes { get; set; }
        //此节点已弃用1
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<AssistantsEntity> assistants { get; set; }
        [SugarColumn(IsNullable = true,IsIgnore = true)]
        public List<CashierEntity> users { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<SupplierEntity> suppliers { get; set; }

        // TODO: 2017/8/17
        [SugarColumn(IsNullable = true)]
        public String templete58 { get; set; }
        [SugarColumn(IsNullable = true)]
        public String templete80 { get; set; }
        // TODO: 2017/10/17
        [SugarColumn(IsNullable = true)]
        public String branchcode { get; set; }

        // add: 2018/2/27
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<BarCodeEntity2> barcodes { get; set; }
        [SugarColumn(IsNullable = true, IsIgnore = true)]
        public List<CommodityPriceEntity> commoditypricelist { get; set; }

        // add: 2018/4/8
        [SugarColumn(IsNullable = true)]
        public String cashiergrant { get; set; }

        [SugarColumn(IsNullable = true)]
        public String isbranchpay { get; set; }

        [SugarColumn(IsNullable = true)]
        public String type { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbcreditrule { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbcreditrange { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbcreditrate { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbcreditpay { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbmsisrecharge { get; set; }

        [SugarColumn(IsNullable = true)]
        public String mbisrecharge { get; set; }


    }
}
