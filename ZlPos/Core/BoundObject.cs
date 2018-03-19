using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZlPos.Core
{
    public class BoundObject
    {
        private ChromiumBrowser chromiumBrowser;

        public string MyProperty { get; set; }
        public void MyMethod()
        {
            // Do something really cool here.
            MessageBox.Show("here is c#");
        }

        public BoundObject(object chromiumBrowser) {
            this.chromiumBrowser = chromiumBrowser as ChromiumBrowser;
        }

        public string MyMethod1(object state)
        {
            //Thread.Sleep(5000);
            return "here is c# solution string>>>>>>>[" + state.ToString() + "]";
        }

        //这个异步回调是有问题的
        public void TestCallback(IJavascriptCallback javascriptCallback)
        {
            const int taskDelay = 1500;

            //Task.Run(async () =>
            //{
            //    await Task.Delay(taskDelay);

            //    using (javascriptCallback)
            //    {
            //        //NOTE: Classes are not supported, simple structs are
            //        var response = new CallbackResponseStruct("This callback from C# was delayed " + taskDelay + "ms");
            //        await javascriptCallback.ExecuteAsync(response);
            //    }
            //});
        }

        public void TestCallback2(IJavascriptCallback javascriptCallback)
        {
            //Thread.Sleep(5000);

            using (javascriptCallback)
            {
                var respose = new CallbackResponseStruct("TestCallback2");


                //TOFIX :  callbakc这里还有点问题 需要调试下
                javascriptCallback.ExecuteAsync(respose);
            }
        }
    }

}
