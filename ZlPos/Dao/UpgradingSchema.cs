using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using ZlPos.Models;
using ZlPos.Utils;

namespace ZlPos.Dao
{
    class UpgradingSchema
    {
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void UpgradingVersion2()
        {
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    //获取旧表数据
                    var oldDt = db.Ado.GetDataTable("select * from CommodityEntity");

                    var dt = from r in oldDt.AsEnumerable()
                             select r;

                    //新表继承老数据
                    DataTable newDt = oldDt;
                    //添加新字段
                    newDt.Columns.Add("validtime", Type.GetType("System.String"));

                    //老数据备份
                    db.DbMaintenance.BackupTable("CommodityEntity", "CommodityEntity" + DateTime.Now);
                    //删除老表
                    db.DbMaintenance.DropTable("CommodityEntity");

                    //创建新表
                    db.CodeFirst.InitTables(Type.GetType("ZlPos.Models.CommodityEntity"));
                    var ls = ConvertUtils.ToList<CommodityEntity>(newDt).ToArray();
                    db.Insertable(ls).Where(true, true).ExecuteCommand();


                }
            }
            catch(Exception e)
            {
                logger.Info("UpgradingVersion2 exception>>" + e.Message + e.StackTrace);
            }
        }
    }
}
