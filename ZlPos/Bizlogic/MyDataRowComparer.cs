using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

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
}
