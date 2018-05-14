using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Utils;

namespace ZlPos.Dao
{
    class DbManagerImpl : DbManager
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Bulkcopy with no the same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataArr"></param>
        public void BulkSaveOrUpdate<T>(List<T> dataArr) where T : class, new()
        {
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    if (dataArr.GetType().IsGenericType)
                    {
                        //Type[] tps = list.GetType().GetGenericArguments();
                        if (!(dataArr.Count > 0))
                        {
                            logger.Info("BulkSaveOrUpdate:array bound error");
                        }

                        Type tp = dataArr[0].GetType();
                        //数据处理
                        if (!db.DbMaintenance.IsAnyTable(tp.Name))
                        {
                            //db.CodeFirst.InitTables(entity.GetType().Name);
                            db.CodeFirst.InitTables(tp);
                            //第一次建表直接插入完事
                            db.Insertable(dataArr.ToArray()).Where(true, true).ExecuteCommand();
                            return;

                        }
                        else
                        {
                            var table = db.Queryable<T>().ToList();
                            var data = dataArr;

                            DataTable dt1 = ConvertUtils.ToDataTable(data);
                            DataTable dt2 = ConvertUtils.ToDataTable(table);

                            //差集
                            IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default);
                            //两个数据源的差集集合
                            if (query1.Any())
                            {
                                DataTable dt3 = query1.CopyToDataTable();
                                var ls1 = ConvertUtils.ToList<T>(dt3).ToArray();
                                if (ls1.Length > 0)
                                {
                                    db.Insertable(ls1).Where(true, true).ExecuteCommand();
                                }
                                dt3.Clear();dt3.Dispose();dt3 = null;

                            }
                            //交集
                            IEnumerable<DataRow> query2 = dt1.AsEnumerable().Intersect(dt2.AsEnumerable(), DataRowComparer.Default);
                            if (query2.Any())
                            {
                                DataTable dt4 = query2.CopyToDataTable();

                                var ls2 = ConvertUtils.ToList<T>(dt4).ToList();
                                if (ls2.Count > 0)
                                {
                                    db.Updateable(ls2).ExecuteCommand();
                                }
                                dt4.Clear(); dt4.Dispose(); dt4 = null;

                            }
                            dt1.Clear(); dt1.Dispose(); dt1 = null;
                            dt2.Clear(); dt2.Dispose(); dt2 = null;

                            ////交集
                            //var intersectedList = data.Intersect(table).ToList();
                            //if (intersectedList.Count > 0)
                            //{
                            //    var s8 = db.Updateable(intersectedList).ExecuteCommand();
                            //}
                            ////并集
                            //var expectedList = dataArr.ToList().Except(table.ToList()).ToList();
                            //if (expectedList.Count > 0)
                            //{

                            //    var s9 = db.Insertable(expectedList).Where(true, true).ExecuteCommand();
                            //}
                        }

                    }
                    else
                    {
                        logger.Info("BulkSaveOrUpdate：Array error");
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("BulkSaveOrUpdate:" + e.Message + e.StackTrace);
            }

        }

        [Obsolete]
        public void Delete<T>(T entity) where T : class, new()
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //TODEBUG...
                    db.Deleteable<T>().ExecuteCommand();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }



        /// <summary>
        /// Single save
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        public void SaveOrUpdate<T>(T entity)
            where T : class, new()
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    //这部分代码移到BulkSaveOrUpdate
                    ////判断一下是否为list集合
                    //if (entity.GetType().IsGenericType)
                    //{
                    //    Type[] tps = entity.GetType().GetGenericArguments();
                    //    if(!(tps.Length > 0))
                    //    {
                    //        throw new Exception("saveOrUpdate list数据types出错");
                    //    }
                    //    if (!db.DbMaintenance.IsAnyTable(tps[0].Name))
                    //    {
                    //        //db.CodeFirst.InitTables(entity.GetType().Name);
                    //        db.CodeFirst.InitTables(tps[0]);

                    //    }
                    //    //TODO...这里还没有完成
                    //    Type tpp = entity.GetType().GetGenericTypeDefinition();
                    //    //var tlist = entity is tpp;
                    //    var s9 = db.Insertable(entity).Where(true, true).ExecuteCommand();
                    //}
                    //else
                    //{
                    //db.Ado.BeginTran();
                    if (!db.DbMaintenance.IsAnyTable(entity.GetType().Name,false))
                    {
                        //db.CodeFirst.InitTables(entity.GetType().Name);
                        db.CodeFirst.InitTables(entity.GetType());

                    }
                    
                    int rsCount = db.Updateable(entity).ExecuteCommand();

                    if (rsCount == 0)
                    {
                        db.Insertable(entity).ExecuteCommand();
                    }
                    //}
                    //db.Ado.CommitTran();


                }
                catch (Exception e)
                {
                    db.Ado.RollbackTran();
                    throw e;
                }
            }
        }
    }
}
