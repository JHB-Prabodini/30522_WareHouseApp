using System;

namespace _30522_WareHouseApp.Models
{
    // Abstract Base Class (Demonstrates Encapsulation & Abstraction)
    public abstract class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // Abstract method (Demonstrates Polymorphism across child classes)
        public abstract string GetRoleDetails();
    }
}