using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using ZlPos.Models;
using ZXing;
using ZXing.Common;

namespace ZlPos.Utils
{
    class DrivePrinterBJQ
    {
        public string printerName { get; internal set; }

        public string pageWidth { get; set; }

        //private static DrivePrinterBQ _DrivePrinter { get; set; }

        private PrintDocument _PrintDocument { get; set; }

        private static Queue<TemplatePropertyEntity> _PrintQueue = new Queue<TemplatePropertyEntity>();

        public DrivePrinterBJQ()
        {
            if (_PrintDocument == null)
            {
                _PrintDocument = new PrintDocument();

                _PrintDocument.PrintPage += new PrintPageEventHandler(DoPrint2);
            }
        }


        internal void SetPrinterName()
        {
            _PrintDocument.PrinterSettings.PrinterName = printerName;
        }

        //private void DoPrint(object sender, PrintPageEventArgs e)
        //{
        //    Font titleFont = new Font("宋体", 8, FontStyle.Bold);//标题字体                   
        //    Brush brush = new SolidBrush(Color.Black);//画刷                    
        //    Point po = new Point(1, 1);
        //    try
        //    {
        //        while (_PrintQueue.Count > 0)
        //        {
        //            e.Graphics.DrawString(_PrintQueue.Dequeue(), titleFont, brush, po);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("打印出错");
        //    }
        //}


        private static void DoPrint2(object sender, PrintPageEventArgs e)
        {
            try
            {

                Graphics g = e.Graphics;

                float linesPerPage = 0; //页面的行号

                float yPosition = 0;   //绘制字符串的纵向位置

                int count = 0; //行计数器

                float leftMargin = e.MarginBounds.Left; //左边距

                float topMargin = e.MarginBounds.Top; //上边距

                //string line = null; //行字符串

                Font printFont = new Font("宋体", 9, FontStyle.Bold);//正文文字           


                SolidBrush myBrush = new SolidBrush(Color.Black);//刷子

                //linesPerPage = e.MarginBounds.Height / printFont.GetHeight(g);//每页可打印的行数

                //逐行的循环打印一页

                while (_PrintQueue.Peek() != null)
                {
                    var line = _PrintQueue.Dequeue();

                    if (line.isBarCode == 1 && !string.IsNullOrEmpty(line.text))
                    {
                        //g.DrawString(CreateBarcodePicture, font, myBrush, float.Parse(line.directionX) * 4, float.Parse(line.directionY) * 4, new StringFormat());
                        g.DrawImage(CreateBarcodePicture(line.text, 50, 32), float.Parse(line.directionX) * 3, float.Parse(line.directionY) * 3);
                    }
                    else
                    {

                        yPosition = topMargin + (count * printFont.GetHeight(g));

                        if ("2".Equals(line.font.Split(',')[0]))
                        {
                            printFont = new Font("宋体", 18, FontStyle.Bold);
                        }
                        else
                        {
                            printFont = new Font("宋体", 9, FontStyle.Bold);//正文文字           
                        }
                        g.DrawString(line.text, printFont, myBrush, float.Parse(line.directionX) * 3, float.Parse(line.directionY) * 3, new StringFormat());
                    }

                    count++;

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("drive print err", ex);
            }
        }


        /// <summary>
        /// 生成一维条形码
        /// </summary>
        /// <param name="text">内容</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <returns></returns>
        public static Bitmap CreateBarcodePicture(string text, int width, int height)
        {
            BarcodeWriter writer = new BarcodeWriter();
            //使用ITF 格式，不能被现在常用的支付宝、微信扫出来
            //如果想生成可识别的可以使用 CODE_128 格式
            //writer.Format = BarcodeFormat.ITF;
            writer.Format = BarcodeFormat.CODE_128;
            EncodingOptions options = new EncodingOptions()
            {
                Width = width,
                Height = height,
                Margin = 2
            };
            writer.Options = options;
            Bitmap map = writer.Write(text);
            return map;
        }


        //private void DrawStringWrap(Graphics graphic, Font font, string text, Rectangle recangle)
        //{
        //    List<string> textRows = GetStringRows(graphic, font, text, recangle.Width);
        //    int rowHeight = (int)(Math.Ceiling(graphic.MeasureString(\"测试\", font).Height));
        //    int maxRowCount = recangle.Height / rowHeight;
        //    int drawRowCount = (maxRowCount < textRows.Count) ? maxRowCount : textRows.Count;
        //    int top = (recangle.Height - rowHeight * drawRowCount) / 2;
        //    StringFormat sf = new StringFormat();
        //    sf.Alignment = StringAlignment.Near;
        //    sf.LineAlignment = StringAlignment.Center;
        //    for (int i = 0; i < drawRowCount; i++)
        //    {
        //        Rectangle fontRectanle = new Rectangle(recangle.Left, top + rowHeight * i, recangle.Width, rowHeight);
        //        graphic.DrawString(textRows, font, new SolidBrush(Color.Black), fontRectanle, sf);
        //    }
        //}

        internal void Print(object v)
        {
            //_PrintQueue.Enqueue(v);

            _PrintDocument.Print();
        }

        internal void PrintString(string v)
        {
            var lst = JsonConvert.DeserializeObject<List<List<TemplatePropertyEntity>>>(v);

            foreach (var printLst in lst)
            {
                foreach (var its in printLst)
                {
                    _PrintQueue.Enqueue(its);
                }

                _PrintDocument.Print();
            }

            ////插队处理
            //public void EnqueuePrint(object v)
            //{
            //    //_PrintQueue.Enqueue(v);
            //}
        }
    }
}
