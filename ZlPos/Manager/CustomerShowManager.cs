using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Utils;

namespace ZlPos.Manager
{
    public class CustomerShowManager
    {
        private static CustomerShowManager _customerShowManager = null;
        

        private static readonly object obj = new object();

        private CustomerShowManager()
        {
        }


        public static CustomerShowManager Instance
        {
            get
            {
                lock (obj)
                {
                    if (_customerShowManager == null)
                    {
                        _customerShowManager = new CustomerShowManager();
                    }
                    return _customerShowManager;
                }
            }
        }

        public bool Init { get; internal set; }
        public serialPort SerialPort { get; internal set; }
    }
}
