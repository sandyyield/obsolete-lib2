using SqlSugar;

namespace ZlPos.Models
{
    public class UserEntity
    {
        [SugarColumn(IsNullable = true)]
        public string membermodel { get; set; }
        /// <summary>
        /// mk
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true, IsIdentity = false,ColumnName = "id")]
        public string userid { get; set; }
        [SugarColumn(IsNullable = true)]
        public string isbuyprice { get; set; }
        [SugarColumn(IsNullable = true)]
        public string branchcode { get; set; }
        /// <summary>
        /// 苏州总店
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string shopname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string cashiergrant { get; set; }
        [SugarColumn(IsNullable = true)]
        public string mindiscount { get; set; }
        [SugarColumn(IsNullable = true)]
        public string headpic { get; set; }
        [SugarColumn(IsNullable = true)]
        public string username { get; set; }
        [SugarColumn(IsNullable = true)]
        public string ishead { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopstate { get; set; }
        [SugarColumn(IsNullable = true)]
        public string account { get; set; }
        [SugarColumn(IsNullable = true)]
        public string shopcode { get; set; }
        /// <summary>
        /// 总部
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string branchname { get; set; }
        /// <summary>
        /// 管理员
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string fullname { get; set; }
        [SugarColumn(IsNullable = true)]
        public string access_code { get; set; }
        [SugarColumn(IsNullable = true)]
        public string password { get; set; }

    }
}
