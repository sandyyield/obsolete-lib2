using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Listener
{
    //定义订阅事件
    public delegate void SubscribeHandle(string str);
    //定义订阅接口
    public interface ISubscribe
    {
        event SubscribeHandle SubscribeEvent;
    }
}
