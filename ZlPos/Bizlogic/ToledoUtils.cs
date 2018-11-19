using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace ZlPos.Bizlogic
{
    class ToledoUtils
    {
        private string ip = "";
        private string port = "";
        private string TaskPath = "";
        private string OutputFile = "";

        //查询执行结果用
        private string CommandID = "";

        public bool ClearData { get; set; }


        private ToledoUtils() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="TaskPath"></param>
        /// <param name="port"></param>
        public ToledoUtils(string ip, string TaskPath, string port = "3001",string OutputFile = "TaskResult.xml")
        {
            this.ip = ip;
            this.TaskPath = TaskPath;
            this.port = port;
            this.OutputFile = OutputFile;
        }


        /// <summary>
        /// 生成一个任务 相关文件 （数据需另外生成）
        /// </summary>
        /// <returns></returns>
        public void BuildTask(string guid)
        {
            XDocument TaskXml = new XDocument();
            TaskXml.Add(GetTaskX(guid));
            TaskXml.Save(TaskPath);

            XDocument DeviceListXml = new XDocument();
            DeviceListXml.Add(GetDeviceListX(ip, port));
            DeviceListXml.Save(TaskPath);

            CommandID = Guid.NewGuid().ToString();
            XDocument CommandXml = new XDocument();
            CommandXml.Add(GetCommandX(CommandID: Guid.NewGuid().ToString(),ClearData:ClearData));
            CommandXml.Save(TaskPath);



        }





        /// <summary>
        /// 获取DeviceList.xml的内容 //暂时只支持单台 后面考虑添加多台兼容
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public XElement GetDeviceListX(string ip, string port)
        {
            XElement DeviceList = new XElement("Devices",
            new XElement("Scale",
                new XElement("DeviceID", "1"),
                new XElement("ScaleNo", "2"),
                new XElement("ScaleType", "bPlus"),
                new XElement("ConnectType", "Network"),
                new XElement("ConnectParams"),
                //new XElement("NetworkParams")),
                //new object[] {
                //    //TOCHANGE
                //    new XAttribute("Type", "Network"),
                //    new XAttribute("Address", "192.168.102.153"),
                //    new XAttribute("Port","3001")
                //    })),
                new XElement("DecimalDigits", "2"),
                new XElement("DataFile", "Command.xml")
                )
            );

            DeviceList.Element("ConnectParams").Add(new XElement("NetworkParams", new object[] {
                                                        //TOCHANGE
                                                        new XAttribute("Type", "Network"),
                                                        new XAttribute("Address", ip),
                                                        new XAttribute("Port",port)
                                                        }));
            return DeviceList;
        }


        /// <summary>
        /// TaskX  
        /// </summary>
        /// <param name="TaskID"></param>
        /// <returns></returns>
        public XElement GetTaskX(string TaskID,string DataFile = "DeviceList.xml",string OutputFile = "TaskResult.xml")
        {
            XElement Task = new XElement("MTTaSK",
                                //TOCHANGE
                                new XElement("TaskID", TaskID),
                                new XElement("TaskType", "0"),
                                //???
                                new XElement("TaskAction", "123"),

                                //TOCHANGE
                                new XElement("DataFile", DataFile),
                                new XElement("OutputFile", OutputFile)
                    );

            return Task;
        }

        /// <summary>
        /// 生成command
        /// </summary>
        /// <param name="CommandID">命令字编号，在每台秤的命令列表中必须唯一</param>
        /// <param name="CommandText">命令字，如Item</param>
        /// <param name="Control">命令控制字：Update：更新数据。Delete：删除指定数据。DeleteAll：删除全部数据。Read：读取当前数据。ReadAll：读取所有数据。其他所有Control节点均与此相同</param>
        /// <param name="ClearData">标志下发前是否清空数据，即是否先把秤内对应数据清空后再下发，仅在命令控制字为Write或Update时有效</param>
        /// <param name="DataFile">以文件方式调用时，存放命令字数据文件名</param>
        /// <returns></returns>
        public XElement GetCommandX(string CommandID, string CommandText = "Item", string Control = "Update",bool ClearData = false, string DataFile = "Data.xml")
        {
            //if (string.IsNullOrEmpty(CommandID))
            //{
            //    throw new Exception("CommandID 不能为空");
            //}
            XElement Command = new XElement("Commands",
                    new XElement("Command",
                        new XElement("CommandText"),
                        new XElement("CommandID", CommandID),
                        new XElement("Control", Control),
                        new XElement("ClearData", ClearData),
                        //
                        new XElement("DataFile", DataFile)
                ));
            return Command;
        }


        /// <summary>
        /// 获取data item
        /// </summary>
        /// <param name="PLU"></param>
        /// <returns></returns>
        public XElement GetItem(string PLU,string commodityName,string price)
        {
            XElement Item = new XElement("Item");

            Item.Add(new XElement("PLU", PLU));
            Item.Add(new XElement("DepartmentID"));
            Item.Add(new XElement("Descriptions"));
            Item.Add(new XElement("Dates"));
            Item.Add(new XElement("ItemGroupID"));
            Item.Add(new XElement("CategoryIDs"));
            Item.Add(new XElement("Tares"));
            Item.Add(new XElement("ItemPrices")); //商品价格列表  可以存放多个商品价格   目前我们这边只要一个价格
            Item.Add(new XElement("Taxes"));
            Item.Add(new XElement("Ingredients"));
            Item.Add(new XElement("Ingredients"));
            Item.Add(new XElement("LabelFormats"));
            Item.Add(new XElement("Barcodes"));
            Item.Add(new XElement("NutritionInformation"));
            Item.Add(new XElement("FixedQuantity"));
            Item.Add(new XElement("TraceInfoID"));
            Item.Add(new XElement("TraceabilityFlag"));
            Item.Add(new XElement("PriceRule"));
            Item.Add(new XElement("Images"));
            Item.Add(new XElement("StaggerPrices"));


            return Item;
        }

        public XElement Description(string CommodityName,string ID,string Language = "zho", string Type = "ItemName")
        {
            XElement description = new XElement("Description", 
                new object[] {
                    new XAttribute("Type", Type),
                    new XAttribute("ID", ID),
                    new XAttribute("Language", Language) });
            description.SetValue(CommodityName);
            return description;
        }

        public XElement ItemPrice(string price,string index = "0", string UnitOfMeasureCode = "KGM", bool PriceOverrideFlag = false, bool DiscountFlag = false, string Currency = "CNY")
        {
            XElement itemPrice = new XElement("Description", 
                new object[] {
                    new XAttribute("Index", index),
                    new XAttribute("UnitOfMeasureCode", UnitOfMeasureCode),
                    new XAttribute("PriceOverrideFlag", PriceOverrideFlag),
                    new XAttribute("Quantity","0"), //这里有可能会变成坑
                    new XAttribute("Currency",Currency)
                });
            itemPrice.SetValue(price);
            return itemPrice;
        }

        public XElement BarcodeID(string barcode)
        {
            return new XElement("BarcodeID", barcode);
        }






    }
}
