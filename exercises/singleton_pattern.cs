using System;

namespace Coding.Exercise
{
	/*
    implement a method that takes a factory method (a method that returns an object from a class) and checks if the object is a singleton object
  */
	
	public class Point
    {
      public int X, Y;
    }

    public class Line
    {
      public Point Start, End;

    }
	
	public class SingletonClass
    {
      		private SingletonClass()
		{
		}
		public static SingletonClass Instance{get;} =new SingletonClass();
    }
	
	
	public class SingletonTester
    {
      public static bool IsSingleton(Func<object> func)
      {
        var a = func();
		  var b = func();
		  return a.Equals(b);
      }
    }
	
	public class Program
	{
		public static void Main()
		{

			var makeSin =  new Func<object>(()=> SingletonClass.Instance);
			var makeLine = new Func<object>(()=>new Line());
			var makeString = new Func<object>(()=> "123");

			Console.WriteLine($"is makeLine singleton? {SingletonTester.IsSingleton(makeLine)}");
			Console.WriteLine($"is makeString singleton? {SingletonTester.IsSingleton(makeString)}");
			Console.WriteLine($"is makeSin singleton? {SingletonTester.IsSingleton(makeSin)}");

		}
	}
	
}