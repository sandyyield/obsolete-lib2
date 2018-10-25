using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ZlPos.Bizlogic
{
    class PropertiesBizlogic
    {
        public static FieldInfo[] GetFieldInfos(Type type)
        {
            Type[] types = new Type[0];
            ConstructorInfo ci = type.GetConstructor(types);
            //获取public字段
            FieldInfo[] fis = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            return fis;
        }
    }
}
