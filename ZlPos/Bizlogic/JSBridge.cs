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
using System.Globalization;
using ZlPos.Utils;
using System.Collections;
using System.IO.Ports;
using ZlPos.PrintServices;
using System.Reflection;
using SqlSugar;
using System.Data.Entity.Infrastructure;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System.Threading.Tasks;
using ZlPos.Enums;
using ZlPos.Core;
using ZlPos.Forms;
using System.Windows.Forms;

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// create by sVen 2018年3月15日： method invoke class
    /// </summary>
    class JSBridge
    {
        //private static ILog logger = null;
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static ChromiumWebBrowser browser;

        public ChromiumWebBrowser _SecondScreenWebView { get; set; }

        private SecondScreenFrm SecondScreen;

        private static LoginUserManager _LoginUserManager = LoginUserManager.Instance;

        static JSBridge instance = null;

        public delegate void JsCallbackHandle(object state);

        //内存镜像
        PrinterConfigEntity _printerConfigEntity;

        /// <summary>
        /// 委托方式托管回调
        /// </summary>
        /// <param name="methodName">js方法名</param>
        /// <param name="responseEntity">回调参数</param>
        public delegate void WebViewHandle(string methodName, ResponseEntity responseEntity);

        public static WebViewHandle mWebViewHandle;

        public static JSBridge Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new JSBridge();
                    mWebViewHandle = new WebViewHandle(AsyncCallbackMethod);
                }
                return instance;
            }
        }

        private JSBridge()
        {

        }

        public ChromiumWebBrowser Browser { get => browser; set => browser = value; }
        //public void DownLoadFile { get => this.downLoadFile; set => this.downLoadFile = value; }


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
            if (RegHelper.GetKey2() == "")
            {
                RegHelper.CreateAndSaveNewKey2();
            }
            return RegHelper.GetKey2();
        }

        public string GetVersionInfo()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        public string GetNetWorkStatus()
        {
            return "json";
        }

        public void SetZoomLevel(double ZoomLevel)
        {
            browser.SetZoomLevel(ZoomLevel);
        }

        public void OpenSecondScreen()
        {
            Screen[] sc;
            sc = Screen.AllScreens;
            if (sc.Length > 1)
            {
                if (SecondScreen == null)
                {
                    SecondScreen = new SecondScreenFrm(_SecondScreenWebView);
                }
                SecondScreen.Show();
            }
            //else
            //{
            //    //加载副屏但不现实
            //    if (SecondScreen == null)
            //    {
            //        SecondScreen = new SecondScreenFrm(_SecondScreenWebView);
            //    }
            //}

        }

        //null method debug
        #region SecondScreenAction
        public void SecondScreenAction(string p1, string p2)
        {
            Task.Factory.StartNew(() =>
            {
                if (_SecondScreenWebView != null)
                {
                    _SecondScreenWebView.ExecuteScriptAsync(p1 + "('" + p2 + "')");
                }
            });
        }
        #endregion

        //null method
        #region GetExceptionInfo
        public string GetExceptionInfo()
        {
            return "";
        }
        #endregion

        #region Login
        /// <summary>
        /// 离线登陆
        /// </summary>
        /// <param name="json"></param>
        public void Login(string json)
        {
            Task.Factory.StartNew(() =>
            {


                ResponseEntity responseEntity = new ResponseEntity();
                if (string.IsNullOrEmpty(json))
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "参数不能为空";

                    //TODO这里考虑开个线程池去操作
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", responseEntity });
                    mWebViewHandle.Invoke("loginCallBack", responseEntity);
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
                                    _LoginUserManager.Login = true;
                                    _LoginUserManager.UserEntity = userList[0];
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
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", responseEntity });
                        mWebViewHandle.Invoke("loginCallBack", responseEntity);
                        //System.Windows.Forms.MessageBox.Show("called loginCallBack js method!!!");
                    }
                    catch (Exception e)
                    {

                    }
                }
            });
        }
        #endregion

        #region SaveOrUpdateUserInfo
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
                    logger.Info("保存或更新用户信息,获取到的userVM：" + json);
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
                    _LoginUserManager.Login = true;
                    _LoginUserManager.UserEntity = user_info;

                    logger.Info("保存或更新用户信息接口：用户在线登陆并保存用户信息成功");

                }
                else
                {
                    logger.Info("存或更新用户信息接口：用户登录成功但用户信息保存失败");
                }
            }
            catch (Exception e)
            {
                logger.Info("保存或更新用户信息接口：保存数据库操作异常");
                //throw;
            }
        }
        #endregion

        #region SaveOrUpdateCommodityInfo
        /// <summary>
        /// 保存或更新商品信息
        /// </summary>
        public string SaveOrUpdateCommodityInfo(string json)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if (_LoginUserManager.Login)
            {
                string shopcode = _LoginUserManager.UserEntity.shopcode;
                string branchcode = _LoginUserManager.UserEntity.branchcode;
                try
                {
                    DbManager dbManager = DBUtils.Instance.DbManager;
                    CommodityInfoVM commodityInfoVM = JsonConvert.DeserializeObject<CommodityInfoVM>(json);
                    if (commodityInfoVM != null)
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
                        responseEntity.code = ResponseCode.SUCCESS;

                        //callback
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "saveOrUpdateCommodityInfoCallBack", responseEntity });
                        //减少线程开销
                        Task.Factory.StartNew(() =>
                        {
                            mWebViewHandle.Invoke("saveOrUpdateCommodityInfoCallBack", responseEntity);
                        });
                    }
                }
                catch (Exception e)
                {
                    logger.Info("保存和更新商品信息接口：" + e.StackTrace);
                    responseEntity.code = ResponseCode.Failed;
                }
            }
            else
            {
                logger.Info("保存和更新商品信息接口：用户未登录");
                responseEntity.code = ResponseCode.Failed;
            }
            logger.Info("数据保存成功");
            return JsonConvert.SerializeObject(responseEntity);

        }
        #endregion

        #region GetCommodityInfo
        /// <summary>
        /// 获取所有商品和分类信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityInfo()
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    List<CommodityEntity> commodityEntities = null;
                    List<CategoryEntity> categoryEntities = null;
                    List<MemberEntity> memberEntities = null;
                    List<PayTypeEntity> payTypeEntities = null;
                    List<AssistantsEntity> assistantsEntities = null;
                    List<CashierEntity> cashierEntities = null;
                    List<SupplierEntity> supplierEntities = null;
                    List<CommodityInfoVM> commodityInfoVMList = null;
                    List<BarCodeEntity> barCodes = null;
                    List<CommodityPriceEntity> commodityPriceList = null;

                    using (var db = SugarDao.GetInstance())
                    {
                        // TODO: 2017/11/3 按固定精确度返回，
                        //固定精确到shopcode
                        supplierEntities = db.Queryable<SupplierEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        payTypeEntities = db.Queryable<PayTypeEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        commodityInfoVMList = db.Queryable<CommodityInfoVM>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        categoryEntities = db.Queryable<CategoryEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.del == "0").OrderBy(it => it.categorycode).ToList();

                        // add: 2018/2/27
                        barCodes = db.Queryable<BarCodeEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        //固定精确到shopcode + branchcode
                        assistantsEntities = db.Queryable<AssistantsEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                                && it.branchcode == userEntity.branchcode).ToList();

                        cashierEntities = db.Queryable<CashierEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.branchcode == userEntity.branchcode).ToList();

                        commodityPriceList = db.Queryable<CommodityPriceEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                                    && it.branchcode == userEntity.branchcode).ToList();

                        //按 membermode 区分SSM和CSM
                        if ("CSM".Equals(userEntity.membermodel)) //跨店
                        {
                            memberEntities = db.Queryable<MemberEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();
                        }
                        else //单店
                        {
                            memberEntities = db.Queryable<MemberEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.branchcode == userEntity.branchcode).ToList();
                        }


                        CommodityInfoVM commodityInfoVM = new CommodityInfoVM();
                        if (commodityInfoVMList != null && commodityInfoVMList.Count > 0)
                        {
                            CommodityInfoVM infoVM = commodityInfoVMList[commodityInfoVMList.Count - 1];//如有多条，取最新的一条
                            commodityInfoVM = infoVM;
                        }
                        if (commodityEntities == null)
                        {
                            commodityEntities = new List<CommodityEntity>();
                        }
                        if (categoryEntities == null)
                        {
                            categoryEntities = new List<CategoryEntity>();
                        }
                        if (memberEntities == null)
                        {
                            memberEntities = new List<MemberEntity>();
                        }
                        if (payTypeEntities == null)
                        {
                            payTypeEntities = new List<PayTypeEntity>();
                        }
                        if (assistantsEntities == null)
                        {
                            assistantsEntities = new List<AssistantsEntity>();
                        }
                        if (cashierEntities == null)
                        {
                            cashierEntities = new List<CashierEntity>();
                        }
                        if (supplierEntities == null)
                        {
                            supplierEntities = new List<SupplierEntity>();
                        }
                        // add: 2018/2/27
                        if (barCodes == null)
                        {
                            barCodes = new List<BarCodeEntity>();
                        }
                        if (commodityPriceList == null)
                        {
                            commodityPriceList = new List<CommodityPriceEntity>();
                        }

                        commodityInfoVM.categorys = categoryEntities;
                        commodityInfoVM.commoditys = commodityEntities;
                        commodityInfoVM.memberlevels = memberEntities;
                        commodityInfoVM.paytypes = payTypeEntities;
                        commodityInfoVM.assistants = assistantsEntities;
                        commodityInfoVM.users = cashierEntities;
                        commodityInfoVM.suppliers = supplierEntities;
                        commodityInfoVM.barcodes = barCodes;
                        commodityInfoVM.commoditypricelist = commodityPriceList;
                        responseEntity.data = commodityInfoVM;
                        responseEntity.code = ResponseCode.SUCCESS;
                        responseEntity.msg = "获取所有商品信息成功";


                    }
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "数据异常";
                }
            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "用户未登陆";
            }

            //return responseEntity;
            return JsonConvert.SerializeObject(responseEntity);

        }
        #endregion

        #region GetLastRequestTime
        /// <summary>
        /// 获取最后更新时间
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetLastRequestTime()
        {
            String lastRequestTime = "";
            DbManager dbManager = DBUtils.Instance.DbManager;
            if (_LoginUserManager.Login)
            {
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        List<CommodityInfoVM> lastRequestTimeList = db.Queryable<CommodityInfoVM>()
                                                                    .Where(it => it.shopcode == _LoginUserManager.UserEntity.shopcode
                                                                    && it.branchcode == _LoginUserManager.UserEntity.branchcode).ToList();
                        if (lastRequestTimeList != null && lastRequestTimeList.Count > 0)
                        {
                            CommodityInfoVM commodityInfoVM = lastRequestTimeList[lastRequestTimeList.Count - 1];
                            if (commodityInfoVM != null)
                            {
                                lastRequestTime = commodityInfoVM.requesttime;
                            }
                        }

                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message + ">>>" + e.StackTrace);
                }
            }
            else
            {

            }

            return lastRequestTime;
        }
        #endregion

        #region GetLastUserName
        /// <summary>
        /// 获取最后一次登录shopcode
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetLastUserName()
        {
            string shopcode = "";
            ResponseEntity responseEntity = new ResponseEntity();
            try
            {
                shopcode = ContextCache.GetShopcode();

                responseEntity.code = ResponseCode.SUCCESS;
                responseEntity.data = shopcode;
                //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "getLastUserNameCallBack", responseEntity });
                Task.Factory.StartNew(() =>
                {
                    mWebViewHandle.Invoke("getLastUserNameCallBack", responseEntity);
                });
            }
            catch (Exception e)
            {
                responseEntity.code = ResponseCode.Failed;
                //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "getLastUserNameCallBack", responseEntity });
                Task.Factory.StartNew(() =>
                {
                    mWebViewHandle.Invoke("getLastUserNameCallBack", responseEntity);
                });
                logger.Error(e.Message + e.StackTrace);
            }
            return shopcode;
        }
        #endregion

        #region SaveOneSaleBill
        /// <summary>
        /// 保存销售单据接口
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public void SaveOneSaleBill(string json)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();

                if (string.IsNullOrEmpty(json))
                {
                    logger.Info("保存销售单据接口：空字符串");
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "参数不能为空";
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "saveOneSaleBillCallBack", responseEntity });
                    mWebViewHandle.Invoke("saveOneSaleBillCallBack", responseEntity);
                }
                DbManager dbManager = DBUtils.Instance.DbManager;

                BillEntity billEntity = JsonConvert.DeserializeObject<BillEntity>(json);
                if (billEntity == null)
                {
                    logger.Info("保存销售单据接口：json解析失败");
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "参数格式错误";
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "saveOneSaleBillCallBack", responseEntity });
                    mWebViewHandle.Invoke("saveOneSaleBillCallBack", responseEntity);
                }
                try
                {
                    DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
                    dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
                    DateTime insertDate = DateTime.MinValue;
                    try
                    {
                        insertDate = Convert.ToDateTime(billEntity.saletime, dtFormat);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message + e.StackTrace);
                    }
                    if (insertDate == DateTime.MinValue)
                    {
                        insertDate = DateTime.Now;
                    }
                    billEntity.insertTime = Utils.DateUtils.ConvertDataTimeToLong(insertDate);

                    dbManager.SaveOrUpdate(billEntity);
                }
                catch (Exception e)
                {
                    logger.Error("保存销售单据接口： 异常");
                }
                List<BillCommodityEntity> commoditys = billEntity.commoditys;
                List<PayDetailEntity> paydetails = billEntity.paydetails;
                if (commoditys == null || commoditys.Count == 0)
                {
                    logger.Info("保存销售单据接口：该单据没有商品信息");
                }
                else
                {
                    foreach (BillCommodityEntity billCommodityEntity in commoditys)
                    {
                        try
                        {
                            billCommodityEntity.uid = billCommodityEntity
                                    .ticketcode
                                    + "_"
                                    + billCommodityEntity.id;
                            dbManager.SaveOrUpdate(billCommodityEntity);
                        }
                        catch (Exception e)
                        {
                            logger.Error("保存销售单据接口：dbManager.saveOrUpdate(billCommodityEntity)--DbException");
                        }
                    }
                }

                if (paydetails == null || paydetails.Count == 0)
                {
                    logger.Info("保存销售单据接口：该单据没有付款方式信息");
                }
                else
                {
                    foreach (PayDetailEntity payDetailEntity in paydetails)
                    {
                        try
                        {
                            dbManager.SaveOrUpdate(payDetailEntity);
                        }
                        catch (Exception e)
                        {
                            logger.Error("保存销售单据接口： dbManager.saveOrUpdate(payDetailEntity)--DbException");
                        }
                    }
                }
                responseEntity.code = ResponseCode.SUCCESS;
                responseEntity.msg = "保存单据成功";
                //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "saveOneSaleBillCallBack", responseEntity });
                mWebViewHandle?.Invoke("saveOneSaleBillCallBack", responseEntity);
            });
            //return;
        }
        #endregion

        #region GetAllSaleBill
        /// <summary>
        /// 获取对应状态的单据信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetAllSaleBill(string state)
        {
            DbManager dbManager = DBUtils.Instance.DbManager;
            UserEntity userEntity = _LoginUserManager.UserEntity;
            string shopcode = userEntity.shopcode;
            string branchcode = userEntity.branchcode;

            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    List<BillEntity> billEntities = db.Queryable<BillEntity>().Where(it => it.ticketstatue == state
                                                                                && it.shopcode == shopcode
                                                                                && it.branchcode == branchcode).ToList();
                    if (billEntities != null)
                    {
                        logger.Info("获取对应状态的单据信息billEntities: " + billEntities.ToString());
                        for (int i = 0; i < billEntities.Count; i++)
                        {
                            List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(it => it.ticketcode == billEntities[i].ticketcode).ToList();
                            List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(it => it.ticketcode == billEntities[i].ticketcode).ToList();
                            List<DisCountDetailEntity> disCountDetailEntities = db.Queryable<DisCountDetailEntity>().Where(it => it.ticketcode == billEntities[i].ticketcode).ToList();

                            if (billCommodityEntities == null)
                            {
                                billCommodityEntities = new List<BillCommodityEntity>();
                            }
                            if (disCountDetailEntities == null)
                            {
                                disCountDetailEntities = new List<DisCountDetailEntity>();
                            }
                            if (payDetailEntities == null)
                            {
                                payDetailEntities = new List<PayDetailEntity>();
                            }
                            billEntities[i].commoditys = billCommodityEntities;
                            billEntities[i].paydetails = payDetailEntities;
                            billEntities[i].discountdetails = disCountDetailEntities;
                        }
                        return JsonConvert.SerializeObject(billEntities);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message + e.StackTrace);
            }
            //有问题直接返回null
            return "";
        }
        #endregion

        //时间相关的操作方法先放一放  等确认调用参数结构
        #region GetSelectTimeSaleBill
        /// <summary>
        /// 指定时间区间查询
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public string GetSelectTimeSaleBill(string start, string end)
        {
            string result = "";
            if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
            {
                return result;
            }
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            UserEntity userEntity = _LoginUserManager.UserEntity;
            string shopcode = userEntity.shopcode;
            string branchCode = userEntity.branchcode;
            try
            {
                DateTime startDate = Convert.ToDateTime(start, dtFormat);
                DateTime endDate = Convert.ToDateTime(end, dtFormat);
                long startDateTime = DateUtils.ConvertDataTimeToLong(startDate);
                long endDateTime = DateUtils.ConvertDataTimeToLong(endDate);
                DbManager dbManager = DBUtils.Instance.DbManager;
                if (startDate != null && endDate != null)
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        string id = _LoginUserManager.UserEntity.userid;
                        List<BillEntity> billEntities = db.Queryable<BillEntity>().Where(i => i.insertTime >= startDateTime
                                                                                        && i.cashierid == id
                                                                                        && i.insertTime <= endDateTime
                                                                                        && (i.ticketstatue == "cached" || i.ticketstatue == "updated")
                                                                                        && i.shopcode == shopcode
                                                                                        && i.branchcode == branchCode).ToList();
                        if (billEntities != null)
                        {
                            for (int i = 0; i < billEntities.Count; i++)
                            {
                                List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                if (billCommodityEntities == null)
                                {
                                    billCommodityEntities = new List<BillCommodityEntity>();
                                }
                                else
                                {

                                }
                                if (payDetailEntities == null)
                                {
                                    payDetailEntities = new List<PayDetailEntity>();
                                }
                                billEntities[i].commoditys = billCommodityEntities;
                                billEntities[i].paydetails = payDetailEntities;
                            }
                            result = JsonConvert.SerializeObject(billEntities);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.Info(e.Message + e.StackTrace);
            }

            return result;
        }
        #endregion

        #region UpdateSaleBill
        /// <summary>
        /// 更新销售单据状态接口
        /// </summary>
        /// <param name="json"></param>
        public void UpdateSaleBill(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return;
            }
            DbManager dbManager = DBUtils.Instance.DbManager;

            List<string> ticketcodeList = JsonConvert.DeserializeObject<List<string>>(json);
            if (ticketcodeList != null)
            {
                using (var db = SugarDao.GetInstance())
                {
                    foreach (string ticketcode in ticketcodeList)
                    {
                        try
                        {
                            BillEntity billEntity = db.Queryable<BillEntity>().Where(it => it.ticketcode == ticketcode).First();
                            if (billEntity != null)
                            {
                                billEntity.ticketstatue = "updated";
                                dbManager.SaveOrUpdate(billEntity);
                                logger.Info("更新销售单据状态接口:成功");
                            }

                        }
                        catch (Exception e)
                        {
                            logger.Error("更新销售单据状态接口:异常");
                        }
                    }
                }
            }
        }
        #endregion

        #region DeleteHangUpSaleBill
        /// <summary>
        /// 删除单个挂单
        /// </summary>
        /// <param name="ticketcode"></param>
        public void DeleteHangUpSaleBill(string ticketcode)
        {
            if (string.IsNullOrEmpty(ticketcode))
            {
                return;
            }
            DbManager dbManager = DBUtils.Instance.DbManager;
            try
            {
                using (var db = SugarDao.GetInstance())
                {

                    BillEntity billEntity = db.Queryable<BillEntity>().Where(it => it.ticketcode == ticketcode).First();
                    if (billEntity != null)
                    {
                        List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(it => it.ticketcode == ticketcode).ToList();
                        List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(it => it.ticketcode == ticketcode).ToList();

                        if (payDetailEntities != null)
                        {
                            foreach (PayDetailEntity payDetailEntity in payDetailEntities)
                            {
                                dbManager.Delete(payDetailEntity);
                            }
                        }
                        dbManager.Delete(billEntity);
                        logger.Info("删除单个挂单接口  成功");

                    }
                }

            }
            catch (Exception e)
            {
                logger.Info("删除单个挂单接口  数据库异常");
            }
        }
        #endregion

        //目前可以不用考虑开发这个功能  直接给个提示框完事  后面再来做这个事情
        #region DeleteSaleBill
        /// <summary>
        /// 删除90天之前所有单据
        /// </summary>
        public void DeleteSaleBill()
        {
            //System.Windows.Forms.MessageBox.Show("（伪）删除成功");
        }
        #endregion

        #region GetHangUpNumber
        /// <summary>
        /// 获取挂单数量
        /// </summary>
        /// <returns></returns>
        public long GetHangUpNumber()
        {
            long number = 0;
            using (var db = SugarDao.GetInstance())
            {
                try
                {

                    number = db.Queryable<BillEntity>().Where(it => it.ticketstatue == "hangup"
                                                        && it.shopcode == _LoginUserManager.UserEntity.shopcode
                                                        && it.branchcode == _LoginUserManager.UserEntity.branchcode).ToList().Count;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message + e.StackTrace);
                }

            }
            return number;
        }
        #endregion

        #region GetCompleteNumber
        public long GetCompleteNumber(string start, string end)
        {
            long number = 0;
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(end))
            {
                logger.Info("按时间区间查询单据，开始时间或结束时间为空：start:" + start + "end:" + end);
                return number;
            }
            DateTimeFormatInfo dtFormat = new DateTimeFormatInfo();
            dtFormat.ShortDatePattern = "yyyy-MM-dd HH:mm:ss";
            //DateTime insertDate = DateTime.MinValue;
            using (var db = SugarDao.GetInstance())
            {
                try
                {
                    DateTime startDate = Convert.ToDateTime(start, dtFormat);
                    DateTime endDate = Convert.ToDateTime(end, dtFormat);
                    long startDateTime = DateUtils.ConvertDataTimeToLong(startDate);
                    long endDateTime = DateUtils.ConvertDataTimeToLong(endDate);
                    number = db.Queryable<BillEntity>().Where(it => it.ticketstatue == "cached"
                                                        || it.ticketstatue == "updated"
                                                        && it.insertTime >= startDateTime
                                                        && it.insertTime <= endDateTime
                                                        && it.shopcode == _LoginUserManager.UserEntity.shopcode
                                                        && it.branchcode == _LoginUserManager.UserEntity.branchcode).Count();
                }
                catch (Exception e)
                {
                    logger.Error(e.Message + e.StackTrace);
                }

            }
            number++;
            return number;
        }
        #endregion


        #region GetCommodityById
        /// <summary>
        /// 根据商品id查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityById(string id)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.commoditystatus == "0"
                                                                                    && i.del == "0"
                                                                                    && i.id == id).ToList();
                    }
                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
            }

            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetCommodityByBarcode
        /// <summary>
        /// 根据商品barcode查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityByBarcode(string barcode)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        BarCodeEntity barCodeEntity = db.Queryable<BarCodeEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                            && i.barcodes.Contains(barcode)).First();
                        if (barCodeEntity != null)
                        {
                            commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                        && i.commoditystatus == "0"
                                                                                        && i.del == "0"
                                                                                        && i.commoditycode == barCodeEntity.commoditycode).ToList();
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
            }

            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetCommodityByCommoditycode
        /// <summary>
        /// 根据商品code,查询商品
        /// </summary>
        /// <param name="commoditycode"></param>
        /// <returns></returns>
        public string GetCommodityByCommoditycode(string commoditycode)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.commoditystatus == "0"
                                                                                    && i.del == "0"
                                                                                    && i.commoditycode == commoditycode).ToList();

                    }
                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
            }

            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetCommodityByCategoryCode
        /// <summary>
        /// 根据商品categoryCode查询商品,barcode倒叙，然后再分页
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityByCategoryCode(String categoryCode, int pageindex, int pagesize)
        {
            logger.Info("getCommodityByCategory\n" + "categoryCode:" + categoryCode + "\tpageindex:" + pageindex + "\tpagesize:" + pagesize);
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            CommodityCacheManager commodityCacheManager = CommodityCacheManager.Instance;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                if (commodityCacheManager != null
                        && commodityCacheManager.CommodityMap != null
                        && !(commodityCacheManager.CommodityMap.Count == 0)
                        && commodityCacheManager.CommodityMap.ContainsKey(categoryCode))
                {
                    commodityEntities = commodityCacheManager.CommodityMap[categoryCode];
                }
                else
                {
                    UserEntity userEntity = _LoginUserManager.UserEntity;
                    try
                    {
                        using (var db = SugarDao.GetInstance())
                        {
                            commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.commoditystatus == "0"
                                                                                    && i.del == "0"
                                                                                    && i.categorycode == categoryCode)
                                                                                    .OrderBy(i => i.categorycode, SqlSugar.OrderByType.Desc)
                                                                                    .ToList();
                        }

                    }
                    catch (Exception db)
                    {
                        logger.Error(db.StackTrace);
                    }
                    if (commodityCacheManager.CommodityMap != null)
                    {
                        commodityCacheManager.CommodityMap.Add(categoryCode, commodityEntities);
                    }
                }
            }
            List<CommodityEntity> selectCommodityList = null;
            if (commodityEntities != null && commodityEntities.Count > 0 && pageindex != -1)
            {
                if (commodityEntities.Count > (pageindex * pagesize + pagesize))
                {
                    selectCommodityList = commodityEntities.GetRange(pageindex * pagesize, pageindex * pagesize + pagesize);
                }
                else
                {
                    if (pageindex * pagesize < commodityEntities.Count)
                    {
                        selectCommodityList = commodityEntities.GetRange(pageindex * pagesize, commodityEntities.Count);
                    }
                }
            }
            else
            {
                selectCommodityList = commodityEntities;
            }
            if (selectCommodityList == null)
            {
                selectCommodityList = new List<CommodityEntity>();
            }
            return JsonConvert.SerializeObject(selectCommodityList);

        }
        #endregion

        #region GetCommodityByMnemonic
        /// <summary>
        /// 根据商品mnemonic不区分大小写模糊查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityByMnemonic(string mnemonic)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        commodityEntities = db.Queryable<CommodityEntity>()
                                                .Where(i => i.shopcode == userEntity.shopcode
                                                && i.commoditystatus == "0"
                                                && i.del == "0"
                                                && i.mnemonic.Contains(mnemonic)).ToList();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
            }
            if (commodityEntities.Count > 100)
            {
                commodityEntities = commodityEntities.GetRange(0, 100);
            }
            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetCommodityByKeyword
        /// <summary>
        /// 根据商品名称,商品条码,助记码（大小写兼容）查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityByKeyword(string keyword)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                            && i.commoditystatus == "0"
                                                                            && i.del == "0"
                                                                            && (i.commodityname.Contains("keyword")
                                                                                || i.mnemonic.Contains("keyword"))).ToList();
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
            }
            if (commodityEntities.Count > 50)
            {
                commodityEntities = commodityEntities.GetRange(0, 50);
            }

            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetPayPriority
        /// <summary>
        /// 获取支付优先级
        /// </summary>
        /// <returns></returns>
        public string GetPayPriority()
        {
            DbManager dbManager = DBUtils.Instance.DbManager;
            string payList = "";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    var paycodeModelList = db.Queryable<PayDetailEntity>().GroupBy(i => i.paycode).ToList();
                    List<string> payCodeList = new List<string>();
                    foreach (var item in paycodeModelList)
                    {
                        string paycode = item.paycode;
                        payCodeList.Add(paycode);

                    }
                    //    for (DbModel dbModel : paycodeModelList) {
                    //        String paycode = dbModel.getDataMap().get("paycode");
                    //payCodeList.add(paycode);
                    //    }
                    payList = JsonConvert.SerializeObject(payCodeList);
                }
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }
            return payList;
        }
        #endregion

        public void saveJSExceotion(string s)
        {
            //TODO...
            return;
        }

        //public void SetSecondScreenWebView()


        //IO 操作相关 ... 
        #region GetPort
        /// <summary>
        /// 获取本机所有串口端口号 win专用
        /// </summary>
        /// <returns></returns>
        public string GetPort()
        {
            string[] arrSerial = SerialPort.GetPortNames();

            return JsonConvert.SerializeObject(arrSerial);
        }
        #endregion

        #region GetUsbDevices
        public void GetUsbDevices()
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                responseEntity.code = ResponseCode.SUCCESS;
                List<string> devices = new List<string> { "usb" };
                DeviceEntity deviceEntity = new DeviceEntity();
                deviceEntity.devices = devices;
                responseEntity.data = deviceEntity;
                mWebViewHandle.Invoke("getUsbDevicesCallBack", responseEntity);
            });
            return;
        }
        #endregion

        #region GetGPrinter
        /// <summary>
        /// 获取标签打印机
        /// </summary>
        /// <returns></returns>
        public string GetGPrinter()
        {
            return CacheManager.GetGprint() as string;
        }
        #endregion

        #region SetGprinter (bug)
        /// <summary>
        /// 设置佳博打印机
        /// </summary>
        /// <param name="json"></param>
        public void SetGprinter(string json)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PrinterConfigEntity printerConfigEntity = JsonConvert.DeserializeObject<PrinterConfigEntity>(json);
                    GPrinterSetter printerSetter = new GPrinterSetter();
                    printerSetter.setPrinter(printerConfigEntity,
                                                p: (result) =>
                                                {
                                                    mWebViewHandle.Invoke("setGPrinterCallBack", result);
                                                    //保存缓存
                                                    if (result.code == ResponseCode.SUCCESS)
                                                    {
                                                        CacheManager.InsertGprint(json);
                                                        if (GPrinterManager.Instance.Init)
                                                        {
                                                            switch (GPrinterManager.Instance.PrinterTypeEnum)
                                                            {
                                                                case "usb":
                                                                    GPrinterUtils.Instance.printUSBTest();
                                                                    break;
                                                                case "port":
                                                                    break;
                                                                case "bluetooth":
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                });
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "设置打印机异常";
                    mWebViewHandle.Invoke("setGPrinterCallBack", responseEntity);
                    logger.Error(e.Message + e.StackTrace);
                }
            });
        }
        #endregion

        #region GetGPrintUsbDevices (bug)
        public void GetGPrintUsbDevices()
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                responseEntity.code = ResponseCode.Failed;
                //ThreadPool.QueueUserWorkItem(CallbackMethod, new object[] { "getGPrintUsbDevicesCallBack", responseEntity });
                mWebViewHandle.Invoke("getGPrintUsbDevicesCallBack", responseEntity);
            });
            return;

        }
        #endregion

        #region print debug code
        public void TestUsbPrint()
        {
            //TODO...
        }

        public void TestBlueTooth()
        {
            //TODO...
        }

        public void TestPortPrint()
        {
            //TODO...
        }
        #endregion

        #region StartBluetoothDeviceInquiry (null method）
        [Obsolete]
        public ResponseEntity StartBluetoothDeviceInquiry()
        {
            //TODO...
            return null;
        }
        #endregion

        #region StartBluetoothSearch (null method)
        public ResponseEntity StartBluetoothSearch()
        {
            //TODO...
            return null;
        }
        #endregion

        #region ReturnBluetoothSearch (null method)
        public ResponseEntity ReturnBluetoothSearch()
        {
            //TODO...
            return null;
        }
        #endregion

        #region GetBluetoothDevices
        public void GetBluetoothDevices()
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    BluetoothRadio BuleRadio = BluetoothRadio.PrimaryRadio;
                    BuleRadio.Mode = RadioMode.Connectable;

                    BluetoothClient Blueclient = new BluetoothClient();
                    Dictionary<string, BluetoothAddress> deviceAddresses = new Dictionary<string, BluetoothAddress>();

                    BluetoothDeviceInfo[] Devices = Blueclient.DiscoverDevices();
                    //List<BluetoothDeviceInfo> bluetoothDeviceInfos = new List<BluetoothDeviceInfo>(Devices);
                    List<string> deviceNames = new List<string>();
                    DeviceEntity deviceEntity = new DeviceEntity();
                    foreach (BluetoothDeviceInfo device in Devices)
                    {
                        deviceNames.Add(device.DeviceName);
                    }
                    deviceEntity.devices = deviceNames;
                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.data = deviceEntity;
                    mWebViewHandle?.Invoke("getBluetoothDevicesCallBack", responseEntity);
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    mWebViewHandle?.Invoke("getBluetoothDevicesCallBack", responseEntity);
                }
            });

            return;
        }
        #endregion

        #region GetComSystemDevices (null method）
        /// <summary>
        /// 获取到的是串行总线上(物理或逻辑上)的设备
        /// </summary>
        /// <returns></returns>
        public string[] GetComSystemDevices()
        {
            //TODO...
            return null;
        }
        #endregion

        #region GetPrinter
        /// <summary>
        /// 获取上次保存的打印机设置
        /// </summary>
        /// <returns></returns>
        public string GetPrinter()
        {
            string config = "";
            try
            {
                using (var db = SugarDao.GetInstance())
                {
                    PrinterConfigEntity printerConfigEntity = db.Queryable<PrinterConfigEntity>().First();
                    if (printerConfigEntity != null)
                    {
                        config = JsonConvert.SerializeObject(printerConfigEntity);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.StackTrace);
            }
            return config;
        }
        #endregion

        #region SetPrinter
        public void SetPrinter(string json)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PrinterConfigEntity printerConfigEntity = JsonConvert.DeserializeObject<PrinterConfigEntity>(json);
                    //PrinterSetter printerSetter = new PrinterSetter(mContext);
                    try
                    {
                        PrinterSetter printerSetter = new PrinterSetter();
                        //委托js回调方法
                        //JsCallbackHandle jsCallbackHandle = new JsCallbackHandle(CallbackMethod4SetPrinter);
                        _printerConfigEntity = printerConfigEntity;
                        //Action<object> t = new Action<object>(CallbackMethod4SetPrinter);
                        //Action<PrinterConfigEntity, Action<object>> action = new Action<PrinterConfigEntity, Action<object>>(printerConfigEntity,t);
                        //Task.Factory.StartNew()
                        printerSetter.SetPrinter(printerConfigEntity, CallbackMethod4SetPrinter);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.StackTrace);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                }
            });

            return;
        }
        #endregion

        #region SetNote (null mehtod)
        public void SetNote(string json)
        {
            //TODO...
        }
        #endregion

        #region PrintLabel (null mehtod)
        /// <summary>
        /// 标签打印
        /// </summary>
        /// <param name="json"></param>
        public void PrintLabel(string json)
        {
            //TODO...
            return;
        }
        #endregion

        #region Print
        /// <summary>
        /// 小票打印接口
        /// </summary>
        public void Print(string s)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                try
                {
                    BillEntity billEntity = JsonConvert.DeserializeObject<BillEntity>(s);
                    if (PrinterManager.Instance.Init)
                    {
                        switch (PrinterManager.Instance.PrinterTypeEnum)
                        {
                            case Enums.PrinterTypeEnum.usb:
                                break;
                            case Enums.PrinterTypeEnum.bluetooth:
                                break;
                            case Enums.PrinterTypeEnum.port:
                                break;
                            default:
                                responseEntity.code = ResponseCode.Failed;
                                responseEntity.msg = "非法打印机类型";
                                break;
                        }
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "打印机未设置，请设置打印机";
                        //SToastUtil.toast(mWebView.getContext(), "打印机未设置，请设置打印机");
                    }



                }
                catch (Exception e)
                {

                }
            });
            return;
        }
        #endregion

        #region Print2
        /// <summary>
        /// 模板打印
        /// </summary>
        public void Print2(string content)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        if (PrinterManager.Instance.Init)
                        {
                            switch (PrinterManager.Instance.PrinterTypeEnum)
                            {
                                case Enums.PrinterTypeEnum.usb:
                                    USBPrinter usbPrinter = PrinterManager.Instance.UsbPrinter;
                                    PrintUtils.printModel(content, usbPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case Enums.PrinterTypeEnum.bluetooth:
                                    BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                                    PrintUtils.printModel(content, bluetoothPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case Enums.PrinterTypeEnum.port:
                                    serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                                    PrintUtils.printModel(content, portPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                default:
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "非法打印机类型";
                                    break;
                            }
                        }
                        else
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "打印机未设置，请设置打印机";
                        }
                    }
                    catch (Exception e)
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "打印出现异常";
                        logger.Error(e.Message + e.StackTrace);
                    }

                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "打印内容不能为空";
                }

                mWebViewHandle.Invoke("print2CallBack", responseEntity);

            });
            return;
        }
        #endregion

        #region PrintBill
        /// <summary>
        /// 对账打印
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public void PrintBill(string json)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                if (!string.IsNullOrEmpty(json))
                {
                    try
                    {
                        StatisticsVM statisticsVM = JsonConvert.DeserializeObject<StatisticsVM>(json);
                        if (PrinterManager.Instance.Init)
                        {
                            switch (PrinterManager.Instance.PrinterTypeEnum)
                            {
                                case PrinterTypeEnum.usb:
                                    USBPrinter usbPrinter = PrinterManager.Instance.UsbPrinter;
                                    PrintUtils.printNote(statisticsVM, usbPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.bluetooth:
                                    BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                                    PrintUtils.printNote(statisticsVM, bluetoothPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.port:
                                    serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                                    PrintUtils.printNote(statisticsVM, portPrinter);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                default:
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "非法打印机类型";
                                    break;
                            }
                        }
                        else
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "打印机未设置，请设置打印机";
                        }
                    }
                    catch (Exception e)
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "小票打印失败";
                        logger.Info(e.Message + e.StackTrace);
                    }
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "打印内容不能为空";
                }
                mWebViewHandle?.Invoke("printBillCallBack", responseEntity);
            });

        }
        #endregion

        #region OpenBox
        /// <summary>
        /// 开钱箱
        /// </summary>
        public bool OpenBox()
        {
            if (PrinterManager.Instance.Init)
            {
                switch (PrinterManager.Instance.PrinterTypeEnum)
                {
                    case PrinterTypeEnum.port:
                        serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                        switch (portPrinter.brand)
                        {
                            case "PD":
                                //portPrinter.PrintString("1B73009640");//开关钱箱代码
                                portPrinter.OpenCash("1B70008888"); //PD新固件开钱箱代码
                                break;
                            case "YK":
                                //                            portPrinter.Write("1B73009640", portPrinter.CMD);//开关钱箱代码
                                //portPrinter.PrintString("1B70001080");
                                portPrinter.OpenCash("1B70001080");
                                break;
                            case "FX":
                                //portPrinter.PrintString("1014010005");//开关钱箱代码
                                portPrinter.OpenCash("1014010005");
                                break;
                            default:
                                break;
                        }
                        return true;
                    case PrinterTypeEnum.usb:
                        USBPrinter usbPrinter = PrinterManager.Instance.UsbPrinter;
                        if (usbPrinter != null && usbPrinter.Init)
                        {
                            usbPrinter.openCash();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    case PrinterTypeEnum.bluetooth:
                        BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                        if (bluetoothPrinter != null && bluetoothPrinter.isConnected())
                        {
                            bluetoothPrinter.openCash();
                            return true;
                        }
                        else
                        {
                            return false;

                        }
                    default:
                        return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 保存客显设置
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public void SetCustomerShow(string json)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                responseEntity.code = ResponseCode.SUCCESS;
                responseEntity.msg = "windows版不需要设置";
                mWebViewHandle?.Invoke("setCustomerShowCallBack", responseEntity);
            });

        }

        /// <summary>
        /// 获取客显设置信息
        /// </summary>
        /// <returns></returns>
        public string GetCustomerShow()
        {
            var json = "{\"printerType\": \"\",             \"printernumber\": \"1\",             \"printerBrand\": \"\",             \"pageWidth\": \"small\",             \"deviceId\": \"\",             \"port\": \"\",             \"intBaud\": \"\"           }";
            return json;
        }

        /// <summary>
        /// 推送到客显信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public ResponseEntity CustomerShow(string json)
        {
            //TODO...
            return null;
        }

        public ResponseEntity SetReadCard2(string json)
        {
            //TODO...
            return null;
        }

        public ResponseEntity GetReadCard2()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取会员卡信息
        /// </summary>
        /// <returns></returns>
        public string GetCardNumber()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取电子秤
        /// </summary>
        /// <returns></returns>
        public string GetScale()
        {
            string scale = CacheManager.GetScale(SPCode.scale) as string;
            return scale;
        }

        /// <summary>
        /// 设置电子秤
        /// </summary>
        /// <returns></returns>
        public string SetScale(string json)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            responseEntity.code = ResponseCode.Failed;

            try
            {
                ScaleConfigEntity scaleConfigEntity = JsonConvert.DeserializeObject<ScaleConfigEntity>(json);
                if (!string.IsNullOrEmpty(scaleConfigEntity.port))
                {
                    WeightUtil.Instance.Open(scaleConfigEntity.port);
                    WeightUtil.Instance.Close();
                    responseEntity.code = ResponseCode.SUCCESS;
                    //缓存
                    CacheManager.InsertScale(SPCode.scale, json);

                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message + e.StackTrace);
            }

            return JsonConvert.SerializeObject(responseEntity);

        }
        /// <summary>
        /// 获取重量
        /// </summary>
        /// <returns></returns>
        public void GetWeight()
        {
            Task.Factory.StartNew(() =>
            {
                string scale = CacheManager.GetScale(SPCode.scale) as string;
                if (!string.IsNullOrEmpty(scale))
                {
                    try
                    {
                        ScaleConfigEntity scaleConfigEntity = JsonConvert.DeserializeObject<ScaleConfigEntity>(scale);
                        if (!string.IsNullOrEmpty(scaleConfigEntity.port))
                        {
                            WeightUtil.Instance.Open(scaleConfigEntity.port);
                            WeightUtil.Instance.Listener = (number) =>
                            {
                                browser.ExecuteScriptAsync("getWeightCallBack('" + number + "')");
                            };

                        }
                        else
                        {
                            browser.ExecuteScriptAsync("getWeightCallBack(" + "'" + "" + "'" + ")");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message + e.StackTrace);
                        browser.ExecuteScriptAsync("getWeightCallBack(" + "'" + "" + "'" + ")");
                    }
                }


            });

        }

        /// <summary>
        /// 关闭电子秤
        /// </summary>
        /// <returns></returns>
        public void closeWeighter()
        {
            WeightUtil.Instance.Close();
        }


        /// <summary>
        /// 获取数据库缓存数据大小
        /// </summary>
        /// <returns></returns>
        public long GetDBSize()
        {
            return 1l;
        }

        /// <summary>
        /// 获取磁盘空间大小
        /// </summary>
        /// <returns></returns>
        public long GetSDSize()
        {
            return 1l;
        }

        /// <summary>
        /// 获取磁盘可用空间大小
        /// </summary>
        /// <returns></returns>
        public long GetSDAvaliableSize()
        {
            return 1l;
        }

        //？？？这里解决一下
        ///// <summary>
        ///// 视频广告下载
        ///// </summary>
        //public void DownLoadFile()
        //{

        //}

        public void DownLoadFile2()
        {

        }




        public void TestUSBPrint()
        {
            USBPrinterService usbPrinterService = new USBPrinterService();
            usbPrinterService.TestPrint();
        }


        public string TestORM(string json)
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
                return "数据处理异常";
            }

            //System.Windows.Forms.MessageBox.Show("ok");
            return "数据处理成功";
        }


        /// <summary>
        /// 多线程方式回调
        /// </summary>
        /// <param name="state"></param>
        private void CallbackMethod(object state)
        {

            object[] paramsArr = (object[])state;
            string methodName = paramsArr[0] as string;
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;
            browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
        }

        /// <summary>
        /// 委托方式回调
        /// </summary>
        /// <param name="methodName">回调得js方法名</param>
        /// <param name="responseEntity">回调的数据包</param>
        private static void AsyncCallbackMethod(string methodName, ResponseEntity responseEntity)
        {
            browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
        }

        private void CallbackMethod4SetPrinter(object state)
        {
            object[] paramsArr = (object[])state;
            string methodName = paramsArr[0] as string;
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;
            browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
            if (responseEntity.code == ResponseCode.SUCCESS)
            {
                try
                {
                    using (var db = SugarDao.GetInstance())
                    {
                        //db.DbMaintenance.DropTable("PrinterConfigEntity");
                        //if (db.DbMaintenance.IsAnyTable(typeof(PrinterConfigEntity).Name))
                        //{
                        //    db.DbMaintenance.DropTable(typeof(PrinterConfigEntity).Name);
                        //}
                        DBUtils.Instance.DbManager.SaveOrUpdate(_printerConfigEntity);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.StackTrace);
                }
            }
        }


    }
}
