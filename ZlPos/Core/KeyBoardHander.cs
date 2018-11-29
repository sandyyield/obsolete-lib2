using CefSharp;
using System.Windows.Forms;
using ZlPos.Bizlogic;

namespace ZlPos.Core
{
    internal class KeyBoardHander : IKeyboardHandler
    {
        public bool OnKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey)
        {
            return false;
        }

        public bool OnPreKeyEvent(IWebBrowser browserControl, IBrowser browser, KeyType type, int windowsKeyCode, int nativeKeyCode, CefEventFlags modifiers, bool isSystemKey, ref bool isKeyboardShortcut)
        {
            if (AppContext.Instance.Debug)
            {

                if (KeyType.RawKeyDown == type)
                {
                    if (windowsKeyCode == (int)Keys.F5)
                    {
                        browser.Reload(); //此处可以添加想要实现的代码段
                    }
                    if (windowsKeyCode == (int)Keys.F12)
                    {
                        browser.ShowDevTools();
                    }
                }
            }
            return false;
        }
    }
}