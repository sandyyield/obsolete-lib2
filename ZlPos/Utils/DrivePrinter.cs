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
    }
}
