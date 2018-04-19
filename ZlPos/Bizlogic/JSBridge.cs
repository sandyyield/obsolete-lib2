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

namespace ZlPos.Bizlogic
{
    /// <summary>
    /// create by sVen 2018年3月15日： method invoke class
    /// </summary>
    class JSBridge
    {
        //private static ILog logger = null;
        private static ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private ChromiumWebBrowser browser;

        private static LoginUserManager _LoginUserManager = LoginUserManager.Instance;

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
            return new Guid().ToString();
        }

        public string GetNetWorkStatus()
        {
            return "json";
        }

        public void SetZoomLevel(double d)
        {
            browser.SetZoomLevel(d);
        }

        //null method
        #region SecondScreenAction
        public void SecondScreenAction(string p1, string p2)
        {
            return;
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
            ResponseEntity responseEntity = new ResponseEntity();
            if (string.IsNullOrEmpty(json))
            {
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";

                //TODO这里考虑开个线程池去操作
                ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", responseEntity });
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
                    ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "loginCallBack", responseEntity });
                    System.Windows.Forms.MessageBox.Show("called loginCallBack js method!!!");
                }
                catch (Exception e)
                {

                }
            }
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
                        ThreadPool.QueueUserWorkItem(new WaitCallback(CallbackMethod), new object[] { "saveOrUpdateCommodityInfoCallBack", responseEntity });
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
            try
            {
                shopcode = ContextCache.GetShopcode();
            }
            catch (Exception e)
            {
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
        public ResponseEntity SaveOneSaleBill(string json)
        {
            ResponseEntity responseEntity = new ResponseEntity();

            if (string.IsNullOrEmpty(json))
            {
                logger.Info("保存销售单据接口：空字符串");
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数不能为空";
                return responseEntity;
            }
            DbManager dbManager = DBUtils.Instance.DbManager;

            BillEntity billEntity = JsonConvert.DeserializeObject<BillEntity>(json);
            if (billEntity == null)
            {
                logger.Info("保存销售单据接口：json解析失败");
                responseEntity.code = ResponseCode.Failed;
                responseEntity.msg = "参数格式错误";
                return responseEntity;
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
            return responseEntity;
        }
        #endregion

        #region GetAllSaleBill
        /// <summary>
        /// 获取对应状态的单据信息
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<BillEntity> GetAllSaleBill(string state)
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
                        return billEntities;
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message + e.StackTrace);
            }
            //有问题直接返回null
            return null;
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
        public List<BillEntity> GetSelectTimeSaleBill(string start, string end)
        {
            return null;
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


        /// <summary>
        /// 根据商品id查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityById(string json)
        {
            //TODO...
            return "";
        }

        /// <summary>
        /// 根据商品barcode查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string GetCommodityByBarcode(string json)
        {
            //TODO...
            return "";
        }

        public string GetCommodityByCommoditycode(string json)
        {
            //TODO...
            return "";
        }

        /// <summary>
        /// 根据商品categoryCode查询商品,barcode倒叙，然后再分页
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string getCommodityByCategoryCode(String categoryCode, int pageindex, int pagesize)
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

        /// <summary>
        /// 根据商品mnemonic不区分大小写模糊查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public string getCommodityByMnemonic(string mnemonic)
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
                }catch(Exception e)
                {
                    logger.Error(e.StackTrace);
                }
        }
        if (commodityEntities == null) {
            commodityEntities = new List<CommodityEntity>();
        }
        if (commodityEntities.Count > 100) {
            commodityEntities = commodityEntities.GetRange(0, 100);
        }
            return JsonConvert.SerializeObject(commodityEntities);
        }

        /// <summary>
        /// 根据商品名称,商品条码,助记码（大小写兼容）查询商品
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public List<CommodityEntity> getCommodityByKeyword(string json)
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取支付优先级
        /// </summary>
        /// <returns></returns>
        public ResponseEntity getPayPriority()
        {
            //TODO...
            return null;
        }




        //IO 操作相关 ... 
        #region GetPort
        /// <summary>
        /// 获取本机所有串口端口号 win专用
        /// </summary>
        /// <returns></returns>
        public string[] GetPort()
        {
            string[] arrSerial = SerialPort.GetPortNames();

            return arrSerial;
        }
        #endregion

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


        public ResponseEntity StartBluetoothDeviceInquiry()
        {
            //TODO...
            return null;
        }

        public ResponseEntity GetBluetoothDevices()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取到的是串行总线上(物理或逻辑上)的设备
        /// </summary>
        /// <returns></returns>
        public string[] GetComSystemDevices()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取上次保存的打印机设置
        /// </summary>
        /// <returns></returns>
        public PrinterConfigEntity GetPrinter()
        {
            //TODO...
            return null;
        }

        public ResponseEntity StartBluetoothSearch()
        {
            //TODO...
            return null;
        }

        public ResponseEntity ReturnBluetoothSearch()
        {
            //TODO...
            return null;
        }

        public ResponseEntity setPrinter()
        {
            //TODO...
            return null;
        }

        public void SetNote(string json)
        {
            //TODO...
        }

        public ResponseEntity Print()
        {
            //TODO...
            return null;
        }

        public ResponseEntity Print2()
        {
            //TODO...
            return null;
        }

        public ResponseEntity PrintBill(string json)
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 开钱箱
        /// </summary>
        public bool OpenBox()
        {
            //TODO...
            return true;
        }

        /// <summary>
        /// 保存客显设置
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public ResponseEntity SetCustomerShow(string json)
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取客显设置信息
        /// </summary>
        /// <returns></returns>
        public CustomerShowConfigEntity GetCustomerShow()
        {
            //TODO...
            return null;
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
        public ResponseEntity GetScale()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 设置电子秤
        /// </summary>
        /// <returns></returns>
        public ResponseEntity SetScale()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 获取重量
        /// </summary>
        /// <returns></returns>
        public ResponseEntity GetWeight()
        {
            //TODO...
            return null;
        }

        /// <summary>
        /// 关闭电子秤
        /// </summary>
        /// <returns></returns>
        public ResponseEntity closeWeighter()
        {
            //TODO...
            return null;
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
            usbPrinterService.Print("hello");
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



        private void CallbackMethod(object state)
        {

            object[] paramsArr = (object[])state;
            //first params is method name  
            string methodName = paramsArr[0] as string;

            //real params tofix
            ResponseEntity responseEntity = paramsArr[1] as ResponseEntity;

            ////模拟耗时操作
            //Thread.Sleep(5000);


            browser.ExecuteScriptAsync(methodName + "('" + JsonConvert.SerializeObject(responseEntity) + "')");
        }
    }
}
