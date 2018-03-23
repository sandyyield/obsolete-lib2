using SqlSugar;

namespace ZlPos.Models
{
    /// <summary>
    /// 测试用的类
    /// </summary>
    //[SugarMapping(TableName = "Employee")]
    //[SugarTable(TableName = "Employee")]
    public class Employee
    {
        [SugarColumn(IsNullable = false,IsPrimaryKey = true, IsIdentity = false)]
        public int EmployeeID { get; set; }
        //[SugarMapping(ColumnName = "FirstName")]
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
