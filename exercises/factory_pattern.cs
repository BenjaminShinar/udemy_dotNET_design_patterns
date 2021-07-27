using System;

namespace Coding.Exercise
{
	/*
    Implement a non-static PersonFactory that has a CreatePerson that takes a person Name and creates a person, the ID of the variable should be incremental and start a zero
  */
  public class Person
  {
    public int Id { get; set; }
    public string Name { get; set; }
  }

	public class PersonFactory
	{
		private int NextId =0;
		public Person CreatePerson(string name)
		{
			return new Person{Id = NextId++
				,Name=name};
		}
	}
	public class Program
	{
		public static void Main()
		{
			var pc = new PersonFactory();
			PrintPerson(pc.CreatePerson("danny"));
			PrintPerson(pc.CreatePerson("joe"));
		}
		public static void PrintPerson(Person person)
		{
			Console.WriteLine($"name:{person.Name}  Id:{person.Id}");

		}
	}
	
}