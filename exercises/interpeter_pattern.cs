using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coding.Exercise
{
	/*
Interpeter Pattern
implement ExpressionProcessor.Calculate()
need to handle numbers, names variables (x,y) and plus Minus Operators.
0 means error

this was a mess, the sample code was bad and I had to recreate parts of it myself.

  	*/

		public class Token
		{
			public enum Type
			{
				Integer, Plus, Minus, Variable
			}
			
			public Type TokenType{get;}
			public string Text {get;}
			public Token (Type tokenType,string text)
			{
				TokenType=tokenType;
				Text=text;
			}
			public override string ToString()
			{
				return $"`{Text}'";
			}
		} // Token Class
    public class ExpressionProcessor
    {
      public Dictionary<char, int> Variables = new Dictionary<char, int>();

      public int Calculate(string expression)
      {
		  var tokens = Lex(expression);
		  //Console.WriteLine(String.Join(", ",tokens));
		  var elem = Parse(tokens);
		  
        return elem.Value;
	  }
		

		
		 private List<Token> Lex(string input)
		{
			var results = new List<Token>();
		
			for (int i =0; i < input.Length ; ++i)
			{
				char c = input[i];
				switch(c)
				{
					case '+':
					results.Add(new Token(Token.Type.Plus,"+"));
					break;
					case '-':
					results.Add(new Token(Token.Type.Minus,"-"));
					break;
					default:
						if (!char.IsDigit(c))
						{
							if (Variables.ContainsKey(c))
							{
								results.Add(new Token(Token.Type.Integer,Variables[c].ToString()));
							}
							else
							{
								return new List<Token>{};
							}
						}
						else
						{	
							var sb = new StringBuilder(c.ToString());
							for (int j=i+1; j <input.Length;++j)
							{
								if (char.IsDigit(input[j]))
								{
									sb.Append(input[j]);
									++i; //this is for the next big loop!
								}
								else
								{
										break; //break the char processing loop!
								}
							}
							results.Add(new Token(Token.Type.Integer,sb.ToString()));

						}
						break; //break the switch statement.				
				}
			}
			 //Console.WriteLine(String.Join(",",results));

			return results;	
			}
		
		  public interface IElement
  {
    int Value { get; }
  }

		//copied straight from the lecture
  public class Integer : IElement
  {
    public Integer(int value)
    {
      Value = value;
    }

    public int Value { get; }
  }

  public class BinaryOperation : IElement
  {
    public enum Type
    {
      Addition,
      Subtraction
    }

    public Type MyType;
    public IElement Left, Right;

    public int Value
    {
      get
      {
        switch (MyType)
        {
          case Type.Addition:
				//Console.WriteLine($"adding {Left.Value} {Right.Value}");

            return Left.Value + Right.Value;
          case Type.Subtraction:
				//Console.WriteLine($"substracting {Left.Value} {Right.Value}");
            return Left.Value - Right.Value;
          default:
            throw new ArgumentOutOfRangeException();
        }
      }
    }
  }
		static IElement Parse(IReadOnlyList<Token> tokens)
    {
			var dq = new List<Token>(tokens);
			if (dq.Count() ==0)
			{ 
				return new Integer(0);
			}
			while (dq.Count() > 2)
			{
				var result = new BinaryOperation();
				
				result.Left = new Integer(int.Parse(dq[0].Text));
				result.Right = new Integer(int.Parse(dq[2].Text));

				//Console.WriteLine($"left {result.Left.Value} right {result.Right.Value}");

				var op = dq[1];
				      switch (op.TokenType)
				{
				  case Token.Type.Plus:
					result.MyType = BinaryOperation.Type.Addition;
					break;
				  case Token.Type.Minus:
					result.MyType = BinaryOperation.Type.Subtraction;
					break;
				  default:
					throw new ArgumentOutOfRangeException();
				}
				dq.RemoveRange(0,3);
				//dq.Enqueue(new Token(Token.Type.Plus,"+"));
				dq.Insert(0,new Token(Token.Type.Integer,result.Value.ToString()));
			}

      return new Integer(int.Parse(dq[0].Text));
    }
	}
	
	public class Program
	{
		public static void Main()
		{
			var EP = new ExpressionProcessor();
			var strings = new List<string>{"1+2+3",
				"1+2+xy","10-2-x","30-6"};
			EP.Variables['x'] = 3;
			foreach (var s in strings)
			{
			Console.WriteLine($"{s} is {EP.Calculate(s)}");
			}

		}
	}
}