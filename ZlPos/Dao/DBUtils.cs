using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Dao
{
    class DBUtils
    {
        private static DBUtils dbUtils = null;

        private DbManager dbManager = null;

        private DBUtils()
        {
            if(null == DbManager)
            {
                DbManager = new DbManagerImpl();
            }

        }


        public static DBUtils Instance
        {
            get
            {
                if (dbUtils == null)
                {
                    dbUtils = new DBUtils();
                }
                return dbUtils;
            }
        }

        internal DbManager DbManager { get => dbManager; set => dbManager = value; }
    }
}
