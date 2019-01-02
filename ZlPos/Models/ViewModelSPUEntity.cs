using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    /// <summary>
    /// 用来查询需要拼接sku进spu
    /// </summary>
    [SugarTable("SPUEntity")]
    class ViewModelSPUEntity : ModelContext
    {
        [SugarColumn(IsIgnore = true)]
        public string id { get; set; }
        [SugarColumn(IsNullable = true, IsPrimaryKey = true)]
        public string uid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string spucode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commodityname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditypic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string spec { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string saleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dispatchprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string wholesaleprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categorycode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string categoryname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category1name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category2name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category3name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string category4name { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canscore { get; set; }
        [SugarColumn(IsNullable = true)]
        public string score { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shortname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mnemonic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string brandname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string brandcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitcode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string unitname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string expiretime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliercode { get; set; }
        [SugarColumn(IsNullable = true)]
        public string suppliername { get; set; }
        [SugarColumn(IsNullable = true)]
        public string buyspec { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditystatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commoditytype { get; set; }
        [SugarColumn(IsNullable = true)]
        public string pricing { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commission { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionratio { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionmoney { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updatetime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string createuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updateuser { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ordernum { get; set; }
        [SugarColumn(IsNullable = true)]
        public string required { get; set; }
        [SugarColumn(IsNullable = true)]
        public string commissionrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string businessmodelname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string intaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string outtaxrate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string canstock { get; set; }
        [SugarColumn(IsNullable = true)]
        public string storagetype { get; set; }
        [SugarColumn(IsNullable = true)]
        public string dishid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string validtime { get; set; }
        [SugarColumn(IsNullable = true)]
        public string remark { get; set; }
        [SugarColumn(IsNullable = true)]
        public string plu { get; set; }
        [SugarColumn(IsNullable = true)]
        public string season { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speclevel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string updownstatus { get; set; }
        [SugarColumn(IsNullable = true)]
        public string del { get; set; }
        [SugarColumn(IsNullable = true)]
        public string datalevel { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv1 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv2 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv3 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv4 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv5 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string memberpricelv6 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specname01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalues01 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specname02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalues02 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string speccode03 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specname03 { get; set; }
        [SugarColumn(IsNullable = true)]
        public string specvalues03 { get; set; }

        [SugarColumn(IsIgnore = true)]
        public List<SKUEntity> recskulist
        {
            get
            {
                return base.CreateMapping<SKUEntity>().Where(i => i.spucode == this.spucode 
                                                                && i.shopcode == this.shopcode 
                                                                && i.branchcode == this.branchcode
                                                                && i.del == "0").ToList();
            }
        }

        //从barcode表中查出
        [SugarColumn(IsIgnore = true)]
        public string barcode { get; set; }
    }
}
