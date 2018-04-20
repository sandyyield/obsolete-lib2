using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Listener
{
    public class Publisher : IPublish
    {
        private string _publisherName;

        public Publisher(string publisherName)
        {
            this._publisherName = publisherName;
        }

        private event PublishHandle PublishEvent;
        event PublishHandle IPublish.PublishEvent
        {
            add { PublishEvent += value; }
            remove { PublishEvent -= value; }
        }

        public void Notify(string str)
        {
            if (PublishEvent != null)
                PublishEvent.Invoke(string.Format("我是{0},我发布{1}消息", _publisherName, str));
        }
    }
}
