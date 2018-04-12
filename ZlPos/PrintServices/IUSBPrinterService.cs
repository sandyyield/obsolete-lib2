using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.PrintServices
{
    interface IUSBPrinterService
    {
        void Print(object state);
    }
}
