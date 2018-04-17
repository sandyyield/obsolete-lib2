using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZlPos.Enums;
using ZlPos.Models;

namespace ZlPos.Manager
{
    public class LoginUserManager
    {
        private static LoginUserManager loginUserManager = null;
        private UserEntity userEntity;
        private bool login;
        private LoginTypeEnum loginType;
        private String requesttime;//上次更新商品信息时间

        private static readonly object obj = new object();

        private LoginUserManager()
        {
        }


        public static LoginUserManager Instance
        {
            get
            {
                lock (obj)
                {
                    if (loginUserManager == null)
                    {
                        loginUserManager = new LoginUserManager();
                    }
                    return loginUserManager;
                }
            }
        }

        public UserEntity UserEntity { get => userEntity; set => userEntity = value; }
        public bool Login { get => login; set => login = value; }
        public LoginTypeEnum LoginType { get => loginType; set => loginType = value; }
        public string Requesttime { get => requesttime; set => requesttime = value; }
    }
}
