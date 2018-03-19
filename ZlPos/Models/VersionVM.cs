using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    class VersionVM
    {
        public ResultBean result { get; set; }
        public String success { get; set; }

    }

    class ResultBean
    {
        public String id { get; set; }
        public String version { get; set; }
        public String packageX { get; set; }
        public String type { get; set; }
        public String changelog { get; set; }
        public String status { get; set; }
        public String publishtime { get; set; }
    }
}
