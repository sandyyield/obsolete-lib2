using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Dao;
using ZlPos.Models;

namespace ZlPos.Bizlogic
{
    class ContextCache
    {
        #region readcard cache
        public static string GetReadCard()
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity"))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.readCard;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SetReadCard(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.readCard = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

        #region Scale cache
        public static string getScale()
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity"))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.scale;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setScale(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.scale = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

        #region shopcode cache
        public static string GetShopcode()
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity"))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.shopcode;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SetShopcode(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity"))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        contextEntity = new ContextEntity();
                    }
                    contextEntity.shopcode = v;
                    DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
                }
                else
                {
                    return;
                }
            }
        }
        #endregion

        #region SerialPort cache
        public static string GetSerialPort()
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity"))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.readCard;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SetSerialPort(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.serialPort = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

    }
}
