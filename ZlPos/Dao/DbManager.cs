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
        void BulkSaveOrUpdate<T>(List<T> list, string primaryKey = "id") where T : class, new();
        void BulkSaveOrUpdate<T>(T[] array, string primaryKey = "id") where T : class, new();
        void Delete<T>(T payDetailEntity) where T : class, new();
        bool IsTableExist<T>(T table) where T : class, new();
        void Insert<T>(T entity) where T : class, new();
        void BulkSaveOrUpdateTurbo<T>(List<T> array, string primaryKey = "id") where T : class, new();
        void BulkCopy<T1, T2>(List<T1> array, List<T2> primaryKey)
            where T1 : class, new();
            
        //void SaveOrUpdateCommodityEntities(CommodityEntity[] commodityEntity);
        //void BulkSaveOrUpdateBarCodes(BarCodeEntity2[] barCodeEntity2);
    }
}
