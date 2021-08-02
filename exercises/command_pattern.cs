using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coding.Exercise
{
	/*
Command Pattern
implement Account.Process()
success if action was ok.
can't withdraw more than what we have.
  	*/

    public class Command
    {
      public enum Action
      {
        Deposit,
        Withdraw
      }

      public Action TheAction;
      public int Amount;
      public bool Success;
    }

    public class Account
    {
      public int Balance { get; set; }

      public void Process(Command c)
      {
        switch (c.TheAction)
		{
			case Command.Action.Deposit:
				{
					Balance += c.Amount;
					c.Success = true;
					break;
				}
			case Command.Action.Withdraw:
				{
					if (Balance - c.Amount >=0)
					{
					Balance -= c.Amount;
					c.Success = true;
					}
					else
					{
						c.Success=false;
					}
					break;
				}
			default:
				break;
		}
      }
    }
	
	public class Program
	{
		public static void Main()
		{
			var ba = new Account();
			Console.WriteLine($"account has {ba.Balance}");
			var c1 = new Command(){TheAction = Command.Action.Deposit,Amount =50};
			ba.Process(c1);
			Console.WriteLine($"account has {ba.Balance}");
			
			var c2 = new Command(){TheAction = Command.Action.Withdraw,Amount =25};
			ba.Process(c2);
			Console.WriteLine($"account has {ba.Balance}");
			
			var c3 = new Command(){TheAction = Command.Action.Withdraw,Amount =75};
			ba.Process(c3);
			Console.WriteLine($"account has {ba.Balance}");
		}
	}
}