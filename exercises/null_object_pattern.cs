using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
	Null object Pattern
	implement a NullLog class that implements ILog without throwing exceptions when used in Account class
*/

namespace Coding.Exercise
{ 
  public interface ILog
  {
    // maximum # of elements in the log
    int RecordLimit { get; }
    
    // number of elements already in the log
    int RecordCount { get; set; }

    // expected to increment RecordCount
    void LogInfo(string message);
  }

  public class Account
  {
    private ILog log;

    public Account(ILog log)
    {
      this.log = log;
    }

    public void SomeOperation()
    {
      int c = log.RecordCount;
      log.LogInfo("Performing an operation");
      if (c+1 != log.RecordCount)
        throw new Exception();
      if (log.RecordCount >= log.RecordLimit)
        throw new Exception();
    }
  }

  public class NullLog : ILog
  {
    // maximum # of elements in the log
    public int RecordLimit { get{return int.MaxValue;}}
    
    // number of elements already in the log
    public int RecordCount { get; set; }

    // expected to increment RecordCount
    public void LogInfo(string message){
	++RecordCount;
	}
  }
	
	public class Program
	{
		public static void Main()
		{
			var nulllog = new NullLog();
			var ac = new Account(nulllog);
			ac.SomeOperation();
		}
	}
}