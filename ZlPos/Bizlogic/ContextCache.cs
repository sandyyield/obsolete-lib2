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

        #region BJQprint
        public static string getBJQprint()
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
                    return contextEntity.BJQprint;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void SetBJQprint(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.BJQprint = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

        #region Scale cache
        public static string getScale()
        {
            using (var db = SugarDao.GetInstance())
            {
                if (db.DbMaintenance.IsAnyTable(typeof(ContextEntity).Name,false))
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
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.shopcode = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);

                
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

        #region Gprint cache
        public static string getGprint()
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
                    return contextEntity.gprint;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setGprint(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.gprint = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

        #region BarcodeStyle cache
        public static string getBarcodeStyle()
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
                    return contextEntity.barcodeStyle;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setBarcodeStyle(string v)
        {
            using (var db = SugarDao.GetInstance())
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.barcodeStyle = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion 

    }
}
