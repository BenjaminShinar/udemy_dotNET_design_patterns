using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coding.Exercise
{
	/*
Proxy
write the 'ResposiblePerson' Proxy class that checks the age property and behaves Accordingly
   can't drink below 18, can't drive below 16, if drink and drive, dead.
  	*/

    public class Person
    {
      public int Age { get; set; }

      public string Drink()
      {
        return "drinking";
      }

      public string Drive()
      {
        return "driving";
      }

      public string DrinkAndDrive()
      {
        return "driving while drunk";
      }
    }

    public class ResponsiblePerson
    {
	  private Person person;
      public ResponsiblePerson(Person person)
      {
        this.person=person;
      }
      
		public string Drink()
      {
			if (Age >= 18)
			{
        		return person.Drink();
			}
			else
			{
				return "too young";
			}
      }

      public string Drive()
      {
		  if (Age >= 16)
			{
        		return person.Drive();
			}
			else
			{
				return "too young";
			}
      }

      public string DrinkAndDrive()
      {
        return "dead";
      }
		
      public int Age {get=>person.Age; set=>person.Age=value;}
    }
	
	public class Program
	{
		public static void Main()
		{
			var rp = new ResponsiblePerson(new Person(){Age = 15});
			
			Console.WriteLine(rp.Drink());
			Console.WriteLine(rp.Drive());
			
			rp.Age=18;
			Console.WriteLine($"now {rp.Age}");
						
			Console.WriteLine(rp.Drink());
			Console.WriteLine(rp.Drive());
			Console.WriteLine(rp.DrinkAndDrive());
		}
	}
}