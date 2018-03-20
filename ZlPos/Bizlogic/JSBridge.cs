using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CefSharp;
using CefSharp.WinForms;
using Newtonsoft.Json;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Dao;
using ZlPos.Models;

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// create by sVen 2018年3月15日： method invoke class
    /// </summary>
    class JSBridge
    {
        private ChromiumWebBrowser browser;

        static JSBridge instance = null;

        public static JSBridge Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JSBridge();
                }
                return instance;
            }
        }

        private JSBridge()
        {

        }

        public ChromiumWebBrowser Browser { get => browser; set => browser = value; }


        /// <summary>
        /// native调用js 示例
        /// </summary>
        public void ExecuteScriptAsync()
        {
            browser.ExecuteScriptAsync("printInvokeJSMethod('hello world')");
        }


        /// <summary>
        /// js调用 native method 示例
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public string NativeUnityMethod(object state)
        {
            return "here is c# solution string>>>>>>>[" + state.ToString() + "]";
        }


        public string GetDeviceId()
        {
            return new Guid().ToString();
        }

        public string GetNetWorkStatus()
        {
            return "json";
        }


        public void Login(string json)
        {
            ResposeEntity resposeEntity = new ResposeEntity();
            if (string.IsNullOrEmpty(json))
            {
                resposeEntity.Code = ResponseCode.Failed;
                resposeEntity.Msg = "参数不能为空";

                //TODO这里考虑开个线程池去操作
                ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", resposeEntity });
            }
            else
            {
                //TODO：wait orm架构定下来再说
                //using (var db = SugarDao.GetInstance())
                {
                    try
                    {
                        LoginEntity loginEntity = JsonConvert.DeserializeObject<LoginEntity>(json);
                        //只是为了调试加的
                        ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", resposeEntity });
                        System.Windows.Forms.MessageBox.Show("called loginCallBack js method!!!");
                    }
                    catch (Exception e)
                    {

                    }

                }

            }
        }



        public void TestORM(string json)
        {
            Employee employees = JsonConvert.DeserializeObject<Employee>(json);

            try
            {
                using (var context = new DatabaseContext())
                {
                    context.Database.CreateIfNotExists();
                    var empList = context.Employees.OrderBy(c => c.FirstName).ToList();
                }
            }
            catch(Exception ex)
            {

            }

            //System.Windows.Forms.MessageBox.Show("ok");
            return;
        }



        private void CallbackMethod(object state)
        {

            object[] paramsArr = (object[])state;
            //first params is method name  
            string methodName = paramsArr[0] as string;

            //real params tofix
            ResposeEntity resposeEntity = paramsArr[1] as ResposeEntity;

            //模拟耗时操作
            Thread.Sleep(5000);


            browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(resposeEntity) + "')");
        }
    }
}
