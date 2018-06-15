using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Enums
{
    public class SyncScaleStatus
    {
        public const string SOCKET_OPENING = "SOCKET_OPENING";
        public const string SOCKET_OPENED = "SOCKET_OPENED";
        public const string SOCKET_NO_OPEN = "SOCKET_NO_OPEN";

        public const string PLU_CLEANING = "PLU_CLEANING";
        public const string PLU_CLEANED = "PLU_CLEANED";

        public const string PLU_SYNCING = "PLU_SYNCING";
        public const string PLU_SYNCED = "PLU_SYNCED";
    }
}
