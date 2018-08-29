using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Models;

namespace ZlPos.Dao
{
    interface DbManager
    {
        void SaveOrUpdate<T>(T entity) where T : class, new();
        void SaveAndInsert<T>(T entity) where T : class, new();
        void BulkSaveOrUpdate<T>(List<T> list) where T : class, new();
        void BulkSaveOrUpdate<T>(T[] array) where T : class, new();
        void Delete<T>(T payDetailEntity) where T : class, new();
        bool IsTableExist<T>(T table) where T:class,new();
        //void SaveOrUpdateCommodityEntities(CommodityEntity[] commodityEntity);
        //void BulkSaveOrUpdateBarCodes(BarCodeEntity2[] barCodeEntity2);
    }
}
