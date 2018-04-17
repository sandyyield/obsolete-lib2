using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZlPos.Models
{
    public class UserVM
    {
        /// <summary>
        /// 
        /// </summary>
        public UserEntity user_info { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ShopConfigEntity config { get; set; }
    }
}
