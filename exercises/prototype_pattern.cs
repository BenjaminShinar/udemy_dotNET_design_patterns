using System;

namespace Coding.Exercise
{
	/*
    Implement a Line.DeepCopy() method that performs a deepcopy of the Line object
  */
	
	public class Point
    {
      public int X, Y;
		public Point DeepCopy()
		{
			return new Point{X=X,Y=Y};
		}
    }

    public class Line
    {
      public Point Start, End;

      public Line DeepCopy()
      {
		  return new Line{
				Start=Start.DeepCopy(),
				End=End.DeepCopy()};
      }
    }
	
	public class Program
	{
		public static void Main()
		{
			var l1 = new Line{
				Start=new Point{X=1,Y=2},
				End=new Point{X=4,Y=5}};
			var l2 = l1.DeepCopy();
			l2.Start.X=0;
			PrintLine(l1);
			PrintLine(l2);
		}
		public static string PointToString(Point point)
		{
			return $"[{point.X},{point.Y}]";
		}
		public static void PrintLine(Line line)
		{
			Console.WriteLine($"from {PointToString(line.Start)} to {PointToString(line.End)}");
		}
	}
	
}