using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Dao
{
    class DbManagerImpl : DbManager
    {

        /// <summary>
        /// Bulkcopy with no the same
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void BulkSaveOrUpdate<T>(T list) where T : class, new()
        {
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    #region 这里可以优化数据库操作  迫于上线需求 后期再来封装完善 实现去重的bulkcopy逻辑 TODO...
                    ////先把数据封装成bulk 
                    //Models.Employee sb = new Models.Employee() { EmployeeID = 95302, FirstName = "hahaha", LastName = "world" };
                    //Models.Employee sb2 = new Models.Employee() { EmployeeID = 00011, FirstName = "凡总划水", LastName = "天下第一" };
                    //List<Models.Employee> lstDataBulk = new List<Models.Employee>() { sb, sb2 };
                    //var s9 = db.Insertable(lstDataBulk.ToArray()).IgnoreColumns(it => sb).ExecuteCommand();
                    //var s9 = db.Insertable(new List<Models.Employee>(){ sb, sb2 }.ToArray()).IgnoreColumns(it=> sb).ExecuteCommand();
                    //db.Deleteable(new List<Models.Employee>() { sb, sb2 }.ToArray()).ExecuteCommand();
                    ////把块插入到一个临时表

                    ////通过temptable和目标表 比对差值 分成update部分和insert部分分别进行操作

                    #endregion
                }
                catch (Exception)
                {

                    throw;
                }
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
                    //判断一下是否为list集合
                    if (entity.GetType().IsGenericType)
                    {
                        Type[] tps = entity.GetType().GetGenericArguments();
                        if(!(tps.Length > 0))
                        {
                            throw new Exception("saveOrUpdate list数据types出错");
                        }
                        if (!db.DbMaintenance.IsAnyTable(tps[0].Name))
                        {
                            //db.CodeFirst.InitTables(entity.GetType().Name);
                            db.CodeFirst.InitTables(tps[0]);

                        }
                        //TODO...这里还没有完成
                        Type tpp = entity.GetType().GetGenericTypeDefinition();
                        //var tlist = entity is tpp;
                        var s9 = db.Insertable(entity).Where(true, true).ExecuteCommand();
                    }
                    else
                    {
                        //db.Ado.BeginTran();
                        if (!db.DbMaintenance.IsAnyTable(entity.GetType().Name))
                        {
                            //db.CodeFirst.InitTables(entity.GetType().Name);
                            db.CodeFirst.InitTables(entity.GetType());

                        }
                        //db.Ado.CommitTran();

                        int rsCount = db.Updateable(entity).ExecuteCommand();

                        if (rsCount == 0)
                        {
                            db.Insertable(entity).ExecuteCommand();
                        }
                    }



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
