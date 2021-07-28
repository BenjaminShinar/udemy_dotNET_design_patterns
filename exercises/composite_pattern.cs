using System;
using System.Collections;
using System.Collections.Generic;

namespace Coding.Exercise
{
	/*
   complete the intrfaces so that the sum extention methods will work
  	*/

    //this was the important part: the interface in IEnumerable of int.
    public interface IValueContainer : IEnumerable<int>
    {

    }

    public class SingleValue : IValueContainer
    {
	 public IEnumerator<int> GetEnumerator()
	 {
	   yield return Value;
	 }
	 
	 IEnumerator IEnumerable.GetEnumerator()
	 {
	   return GetEnumerator();
	 }
	 
      public int Value;

    }

    public class ManyValues : List<int>,IValueContainer
    {
        //nothing to do here!
    }

    public static class ExtensionMethods
    {

      public static int Sum(this List<IValueContainer> containers)
      {
        int result = 0;
        foreach (var c in containers)
        foreach (var i in c)
          result += i;
        return result;
      }
    }
 	
	public class Program
	{
		public static void Main()
		{
			var l = new List<IValueContainer>
			{
			new SingleValue{Value =19},
			new SingleValue{Value =6},
			new ManyValues{1,2,3,4}
			};
			Console.WriteLine($"sum is {l.Sum()}");
		}
	}
}