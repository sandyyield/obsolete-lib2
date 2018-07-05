using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Core
{
    public class DataProcessor
    {
        private static DataProcessor _dataProcessor = null;

        private DataProcessor()
        {

        }

        public static DataProcessor Instance
        {
            get
            {
                if (_dataProcessor == null)
                {
                    _dataProcessor = new DataProcessor();
                }
                return _dataProcessor;
            }
        }


        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lst"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns>直接返回分页json串  如无数据则返回""</returns>
        public object PaginationData<T>(List<T> lst,int pageindex,int pagesize)
            where T : class, new()
        {
            List<T> selectList = null;
            if (lst != null && lst.Count > 0 && pageindex != -1)
            {
                if (lst.Count > (pageindex * pagesize + pagesize))
                {
                    selectList = lst.GetRange(pageindex * pagesize, pagesize);
                }
                else
                {
                    if (lst.Count - pageindex * pagesize >= 0)
                    {
                        if (lst.Count <= (pageindex * pagesize + pageindex * pagesize + pagesize))
                        {
                            selectList = lst.GetRange(pageindex * pagesize, lst.Count - pageindex * pagesize);
                        }

                    }
                }
            }
            else
            {
                selectList = lst;
            }
            if (selectList == null)
            {
                selectList = new List<T>();
            }
            return selectList;
            //return JsonConvert.SerializeObject(selectList);
        }

    }
}
