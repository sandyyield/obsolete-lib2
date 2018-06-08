using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class BarcodeScaleConfigEntity
    {
        public string barcodeStyle { get; set; }
        public List<BarcodeScaleEntity> barcodeScaleEntityList { get; set; }
    }
}
