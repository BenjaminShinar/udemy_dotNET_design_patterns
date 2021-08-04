using System;
using System.Numerics;

using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
	Strategy  Pattern
	Quadetric equation
	ax2 + bx + c =0
	x = -b +/- sqrt(b^2 - 4ac) / 2a;
	the "b^2 - 4*a*c" partcalled Discriminant.
	OrdinaryDiscriminantStrategy  returns as is.
	RealDiscriminantStrategy retuns Nan if negative
	it was important to use complex.sqrt() here rather than math.sqrt();

*/

namespace Coding.Exercise
{ 
  public interface IDiscriminantStrategy
  {
    double CalculateDiscriminant(double a, double b, double c);
  }

  public class OrdinaryDiscriminantStrategy : IDiscriminantStrategy
  {
    public double CalculateDiscriminant(double a, double b, double c)
	{
	var calc = Math.Pow(b,2) - 4* a* c;
	return calc;
	}
  }

  public class RealDiscriminantStrategy : IDiscriminantStrategy
  {
    public double CalculateDiscriminant(double a, double b, double c)
	{
	var calc = Math.Pow(b,2) - 4* a* c;
	if (calc < 0) return Double.NaN;
	return calc;
	}
  }

  public class QuadraticEquationSolver
  {
    private readonly IDiscriminantStrategy strategy;

    public QuadraticEquationSolver(IDiscriminantStrategy strategy)
    {
      this.strategy = strategy;
    }

    public Tuple<Complex, Complex> Solve(double a, double b, double c)
    {
      var Discriminant = strategy.CalculateDiscriminant(a,b,c);
		Complex c1 = (-b+Complex.Sqrt(Discriminant))/(2*a);
		Complex c2 = (-b-Complex.Sqrt(Discriminant))/(2*a);
		return new Tuple<Complex,Complex>(c1,c2);

    }
  }
	
	public class Program
	{
		public static void Main()
		{
        var ordinarySolver = new QuadraticEquationSolver(new OrdinaryDiscriminantStrategy());
		var realSolver = new QuadraticEquationSolver(new RealDiscriminantStrategy());
        Console.WriteLine(ordinarySolver.Solve(1, 4, 5));			
		Console.WriteLine(realSolver.Solve(1, 4, 5));			
		}
	}
}