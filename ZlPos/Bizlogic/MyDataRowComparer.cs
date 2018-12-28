using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using ZlPos.Models;

namespace ZlPos.Bizlogic
{
    public class MyDataRowComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow x, DataRow y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            else
                return x["id"].Equals(y["id"]);
        }

        public int GetHashCode(DataRow obj)
        {
            return obj.GetHashCode();
        }
    }

    public class SKUComparer : IEqualityComparer<SKUEntity>
    {
        public bool Equals(SKUEntity x, SKUEntity y)
        {
            return x.uid.Equals(y.uid);
        }

        public int GetHashCode(SKUEntity obj)
        {
            return obj.GetHashCode();
        }
    }

    public class SPUComparer : IEqualityComparer<SPUEntity>
    {
        public bool Equals(SPUEntity x, SPUEntity y)
        {
            return x.uid.Equals(y.uid);
        }

        public int GetHashCode(SPUEntity obj)
        {
            return obj.GetHashCode();
        }
    }
}
