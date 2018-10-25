using log4net;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Bizlogic;
using ZlPos.Models;
using ZlPos.Utils;

namespace ZlPos.Dao
{
    class DbManagerImpl : DbManager
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        /// <summary>
        /// Bulkcopy with no the same
        /// 这个接口还有优化问题没有解决
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataArr"></param>
        public void BulkSaveOrUpdate<T>(List<T> dataArr) where T : class, new()
        {
            BulkSaveOrUpdate(dataArr.ToArray());

            #region old code
            //try
            //{
            //    using (var db = SugarDao.Instance)
            //    {
            //        if (dataArr.GetType().IsGenericType)
            //        {
            //            //Type[] tps = list.GetType().GetGenericArguments();
            //            if (!(dataArr.Count > 0))
            //            {
            //                logger.Info("BulkSaveOrUpdate:array bound error");
            //                return;
            //            }

            //            Type tp = dataArr[0].GetType();
            //            //数据处理
            //            if (!db.DbMaintenance.IsAnyTable(tp.Name))
            //            {
            //                //db.CodeFirst.InitTables(entity.GetType().Name);
            //                db.CodeFirst.InitTables(tp);
            //                //第一次建表直接插入完事
            //                db.Insertable(dataArr.ToArray()).Where(true, true).ExecuteCommand();
            //                return;

            //            }
            //            else
            //            {
            //                var data = dataArr;
            //                var table = db.Queryable<T>().ToList();

            //                DataTable dt1 = ConvertUtils.ToDataTable(data);
            //                DataTable dt2 = ConvertUtils.ToDataTable(table);

            //                //这里还是可以寻求一种高效的内存处理方式 取代数据库多次读取
            //                /////筛选出主键相同需要更新的数据
            //                //var dtt = from r in dt1.AsEnumerable()
            //                //          where !(from rr in dt2.AsEnumerable() select rr.Field<int>("id")).Contains(r.Field<int>("id"))
            //                //          select r;

            //                //差集(所有值 非只主键）
            //                IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default);
            //                //IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), new MyDataRowComparer());
            //                //两个数据源的差集集合
            //                if (query1.Any())
            //                {
            //                    DataTable dt3 = query1.CopyToDataTable();
            //                    var ls1 = ConvertUtils.ToList<T>(dt3).ToArray();
            //                    //这个差值包含了insert和update的数据 用foreach挑拣出来
            //                    foreach (var item in ls1)
            //                    {
            //                        //一条一条判断 暂时先这个等速度吃不消了再说
            //                        int rsCount = db.Updateable(item).ExecuteCommand();
            //                        if (rsCount == 0)
            //                        {
            //                            db.Insertable(item).Where(true, true).ExecuteCommand();
            //                        }
            //                    }

            //                    //因为包含了update和insert的数据 所以不能直接insertable
            //                    //if (ls1.Length > 0)
            //                    //{
            //                    //    db.Insertable(ls1).Where(true, true).ExecuteCommand();
            //                    //}
            //                    dt3.Clear(); dt3.Dispose(); dt3 = null;

            //                }

            //                //这里的交集是完全一样的交集  并不是类似于主键相同 其余有不同更新 所以没用
            //                ////交集
            //                //IEnumerable<DataRow> query2 = dt1.AsEnumerable().Intersect(dt2.AsEnumerable(), DataRowComparer.Default);
            //                //if (query2.Any())
            //                //{
            //                //    DataTable dt4 = query2.CopyToDataTable();

            //                //    var ls2 = ConvertUtils.ToList<T>(dt4).ToList();
            //                //    if (ls2.Count > 0)
            //                //    {
            //                //        db.Updateable(ls2).ExecuteCommand();
            //                //    }
            //                //    dt4.Clear(); dt4.Dispose(); dt4 = null;

            //                //}
            //                //dt1.Clear(); dt1.Dispose(); dt1 = null;
            //                //dt2.Clear(); dt2.Dispose(); dt2 = null;

            //                ////交集
            //                //var intersectedList = data.Intersect(table).ToList();
            //                //if (intersectedList.Count > 0)
            //                //{
            //                //    var s8 = db.Updateable(intersectedList).ExecuteCommand();
            //                //}
            //                ////并集
            //                //var expectedList = dataArr.ToList().Except(table.ToList()).ToList();
            //                //if (expectedList.Count > 0)
            //                //{

            //                //    var s9 = db.Insertable(expectedList).Where(true, true).ExecuteCommand();
            //                //}
            //            }

            //        }
            //        else
            //        {
            //            logger.Info("BulkSaveOrUpdate：Array error");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    logger.Error("BulkSaveOrUpdate:" + e.Message + e.StackTrace);
            //}
            #endregion
        }



        [Obsolete]
        public void Delete<T>(T entity) where T : class, new()
        {
            using (var db = SugarDao.Instance)
            {
                try
                {
                    //TODEBUG...
                    db.Deleteable<T>(entity).ExecuteCommand();
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public void SaveAndInsert<T>(T entity) where T : class, new()
        {
            using (var db = SugarDao.Instance)
            {
                try
                {
                    if (!db.DbMaintenance.IsAnyTable(entity.GetType().Name, false))
                    {
                        db.CodeFirst.InitTables(entity.GetType());
                    }
                    db.Insertable(entity).Where(true).ExecuteCommand();
                }
                catch (Exception e)
                {
                    //db.Ado.RollbackTran();
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
            using (var db = SugarDao.Instance)
            {
                try
                {
                    //db.Ado.BeginTran();
                    if (!db.DbMaintenance.IsAnyTable(entity.GetType().Name, false))
                    {
                        //db.CodeFirst.InitTables(entity.GetType().Name);
                        db.CodeFirst.InitTables(entity.GetType());

                    }

                    int rsCount = db.Updateable(entity).ExecuteCommand();

                    if (rsCount == 0)
                    {
                        db.Insertable(entity).Where(true, true).ExecuteCommand();
                    }
                    //}
                    //db.Ado.CommitTran();


                }
                catch (Exception e)
                {
                    //db.Ado.RollbackTran();
                    throw e;
                }
            }
        }


        public void SaveOrUpdateCommodityEntities(CommodityEntity[] commoditys)
        {
            try
            {
                using (var db = SugarDao.Instance)
                {

                    if (commoditys == null)
                    {
                        logger.Info("SaveOrUpdateCommodityEntities:commoditys is null");
                        return;
                    }
                    if (commoditys.Length == 0)
                    {
                        logger.Info("SaveOrUpdateCommodityEntities:commoditys coult is 0");
                        return;
                    }

                    //数据处理
                    if (!db.DbMaintenance.IsAnyTable(typeof(CommodityEntity).Name,false))
                    {
                        //db.CodeFirst.InitTables(entity.GetType().Name);
                        db.CodeFirst.InitTables(typeof(CommodityEntity));
                        //第一次建表直接插入完事
                        db.Insertable(commoditys).Where(true, true).ExecuteCommand();
                        return;

                    }
                    else
                    {
                        var data = commoditys;
                        var table = db.Queryable<CommodityEntity>().ToList();

                        DataTable dt1 = ConvertUtils.ToDataTable(data);
                        DataTable dt2 = ConvertUtils.ToDataTable(table);

                        //这里还是可以寻求一种高效的内存处理方式 取代数据库多次读取
                        /////筛选出主键相同需要更新的数据
                        //var dtt = from r in dt1.AsEnumerable()
                        //          where !(from rr in dt2.AsEnumerable() select rr.Field<int>("id")).Contains(r.Field<int>("id"))
                        //          select r;

                        //差集(所有值 非只主键）
                        IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default);
                        //IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), new MyDataRowComparer());
                        //两个数据源的差集集合
                        if (query1.Any())
                        {
                            DataTable dt3 = query1.CopyToDataTable();

                            var dataFilter = from r in dt2.AsEnumerable()
                                             select r.Field<string>("id");

                            DataTable dtUpdate = dt3.Clone();
                            DataTable dtInsert = dt3.Clone();
                            foreach (DataRow item in dt3.Rows)
                            {
                                if (dataFilter.Contains(item["id"]))
                                {
                                    dtUpdate.Rows.Add(item.ItemArray);
                                }
                                else
                                {
                                    dtInsert.Rows.Add(item.ItemArray);
                                }
                            }

                            //需要更新的数据
                            var lsUpdate = ConvertUtils.ToList<CommodityEntity>(dtUpdate).ToArray();
                            if (lsUpdate.Count() > 0)
                            {
                                db.Updateable(lsUpdate).ExecuteCommand();
                            }

                            var lsInsert = ConvertUtils.ToList<CommodityEntity>(dtInsert).ToArray();
                            if (lsInsert.Count() > 0)
                            {
                                db.Insertable(lsInsert).Where(true, true).ExecuteCommand();
                            }

                            dtUpdate.Clear(); dtUpdate.Dispose(); dtUpdate = null;
                            dtInsert.Clear(); dtInsert.Dispose(); dtInsert = null;

                            ////需要插入的数据 
                            ////TODO...


                            //var ls1 = ConvertUtils.ToList<CommodityEntity>(dt3).ToArray();
                            ////这个差值包含了insert和update的数据 用foreach挑拣出来
                            //foreach (var item in ls1)
                            //{
                            //    //一条一条判断 暂时先这个等速度吃不消了再说
                            //    int rsCount = db.Updateable(item).ExecuteCommand();
                            //    if (rsCount == 0)
                            //    {
                            //        db.Insertable(item).Where(true, true).ExecuteCommand();
                            //    }
                            //}

                            //因为包含了update和insert的数据 所以不能直接insertable
                            //if (ls1.Length > 0)
                            //{
                            //    db.Insertable(ls1).Where(true, true).ExecuteCommand();
                            //}

                            dt3.Clear(); dt3.Dispose(); dt3 = null;

                        }


                        else
                        {
                            logger.Info("SaveOrUpdateCommodityEntities：Array error");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("SaveOrUpdateCommodityEntities:" + e.Message + e.StackTrace);
            }
        }

        public void BulkSaveOrUpdateBarCodes(BarCodeEntity2[] barCodeEntity2)
        {
            try
            {
                using (var db = SugarDao.Instance)
                {

                    if (barCodeEntity2 == null)
                    {
                        logger.Info("BulkSaveOrUpdateCommodityPriceEntityList:commoditys is null");
                        return;
                    }
                    if (barCodeEntity2.Length == 0)
                    {
                        logger.Info("BulkSaveOrUpdateCommodityPriceEntityList:commoditys coult is 0");
                        return;
                    }

                    //数据处理
                    if (!db.DbMaintenance.IsAnyTable(typeof(BarCodeEntity2).Name,false))
                    {
                        //db.CodeFirst.InitTables(entity.GetType().Name);
                        db.CodeFirst.InitTables(typeof(BarCodeEntity2));
                        //第一次建表直接插入完事
                        db.Insertable(barCodeEntity2).Where(true, true).ExecuteCommand();
                        return;

                    }
                    else
                    {
                        var data = barCodeEntity2;
                        var table = db.Queryable<BarCodeEntity2>().ToList();

                        DataTable dt1 = ConvertUtils.ToDataTable(data);
                        DataTable dt2 = ConvertUtils.ToDataTable(table);

                        //这里还是可以寻求一种高效的内存处理方式 取代数据库多次读取
                        /////筛选出主键相同需要更新的数据
                        //var dtt = from r in dt1.AsEnumerable()
                        //          where !(from rr in dt2.AsEnumerable() select rr.Field<int>("id")).Contains(r.Field<int>("id"))
                        //          select r;

                        //差集(所有值 非只主键）
                        IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default);
                        //IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), new MyDataRowComparer());
                        //两个数据源的差集集合
                        if (query1.Any())
                        {
                            DataTable dt3 = query1.CopyToDataTable();

                            var dataFilter = from r in dt2.AsEnumerable()
                                             select r.Field<string>("id");

                            DataTable dtUpdate = dt3.Clone();
                            DataTable dtInsert = dt3.Clone();
                            foreach (DataRow item in dt3.Rows)
                            {
                                if (dataFilter.Contains(item["id"]))
                                {
                                    dtUpdate.Rows.Add(item.ItemArray);
                                }
                                else
                                {
                                    dtInsert.Rows.Add(item.ItemArray);
                                }
                            }

                            //需要更新的数据
                            var lsUpdate = ConvertUtils.ToList<BarCodeEntity2>(dtUpdate).ToArray();
                            if (lsUpdate.Count() > 0)
                            {
                                db.Updateable(lsUpdate).ExecuteCommand();
                            }

                            var lsInsert = ConvertUtils.ToList<BarCodeEntity2>(dtInsert).ToArray();
                            if (lsInsert.Count() > 0)
                            {
                                db.Insertable(lsInsert).Where(true, true).ExecuteCommand();
                            }

                            dtUpdate.Clear(); dtUpdate.Dispose(); dtUpdate = null;
                            dtInsert.Clear(); dtInsert.Dispose(); dtInsert = null;

                            dt3.Clear(); dt3.Dispose(); dt3 = null;

                        }


                        else
                        {
                            logger.Info("BulkSaveOrUpdateCommodityPriceEntityList：Array error");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("BulkSaveOrUpdateCommodityPriceEntityList:" + e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// 优化速度的bluksave
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        public void BulkSaveOrUpdate<T>(T[] array) where T : class, new()
        {
            try
            {
                if (array == null)
                {
                    logger.Info("BulkSaveOrUpdateCommodityPriceEntityList:commoditys is null");
                    return;
                }
                if (array.Length == 0)
                {
                    logger.Info("BulkSaveOrUpdateCommodityPriceEntityList:commoditys coult is 0");
                    return;
                }
                using (var db = SugarDao.Instance)
                {


                    //数据处理
                    if (!db.DbMaintenance.IsAnyTable(typeof(T).Name,false))
                    {
                        db.CodeFirst.InitTables(typeof(T));
                        //第一次建表直接插入完事
                        db.Insertable(array).Where(true, true).ExecuteCommand();
                        //return;

                    }
                    else
                    {
                        var data = array;
                        var table = db.Queryable<T>().ToList();

                        DataTable dt1 = ConvertUtils.ToDataTable(data);
                        DataTable dt2 = ConvertUtils.ToDataTable(table);

                        //这里还是可以寻求一种高效的内存处理方式 取代数据库多次读取
                        /////筛选出主键相同需要更新的数据
                        //var dtt = from r in dt1.AsEnumerable()
                        //          where !(from rr in dt2.AsEnumerable() select rr.Field<int>("id")).Contains(r.Field<int>("id"))
                        //          select r;

                        //差集(所有值 非只主键）
                        IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), DataRowComparer.Default);
                        //IEnumerable<DataRow> query1 = dt1.AsEnumerable().Except(dt2.AsEnumerable(), new MyDataRowComparer());
                        //两个数据源的差集集合
                        if (query1.Any())
                        {
                            DataTable dt3 = query1.CopyToDataTable();

                            var dataFilter = from r in dt2.AsEnumerable()
                                             select r.Field<string>("id");

                            DataTable dtUpdate = dt3.Clone();
                            DataTable dtInsert = dt3.Clone();
                            foreach (DataRow item in dt3.Rows)
                            {
                                if (dataFilter.Contains(item["id"]))
                                {
                                    dtUpdate.Rows.Add(item.ItemArray);
                                }
                                else
                                {
                                    dtInsert.Rows.Add(item.ItemArray);
                                }
                            }

                            //需要更新的数据
                            var lsUpdate = ConvertUtils.ToList<T>(dtUpdate).ToArray();
                            if (lsUpdate.Count() > 0)
                            {
                                db.Updateable(lsUpdate).ExecuteCommand();
                            }

                            var lsInsert = ConvertUtils.ToList<T>(dtInsert).ToArray();
                            if (lsInsert.Count() > 0)
                            {
                                db.Insertable(lsInsert).Where(true, true).ExecuteCommand();
                            }

                            dtUpdate.Clear(); dtUpdate.Dispose(); dtUpdate = null;
                            dtInsert.Clear(); dtInsert.Dispose(); dtInsert = null;

                            dt3.Clear(); dt3.Dispose(); dt3 = null;

                        }


                        else
                        {
                            logger.Info("BulkSaveOrUpdateCommodityPriceEntityList：Array error");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error("BulkSaveOrUpdateCommodityPriceEntityList:" + e.Message + e.StackTrace);
            }
        }

        public bool IsTableExist<T>(T table) where T : class, new()
        {
            try
            {
                using (var db = SugarDao.Instance)
                {
                    //db.Ado.BeginTran();
                    if (!db.DbMaintenance.IsAnyTable(table.GetType().Name, false))
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
