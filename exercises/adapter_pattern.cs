using System;

namespace Coding.Exercise
{
    /*
    implement a SquareToRectangleAdapter that Adapts the class Square to IRectangle Interface
  */

    public class Square
    {
        public int Side;
    }

    public interface IRectangle
    {
        int Width { get; }
        int Height { get; }
    }

    public static class ExtensionMethods
    {
        public static int Area(this IRectangle rc)
        {
            return rc.Width * rc.Height;
        }
    }

    public class SquareToRectangleAdapter : IRectangle
    {
        public SquareToRectangleAdapter(Square square)
        {
            side = square.Side;
        }
        private int side;
        public int Width => side;
        public int Height => side;

    }


    public class Program
    {
        public static void Main()
        {
            var sq = new Square { Side = 10 };
            var adapted = new SquareToRectangleAdapter(sq);
            Console.WriteLine($"Area is {adapted.Area()}");
        }
    }
}