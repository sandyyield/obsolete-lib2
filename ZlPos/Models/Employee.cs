using SqlSugar;

namespace ZlPos.Models
{
    public class Employee
    {
        [SugarColumn(IsNullable = false,IsPrimaryKey = true, IsIdentity = true)]
        public int EmployeeID { get; set; }
        //[SugarMapping(ColumnName = "FirstName")]
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
