using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class CardEntity
    {
        public int id { get; set; }//mk

        public String countcardid { get; set; }

        public String countcardnums { get; set; }

        public String countcardamount { get; set; }
    
        public String recordid { get; set; }

        public String uid { get; set; }//外键
    }
}
