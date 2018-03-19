using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class DeviceEntity
    {
        public List<String> devices { get; set; }
        public PrinterConfigEntity config { get; set; }
    }
}
