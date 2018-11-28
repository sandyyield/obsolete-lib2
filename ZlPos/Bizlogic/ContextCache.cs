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
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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

        #region BarcodeScale cache
        public static string getBarcodeStyle()
        {
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
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
            using (var db = SugarDao.Instance)
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


        #region SPBQTemplet cache
        public static string getSPBQTemplet()
        {
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.SPBQTemplet;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setSPBQTemplet(string v)
        {
            using (var db = SugarDao.Instance)
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.SPBQTemplet = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion


        #region DDBQTemplet cache
        public static string getDDBQTemplet()
        {
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity", false))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.DDBQTemplet;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setDDBQTemplet(string v)
        {
            using (var db = SugarDao.Instance)
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.DDBQTemplet = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion 


        #region BJQTemplet cache
        public static string getBJQTemplet()
        {
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity",false))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.BJQTemplet;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setBJQTemplet(string v)
        {
            using (var db = SugarDao.Instance)
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.BJQTemplet = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion


        #region 
        public static string getBarcodeScale()
        {
            using (var db = SugarDao.Instance)
            {
                if (db.DbMaintenance.IsAnyTable("ContextEntity", false))
                {
                    ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                    if (contextEntity == null)
                    {
                        return "";
                    }
                    return contextEntity.barcodeScale;
                }
                else
                {
                    return "";
                }
            }
        }

        public static void setBarcodeScale(string v)
        {
            using (var db = SugarDao.Instance)
            {
                ContextEntity contextEntity = db.Queryable<ContextEntity>().First();
                if (contextEntity == null)
                {
                    contextEntity = new ContextEntity();
                }
                contextEntity.barcodeScale = v;
                DBUtils.Instance.DbManager.SaveOrUpdate(contextEntity);
            }
        }
        #endregion

    }
}
