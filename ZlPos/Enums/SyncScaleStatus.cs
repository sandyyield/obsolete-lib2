using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Enums
{
    public enum SyncScaleStatus
    {
        SOCKET_OPENING,
        SOCKET_OPENED,
        SOCKET_NO_OPEN,

        PLU_CLEANING,
        PLU_CLEANED,

        PLU_SYNCING,
        PLU_SYNCED,
    }
}
