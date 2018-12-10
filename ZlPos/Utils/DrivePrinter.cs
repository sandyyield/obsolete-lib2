using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;

namespace ZlPos.Utils
{
    /// <summary>
    /// 驱动打印
    /// </summary>
    public class DrivePrinter
    {
        public string printerName { get; internal set; }

        public string pageWidth { get; set; }

        private static DrivePrinter _DrivePrinter { get; set; }

        private static PrintDocument _PrintDocument { get; set; }

        private static Queue<string> _PrintQueue = new Queue<string>();

        private DrivePrinter()
        {
        }

        public static DrivePrinter Instance
        {
            get
            {
                if (_DrivePrinter == null)
                {
                    _DrivePrinter = new DrivePrinter();
                }
                if(_PrintDocument == null)
                {
                    _PrintDocument = new PrintDocument();

                    _PrintDocument.PrintPage += new PrintPageEventHandler(DoPrint);
                    //_PrintDocument.PrintPage += new PrintPageEventHandler(DoPrint2);
                }
                return _DrivePrinter;
            }
        }

        internal void SetPrinterName()
        {
            _PrintDocument.PrinterSettings.PrinterName = printerName;
        }

        private static void DoPrint(object sender, PrintPageEventArgs e)
        {
            Font titleFont = new Font("宋体", 8, FontStyle.Bold);//标题字体                   
            Brush brush = new SolidBrush(Color.Black);//画刷                    
            Point po = new Point(1, 1);
            try
            {
                while(_PrintQueue.Count > 0)
                {
                    e.Graphics.DrawString(_PrintQueue.Dequeue(), titleFont, brush, po);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("打印出错");
            }
        }


        private static void DoPrint2(object sender,PrintPageEventArgs e)
        {
            try
            {

                Graphics g = e.Graphics;

                float linesPerPage = 0; //页面的行号

                float yPosition = 0;   //绘制字符串的纵向位置

                int count = 0; //行计数器

                float leftMargin = e.MarginBounds.Left; //左边距

                float topMargin = e.MarginBounds.Top; //上边距

                string line = null; //行字符串

                Font printFont = new Font("宋体", 9, FontStyle.Regular);//正文文字           


                SolidBrush myBrush = new SolidBrush(Color.Black);//刷子

                linesPerPage = e.MarginBounds.Height / printFont.GetHeight(g);//每页可打印的行数

                //逐行的循环打印一页

                while (count < linesPerPage && ((line = _PrintQueue.Dequeue()) != null))

                {

                    yPosition = topMargin + (count * printFont.GetHeight(g));

                    g.DrawString(line, printFont, myBrush, 1, yPosition, new StringFormat());

                    count++;

                }
            }catch(Exception ex)
            {
                Console.WriteLine("drive print err", ex);
            }
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

        internal void Print(string v)
        {
            _PrintQueue.Enqueue(v);

            _PrintDocument.Print();
        }

        internal void PrintString(string v)
        {
            Print(v);
        }

        //插队处理
        public void EnqueuePrint(string base64)
        {

        } 
    }
}
