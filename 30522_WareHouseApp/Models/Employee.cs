using System;

namespace _30522_WareHouseApp.Models
{
    public class Employee : Person
    {
        public string Role { get; set; }
        public decimal Salary { get; set; }

        public override string GetRoleDetails()
        {
            return $"Employee Role: {Role}, Salary: {Salary:C2}";
        }
    }
}