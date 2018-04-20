using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Listener
{
    public class SubPubComponet : ISubscribe, IPublish
    {
        private string _subName;
        public SubPubComponet(string subName)
        {
            this._subName = subName;
            PublishEvent += new PublishHandle(Notify);
        }

        #region ISubscribe Members
        event SubscribeHandle subscribeEvent;
        event SubscribeHandle ISubscribe.SubscribeEvent
        {
            add { subscribeEvent += value; }
            remove { subscribeEvent -= value; }
        }
        #endregion 

        #region IPublish Members
        public PublishHandle PublishEvent;

        event PublishHandle IPublish.PublishEvent
        {
            add { PublishEvent += value; }
            remove { PublishEvent -= value; }
        }
        #endregion 

        public void Notify(string str)
        {
            if (subscribeEvent != null)
                subscribeEvent.Invoke(string.Format("消息来源{0}:消息内容:{1}", _subName, str));
        }
    }
}

