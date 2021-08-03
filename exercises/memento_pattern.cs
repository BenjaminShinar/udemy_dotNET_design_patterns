using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
	implement a Memento object. oattern
	Token contains a number, the token list represents the date stored.
	the object is mutated by adding values/ tokens.
	reverting simply restores the token list.
    we need to do some deep copying when creating tokens because of how c# works.

*/

namespace Coding.Exercise
{ 
      public class Token
  {
    public int Value = 0;

    public Token(int value)
    {
      this.Value = value;
    }
  }

  public class Memento
  {
    	public List<Token> Tokens;
	 
	  public Memento (List<Token> tokens)
	  {
		  this.Tokens = tokens.Select(t=> new Token(t.Value)).ToList();
	  }
  }

  public class TokenMachine
  {
    public List<Token> Tokens = new List<Token>();

    public Memento AddToken(int value)
    {
      return AddToken(new Token(value));
    }

    public Memento AddToken(Token token)
    {
		Tokens.Add(token);
		return new Memento(Tokens);
      // todo (yes, please do both overloads)
    }

    public void Revert(Memento m)
    {
       this.Tokens = m.Tokens.Select(t=> new Token(t.Value)).ToList();
    }
  }
	
	public class Program
	{
		public static void Main()
		{
			var tm = new TokenMachine();
			var mem1 = tm.AddToken(1);
			Console.WriteLine($"tokens {string.Join(",",tm.Tokens.Select(t=>t.Value))}");
			tm.AddToken(2);
			tm.AddToken(3);
			tm.AddToken(4);
			Console.WriteLine($"tokens {string.Join(",",tm.Tokens.Select(t=>t.Value))}");
			tm.Revert(mem1);
			Console.WriteLine($"tokens {string.Join(",",tm.Tokens.Select(t=>t.Value))}");
		}
	}
}