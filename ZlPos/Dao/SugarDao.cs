using log4net;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ZlPos.Dao
{
    /// <summary>
    /// SqlSugar
    /// </summary>
    public class SugarDao
    {
        private static ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private SugarDao()
        { }
        public static string ConnectionString
        {
            get
            {
                //string reval = "DataSource=" + System.AppDomain.CurrentDomain.BaseDirectory + "DataBase\\zlCloudPos.db"; //这里可以动态根据cookies或session实现多库切换
                string reval = "Data Source=" + System.AppDomain.CurrentDomain.BaseDirectory + "DataBase\\zlCloudPos.db;";
                return reval;
            }
        }
        public static SqlSugarClient GetInstance()
        {

            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            });
            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                logger.Info("Sql>>>" + sql + Environment.NewLine + db.Utilities.SerializeObject(pars.ToDictionary(i => i.ParameterName, i => i.Value)));
                //if (db.TempItems == null)
                //{
                //    db.TempItems = new Dictionary<string, object>();
                //}
                //db.TempItems.Add("logTime", DateTime.Now);
            };
            //db.Aop.OnLogExecuted = (sql, pars) =>
            //{
            //    var startingTime = db.TempItems["logTime"];
            //    db.TempItems.Remove("time");
            //    var completedTime = DateTime.Now;
            //    logger.Info("Sql executing time >>>  [" + (completedTime - Convert.ToDateTime(startingTime)).Milliseconds + "]ms");
            //};
            return db;
        }
    }
}