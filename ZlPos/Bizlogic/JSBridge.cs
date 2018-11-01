using System;
using System.Collections.Generic;
using System.Threading;
//using CefSharp;
//using CefSharp.WinForms;
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
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Linq;
using System.Drawing.Printing;

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// create by sVen 2018年3月15日： method invoke class
    /// </summary>
    class JSBridge
    {
        //private static ILog logger = null;
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //private static ChromiumWebBrowser browser;
        private static Control browser;

        //public ChromiumWebBrowser _SecondScreenWebView { get; set; }
        public Control _SecondScreenWebView { get; set; }

        private SecondScreenFrm SecondScreen;

        private static LoginUserManager _LoginUserManager = LoginUserManager.Instance;

        static JSBridge instance = null;

        //主窗体的指针
        public PosForm frmMain;

        public delegate void JsCallbackHandle(object state);

        //网络检查开关
        private bool _NetworkChecking = false;

        //网络状态
        private bool _NetworkStatus = false;

        //最近上次网络状态
        private bool? _LastNetworkStatus = null;

        //内存镜像
        PrinterConfigEntity _printerConfigEntity;

        //操作系统版本
        private string OSVer = "";

        private DataProcessor _DataProcessor = DataProcessor.Instance;

        //加载依赖程序集 
        static Assembly assemblyCefSharp = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.dll");
        static Assembly assemblyCefSharp_core = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.Core.dll");
        static Assembly assemblyCefSharp_WinForms = Assembly.LoadFrom(Application.StartupPath + "\\CefSharp.WinForms.dll");

        static Type t_WebBrowserExtensions;

        static MethodInfo ExecuteScriptAsyncMethod;


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
                    t_WebBrowserExtensions = assemblyCefSharp.GetType("CefSharp.WebBrowserExtensions");
                    ExecuteScriptAsyncMethod = t_WebBrowserExtensions.GetMethods().FirstOrDefault(m => m.Name == "ExecuteScriptAsync" && m.GetParameters().Length == 2);
                    instance = new JSBridge();
                    mWebViewHandle = new WebViewHandle(AsyncCallbackMethod);
                }
                return instance;
            }
        }

        private JSBridge()
        {

        }

        //public ChromiumWebBrowser Browser { get => browser; set => browser = value; }
        public Control Browser { get => browser; set => browser = value; }
        //public void DownLoadFile { get => this.downLoadFile; set => this.downLoadFile = value; }


        /// <summary>
        /// native调用js 示例
        /// </summary>
        public void ExecuteScriptAsync()
        {
            //browser.ExecuteScriptAsync("printInvokeJSMethod('hello world')");
            ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "printInvokeJSMethod('hello world')" });
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

        #region GetDeviceId
        /// <summary>
        /// 获取机器唯一码
        /// </summary>
        /// <returns></returns>
        public string GetDeviceId()
        {
            if (RegHelper.GetKey2() == "")
            {
                RegHelper.CreateAndSaveNewKey2();
            }
            //add by sVen 2018年5月14日 增加一个系统版本回调给js
            Task.Factory.StartNew(() =>
            {
                var os = Environment.OSVersion.Version;
                OSVer = os.Major + "." + os.Minor;

                //browser.ExecuteScriptAsync("getDeviceIdCallBack('" + OSVer + "')");
            });
            return RegHelper.GetKey2();
        }
        #endregion

        public string GetDeviceModel()
        {
            return "WINDOWS";
        }

        #region GetVersionInfo
        /// <summary>
        /// 获取文件版本
        /// </summary>
        /// <returns></returns>
        public string GetVersionInfo()
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        #endregion

        #region GetNetWorkStatus
        /// <summary>
        /// 判断网络状态 包含持续判断网络状态回调线程
        /// </summary>
        /// <returns></returns>
        public string GetNetWorkStatus()
        {
            ResponseEntity responseEntity = new ResponseEntity();
            ////xp系统暂时去掉网络检测功能
            //if (OSVer.Equals("5.1"))
            //{
            //    responseEntity.code = ResponseCode.SUCCESS;
            //    responseEntity.msg = "获取网络状态操作成功！";
            //}
            //else
            //{
            try
            {
                //如果网络正常
                if (InternetHelper.IsConnectInternet())
                {
                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.msg = "获取网络状态操作成功！";
                    logger.Info("获取网络状态操作成功");
                    _NetworkStatus = true;
                    if (!_NetworkChecking)
                    {
                        NetWorkListener();
                    }
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "NETTYPE_NONE";
                    logger.Info("获取网络状态: NETTYPE_NONE");
                    _NetworkStatus = false;
                    if (!_NetworkChecking)
                    {
                        NetWorkListener();
                    }
                }
            }
            catch (Exception e)
            {
                logger.Info(e.Message + e.StackTrace);
            }
            //}
            return JsonConvert.SerializeObject(responseEntity);
        }

        private void NetWorkListener()
        {
            //防止开多条线程
            if (!_NetworkChecking)
            {
                _NetworkChecking = true;
            }
            //这里还是用前台线程处理 保证升级后推出
            Thread netCheckThread = new Thread(new ThreadStart(
                () =>
                {
                    int i = 0;
                    ResponseEntity responseEntity = new ResponseEntity();
                    while (_NetworkChecking)
                    {
                        try
                        {
                            bool isConnectInternet = "5.1".Equals(OSVer) ? InternetHelper.IsConnectInternetXP() : InternetHelper.IsConnectInternet();
                            //bool isConnectInternet = InternetHelper.IsConnectInternet();
                            if (!isConnectInternet)
                            {
                                logger.Info("isConnectInternet = false>>>" + isConnectInternet);
                                //第一次判断 无论什么情况都要返回
                                if (i == 0)
                                {
                                    _NetworkStatus = isConnectInternet;
                                    responseEntity.code = ResponseCode.Failed;
                                    mWebViewHandle?.Invoke("networkChangeCallBack", responseEntity);
                                }
                                //即当前网络发生变化时
                                if (isConnectInternet != _NetworkStatus)
                                {
                                    _NetworkStatus = isConnectInternet;
                                    responseEntity.code = ResponseCode.Failed;
                                    mWebViewHandle?.Invoke("networkChangeCallBack", responseEntity);

                                }
                            }
                            else
                            {
                                logger.Info("isConnectInternet = true>>>" + isConnectInternet);
                                //第一次判断 无论什么情况都要返回
                                if (i == 0)
                                {
                                    _NetworkStatus = isConnectInternet;
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    mWebViewHandle?.Invoke("networkChangeCallBack", responseEntity);
                                }
                                if (isConnectInternet != _NetworkStatus)
                                {
                                    _NetworkStatus = isConnectInternet;
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    mWebViewHandle?.Invoke("networkChangeCallBack", responseEntity);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Info("网络状态回调出现问题");
                        }
                        Thread.Sleep(5000);
                        //i赋值1 让其判断是否网络改变
                        i = 1;
                    }
                }
                ));

            netCheckThread.IsBackground = true;
            netCheckThread.Start();

        }
        #endregion

        #region SetZoomLevel
        /// <summary>
        /// 设置浏览器分辨率大小
        /// </summary>
        /// <param name="ZoomLevel"></param>
        public void SetZoomLevel(double ZoomLevel)
        {
            //browser.SetZoomLevel(ZoomLevel);
            t_WebBrowserExtensions.GetMethods().FirstOrDefault(m => m.Name == "SetZoomLevel" && m.GetParameters().Length == 2)
                .Invoke(null, new object[] { browser, ZoomLevel });

        }
        #endregion

        #region OpenSecondScreen
        /// <summary>
        /// 打开副屏
        /// </summary>
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

        }
        #endregion

        //null method debug
        #region SecondScreenAction
        public void SecondScreenAction(string p1, string p2 = "")
        {
            Task.Factory.StartNew(() =>
            {
                if (_SecondScreenWebView != null)
                {
                    try
                    {

                        //_SecondScreenWebView.ExecuteScriptAsync(p1 + "('" + p2 + "')");


                        //t_WebBrowserExtensions = assemblyCefSharp.GetType("CefSharp.WebBrowserExtensions");
                        //MethodInfo method = t_WebBrowserExtensions.GetMethods().FirstOrDefault(m => m.Name == "ExecuteScriptAsync" && m.GetParameters().Length == 2);
                        ExecuteScriptAsyncMethod.Invoke(null, new object[] { _SecondScreenWebView, p1 + "('" + p2 + "')" });
                    }
                    catch (Exception e)
                    {
                        logger.Error("SecondScreenAction err", e);
                    }
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
                            using (var db = SugarDao.Instance)
                            {
                                List<UserEntity> userList;
                                if (!(String.IsNullOrEmpty(loginEntity.account)))
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
                                    if (configEntities != null && configEntities.Count == 1)
                                    {
                                        ShopConfigEntity shopConfigEntity = configEntities[0];
                                        userVM.config = shopConfigEntity;
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "登陆成功";
                                    responseEntity.data = userVM;
                                    mWebViewHandle.Invoke("loginCallBack", responseEntity);
                                }
                                else
                                {
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "登陆失败";
                                    mWebViewHandle.Invoke("loginCallBack", responseEntity);
                                }
                            }
                        }
                        else
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "登陆失败";
                            mWebViewHandle.Invoke("loginCallBack", responseEntity);
                        }
                        //只是为了调试加的
                        //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", responseEntity });
                        //mWebViewHandle.Invoke("loginCallBack", responseEntity);
                        //System.Windows.Forms.MessageBox.Show("called loginCallBack js method!!!");
                    }
                    catch (Exception e)
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "参数格式错误";
                        mWebViewHandle.Invoke("loginCallBack", responseEntity);
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
                logger.Info("保存或更新用户信息接口：保存数据库操作异常: " + e.Message + e.StackTrace);
                //throw;
            }
        }
        #endregion

        #region Logout
        /// <summary>
        /// 登出清缓存接口
        /// </summary>
        public void Logout()
        {
            CommodityCacheManager.Instance.cleanCache();
        }
        #endregion

        #region SaveOrUpdateCommodityInfo
        /// <summary>
        /// 保存或更新商品信息
        /// </summary>
        public void SaveOrUpdateCommodityInfo(string json)
        {
            Task.Factory.StartNew(() =>
            {
                CommodityCacheManager.Instance.cleanCache();
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
                            List<BarCodeEntity2> barCodes = commodityInfoVM.barcodes;
                            List<CommodityPriceEntity> commodityPriceEntityList = commodityInfoVM.commoditypricelist;

                            #region 已采用bulksave 方式提高存库效率
                            //保存商品分类信息
                            if (categoryEntities != null)
                            {
                                dbManager.BulkSaveOrUpdate(categoryEntities);
                            }
                            //保存商品信息
                            if (commoditys != null)
                            {
                                //这里是数据的大头
                                dbManager.BulkSaveOrUpdate(commoditys.ToArray());

                                //后台数据主键设置不统一  还要兼容android数据库不能自定义字段  只能写死的方式更新提升速度
                                //dbManager.SaveOrUpdateCommodityEntities(commoditys.ToArray());

                            }
                            //保存会员等级信息
                            if (memberEntities != null)
                            {
                                dbManager.BulkSaveOrUpdate(memberEntities);
                            }
                            //保存付款方式信息
                            if (paytypes != null)
                            {
                                dbManager.BulkSaveOrUpdate(paytypes);
                            }
                            //保存收银员信息
                            if (assistants != null)
                            {
                                dbManager.BulkSaveOrUpdate(assistants);
                            }
                            //保存收银员信息
                            if (users != null)
                            {
                                dbManager.BulkSaveOrUpdate(users);
                            }
                            //保存供应商信息
                            if (suppliers != null)
                            {
                                dbManager.BulkSaveOrUpdate(suppliers);
                            }
                            // add: 2018/2/27
                            //保存条码表信息
                            if (barCodes != null)
                            {
                                dbManager.BulkSaveOrUpdate(barCodes.ToArray());
                            }
                            //保存调价表信息
                            if (commodityPriceEntityList != null)
                            {
                                dbManager.BulkSaveOrUpdate(commodityPriceEntityList.ToArray());
                            }
                            #endregion

                            dbManager.SaveOrUpdate(commodityInfoVM);
                            logger.Info("保存和更新商品信息接口：信息保存成功");
                            responseEntity.code = ResponseCode.SUCCESS;

                            mWebViewHandle.Invoke("saveOrUpdateCommodityInfoCallBack", responseEntity);
                            return;
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
                mWebViewHandle.Invoke("saveOrUpdateCommodityInfoCallBack", responseEntity);

            });
            return; /*JsonConvert.SerializeObject(responseEntity);*/

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
                    List<BarCodeEntity2> barCodes = null;
                    List<CommodityPriceEntity> commodityPriceList = null;

                    using (var db = SugarDao.Instance)
                    {
                        // TODO: 2017/11/3 按固定精确度返回，
                        //固定精确到shopcode
                        supplierEntities = db.Queryable<SupplierEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        payTypeEntities = db.Queryable<PayTypeEntity>().Where(it => it.shopcode == userEntity.shopcode).ToList();

                        commodityInfoVMList = db.Queryable<CommodityInfoVM>().Where(it => it.shopcode == userEntity.shopcode).OrderBy(it => it.id).ToList();

                        categoryEntities = db.Queryable<CategoryEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.del == "0").OrderBy(it => it.categorycode).ToList();

                        // add: 2018/2/27
                        barCodes = db.Queryable<BarCodeEntity2>().Where(it => it.shopcode == userEntity.shopcode
                                                                        && it.del == "0").ToList();

                        //固定精确到shopcode + branchcode
                        assistantsEntities = db.Queryable<AssistantsEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                                && it.branchcode == userEntity.branchcode).ToList();

                        cashierEntities = db.Queryable<CashierEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.branchcode == userEntity.branchcode).ToList();

                        commodityPriceList = db.Queryable<CommodityPriceEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                                    && it.branchcode == userEntity.branchcode).ToList();

                        //按 membermode 区分1共享和0非共享
                        if ("1".Equals(userEntity.membermodel)) //共享
                        {
                            memberEntities = db.Queryable<MemberEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.membermodel == "1").ToList();
                        }
                        else //非共享
                        {
                            memberEntities = db.Queryable<MemberEntity>().Where(it => it.shopcode == userEntity.shopcode
                                                                            && it.branchcode == userEntity.branchcode
                                                                            && it.membermodel == "0").ToList();
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
                            barCodes = new List<BarCodeEntity2>();
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
                    using (var db = SugarDao.Instance)
                    {
                        string shopcode = _LoginUserManager.UserEntity.shopcode;
                        string branchcode = _LoginUserManager.UserEntity.branchcode;
                        List<CommodityInfoVM> lastRequestTimeList = db.Queryable<CommodityInfoVM>()
                                                                    .Where(it => it.shopcode == shopcode
                                                                    && it.branchcode == branchcode).ToList();
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
        public void GetLastUserName()
        {
            string shopcode = "";
            shopcode = ContextCache.GetShopcode();
            Task.Factory.StartNew(() =>
            {
                //browser.ExecuteScriptAsync("getLastUserNameCallBack('" + shopcode + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getLastUserNameCallBack('" + shopcode + "')" });
            });
            //ResponseEntity responseEntity = new ResponseEntity();
            //try
            //{
            //    shopcode = ContextCache.GetShopcode();

            //    responseEntity.code = ResponseCode.SUCCESS;
            //    responseEntity.data = shopcode;
            //    //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "getLastUserNameCallBack", responseEntity });
            //    Task.Factory.StartNew(() =>
            //    {
            //        browser.ExecuteScriptAsync("getWeightCallBack('" + number + "')");
            //    });
            //}
            //catch (Exception e)
            //{
            //    responseEntity.code = ResponseCode.Failed;
            //    responseEntity.data = shopcode;
            //    //ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "getLastUserNameCallBack", responseEntity });
            //    Task.Factory.StartNew(() =>
            //    {
            //        mWebViewHandle.Invoke("getLastUserNameCallBack", shopcode);
            //    });
            //    logger.Error(e.Message + e.StackTrace);
            //}
            ////return shopcode;
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
                    //add 2018年10月18日
                    billEntity.whichversion = "1018";
                    dbManager.SaveOrUpdate(billEntity);
                }
                catch (Exception e)
                {
                    logger.Error("保存销售单据接口： 异常");
                }
                List<BillCommodityEntity> commoditys = billEntity.commoditys;
                List<PayDetailEntity> paydetails = billEntity.paydetails;
                List<DisCountDetailEntity> discountdetails = billEntity.discountdetails;
                if (commoditys == null || commoditys.Count == 0)
                {
                    logger.Info("保存销售单据接口：该单据没有商品信息");
                }
                else
                {
                    int x = 0;
                    foreach (BillCommodityEntity billCommodityEntity in commoditys)
                    {
                        try
                        {
                            if ("SKU00001".Equals(billCommodityEntity.commoditycode))
                            {
                                billCommodityEntity.uid = billCommodityEntity
                                    .ticketcode
                                    + "_"
                                    + billCommodityEntity.id + "_" + x++;
                            }
                            else
                            {

                                billCommodityEntity.uid = billCommodityEntity
                                    .ticketcode
                                    + "_"
                                    + billCommodityEntity.id;
                            }
                            dbManager.SaveOrUpdate(billCommodityEntity);
                        }
                        catch (Exception e)
                        {
                            logger.Error("保存销售单据接口：dbManager.saveOrUpdate(billCommodityEntity)--DbException", e);
                        }
                    }
                }

                if (paydetails == null || paydetails.Count == 0)
                {
                    logger.Info("保存销售单据接口：该单据没有付款方式信息");
                }
                else
                {
                    ////edit by sVen 2018年5月9日 优化为块存方式提高效率
                    //dbManager.BulkSaveOrUpdate(paydetails);
                    //paydetail直接插入 因为默认为0 saveorupdate会执行update操作覆盖
                    try
                    {
                        foreach (PayDetailEntity payDetailEntity in paydetails)
                        {
                            //dbManager.SaveOrUpdate(payDetailEntity);
                            dbManager.SaveAndInsert(payDetailEntity);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Info("payDetail save err>>" + e.Message + e.StackTrace);
                    }
                }

                if (discountdetails == null || discountdetails.Count == 0)
                {
                    logger.Info("保存销售单据接口：该单据没有支付行信息");
                }
                else
                {
                    foreach (DisCountDetailEntity disCountDetailEntity in discountdetails)
                    {
                        dbManager.SaveOrUpdate(disCountDetailEntity);
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
        public void GetAllSaleBill(string state, string target)
        {
            Task.Factory.StartNew(() =>
            {
                string allBill = "[]";

                DbManager dbManager = DBUtils.Instance.DbManager;
                UserEntity userEntity = _LoginUserManager.UserEntity;
                string shopcode = userEntity.shopcode;
                string branchcode = userEntity.branchcode;

                try
                {
                    using (var db = SugarDao.Instance)
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
                            allBill = JsonConvert.SerializeObject(billEntities);
                            //return JsonConvert.SerializeObject(billEntities);
                            //browser.ExecuteScriptAsync("getAllSaleBillCallBack('" + target +  "','" + allBill + "')");
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e.Message + e.StackTrace);
                }
                //browser.ExecuteScriptAsync("getAllSaleBillCallBack('" + target + "','" + allBill + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getAllSaleBillCallBack('" + target + "','" + allBill + "')" });
                //return "";
            });
        }
        #endregion

        #region GetAllSaleBillByParams
        /// <summary>
        /// 离线交易查询（JS存在bug 待处理 2018年9月5日）
        /// </summary>
        /// <param name="s"></param>
        public void GetAllSaleBillByParams(string s)
        {
            Task.Factory.StartNew(() =>
            {
                string result = "[]";
                BillAndCountEntity billAndCountEntity = new BillAndCountEntity();
                try
                {
                    QueryBillEntity queryBillEntity = JsonConvert.DeserializeObject<QueryBillEntity>(s);
                    if (queryBillEntity != null)
                    {
                        string start = queryBillEntity.starttime;
                        string end = queryBillEntity.endtime;
                        if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
                        {
                            //browser.ExecuteScriptAsync("getAllSaleBillByParamsCallBack('" + result + "')");
                            ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getAllSaleBillByParamsCallBack('" + result + "')" });
                            return;
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
                                var totalCount = 0;
                                using (var db = SugarDao.Instance)
                                {
                                    List<BillEntity> billEntities = db.Queryable<BillEntity>().Where(i => i.insertTime >= startDateTime
                                                                                                    && i.cashierid == queryBillEntity.cashierid
                                                                                                    && i.insertTime <= endDateTime
                                                                                                    && (i.ticketstatue == "cached" || i.ticketstatue == "updated")
                                                                                                    && i.shopcode == shopcode
                                                                                                    && i.branchcode == branchCode
                                                                                                    && SqlFunc.Contains(i.ticketcode, queryBillEntity.ticketcode))
                                                                                                    .ToPageList(queryBillEntity.pageindex + 1, queryBillEntity.pagesize, ref totalCount);
                                    if (billEntities != null)
                                    {
                                        logger.Info("获取对应状态的单据信息 billEntities:" + billEntities.ToString());
                                        for (int i = 0; i < billEntities.Count; i++)
                                        {
                                            List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                            List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                            List<DisCountDetailEntity> disCountDetailEntities = db.Queryable<DisCountDetailEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                            if (billCommodityEntities == null)
                                            {
                                                billCommodityEntities = new List<BillCommodityEntity>();
                                            }
                                            if (payDetailEntities == null)
                                            {
                                                payDetailEntities = new List<PayDetailEntity>();
                                            }
                                            if (disCountDetailEntities == null)
                                            {
                                                disCountDetailEntities = new List<DisCountDetailEntity>();
                                            }
                                            billEntities[i].commoditys = billCommodityEntities;
                                            billEntities[i].paydetails = payDetailEntities;
                                            billEntities[i].discountdetails = disCountDetailEntities;
                                        }
                                        billAndCountEntity.count = totalCount;
                                        billAndCountEntity.billEntityList = billEntities;
                                        //result = JsonConvert.SerializeObject(_DataProcessor.PaginationData(billEntities, pageindex, pagesize));
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Info("getAllSaleBillByParams : 数据库查询出错", e);
                        }


                    }
                }
                catch (Exception e)
                {
                    logger.Error("getAllSaleBillByParams : json 解析异常", e);
                }
                string finalAllBills = JsonConvert.SerializeObject(billAndCountEntity);
                //browser.ExecuteScriptAsync("getAllSaleBillByParamsCallBack('" + finalAllBills + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getAllSaleBillByParamsCallBack('" + finalAllBills + "')" });
            });
        }
        #endregion

        #region GetSelectTimeSaleBillByPagination
        /// <summary>
        /// 分页查询销售订单
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public void GetSelectTimeSaleBillByPagination(string start, string end, int pageindex, int pagesize)
        {
            Task.Factory.StartNew(() =>
            {
                string result = "[]";
                if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
                {
                    //browser.ExecuteScriptAsync("getSelectTimeSaleBillByPaginationCallBack('" + result + "')");
                    ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getSelectTimeSaleBillByPaginationCallBack('" + result + "')" });
                    return;
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
                        using (var db = SugarDao.Instance)
                        {
                            string id = _LoginUserManager.UserEntity.userid;
                            List<BillEntity> billEntities = db.Queryable<BillEntity>().Where(i => i.insertTime >= startDateTime
                                                                                            && i.cashierid == id
                                                                                            && i.insertTime <= endDateTime
                                                                                            && (i.ticketstatue == "cached" || i.ticketstatue == "updated")
                                                                                            && i.shopcode == shopcode
                                                                                            && i.branchcode == branchCode)
                                                                                            .OrderBy(i => i.insertTime, OrderByType.Desc)
                                                                                            .ToList();
                            if (billEntities != null)
                            {
                                //for (int i = 0; i < billEntities.Count; i++)
                                //{
                                //    List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                //    List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                                //    if (billCommodityEntities == null)
                                //    {
                                //        billCommodityEntities = new List<BillCommodityEntity>();
                                //    }
                                //    else
                                //    {

                                //    }
                                //    if (payDetailEntities == null)
                                //    {
                                //        payDetailEntities = new List<PayDetailEntity>();
                                //    }
                                //    billEntities[i].commoditys = billCommodityEntities;
                                //    billEntities[i].paydetails = payDetailEntities;
                                //}
                                //return JsonConvert.SerializeObject(_DataProcessor.PaginationData(billEntities, pageindex, pagesize));
                                result = JsonConvert.SerializeObject(_DataProcessor.PaginationData(billEntities, pageindex, pagesize));
                                //browser.ExecuteScriptAsync("getSelectTimeSaleBillByPaginationCallBack('" + JsonConvert.SerializeObject(_DataProcessor.PaginationData(billEntities, pageindex, pagesize)) + "')");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
                //browser.ExecuteScriptAsync("getSelectTimeSaleBillByPaginationCallBack('" + result + "','" + pageindex + "','" + pagesize + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getSelectTimeSaleBillByPaginationCallBack('" + result + "','" + pageindex + "','" + pagesize + "')" });
            });

            //return result;
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
        public void GetSelectTimeSaleBill(string start, string end)
        {
            string result = "";
            Task.Factory.StartNew(() =>
            {

                if (string.IsNullOrEmpty(start) && string.IsNullOrEmpty(end))
                {
                    //browser.ExecuteScriptAsync("getSelectTimeSaleBillCallBack('" + result + "')");
                    ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getSelectTimeSaleBillCallBack('" + result + "')" });
                    return;
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
                        TotalBillVM totalBillVM = new TotalBillVM();
                        List<TotalBillVM.PayTypeEntity> paytypes = new List<TotalBillVM.PayTypeEntity>();
                        using (var db = SugarDao.Instance)
                        {

                            string id = _LoginUserManager.UserEntity.userid;
                            //List<BillEntity> billEntities = db.Queryable<BillEntity>().Where(i => i.insertTime >= startDateTime
                            //                                                                && i.cashierid == id
                            //                                                                && i.insertTime <= endDateTime
                            //                                                                && (i.ticketstatue == "cached" || i.ticketstatue == "updated")
                            //                                                                && i.shopcode == shopcode
                            //                                                                && i.branchcode == branchCode).ToList();
                            //if (billEntities != null)
                            //{
                            //    for (int i = 0; i < billEntities.Count; i++)
                            //    {
                            //        //List<BillCommodityEntity> billCommodityEntities = db.Queryable<BillCommodityEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                            //        List<PayDetailEntity> payDetailEntities = db.Queryable<PayDetailEntity>().Where(x => x.ticketcode == billEntities[i].ticketcode).ToList();
                            //        //if (billCommodityEntities == null)
                            //        //{
                            //        //    billCommodityEntities = new List<BillCommodityEntity>();
                            //        //}
                            //        //else
                            //        //{

                            //        //}
                            //        if (payDetailEntities == null)
                            //        {
                            //            payDetailEntities = new List<PayDetailEntity>();
                            //        }
                            //        //billEntities[i].commoditys = billCommodityEntities;
                            //        billEntities[i].paydetails = payDetailEntities;
                            //    }
                            //    result = JsonConvert.SerializeObject(billEntities);
                            //    //return _DataProcessor.PaginationData(billEntities, pageindex, pagesize);
                            //}

                            var totalDM = db.Queryable<BillEntity>().Where(i => i.insertTime >= startDateTime
                                                                            && i.cashierid == id
                                                                            && i.insertTime <= endDateTime
                                                                            && (i.ticketstatue == "cached" || i.ticketstatue == "updated")
                                                                            && i.shopcode == shopcode
                                                                            && i.branchcode == branchCode)
                                                                            .Select(i => new { totalpay = SqlFunc.AggregateSum(i.paytotal), count = SqlFunc.AggregateCount(i.ticketcode) })
                                                                            .ToList();
                            totalBillVM.totalpay = Math.Round(Convert.ToDouble(totalDM[0].totalpay), 2) + "";

                            totalBillVM.totalnum = Convert.ToInt32(totalDM[0].count);


                            try
                            {
                                var list = db.Queryable<BillEntity, PayDetailEntity>((be, pde) => new object[] { JoinType.Inner, be.ticketcode == pde.ticketcode })
                                                                                    .Where((be, pde) => be.insertTime >= startDateTime
                                                                                    && be.cashierid == id
                                                                                    && be.insertTime <= endDateTime
                                                                                    && (be.ticketstatue == "cached" || be.ticketstatue == "updated")
                                                                                    && be.shopcode == shopcode
                                                                                    && be.branchcode == branchCode)
                                                                                    .GroupBy((be, pde) => pde.payname)
                                                                                    .Select((be, pde) => new { pde.payname, payamount = SqlFunc.AggregateSum(pde.payamount), totalnumtcout = SqlFunc.AggregateCount(pde.id) })
                                                                                    .ToList();

                                //list.ForEach(i => paytypes.Add(new TotalBillVM.PayTypeEntity { payname = i.payname, totalpay = i.payamount, totalnum = i.totalnumtcout.ToString() }));
                                //2018年9月7日 防止高精度小数出现 进行四舍五入

                                list.ForEach(i => paytypes.Add(new TotalBillVM.PayTypeEntity { payname = i.payname, totalpay = Math.Round(Convert.ToDouble(i.payamount), 2) + "", totalnum = i.totalnumtcout.ToString() }));
                                totalBillVM.paytypes = paytypes;
                            }
                            catch (Exception ex)
                            {
                                logger.Info("统计支付方式异常");
                            }
                        }
                        result = JsonConvert.SerializeObject(totalBillVM);
                    }
                }
                catch (Exception e)
                {
                    logger.Info(e.Message + e.StackTrace);
                }
                //browser.ExecuteScriptAsync("getSelectTimeSaleBillCallBack('" + result + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getSelectTimeSaleBillCallBack('" + result + "')" });
            });
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
                using (var db = SugarDao.Instance)
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
                using (var db = SugarDao.Instance)
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
            using (var db = SugarDao.Instance)
            {
                try
                {
                    string shopcode = _LoginUserManager.UserEntity.shopcode;
                    string branchcode = _LoginUserManager.UserEntity.branchcode;
                    number = db.Queryable<BillEntity>().Where(it => it.ticketstatue == "hangup"
                                                        && it.shopcode == shopcode
                                                        && it.branchcode == branchcode).ToList().Count;
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
            using (var db = SugarDao.Instance)
            {
                try
                {
                    if (!db.DbMaintenance.IsAnyTable("BillEntity", false))
                    {
                        logger.Info("数据库暂时没有单据");
                        return 0;
                    }
                    DateTime startDate = Convert.ToDateTime(start, dtFormat);
                    DateTime endDate = Convert.ToDateTime(end, dtFormat);
                    long startDateTime = DateUtils.ConvertDataTimeToLong(startDate);
                    long endDateTime = DateUtils.ConvertDataTimeToLong(endDate);
                    string shopcode = _LoginUserManager.UserEntity.shopcode;
                    string branchcode = _LoginUserManager.UserEntity.branchcode;
                    number = db.Queryable<BillEntity>().Where(it => it.ticketstatue == "cached"
                                                        || it.ticketstatue == "updated"
                                                        && it.insertTime >= startDateTime
                                                        && it.insertTime <= endDateTime
                                                        && it.shopcode == shopcode
                                                        && it.branchcode == branchcode).Count();
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

        #region GetCommodityList
        /// <summary>
        /// 根据相应条件获取商品信息
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="categorycode"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        public void GetCommodityList(string keyword, string categorycode, int pageindex, int pagesize)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                List<CommodityEntity> commodityEntities = null;
                List<BarCodeEntity2> barCodeEntityList = null;
                List<CommodityPriceEntity> commodityPriceEntityList = null;
                if (_LoginUserManager.Login)
                {
                    UserEntity userEntity = _LoginUserManager.UserEntity;
                    try
                    {
                        using (var db = SugarDao.Instance)
                        {
                            ShopConfigEntity shopConfigEntity = db.Queryable<ShopConfigEntity>().Where(
                                                                           it => it.id == int.Parse(userEntity.shopcode) + int.Parse(userEntity.branchcode)).First();
                            if (shopConfigEntity != null && "12".Equals(shopConfigEntity.industryid))//服装版
                            {
                                commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                && i.commoditystatus == "0"
                                                                                && i.del == "0"
                                                                                && i.commoditylevel == "1"
                                                                                && (string.IsNullOrEmpty(categorycode) || i.categorycode == categorycode)
                                                                                && (i.commodityname.Contains(keyword) || i.commoditycode.Contains(keyword) || i.mnemonic.Contains(keyword))
                                                                                ).ToList();
                                barCodeEntityList = db.Queryable<BarCodeEntity2>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.del == "0").ToList();
                                commodityPriceEntityList = db.Queryable<CommodityPriceEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                        && i.branchcode == userEntity.branchcode).ToList();
                            }
                            else
                            {
                                commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                && i.commoditystatus == "0"
                                                                                && i.del == "0"
                                                                                && i.commoditylevel == "1"
                                                                                && (string.IsNullOrEmpty(categorycode) || i.categorycode == categorycode)
                                                                                && (i.commodityname.Contains(keyword) || i.commoditycode.Contains(keyword) || i.mnemonic.Contains(keyword))
                                                                                ).ToList();
                                barCodeEntityList = db.Queryable<BarCodeEntity2>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.del == "0").ToList();
                                commodityPriceEntityList = db.Queryable<CommodityPriceEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                        && i.branchcode == userEntity.branchcode).ToList();
                            }
                        }

                        for (int i = 0; i < commodityEntities.Count; i++)
                        {
                            CommodityEntity commodityEntity = commodityEntities[i];
                            //从条码表获取商品对应的条码
                            if (barCodeEntityList != null)
                            {
                                for (int a = 0; a < barCodeEntityList.Count; a++)
                                {
                                    if (commodityEntity.commoditycode.Equals(barCodeEntityList[a].commoditycode))
                                    {
                                        string barcodes = barCodeEntityList[a].barcode;
                                        if (!string.IsNullOrEmpty(barcodes) && barcodes.Length > 0)
                                        {
                                            barcodes = barcodes.Split(',')[0];
                                        }
                                        if (string.IsNullOrEmpty(barcodes))
                                        {
                                            barcodes = "";
                                        }
                                        commodityEntity.barcode = barcodes;//用商品条码
                                        break;
                                    }
                                }
                            }
                            if (commodityPriceEntityList != null)
                            {
                                for (int a = 0; a < commodityPriceEntityList.Count; a++)
                                {
                                    if (commodityEntity.commoditycode.Equals(commodityPriceEntityList[a].commoditycode))
                                    {
                                        commodityEntity.saleprice = commodityPriceEntityList[a].saleprice;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "获取商品列表出错";
                        mWebViewHandle.Invoke("getCommodityListCallBack", responseEntity);
                        logger.Info("GetCommodityList error:" + e.Message + e.StackTrace);
                    }
                }
                CommodityAndCountEntity commodityAndCountEntity = new CommodityAndCountEntity();
                commodityAndCountEntity.count = commodityEntities.Count;
                commodityAndCountEntity.commodityEntities = _DataProcessor.PaginationData(commodityEntities, pageindex, pagesize) as List<CommodityEntity>;
                responseEntity.code = ResponseCode.SUCCESS;
                responseEntity.data = commodityAndCountEntity;
                mWebViewHandle.Invoke("getCommodityListCallBack", responseEntity);
            });


        }
        #endregion

        //add 2018年9月3日
        #region SaveCommodityList
        public void SaveCommodityList(string target, string commodityListString)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                if (_LoginUserManager.Login)
                {
                    UserEntity userEntity = _LoginUserManager.UserEntity;
                    try
                    {
                        DbManager dbManager = DBUtils.Instance.DbManager;
                        List<CommodityEntity> commodities = JsonConvert.DeserializeObject<List<CommodityEntity>>(commodityListString);
                        if (commodities != null)
                        {
                            dbManager.BulkSaveOrUpdate(commodities);
                        }
                        logger.Info("保存和更新商品信息接口：信息保存成功");
                        responseEntity.code = ResponseCode.SUCCESS;
                    }
                    catch (Exception e)
                    {
                        logger.Error("保存和更新商品信息接口：json解析异常or保存数据库操作异常");
                        responseEntity.code = ResponseCode.Failed;
                    }
                }
                else
                {
                    logger.Info("保存和更新商品信息接口：用户未登录");
                    responseEntity.code = ResponseCode.Failed;
                }
                //browser.ExecuteScriptAsync("saveCommodityListCallBack('" + target + "', '" + JsonConvert.SerializeObject(responseEntity) + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "saveCommodityListCallBack('" + target + "', '" + JsonConvert.SerializeObject(responseEntity) + "')" });
            });
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
                    using (var db = SugarDao.Instance)
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
                    using (var db = SugarDao.Instance)
                    {
                        BarCodeEntity2 barCodeEntity = db.Queryable<BarCodeEntity2>().Where(i => i.shopcode == userEntity.shopcode
                                                                                            //&& i.barcodes.Contains(barcode)).First();
                                                                                            && i.barcode == barcode
                                                                                            && i.del == "0").First();//模糊查询改为精确查询
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
                    using (var db = SugarDao.Instance)
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
        public string GetCommodityByCategoryCode(string categoryCode, int pageindex, int pagesize)
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
                        using (var db = SugarDao.Instance)
                        {
                            ShopConfigEntity shopConfigEntity = db.Queryable<ShopConfigEntity>().Where(
                                                                            it => it.id == int.Parse(userEntity.shopcode) + int.Parse(userEntity.branchcode)).First();
                            if (shopConfigEntity != null && "12".Equals(shopConfigEntity.industryid))
                            {
                                commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                    && i.commoditystatus == "0"
                                                                                    && i.del == "0"
                                                                                    && i.categorycode == categoryCode
                                                                                    && i.commodityclassify != "3"
                                                                                    && i.commoditylevel == "2")
                                                                                    .OrderBy(i => i.commoditycode, SqlSugar.OrderByType.Asc)
                                                                                    .ToList();
                            }
                            else
                            {
                                commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                                        && i.commoditystatus == "0"
                                                                                        && i.del == "0"
                                                                                        && i.categorycode == categoryCode
                                                                                        && i.commodityclassify != "3")
                                                                                        .OrderBy(i => i.commoditycode, SqlSugar.OrderByType.Asc)
                                                                                        .ToList();
                            }
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
            return JsonConvert.SerializeObject(_DataProcessor.PaginationData(commodityEntities, pageindex, pagesize));
            //List<CommodityEntity> selectCommodityList = null;
            //if (commodityEntities != null && commodityEntities.Count > 0 && pageindex != -1)
            //{
            //    if (commodityEntities.Count > (pageindex * pagesize + pagesize))
            //    {
            //        selectCommodityList = commodityEntities.GetRange(pageindex * pagesize, pagesize);
            //    }
            //    else
            //    {
            //        if (commodityEntities.Count - pageindex * pagesize >= 0)
            //        {
            //            if (commodityEntities.Count <= (pageindex * pagesize + pageindex * pagesize + pagesize))
            //            {
            //                selectCommodityList = commodityEntities.GetRange(pageindex * pagesize, commodityEntities.Count - pageindex * pagesize);
            //            }

            //        }
            //    }
            //}
            //else
            //{
            //    selectCommodityList = commodityEntities;
            //}
            //if (selectCommodityList == null)
            //{
            //    selectCommodityList = new List<CommodityEntity>();
            //}
            //return JsonConvert.SerializeObject(selectCommodityList);

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
                    using (var db = SugarDao.Instance)
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
        public string GetCommodityByKeyword(string keyword, int pageindex, int pagesize)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            DbManager dbManager = DBUtils.Instance.DbManager;
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.Instance)
                    {
                        var total = 0;
                        ShopConfigEntity shopConfigEntity = db.Queryable<ShopConfigEntity>().Where(
                                                                            it => it.id == int.Parse(userEntity.shopcode) + int.Parse(userEntity.branchcode)).First();
                        if (shopConfigEntity != null && "12".Equals(shopConfigEntity.industryid))//服装版
                        {
                            //数据pageindex 从1开始 故++
                            //commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                            //                                                    && i.commoditystatus == "0"
                            //                                                    && i.del == "0"
                            //                                                    && i.commoditylevel == "2"
                            //                                                    && (SqlFunc.Contains(i.commodityname, keyword) || SqlFunc.Contains(i.mnemonic, keyword))
                            //                                                    && i.commodityclassify != "3")
                            //                                                    //&& (i.commodityname.Contains("keyword")
                            //                                                    //    || i.mnemonic.Contains("keyword")))
                            //                                                    //.ToList();
                            //                                                    .OrderBy(i => i.commoditycode, OrderByType.Asc)
                            //                                                    .ToPageList(pageindex + 1, pagesize, ref total);
                            //还需要从barcode中查出来
                            commodityEntities = db.Queryable<CommodityEntity, BarCodeEntity2>((c, bc) => new object[]
                              {
                                JoinType.Left,c.commoditycode == bc.commoditycode
                              })
                              .Where((c, bc) =>
                                  c.shopcode == userEntity.shopcode
                                  && c.commoditystatus == "0"
                                  && c.del == "0"
                                  && c.commodityclassify != "3"
                                  && c.commoditylevel == "2"
                                  && (SqlFunc.Contains(c.commodityname, keyword) || SqlFunc.Contains(c.mnemonic, keyword) || SqlFunc.Contains(bc.barcode, keyword))
                                  ).OrderBy((c, bc) => c.commoditycode, OrderByType.Asc)
                                  .ToPageList(pageindex + 1, pagesize, ref total);



                        }
                        else
                        {
                            //数据pageindex 从1开始 故++
                            //commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                            //                                                    && i.commoditystatus == "0"
                            //                                                    && i.del == "0"
                            //                                                    && (SqlFunc.Contains(i.commodityname, keyword) || SqlFunc.Contains(i.mnemonic, keyword))
                            //                                                    && i.commodityclassify != "3")
                            //                                                    //&& (i.commodityname.Contains("keyword")
                            //                                                    //    || i.mnemonic.Contains("keyword")))
                            //                                                    //.ToList();
                            //                                                    .OrderBy(i => i.commoditycode, OrderByType.Asc)
                            //                                                    .ToPageList(pageindex + 1, pagesize, ref total);
                            commodityEntities = db.Queryable<CommodityEntity, BarCodeEntity2>((c, bc) => new object[]
                              {
                                JoinType.Left,c.commoditycode == bc.commoditycode
                              })
                              .Where((c, bc) =>
                                  c.shopcode == userEntity.shopcode
                                  && c.commoditystatus == "0"
                                  && c.del == "0"
                                  && c.commodityclassify != "3"
                                  && (SqlFunc.Contains(c.commodityname, keyword) || SqlFunc.Contains(c.mnemonic, keyword) || SqlFunc.Contains(bc.barcode, keyword))
                                  ).OrderBy((c, bc) => c.commoditycode, OrderByType.Asc)
                                  .ToPageList(pageindex + 1, pagesize, ref total);
                        }
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
            //if (commodityEntities.Count > 50)
            //{
            //    commodityEntities = commodityEntities.GetRange(0, 50);
            //}

            return JsonConvert.SerializeObject(commodityEntities);
        }
        #endregion

        #region GetSKUCommodityBySPUCode
        /// <summary>
        /// 服装版根据spucode查询所有SKU商品
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetSKUCommodityBySPUCode(string code)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            List<CommodityEntity> commodityEntities = null;
            if (_LoginUserManager.Login)
            {
                UserEntity userEntity = _LoginUserManager.UserEntity;
                try
                {
                    using (var db = SugarDao.Instance)
                    {
                        commodityEntities = db.Queryable<CommodityEntity>().Where(i => i.shopcode == userEntity.shopcode
                                                                            && i.commoditystatus == "0"
                                                                            && i.del == "0"
                                                                            && i.spucode == code)
                                                                            .ToList();
                    }
                }
                catch (Exception e)
                {
                    logger.Error("GetSKUCommodityBySPUCode err", e);
                }
            }
            if (commodityEntities == null)
            {
                commodityEntities = new List<CommodityEntity>();
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
                using (var db = SugarDao.Instance)
                {
                    var paycodeModelList = db.Queryable<PayDetailEntity>().GroupBy(i => i.paycode).ToList();
                    List<string> payCodeList = new List<string>();
                    if (paycodeModelList != null)
                    {
                        foreach (var item in paycodeModelList)
                        {
                            string paycode = item.paycode;
                            payCodeList.Add(paycode);
                        }
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

        #region VersionUpdate
        public void VersionUpdate()
        {
            FSLib.App.SimpleUpdater.Updater.CheckUpdateSimple(AppContext.Instance.UpdateUrl, AppContext.Instance.XmlFile);
        }

        #endregion


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

        /// <summary>
        /// 获取本机所有并口 这里目前只有一台一体机  直接以写死的形式来
        /// </summary>
        /// <returns></returns>
        public string GetLPT()
        {
            string[] arrLPT = new string[] { "LPT1" };

            return JsonConvert.SerializeObject(arrLPT);
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
                //mWebViewHandle.Invoke("getUsbDevicesCallBack", responseEntity);
                //browser.ExecuteScriptAsync("getUsbDevicesCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getUsbDevicesCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')" });
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
            return CacheManager.GetGprint() as string ?? "";
        }
        #endregion

        //add 2018年9月3日 新增标价签设置
        #region GetBJQPrinter
        public string GetBJQPrinter()
        {
            return CacheManager.GetBJQprint() as string ?? "";
        }
        #endregion

        #region SetBJQPrinter (这个接口由于windows上usb设备为广播形式所以不需要实现 直接返回ture就完事了)
        public void SetBJQPrinter(string json)
        {
            //ResponseEntity responseEntity = new ResponseEntity();
            //Task.Factory.StartNew(() =>
            //{
            //    BJQPrinterManager.Instance.PrintNumber = int.Parse(JsonConvert.DeserializeObject<PrinterConfigEntity>(s).printernumber);
            //    responseEntity.code = ResponseCode.SUCCESS;
            //    responseEntity.msg = "windows不需要设置";
            //    mWebViewHandle?.Invoke("setBJQPrinterCallBack", responseEntity);
            //});

            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PrinterConfigEntity printerConfigEntity = JsonConvert.DeserializeObject<PrinterConfigEntity>(json);
                    BJQPrinterSetter printerSetter = new BJQPrinterSetter();
                    printerSetter.setPrinter(printerConfigEntity,
                                                p: (result) =>
                                                {
                                                    mWebViewHandle.Invoke("setBJQPrinterCallBack", result);
                                                    //保存缓存
                                                    if (result.code == ResponseCode.SUCCESS)
                                                    {
                                                        //添加缓存

                                                        CacheManager.InsertBJQprint(json);
                                                        if (BJQPrinterManager.Instance.Init)
                                                        {
                                                            switch (BJQPrinterManager.Instance.PrinterTypeEnum)
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
                                                    else
                                                    {
                                                        mWebViewHandle.Invoke("setGPrinterCallBack", result);
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

        #region SetGprinter (bug)
        /// <summary>
        /// 设置佳博打印机
        /// </summary>
        /// <param name="json"></param>
        public void SetGPrinter(string json)
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
                                                        //添加缓存

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
                                                    else
                                                    {
                                                        mWebViewHandle.Invoke("setGPrinterCallBack", result);
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
                USBGPrinterSetter usbGPrinterSetter = new USBGPrinterSetter();
                //ThreadPool.QueueUserWorkItem(CallbackMethod, new object[] { "getGPrintUsbDevicesCallBack", responseEntity });
                //browser.ExecuteScriptAsync("getGPrintUsbDevicesCallBack('" + usbGPrinterSetter.GetUsbDevices() + "','" + GetGPrinter() + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getGPrintUsbDevicesCallBack('" + usbGPrinterSetter.GetUsbDevices() + "','" + GetGPrinter() + "')" });
            });
            return;

        }
        #endregion

        #region GetBJQUsbDevices
        public void GetBJQUsbDevices()
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                USBGPrinterSetter usbGPrinterSetter = new USBGPrinterSetter();
                //ThreadPool.QueueUserWorkItem(CallbackMethod, new object[] { "getGPrintUsbDevicesCallBack", responseEntity });
                //browser.ExecuteScriptAsync("getBJQUsbDevicesCallBack('" + usbGPrinterSetter.GetUsbDevices() + "','" + GetBJQPrinter() + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getBJQUsbDevicesCallBack('" + usbGPrinterSetter.GetUsbDevices() + "','" + GetBJQPrinter() + "')" });
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
                    //mWebViewHandle?.Invoke("getBluetoothDevicesCallBack", responseEntity);
                    //browser.ExecuteScriptAsync("getBluetoothDevicesCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')");
                    ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getBluetoothDevicesCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')" });
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
                using (var db = SugarDao.Instance)
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

        #region GetPrinterQueue
        /// <summary>
        /// 获取打印队列
        /// </summary>
        public void GetPrinterQueue()
        {
            ResponseEntity responseEntity = new ResponseEntity();
            List<string> printQueue = new List<string>();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (PrintDocument pd = new PrintDocument())
                    {
                        for (int i = 0; i < PrinterSettings.InstalledPrinters.Count; i++)  //开始遍历
                        {
                            printQueue.Add(PrinterSettings.InstalledPrinters[i]);  //取得名称
                        }

                        responseEntity.code = ResponseCode.SUCCESS;
                        responseEntity.msg = "获取打印机队列成功";
                        responseEntity.data = printQueue;
                    }
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "GetPrinterQueue err";
                    logger.Error("获取打印机队列失败", e);
                }
                //browser.ExecuteScriptAsync("getPrinterQueueCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getPrinterQueueCallBack('" + JsonConvert.SerializeObject(responseEntity) + "','" + GetPrinter() + "')" });
                //mWebViewHandle.Invoke("getPrinterQueueCallBack", responseEntity);
            });
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

        #region SetPrinterTemplet
        /// <summary>
        /// 模板设置(标签&标价签）
        /// </summary>
        /// <param name="json"></param>
        /// <param name="printerType">"BQ" OR "BJQ"</param>
        public void SetPrinterTemplet(string s, string printerType)
        {
            if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(printerType))
            {
                logger.Info("SetPrinterTemplet json or printertype is null.");
                return;
            }
            switch (printerType)
            {
                case "SPBQ":
                    CacheManager.InsertSPBQTemplet(s);
                    break;
                case "DDBQ":
                    CacheManager.InsertDDBQTemplet(s);
                    break;
                case "BJQ":
                    CacheManager.InsertBJQTemplet(s);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region GetPrinterTemplet
        /// <summary>
        /// 获取模板设置
        /// </summary>
        /// <param name="printerType"> "BQ" OR "BJQ"</param>
        /// <returns></returns>
        public string GetPrinterTemplet(string printerType)
        {
            string s = "";
            if (!string.IsNullOrEmpty(printerType))
            {
                switch (printerType)
                {
                    case "SPBQ":
                        s = CacheManager.GetSPBQTemplet();
                        break;
                    case "DDBQ":
                        s = CacheManager.GetDDBQTemplet();
                        break;
                    case "BJQ":
                        s = CacheManager.GetBJQTemplet();
                        break;
                    default:
                        break;
                }
            }
            return s;
        }

        #endregion

        #region PrintTemplet
        /// <summary>
        /// 标签 标价签模板打印
        /// </summary>
        /// <param name="s"></param>
        /// <param name="printerType"></param>
        public void PrintTemplet(string s, string printerType, string width, string height)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                if (string.IsNullOrEmpty(s) || string.IsNullOrEmpty(printerType))
                {
                    logger.Info("PrintTemplet json or printertype is null.");
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "打印参数或打印类型为空";
                    mWebViewHandle.Invoke("printTempletCallBack", responseEntity);
                }
                try
                {

                    if (CacheManager.GetGprint() as string != null)
                    {
                        List<string> usblist = GPrinterUtils.Instance.FindUSBPrinter();
                        if (usblist == null)
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "未发现可用USB设备";

                        }
                        else
                        {
                            //GPrinterManager.Instance.usbDeviceArrayList = usblist;
                            //GPrinterManager.Instance.Init = true;
                            //GPrinterManager.Instance.PrinterTypeEnum = "usb";
                            //每次都先设置完再打印
                            if (GPrinterUtils.Instance.Connect_Printer())
                            {
                                if (GPrinterManager.Instance.Init)
                                {
                                    switch (printerType)
                                    {
                                        case "SPBQ":
                                        case "DDBQ":
                                            GPrinterUtils.Instance.BQPrintTemplet(s, width, height);
                                            responseEntity.code = ResponseCode.SUCCESS;
                                            break;
                                        case "BJQ":
                                            GPrinterUtils.Instance.BJQPrintTemplet(s, width, height);
                                            responseEntity.code = ResponseCode.SUCCESS;
                                            break;
                                        default:
                                            logger.Error("非法打印机类型");
                                            break;
                                    }
                                }
                                else
                                {
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "请设置标签打印机";
                                }
                            }
                            else
                            {
                                responseEntity.code = ResponseCode.Failed;
                                responseEntity.msg = "连接标签打印机失败";
                            }
                        }
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "请设置标签打印机";
                    }
                }
                catch (Exception e)
                {
                    logger.Error("PrintTemplet err >> ", e);
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = e.Message;
                }

                mWebViewHandle.Invoke("printTempletCallBack", responseEntity);
            });

        }
        #endregion

        public void BQPrintTemplet(string s)
        {

        }

        public void BJQPrintTemplet(string s)
        {

        }

        #region SetNote (null mehtod)
        public void SetNote(string json)
        {
            //TODO...
        }
        #endregion

        #region PrintLabel
        /// <summary>
        /// 打印标签
        /// </summary>
        /// <param name="json"></param>
        public void PrintLabel(string json)
        {
            if (GPrinterManager.Instance.Init)
            {
                GPrinterUtils.Instance.printLabel(json);
            }
        }
        #endregion

        #region PrintGarmentLabel
        public void PrintGarmentLabel(string s)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    if (GPrinterManager.Instance.Init)
                    {
                        GPrinterUtils.Instance.PrintGarmentLabel(s);
                        responseEntity.code = ResponseCode.SUCCESS;
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "请设置标签打印机";
                    }
                }
                catch (Exception e)
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = e.Message;
                    logger.Error("PrintGarmentLabel err", e);
                }

                mWebViewHandle.Invoke("printGarmentLabelCallBack", responseEntity);
            });
        }
        #endregion


        ///// <summary>
        ///// 打印条码标签
        ///// </summary>
        //public void PrintBarcodeLabel(string s)
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        ResponseEntity responseEntity = new ResponseEntity();
        //        try
        //        {
        //            if (GPrinterManager.Instance.Init)
        //            {
        //                GPrinterUtils.Instance.PrintBarcodeLable(s);
        //                responseEntity.code = ResponseCode.SUCCESS;
        //            }
        //            else
        //            {
        //                responseEntity.code = ResponseCode.Failed;
        //                responseEntity.msg = "请设置打印机";
        //            }
        //        }
        //        catch(Exception e)
        //        {
        //            logger.Info("PrintBarcodeLabel err>>" + e.Message + e.StackTrace);
        //            responseEntity.code = ResponseCode.SUCCESS;
        //            responseEntity.msg = e.Message;
        //        }
        //        mWebViewHandle?.Invoke(")
        //    });
        //}

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
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printModel(content, usbPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case Enums.PrinterTypeEnum.bluetooth:
                                    BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printModel(content, bluetoothPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case Enums.PrinterTypeEnum.port:
                                    serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printModel(content, portPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.LPT:
                                    LPTPrinter lptPrinter = PrinterManager.Instance.LptPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printModel(content, lptPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                //add 2018年10月15日 
                                case PrinterTypeEnum.drive:
                                    DrivePrinter drivePrinter = PrinterManager.Instance.DrivePrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printModel(content, drivePrinter);
                                    }
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

        #region PrintQRCode
        public void PrintQRCode(string code)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if (!string.IsNullOrEmpty(code))
            {
                try
                {
                    if (PrinterManager.Instance.Init)
                    {
                        switch (PrinterManager.Instance.PrinterTypeEnum)
                        {
                            case Enums.PrinterTypeEnum.usb:
                                USBPrinter usbPrinter = PrinterManager.Instance.UsbPrinter;
                                for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                {
                                    PrintUtils.PrintQRCode(code, usbPrinter);
                                }
                                responseEntity.code = ResponseCode.SUCCESS;
                                responseEntity.msg = "小票打印成功";
                                break;
                            case Enums.PrinterTypeEnum.bluetooth:
                                //BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                                //for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                //{
                                //    PrintUtils.printModel(content, bluetoothPrinter);
                                //}
                                //responseEntity.code = ResponseCode.SUCCESS;
                                //responseEntity.msg = "小票打印成功";
                                break;
                            case Enums.PrinterTypeEnum.port:
                                serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                                for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                {
                                    PrintUtils.PrintQRCode(code, portPrinter);
                                }
                                responseEntity.code = ResponseCode.SUCCESS;
                                responseEntity.msg = "小票打印成功";
                                break;
                            case PrinterTypeEnum.LPT:
                                //LPTPrinter lptPrinter = PrinterManager.Instance.LptPrinter;
                                //for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                //{
                                //    PrintUtils.printModel(content, lptPrinter);
                                //}
                                //responseEntity.code = ResponseCode.SUCCESS;
                                //responseEntity.msg = "小票打印成功";
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
                    logger.Error("PrintQRCode err", e);
                }
            }
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
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printNote(statisticsVM, usbPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.bluetooth:
                                    BluetoothPrinter bluetoothPrinter = PrinterManager.Instance.BluetoothPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printNote(statisticsVM, bluetoothPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.port:
                                    serialPort portPrinter = PrinterManager.Instance.PortPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printNote(statisticsVM, portPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.LPT:
                                    LPTPrinter lptPrinter = PrinterManager.Instance.LptPrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printNote(statisticsVM, lptPrinter);
                                    }
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.msg = "小票打印成功";
                                    break;
                                case PrinterTypeEnum.drive:
                                    DrivePrinter drivePrinter = PrinterManager.Instance.DrivePrinter;
                                    for (int i = 0; i < PrinterManager.Instance.PrintNumber; i++)
                                    {
                                        PrintUtils.printNote(statisticsVM, drivePrinter);
                                    }
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
                                //portPrinter.OpenCash("1B70001080");
                                //这里yk那边说是1B 70 00 10 90 2018年5月29日 暂时用这个调试以下
                                portPrinter.OpenCash("1B70001090");
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
                    case PrinterTypeEnum.LPT:
                        if (PrinterManager.Instance.LptPrinter != null && PrinterManager.Instance.LptPrinter.Enable)
                        {
                            PrinterManager.Instance.LptPrinter.OpenCash();
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

        #region SetCustomerShow
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
                if (!string.IsNullOrEmpty(json))
                {
                    logger.Info("SetCustomerShow:" + json);
                    CustomerShowConfigEntity customerShowConfigEntity = JsonConvert.DeserializeObject<CustomerShowConfigEntity>(json);
                    if (customerShowConfigEntity != null)
                    {
                        try
                        {
                            serialPort serialport = new serialPort(customerShowConfigEntity.port, customerShowConfigEntity.intBaud);
                            logger.Info("SetCustomerShow: port =" + customerShowConfigEntity.port + "baud:" + customerShowConfigEntity.intBaud);
                            serialport.init();
                            if (serialport.Open(customerShowConfigEntity.port, Int32.Parse(customerShowConfigEntity.intBaud)))
                            {
                                serialport.Close();
                                logger.Info("串口关闭" + serialport.port);
                                responseEntity.code = ResponseCode.SUCCESS;
                                responseEntity.msg = "设置成功";
                                CustomerShowManager.Instance.Init = true;
                                CustomerShowManager.Instance.SerialPort = serialport;
                                using (var db = SugarDao.Instance)
                                {
                                    DbManager dbManager = DBUtils.Instance.DbManager;
                                    if (db.DbMaintenance.IsAnyTable(typeof(CustomerShowConfigEntity).Name, false))
                                    {
                                        db.DbMaintenance.DropTable(typeof(CustomerShowConfigEntity).Name);
                                    }
                                    dbManager.SaveOrUpdate(customerShowConfigEntity);
                                }

                            }
                            else
                            {
                                responseEntity.code = ResponseCode.Failed;
                                responseEntity.msg = "设置失败";
                            }
                        }
                        catch (Exception ex)
                        {
                            logger.Info("SetCustomerShow 异常", ex);
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "异常";
                        }
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "参数格式错误";
                    }
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "参数不能为空";
                }
                logger.Info("SetCustomerShow:" + JsonConvert.SerializeObject(responseEntity));
                mWebViewHandle?.Invoke("setCustomerShowCallBack", responseEntity);
            });

        }
        #endregion

        #region GetCustomerShow
        /// <summary>
        /// 获取客显设置信息
        /// </summary>
        /// <returns></returns>
        public string GetCustomerShow()
        {
            ResponseEntity responseEntity = new ResponseEntity();
            CustomerShowConfigEntity customerShowConfigEntity = null;
            DbManager dbManager = DBUtils.Instance.DbManager;
            using (var db = SugarDao.Instance)
            {
                try
                {
                    customerShowConfigEntity = db.Queryable<CustomerShowConfigEntity>().First();
                }
                catch (Exception e)
                {
                    logger.Info("获取客显数据库信息失败");
                }
                if (customerShowConfigEntity == null)
                {
                    customerShowConfigEntity = new CustomerShowConfigEntity();
                }
                logger.Info("GetCustomerShow:" + JsonConvert.SerializeObject(customerShowConfigEntity));
                return JsonConvert.SerializeObject(customerShowConfigEntity);
            }
        }
        #endregion

        #region CustomerShow
        /// <summary>
        /// 推送到客显信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string CustomerShow(string s)
        {
            ResponseEntity responseEntity = new ResponseEntity();
            if (!string.IsNullOrEmpty(s))
            {
                serialPort serialport = CustomerShowManager.Instance.SerialPort;
                if (CustomerShowManager.Instance.Init && serialport != null)
                {
                    logger.Info("CustomerShow: write into " + s);
                    serialport.CustomerWrite(s);
                    responseEntity.code = ResponseCode.SUCCESS;
                    responseEntity.msg = "成功";
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "请到设置页面设置客显端口和波特率";
                }
            }
            else
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";
            }
            return JsonConvert.SerializeObject(responseEntity);
        }
        #endregion


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

        #region GetScale
        /// <summary>
        /// 获取电子秤
        /// </summary>
        /// <returns></returns>
        public string GetScale()
        {
            string scale = CacheManager.GetScale(SPCode.scale) as string;
            if (string.IsNullOrEmpty(scale))
            {
                return "";
            }
            else
            {
                return scale;
            }
        }
        #endregion

        #region SetScale
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
                    logger.Info(json);
                    WeightUtil.Instance.Open(scaleConfigEntity.port, scaleConfigEntity.brand);
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
        #endregion

        #region GetWeight
        /// <summary>
        /// 获取重量
        /// </summary>
        /// <returns></returns>
        public void GetWeight()
        {

            Task.Factory.StartNew(() =>
            {
                string scale = CacheManager.GetScale(SPCode.scale) as string;
                ResponseEntity responseEntity = new ResponseEntity();
                if (!string.IsNullOrEmpty(scale))
                {
                    try
                    {
                        ScaleConfigEntity scaleConfigEntity = JsonConvert.DeserializeObject<ScaleConfigEntity>(scale);
                        logger.Info("GetWeight -> scale 缓存读出:" + scale);
                        if (!string.IsNullOrEmpty(scaleConfigEntity.port))
                        {
                            WeightUtil.Instance.Open(scaleConfigEntity.port, scaleConfigEntity.brand);
                            WeightUtil.Instance.Listener = (number) =>
                            {
                                //browser.ExecuteScriptAsync("getWeightCallBack('" + number + "')");
                                logger.Info("getweigth callback invoke : number =>> " + number);
                                if (string.IsNullOrEmpty(number))
                                {
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "电子秤设置错误，请重新设置";
                                }
                                else
                                {
                                    responseEntity.code = ResponseCode.SUCCESS;
                                    responseEntity.data = number;
                                }
                                mWebViewHandle.Invoke("getWeightCallBack", responseEntity);
                            };

                        }
                        else
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "未设置电子秤";
                            mWebViewHandle.Invoke("getWeightCallBack", responseEntity);
                            //browser.ExecuteScriptAsync("getWeightCallBack(" + "'" + "" + "'" + ")");
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e.Message + e.StackTrace);
                        //browser.ExecuteScriptAsync("getWeightCallBack(" + "'" + "" + "'" + ")");
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = e.Message + e.StackTrace;
                        mWebViewHandle.Invoke("getWeightCallBack", responseEntity);
                    }
                }
            });

        }
        #endregion

        #region CloseWeighter
        /// <summary>
        /// 关闭电子秤
        /// </summary>
        /// <returns></returns>
        public void CloseWeighter()
        {
            WeightUtil.Instance.Close();
        }
        #endregion

        #region SaveBarcodeScale
        /// <summary>
        /// 保存条码信息
        /// </summary>
        /// <param name="scaleConfig"></param>
        public void SaveBarcodeScale(string scaleConfig)
        {
            if (!string.IsNullOrEmpty(scaleConfig))
            {
                BarcodeScaleConfigEntity barcodeScaleConfigEntity = JsonConvert.DeserializeObject<BarcodeScaleConfigEntity>(scaleConfig);
                List<BarcodeScaleEntity> scaleList = barcodeScaleConfigEntity.barcodeScaleEntityList;
                CacheManager.InsertBarcodeScale(barcodeScaleConfigEntity.barcodeStyle);
                DbManager dbManager = DBUtils.Instance.DbManager;
                if (scaleList != null && scaleList.Count > 0)
                {
                    foreach (BarcodeScaleEntity barcodeScaleEntity in scaleList)
                    {
                        try
                        {
                            dbManager.SaveOrUpdate(barcodeScaleEntity);
                        }
                        catch (Exception e)
                        {
                            logger.Info("保存条码信息出错>>" + e.Message + e.StackTrace);
                        }
                    }
                }
            }
            //CacheManager.InsertBarcodeScale(scaleConfig);
        }
        #endregion

        #region GetBarcodeScale
        /// <summary>
        ///  获取保存的条码秤信息
        /// </summary>
        /// <returns></returns>
        public string GetBarcodeScale()
        {
            BarcodeScaleConfigEntity barcodeScaleConfigEntity = new BarcodeScaleConfigEntity();
            List<BarcodeScaleEntity> barcodeScaleEntityList = null;
            DbManager dbManager = DBUtils.Instance.DbManager;
            try
            {
                using (var db = SugarDao.Instance)
                {
                    barcodeScaleEntityList = db.Queryable<BarcodeScaleEntity>().ToList();
                    barcodeScaleConfigEntity.barcodeScaleEntityList = barcodeScaleEntityList;
                }
            }
            catch (Exception e)
            {
                logger.Info("获取保存的条码秤信息" + e.Message + e.StackTrace);
            }

            string barcodeStyle = CacheManager.GetBarcodeScale();
            barcodeScaleConfigEntity.barcodeStyle = barcodeStyle;
            if (barcodeScaleEntityList == null)
            {
                barcodeScaleEntityList = new List<BarcodeScaleEntity>();
            }
            return JsonConvert.SerializeObject(barcodeScaleConfigEntity);
            //return CacheManager.GetBarcodeScale();
        }
        #endregion

        #region SyncCommoditytoBarcodeScale
        /**
        * @ ip:同步到秤的ip地址
        * @ pluMessageEntityList:
        * @ clear:是否全量同步          1:全量
        * @ brand:品牌    ACS_TM_jijing,//吉景条码秤 ACS_TM_dahua,//大华条码秤
        */
        public void SyncCommoditytoBarcodeScale(string ss)
        {
            logger.Info("syncCommoditytoBarcodeScale(" + ss + ")-- switch");
            SyncScaleVM syncScaleVM = JsonConvert.DeserializeObject<SyncScaleVM>(ss);
            switch (syncScaleVM.brand)
            {
                case "ACS_TM_dahua":
                    SyncCommoditytoBarcodeScale_dahua(ss);
                    break;
                case "ACS_TM_jijing":
                    SyncCommoditytoBarcodeScale_jijing(ss);
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region SyncCommoditytoBarcodeScale_jijing
        private void SyncCommoditytoBarcodeScale_jijing(string ss)
        {
            logger.Info("syncCommoditytoBarcodeScale_dahua(" + ss + ")");
            SyncScaleVM syncScaleVM = JsonConvert.DeserializeObject<SyncScaleVM>(ss);
            IPAddress ip = IPAddress.Parse(syncScaleVM.ip);
            List<PluMessageEntity> pluMessageEntities = syncScaleVM.pluMessageEntityList;
            int clear = syncScaleVM.clean;


            Action<string, int> sendMessage = (syncScaleStatus, point) =>
            {
                SyncScaleEntity syncScaleEntity = new SyncScaleEntity();
                syncScaleEntity.status = syncScaleStatus;
                syncScaleEntity.point = point;
                Task.Factory.StartNew(() =>
                {
                    //browser.ExecuteScriptAsync("syncCommoditytoBarcodeScaleCallBack('" + JsonConvert.SerializeObject(syncScaleEntity) + "')");
                    ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "syncCommoditytoBarcodeScaleCallBack('" + JsonConvert.SerializeObject(syncScaleEntity) + "')" });
                });
            };


            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                int TIMOUT = 5000;
                int MAXNUM = 5;
                try
                {
                    sendMessage(SyncScaleStatus.SOCKET_OPENING, 0);
                    byte[] buf = new byte[1024];
                    IPEndPoint udpPoint = new IPEndPoint(ip, 9001);

                }
                catch (Exception e)
                {
                    logger.Info("SyncCommoditytoBarcodeScale异常>>" + e.Message + e.StackTrace);
                }
            }), new object[] { });
            return;
        }
        #endregion

        #region SyncCommoditytoBarcodeScale_dahua
        /// <summary>
        /// 同步条码称方法
        /// </summary>
        /// <param name="ss"> @ ip:同步到秤的ip地址 @ pluMessageEntityList: @ clear:是否全量同步          1:全量</param>
        public void SyncCommoditytoBarcodeScale_dahua(string ss)
        {
            logger.Info("syncCommoditytoBarcodeScale_dahua(" + ss + ")");
            SyncScaleVM syncScaleVM = JsonConvert.DeserializeObject<SyncScaleVM>(ss);
            IPAddress ip = IPAddress.Parse(syncScaleVM.ip);
            List<PluMessageEntity> pluMessageEntities = syncScaleVM.pluMessageEntityList;
            int clear = syncScaleVM.clean;


            Action<string, int> sendMessage = (syncScaleStatus, point) =>
             {
                 SyncScaleEntity syncScaleEntity = new SyncScaleEntity();
                 syncScaleEntity.status = syncScaleStatus;
                 syncScaleEntity.point = point;
                 Task.Factory.StartNew(() =>
                 {
                     //browser.ExecuteScriptAsync("syncCommoditytoBarcodeScaleCallBack('" + JsonConvert.SerializeObject(syncScaleEntity) + "')");
                     ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "syncCommoditytoBarcodeScaleCallBack('" + JsonConvert.SerializeObject(syncScaleEntity) + "')" });
                 });
             };


            ThreadPool.QueueUserWorkItem(new WaitCallback((state) =>
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                bool isConnect = false;
                try
                {
                    sendMessage(SyncScaleStatus.SOCKET_OPENING, 0);

                    socket.Connect(new IPEndPoint(ip, 4001));
                    logger.Info("尝试打开socket...");
                    if (socket.Connected)
                    {
                        logger.Info("socket is connect...");
                        sendMessage(SyncScaleStatus.SOCKET_OPENED, 0);
                        isConnect = true;
                        logger.Info("socket打开成功...");
                    }
                    else
                    {
                        int i = 0;
                        while (!isConnect && i < 10)
                        {
                            i++;
                            socket.Connect(new IPEndPoint(ip, 4001));
                            Thread.Sleep(100);
                            if (socket.Connected)
                            {
                                logger.Info("sockt is connected");
                                isConnect = true;
                                sendMessage(SyncScaleStatus.SOCKET_OPENED, 0);
                                logger.Info("socket 打开成功");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Info("SyncCommoditytoBarcodeScale异常>>" + e.Message + e.StackTrace);
                }
                if (isConnect)
                {
                    try
                    {
                        socket.SendTimeout = 5000;
                        socket.ReceiveTimeout = 5000;
                        if (clear == 1)
                        {
                            sendMessage(SyncScaleStatus.PLU_CLEANING, 0);
                            sendMessage_btye(StringConvert.convertStringToBytesForTMC("!0IA"), socket);
                            sendMessage_btye(StringConvert.convertStringToBytesForTMC("!0HA"), socket);
                            logger.Info("正在清除历史数据");
                            Thread.Sleep(20000);
                        }
                        sendMessage(SyncScaleStatus.PLU_CLEANED, 0);
                        sendMessage(SyncScaleStatus.PLU_SYNCING, 0);
                        logger.Info("开始同步");
                        string barcodeStyle = CacheManager.GetBarcodeScale();
                        string style = "!0O01050301" + barcodeStyle + "000100000100010001000000000000000001";
                        sendMessage_btye(StringConvert.convertStringToBytesForTMC(style), socket);
                        string branchName = "!0Z01A" + StringConvert.getWordCode(_LoginUserManager.UserEntity.branchname) + "0000B";
                        sendMessage_btye(StringConvert.convertStringToBytesForTMC(branchName), socket);
                        for (int i = 0; i < pluMessageEntities.Count; i++)
                        {
                            string shopInfo = CommodityToPLU(pluMessageEntities[i]);
                            if (!string.IsNullOrEmpty(shopInfo))
                            {
                                sendMessage_btye(StringConvert.convertStringToBytesForTMC(shopInfo), socket);
                                sendMessage(SyncScaleStatus.PLU_SYNCING, i + 1);
                            }
                            logger.Info("正在同步第" + (i + 1) + "条商品数据");
                            Thread.Sleep(500);
                        }
                        try
                        {
                            //socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                        catch (Exception e)
                        {
                            logger.Info("socket close err>>" + e.Message + e.StackTrace);
                        }
                        sendMessage(SyncScaleStatus.PLU_SYNCED, 0);
                        logger.Info("同步完成");
                    }
                    catch (Exception ex)
                    {
                        logger.Error("socket 超时异常", ex);
                        sendMessage(SyncScaleStatus.SOCKET_ERR, 0);
                    }
                }
                else
                {
                    socket.Close();
                    sendMessage(SyncScaleStatus.SOCKET_NO_OPEN, 0);
                }
            }), new object[] { });
            return;
        }
        #endregion

        #region
        /// <summary>
        /// 获取条码称重量
        /// </summary>
        public void GetWeightCommodity()
        {
            Task.Factory.StartNew(() =>
            {
                //ResponseEntity responseEntity = new ResponseEntity();
                DbManager dbManager = DBUtils.Instance.DbManager;
                List<CommodityEntity> commodityEntityList = null;
                List<BarCodeEntity2> barCodeEntityList = null;
                List<CommodityPriceEntity> commodityPriceEntityList = null;
                List<PluMessageEntity> pluMessageEntityList = null;
                try
                {
                    string shopcode = _LoginUserManager.UserEntity.shopcode;
                    string branchcode = _LoginUserManager.UserEntity.branchcode;
                    using (var db = SugarDao.Instance)
                    {
                        commodityEntityList = db.Queryable<CommodityEntity>().Where(i => i.shopcode == shopcode
                                                                                && i.commoditystatus == "0"
                                                                                && i.del == "0"
                                                                                && (i.pricing == "1" || i.pricing == "2")).ToList();
                        pluMessageEntityList = db.Queryable<PluMessageEntity>().Where(i => i.shopCode == shopcode).ToList();
                        barCodeEntityList = db.Queryable<BarCodeEntity2>().Where(i => i.shopcode == shopcode
                                                                                && i.del == "0").ToList();
                        commodityPriceEntityList = db.Queryable<CommodityPriceEntity>().Where(i => i.shopcode == shopcode
                                                                                    && i.branchcode == branchcode).ToList();

                    }
                }
                catch (Exception e)
                {
                    logger.Info("db err>>" + e.Message + e.StackTrace);
                }

                if (commodityEntityList != null)
                {
                    for (int i = 0; i < commodityEntityList.Count; i++)
                    {
                        CommodityEntity commodityEntity = commodityEntityList[i];
                        //从plu编号表获取上次保存的信息
                        if (pluMessageEntityList != null && pluMessageEntityList.Count > 0)
                        {
                            foreach (PluMessageEntity pluMessageEntity in pluMessageEntityList)
                            {
                                if (commodityEntity.commoditycode.Equals(pluMessageEntity.commoditycode))
                                {
                                    commodityEntity.plu = pluMessageEntity.plu;
                                    //                            commodityEntity.setValidtime(pluMessageEntity.getIndate());
                                    commodityEntity.tare = pluMessageEntity.tare;
                                    pluMessageEntityList.Remove(pluMessageEntity);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            commodityEntity.plu = "" + (i + 1);
                        }
                        //从条码表获取商品对应的条码
                        if (barCodeEntityList != null)
                        {
                            for (int a = 0; a < barCodeEntityList.Count; a++)
                            {
                                if (commodityEntity.commoditycode.Equals(barCodeEntityList[a].commoditycode))
                                {
                                    string barcodes = barCodeEntityList[a].barcode;
                                    if (!string.IsNullOrEmpty(barcodes) && barcodes.Length > 0)
                                    {
                                        barcodes = barcodes.Split(',')[0];
                                    }
                                    if (string.IsNullOrEmpty(barcodes))
                                    {
                                        barcodes = "";
                                    }
                                    commodityEntity.barcode = barcodes;//用商品条码
                                                                       //barCodeEntityList.RemoveAt(a);
                                    break;
                                }
                            }
                        }
                        if (commodityEntity.barcode == null)
                        {
                            commodityEntity.barcode = "";
                        }
                        //从调价表获取商品单价
                        if (commodityPriceEntityList != null)
                        {
                            for (int a = 0; a < commodityPriceEntityList.Count; a++)
                            {
                                if (commodityEntity.commoditycode.Equals(commodityPriceEntityList[a].commoditycode))
                                {
                                    commodityEntity.saleprice = commodityPriceEntityList[a].saleprice;
                                    //commodityPriceEntityList.RemoveAt(a);
                                    break;
                                }
                            }
                        }
                    }
                }
                //responseEntity.data = commodityEntityList;
                //mWebViewHandle.Invoke("getWeightCommodityCallBack", responseEntity);
                //browser.ExecuteScriptAsync("getWeightCommodityCallBack('" + JsonConvert.SerializeObject(commodityEntityList) + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "getWeightCommodityCallBack('" + JsonConvert.SerializeObject(commodityEntityList) + "')" });
            });
            return;
            //return JsonConvert.SerializeObject(commodityEntityList);
        }
        #endregion

        #region SaveWeightCommodity
        /// <summary>
        /// 保存商品重量
        /// </summary>
        /// <param name="str"></param>
        public void SaveWeightCommodity(string str)
        {
            Task.Factory.StartNew(() =>
            {
                DbManager dbManager = DBUtils.Instance.DbManager;
                List<PluMessageEntity> pluMessageEntityList = JsonConvert.DeserializeObject<List<PluMessageEntity>>(str);
                try
                {
                    if (pluMessageEntityList != null)
                    {
                        foreach (PluMessageEntity pluMessageEntity in pluMessageEntityList)
                        {
                            pluMessageEntity.shopCode = _LoginUserManager.UserEntity.shopcode;
                            pluMessageEntity.uid = pluMessageEntity.shopCode + "_" + pluMessageEntity.commoditycode;
                            dbManager.SaveOrUpdate(pluMessageEntity);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Info("saveWeightCommodity err>>" + e.Message + e.StackTrace);
                }
                //browser.ExecuteScriptAsync("saveWeightCommodityCallBack()");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "saveWeightCommodityCallBack()" });
            });
        }
        #endregion

        #region Method of BarcodeScale
        /// <summary>
        /// 将单条商品信息转换成条码秤PLU码
        /// </summary>
        /// <param name="pluMessageEntity"></param>
        /// <returns></returns>
        private string CommodityToPLU(PluMessageEntity pluMessageEntity)
        {
            try
            {
                StringBuilder sb = new StringBuilder("!0W");
                if (Int32.Parse(pluMessageEntity.plu) > 4000 || Int32.Parse(pluMessageEntity.plu) < 0)
                {
                    return "";
                }
                sb.Append(formatCode(pluMessageEntity.plu, 4));//PLU编码
                sb.Append("A");
                sb.Append(formatCode(pluMessageEntity.barcode, 6));//条码
                sb.Append("B");
                sb.Append(formatCode(Convert.ToInt32(float.Parse(pluMessageEntity.price) * 100) + "", 5));//单价
                sb.Append("C");
                sb.Append(pluMessageEntity.type);//0-称重状态 1-计件
                sb.Append("D");
                sb.Append(formatCode(pluMessageEntity.indate, 3));//有效期
                sb.Append("E");
                sb.Append("22");//店名
                sb.Append("F");
                if (Int32.Parse(pluMessageEntity.tare) > 15000 || Int32.Parse(pluMessageEntity.tare) < 0)
                {
                    return "";
                }
                sb.Append(formatCode(pluMessageEntity.tare, 5));//皮重
                sb.Append("G");
                sb.Append("0");//特殊信息
                sb.Append("H");
                sb.Append(wordCheck(pluMessageEntity.commodityName));//商品名称
                sb.Append("00I");
                sb.Append("00000000");//配料
                sb.Append("00J");
                return sb.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 校验商品长度，并转换成区位码
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private string wordCheck(string commodityName)
        {
            if (commodityName.Length > 9)
            {
                commodityName = commodityName.Substring(0, 9);
            }
            return StringConvert.getWordCode(commodityName);
        }


        /// <summary>
        /// 补零操作
        /// </summary>
        /// <param name="p"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private string formatCode(string code, int count)
        {
            if (code == null)
            {//为空处理
                code = "";
            }
            StringBuilder result = new StringBuilder(code);
            if (count > code.Length)
            {
                for (int i = 0; i < count - code.Length; i++)
                {
                    result.Insert(0, "0");
                }
            }
            else
            {
                result.Remove(0, code.Length - count);
            }
            return result.ToString();
        }

        /// <summary>
        /// 定义下发数据的方法
        /// </summary>
        /// <param name="buf"></param>
        /// <param name="socket"></param>
        private void sendMessage_btye(byte[] buf, Socket socket)
        {
            try
            {
                logger.Info("send:" + Encoding.Default.GetString(buf));
                socket.Send(buf);

                //读取服务器返回的消息
                byte[] revBuff = new byte[1024];
                socket.Receive(revBuff);
                string mess = Encoding.Default.GetString(revBuff);
                logger.Info("rev:" + mess);

            }
            catch (Exception e)
            {
                logger.Error("sendMessage_btye error:", e);
                throw e;
            }
        }
        #endregion

        #region OnDesktop
        /// <summary>
        /// 返回桌面
        /// </summary>
        public void OnDesktop()
        {
            frmMain.HideSelf();
        }
        #endregion

        #region PrintCommodityLabel
        /// <summary>
        /// 打印商品标签
        /// </summary>
        /// <param name="json"></param>
        public void PrintCommodityLabel(string json)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                if (!string.IsNullOrEmpty(json))
                {
                    GPrinterUtils.Instance.PrintCommodotyLabel(json);
                    responseEntity.code = ResponseCode.SUCCESS;
                }
                else
                {
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = "打印失败";
                }
                mWebViewHandle.Invoke("printCommodityLabelCallBack", responseEntity);
            });
        }
        #endregion

        #region
        /// <summary>
        /// 打印条码标签 (非收银时候用 专门用来打的)
        /// </summary>
        /// <param name="json"></param>
        public void PrintBarcodeLabel(string json)
        {
            Task.Factory.StartNew(() =>
            {
                ResponseEntity responseEntity = new ResponseEntity();
                try
                {
                    //CacheManager.GetGprint() as string
                    if (CacheManager.GetGprint() as string != null)
                    {
                        List<string> usblist = GPrinterUtils.Instance.FindUSBPrinter();
                        if (usblist == null)
                        {
                            responseEntity.code = ResponseCode.Failed;
                            responseEntity.msg = "未发现可用USB设备";

                        }
                        else
                        {
                            GPrinterManager.Instance.usbDeviceArrayList = usblist;
                            GPrinterManager.Instance.Init = true;
                            GPrinterManager.Instance.PrinterTypeEnum = "usb";
                            //每次都先设置完再打印
                            if (GPrinterUtils.Instance.Connect_Printer())
                            {
                                if (GPrinterManager.Instance.Init)
                                {
                                    GPrinterUtils.Instance.PrintBarcodeLable(json);
                                    responseEntity.code = ResponseCode.SUCCESS;
                                }
                                else
                                {
                                    responseEntity.code = ResponseCode.Failed;
                                    responseEntity.msg = "请设置标签打印机";
                                }
                            }
                            else
                            {
                                responseEntity.code = ResponseCode.Failed;
                                responseEntity.msg = "连接标签打印机失败";
                            }
                        }
                    }
                    else
                    {
                        responseEntity.code = ResponseCode.Failed;
                        responseEntity.msg = "请设置标签打印机";
                    }




                }
                catch (Exception e)
                {
                    logger.Error("PrintBarcodeLabel >> " + e.Message + e.StackTrace);
                    responseEntity.code = ResponseCode.Failed;
                    responseEntity.msg = e.Message;
                }

                mWebViewHandle.Invoke("printBarcodeLabelCallBack", responseEntity);
            });
        }
        #endregion

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

        #region IntentTo
        /// <summary>
        /// 跳转网址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public bool IntentTo(string url)
        {
            Task.Factory.StartNew(() =>
            {
                //browser.ExecuteScriptAsync("intentToCallBack('" + url + "')");
                ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, "intentToCallBack('" + url + "')" });
            });
            return true;
        }
        #endregion


        public void TestUSBPrint()
        {
            USBPrinterService usbPrinterService = new USBPrinterService();
            usbPrinterService.TestPrint();
        }


        public string TestORM(string json)
        {

            DbManager dbManager = DBUtils.Instance.DbManager;
            //dbManager.SaveOrUpdate(employees);

            try
            {

                using (var db = SugarDao.Instance)
                {
                    Employee employees = JsonConvert.DeserializeObject<Employee>(json);
                    db.Insertable(employees).ExecuteCommand();
                    dbManager.SaveOrUpdate(employees);

                    //PayDetailEntity payDetails = JsonConvert.DeserializeObject<PayDetailEntity>(json);
                    //dbManager.SaveOrUpdate(payDetails);

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


        #region CallBack Methods
        /// <summary>
        /// 多线程方式回调
        /// </summary>
        /// <param name="state"></param>
        private void CallbackMethod(object state)
        {

            object[] paramsArr = (object[])state;
            string methodName = paramsArr[0] as string;
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;
            //browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
            ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')" });
        }

        /// <summary>
        /// 委托方式回调
        /// </summary>
        /// <param name="methodName">回调得js方法名</param>
        /// <param name="responseEntity">回调的数据包</param>
        private static void AsyncCallbackMethod(string methodName, ResponseEntity responseEntity)
        {
            //browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
            ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')" });
        }

        private void CallbackMethod4SetPrinter(object state)
        {
            object[] paramsArr = (object[])state;
            string methodName = paramsArr[0] as string;
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;
            //browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
            ExecuteScriptAsyncMethod.Invoke(null, new object[] { browser, methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')" });
            if (responseEntity.code == ResponseCode.SUCCESS)
            {
                try
                {
                    using (var db = SugarDao.Instance)
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
        #endregion


    }
}
