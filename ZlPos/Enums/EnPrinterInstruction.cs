using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Enums
{
    public enum EnPrinterInstruction
    {
        PI_SeletCutModeAndCutPaper,//1D 56 M N
        PI_SelectPrintMode,//1B 21 N
        PI_PrintDownLoadedBMP,//1D 2F M
        PI_GenerateDrawerPlse,//1B 70 M T1 T2
        PI_PrintSingleBeeper,//1B 42 N T
        PI_PrintSingleBeeperAndAlarmLightFlashes//1B 43 M T N
    }
}
