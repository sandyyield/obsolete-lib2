using System;
using System.Collections.Generic;
using System.Threading;
using CefSharp;
using CefSharp.WinForms;
using ZlPos.Dao;
using Newtonsoft.Json;
using ZlPos.Bean;
using ZlPos.Config;
using ZlPos.Models;
using ZlPos.Manager;
using log4net;

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// create by sVen 2018年3月15日： method invoke class
    /// </summary>
    class JSBridge
    {
        private static ILog logger = null;

        private ChromiumWebBrowser browser;

        private static LoginUserManager _LoginUserManager;

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
                try
                {
                    // 将H5传过来的用户输入信息进行解析，用于离线登录匹配
                    LoginEntity loginEntity = JsonConvert.DeserializeObject<LoginEntity>(json);
                    DbManager dbManager = DBUtils.Instance.DbManager;
                    if (loginEntity != null)
                    {
                        using (var db = SugarDao.GetInstance())
                        {
                            List<UserEntity> userList;
                            if (String.IsNullOrEmpty(loginEntity.account) || loginEntity.account == "0")
                            {
                                userList = db.Queryable<UserEntity>().Where(it => it.account == loginEntity.account
                                                                            && it.password == loginEntity.password).ToList();
                            }
                            else
                            {
                                userList = db.Queryable<UserEntity>().Where(it => it.username == loginEntity.username
                                                                            && it.shopcode == loginEntity.shopcode
                                                                            && it.password == loginEntity.password).ToList();
                            }
                            if (userList != null && userList.Count == 1)
                            {
                                _LoginUserManager.Instance.Login = true;
                                _LoginUserManager.Instance.UserEntity = userList[0];
                                UserVM userVM = new UserVM();
                                UserEntity userEntity = userList[0];
                                userVM.user_info = userEntity;
                                List<ShopConfigEntity> configEntities = db.Queryable<ShopConfigEntity>().Where(
                                                                        it => it.id == int.Parse(userEntity.shopcode) + int.Parse(userEntity.branchcode)).ToList();
                                //TODO...先去写saveOrUpadteUserInfo 再来完成这边login的逻辑

                            }
                        }
                    }
                    //只是为了调试加的
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", resposeEntity });
                    System.Windows.Forms.MessageBox.Show("called loginCallBack js method!!!");
                }
                catch (Exception e)
                {

                }
            }
        }

        /// <summary>
        /// 保存或更新用户信息
        /// </summary>
        /// <param name="json"></param>
        public void SaveOrUpdateUserInfo(string json)
        {
            try
            {
                DbManager dbManager = DBUtils.Instance.DbManager;
                UserVM userVM = JsonConvert.DeserializeObject<UserVM>(json);
                if (userVM != null)
                {
                    logger.Info("保存或更新用户信息,获取到的userVM：" + userVM.ToString());
                    ShopConfigEntity config = userVM.config;
                    UserEntity user_info = userVM.user_info;
                    if (config != null && user_info != null)
                    {
                        config.id = int.Parse(user_info.branchcode) + int.Parse(user_info.shopcode);
                        dbManager.SaveOrUpdate(config);
                    }
                    if (user_info != null)
                    {
                        dbManager.SaveOrUpdate(user_info);

                        //TODO... 2018年3月23日 这里需要设计一个缓存来存一下shopcode
                        //.... 这周已经周五了  双休日不加班~ 下周一写~
                    }
                    _LoginUserManager.Instance.Login = true;
                    _LoginUserManager.Instance.UserEntity = user_info;

                    logger.Info("保存或更新用户信息接口：用户在线登陆并保存用户信息成功");

                }
                else
                {
                    logger.Info("存或更新用户信息接口：用户登录成功但用户信息保存失败");
                }
            }
            catch (Exception)
            {
                logger.Info("保存或更新用户信息接口：保存数据库操作异常");
                //throw;
            }
        }



        public void TestORM(string json)
        {
            Employee employees = JsonConvert.DeserializeObject<Employee>(json);

            try
            {

                DbManager dbManager = DBUtils.Instance.DbManager;

                dbManager.SaveOrUpdate(employees);
                using (var db = SugarDao.GetInstance())
                {
                    //db.CodeFirst.BackupTable().InitTables(typeof(Employee));
                    //db.DbMaintenance.IsAnyTable(typeof(Employee).Name);
                    //db.CodeFirst.InitTables(typeof(Employee));

                    //db.Insertable(employees).ExecuteReturnEntity();


                    //db.BeginTran();

                    //db.IsEnableAttributeMapping = true;

                    //no table ex
                    //db.SqlBulkCopy(new List<Employee>(){ employees });

                    //db.InsertOrUpdate(employees);

                    //db.CommitTran();
                }
            }
            catch (Exception ex)
            {
                string s = ex.StackTrace;
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
