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

        /// <summary>
        /// 离线登陆
        /// </summary>
        /// <param name="json"></param>
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

                        ContextCache.SetShopcode(user_info.shopcode);
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

        /// <summary>
        /// 保存或更新商品信息
        /// </summary>
        public ResposeEntity SaveOrUpdateCommodityInfo(string json)
        {
            ResposeEntity resposeEntity = new ResposeEntity();
            using (var db = SugarDao.GetInstance())
            {
                if (_LoginUserManager.Instance.Login)
                {
                    string shopcode = _LoginUserManager.Instance.UserEntity.shopcode;
                    string branchcode = _LoginUserManager.Instance.UserEntity.branchcode;
                    try
                    {
                        DbManager dbManager = DBUtils.Instance.DbManager;
                        CommodityInfoVM commodityInfoVM = JsonConvert.DeserializeObject<CommodityInfoVM>(json);
                        if(commodityInfoVM != null)
                        {
                            commodityInfoVM.shopcode = shopcode;
                            commodityInfoVM.branchcode = branchcode;

                            List<CategoryEntity> categoryEntities = commodityInfoVM.categorys;
                            List<CommodityEntity> commoditys = commodityInfoVM.commoditys;
                            List<MemberEntity> memberEntities = commodityInfoVM.memberlevels;
                            List<PayTypeEntity> paytypes = commodityInfoVM.paytypes;
                            List<AssistantsEntity> assistants = commodityInfoVM.assistants;
                            List<CashierEntity> users = commodityInfoVM.users;
                            List<SupplierEntity> suppliers = commodityInfoVM.suppliers;
                            // add: 2018/2/27
                            List<BarCodeEntity> barCodes = commodityInfoVM.barcodes;
                            List<CommodityPriceEntity> commodityPriceEntityList = commodityInfoVM.commoditypricelist;

                            #region 循环saveorupdate 效率很慢 TODO...: 这里应该改造成bulksaveorupdate提高效率
                            //保存商品分类信息
                            if (categoryEntities != null)
                            {
                                foreach (CategoryEntity categoryEntity in categoryEntities)
                                {
                                    dbManager.SaveOrUpdate(categoryEntity);
                                }
                            }
                            //保存商品信息
                            if (commoditys != null)
                            {
                                foreach (CommodityEntity commodityEntity in commoditys)
                                {
                                    dbManager.SaveOrUpdate(commodityEntity);
                                }
                            }
                            //保存会员等级信息
                            if (memberEntities != null)
                            {
                                foreach (MemberEntity memberEntity in memberEntities)
                                {
                                    dbManager.SaveOrUpdate(memberEntity);
                                }
                            }
                            //保存付款方式信息
                            if (paytypes != null)
                            {
                                foreach (PayTypeEntity payTypeEntity in paytypes)
                                {
                                    dbManager.SaveOrUpdate(payTypeEntity);
                                }
                            }
                            //保存收银员信息
                            if (assistants != null)
                            {
                                foreach (AssistantsEntity assistantsEntity in assistants)
                                {
                                    dbManager.SaveOrUpdate(assistantsEntity);
                                }
                            }
                            //保存收银员信息
                            if (users != null)
                            {
                                foreach (CashierEntity cashierEntity in users)
                                {
                                    dbManager.SaveOrUpdate(cashierEntity);
                                }
                            }
                            //保存供应商信息
                            if (suppliers != null)
                            {
                                foreach (SupplierEntity supplierEntity in suppliers)
                                {
                                    dbManager.SaveOrUpdate(supplierEntity);
                                }
                            }
                            // add: 2018/2/27
                            //保存条码表信息
                            if (barCodes != null)
                            {
                                foreach (BarCodeEntity barCodeEntity in barCodes)
                                {
                                    //由shopcode+commoditycode做联合主键,防止跨商户商品数据的commoditycode相同
                                    barCodeEntity.uid = shopcode + "_" + barCodeEntity.commoditycode;
                                    barCodeEntity.shopcode = shopcode;
                                    dbManager.SaveOrUpdate(barCodeEntity);
                                }
                            }
                            //保存调价表信息
                            if (commodityPriceEntityList != null)
                            {
                                foreach (CommodityPriceEntity commodityPriceEntity in commodityPriceEntityList)
                                {
                                    dbManager.SaveOrUpdate(commodityPriceEntity);
                                }
                            }
                            #endregion

                            dbManager.SaveOrUpdate(commodityInfoVM);
                            logger.Info("保存和更新商品信息接口：信息保存成功");
                            resposeEntity.Code = ResponseCode.SUCCESS;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Info("保存和更新商品信息接口：" + e.StackTrace);
                        resposeEntity.Code = ResponseCode.Failed;
                    }
                }
                else
                {
                    logger.Info("保存和更新商品信息接口：用户未登录");
                    resposeEntity.Code = ResponseCode.Failed;
                }
                logger.Info("数据保存成功");
                return resposeEntity;
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
