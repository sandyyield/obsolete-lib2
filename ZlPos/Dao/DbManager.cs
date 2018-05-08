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

        void BulkSaveOrUpdate<T>(T[] list) where T : class, new();
        void Delete<T> (T payDetailEntity) where T : class, new();
    }
}
