using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ZlPos.Core
{
    class MyDownLoadFile : IDownloadHandler
    {
        public void OnBeforeDownload(IBrowser browser, DownloadItem downloadItem, IBeforeDownloadCallback callback)
        {
            if (!callback.IsDisposed)
            {
                using (callback)
                {
                    callback.Continue(Application.StartupPath +
                            @"\Downloads\" +
                            downloadItem.SuggestedFileName,
                        showDialog: false);
                }
            }

        }

        public void OnDownloadUpdated(IBrowser browser, DownloadItem downloadItem, IDownloadItemCallback callback)
        {
            if (downloadItem.IsComplete)
            {
                MessageBox.Show("下载完成");
            }
        }
    }
}
