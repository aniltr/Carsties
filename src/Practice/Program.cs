// See https://aka.ms/new-console-template for more information
var developer = new Employee { Id = 1001, Age = 45, FirstName = "Bob", LastName = "Smith", DateOfBirth = DateTime.Parse("1981-05-10T11:30:00") };
Console.WriteLine("Single Developer");
Console.WriteLine("****************");
Console.WriteLine($"Id: {developer.Id}");
Console.WriteLine($"Full name: {developer.FirstName}");
Console.WriteLine($"Age: {developer.Age}");
Console.WriteLine($"Date of birth: {developer.DateOfBirth}");

Console.WriteLine("DevTeam: Platypus");
Console.WriteLine("*****************");

public class Employee()
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public DateTime DateOfBirth { get; set; }
}
