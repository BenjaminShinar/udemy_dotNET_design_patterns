using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
	State  Pattern
	a combination lock. we can use 'Status' to check the state of the lock, if it's still locked we get back the code we entered so far.
	open, Locked,error states.
	annoting because we need manage status all the time
*/

namespace Coding.Exercise
{ 
  public class CombinationLock
  {
	  enum LockState
	  {
		  Locked,
		  Error,
		  Unlocked
	  }
	private readonly string code;
	private LockState lockState = LockState.Locked;
	private bool firstTime= true;  
    public CombinationLock(int [] combination)
    {
      code = String.Join("",combination);
    }

    // you need to be changing this on user input
    public string Status {get; private set;} = "LOCKED";

    public void EnterDigit(int digit)
    {
		if (firstTime)
		{
			Status=string.Empty;
			firstTime= false;
		}
        if (lockState == LockState.Unlocked) return;
		if (lockState == LockState.Error)
		{
			lockState =	LockState.Locked;
			Status = string.Empty;
		}
		Status += digit.ToString();
		if (Status == code)
		{
			lockState =	LockState.Unlocked;
			Status = "OPEN";
			return;
		}
		if (!code.StartsWith(Status))
		{
			lockState =	LockState.Error;
			Status = "ERROR";
		}
    }
  }
	
	public class Program
	{
		public static void Main()
		{

			
			var cl = new CombinationLock(new []{1,2,3,4,5});
			Console.WriteLine($"{cl.Status}");
			cl.EnterDigit(1);
			cl.EnterDigit(7);
			Console.WriteLine($"{cl.Status}");
			Console.WriteLine($"{cl.Status}");
			cl.EnterDigit(1);
			Console.WriteLine($"{cl.Status}");
			cl.EnterDigit(2);
			cl.EnterDigit(3);
			cl.EnterDigit(4);
			Console.WriteLine($"{cl.Status}");
			cl.EnterDigit(5);
			Console.WriteLine($"{cl.Status}");
			

			var cl2 = new CombinationLock(new []{1,2,3});
			Console.WriteLine($"{cl2.Status}");
			cl2.EnterDigit(1);
			Console.WriteLine($"{cl2.Status}");
			cl2.EnterDigit(2);
			Console.WriteLine($"{cl2.Status}");
			cl2.EnterDigit(7);
			Console.WriteLine($"{cl2.Status}");
		}
	}
}