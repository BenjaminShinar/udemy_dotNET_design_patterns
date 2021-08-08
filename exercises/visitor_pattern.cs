using System;
using System.Text;

namespace Coding.Exercise
{

/*
Visitor Pattern, expression printer for values, addition and multipication


*/
public class Program
{
	public static void Main(string[] args)
	{
	var simple = new AdditionExpression(new Value(1),new Value(3));
		var ep = new ExpressionPrinter();
		ep.Visit(simple);
		Console.WriteLine($"exprsiion is {ep}");
	}
}
  public abstract class ExpressionVisitor
  {
    public abstract void Visit(Expression ex);
	  public abstract void Visit(AdditionExpression ae);
	  public abstract void Visit(MultiplicationExpression me);
	  public abstract void Visit(Value ve);
  }

  public abstract class Expression
  {
    public abstract void Accept(ExpressionVisitor ev);
  }

  public class Value : Expression
  {
    public readonly int TheValue;

    public Value(int value)
    {
      TheValue = value;
    }
	public override void Accept(ExpressionVisitor ex)
	{
		ex.Visit(this);
	}
    // todo
  }

  public class AdditionExpression : Expression
  {
    public readonly Expression LHS, RHS;

    public AdditionExpression(Expression lhs, Expression rhs)
    {
      LHS = lhs;
      RHS = rhs;
    }

	public override void Accept(ExpressionVisitor ex)
	{
		ex.Visit(this);
	}
    // todo
  }

  public class MultiplicationExpression : Expression
  {
    public readonly Expression LHS, RHS;

    public MultiplicationExpression(Expression lhs, Expression rhs)
    {
      LHS = lhs;
      RHS = rhs;
    }

	public override void Accept(ExpressionVisitor ex)
	{
		ex.Visit(this);
	}
    // todo
  }

  public class ExpressionPrinter : ExpressionVisitor
  {
  	private StringBuilder sb = new StringBuilder();
	public override void Visit(Expression ex)
    {
      ex.Accept(this);
    }
	
    public override void Visit(Value value)
    {

      sb.Append(value.TheValue);
    }

    public override void Visit(AdditionExpression ae)
    {

		  sb.Append("(");
		  Visit(ae.LHS);
		  sb.Append("+");
		  Visit(ae.RHS);
		  sb.Append(")");
    }

    public override void Visit(MultiplicationExpression me)
    {
		  Visit(me.LHS);
		  sb.Append("*");
		  Visit(me.RHS);
    }

    public override string ToString()
    {
      return sb.ToString();
    }
  }
}
	
