using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Models;

namespace ZlPos.Manager
{
    public class CommodityCacheManager
    {
        private static object obj = new object();//对象锁
        private static CommodityCacheManager commodityCacheManager;
        private List<CategoryEntity> categoryEntities;//商品分类
        private List<MemberEntity> memberEntities;
        private List<PayTypeEntity> paytypes;
        private List<AssistantsEntity> assistants;
        private List<CashierEntity> users;
        private List<SupplierEntity> suppliers;
        private Dictionary<String, List<CommodityEntity>> commodityMap;//存放各个分类以及分类对应的商品全量

        // add: 2018/2/27
        private List<BarCodeEntity2> barCodes;
        private List<CommodityPriceEntity> commodityPriceEntityList;

        private CommodityCacheManager()
        {
            commodityMap = new Dictionary<String, List<CommodityEntity>>();
        }


        public static CommodityCacheManager Instance
        {
            get
            {
                if (commodityCacheManager == null)
                {
                    lock (obj)
                    {
                        if (commodityCacheManager == null)
                        {
                            commodityCacheManager = new CommodityCacheManager();
                        }
                    }
                }
                return commodityCacheManager;
            }
        }

        public static object Obj { get => obj; set => obj = value; }
        public List<CategoryEntity> CategoryEntities { get => categoryEntities; set => categoryEntities = value; }
        public List<MemberEntity> MemberEntities { get => memberEntities; set => memberEntities = value; }
        public List<PayTypeEntity> Paytypes { get => paytypes; set => paytypes = value; }
        public List<AssistantsEntity> Assistants { get => assistants; set => assistants = value; }
        public List<CashierEntity> Users { get => users; set => users = value; }
        public List<SupplierEntity> Suppliers { get => suppliers; set => suppliers = value; }
        public Dictionary<string, List<CommodityEntity>> CommodityMap { get => commodityMap; set => commodityMap = value; }
        public List<BarCodeEntity2> BarCodes { get => barCodes; set => barCodes = value; }
        public List<CommodityPriceEntity> CommodityPriceEntityList { get => commodityPriceEntityList; set => commodityPriceEntityList = value; }

        /// <summary>
        /// 清空缓存
        /// </summary>
        public void cleanCache()
        {
            if (commodityCacheManager != null)
            {
                commodityCacheManager = null;
            }
        }
    }
}
