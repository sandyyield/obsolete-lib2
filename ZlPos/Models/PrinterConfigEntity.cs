using SqlSugar;

namespace ZlPos.Models
{
    public class PrinterConfigEntity
    {
        //public int id { get; set; }
        private int id = 1;
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false, IsNullable = false)]
        public int Id { get => id; set => id = value; }

        [SugarColumn(IsNullable = true)]
        public string printerType { get; set; }

        [SugarColumn(IsNullable = true)]
        public string pageWidth { get; set; }

        //public object PageWidth { get; internal set; }
        [SugarColumn(IsNullable = true)]
        public string port { get; set; }

        [SugarColumn(IsNullable = true)]
        public string intBaud { get; set; }

        [SugarColumn(IsNullable = true)]
        public string deviceId { get; set; }

        [SugarColumn(IsNullable = true)]
        public string usbSystem { get; set; }

        [SugarColumn(IsNullable = true)]
        public string printerBrand { get; set; }

        //[SugarColumn(IsNullable = true)]
        //public string PrinterBrand { get;  set; }

        //add 2018/01/15 打印小票份数
        [SugarColumn(IsNullable = true)]
        public string printernumber { get; set; }
        
    }
}
