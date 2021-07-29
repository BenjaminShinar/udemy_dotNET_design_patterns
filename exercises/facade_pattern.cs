using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coding.Exercise
{
	/*
   magic square.
   generator - makes an array of random digits
   splitter: splits a2d square into lists of all the rows, all the columns, and all the diagonals.
   verifer - ensures that all lists are of the same type
   
   need to write the facade class that does everthing
  	*/

  public class Generator
  {
    private static readonly Random random = new Random();

    public List<int> Generate(int count)
    {
      return Enumerable.Range(0, count)
        .Select(_ => random.Next(1, 6))
        .ToList();
    }
  }

  public class Splitter
  {
    public List<List<int>> Split(List<List<int>> array)
    {
      var result = new List<List<int>>();

      var rowCount = array.Count;
      var colCount = array[0].Count;

      // get the rows
      for (int r = 0; r < rowCount; ++r)
      {
        var theRow = new List<int>();
        for (int c = 0; c < colCount; ++c)
          theRow.Add(array[r][c]);
        result.Add(theRow);
      }

      // get the columns
      for (int c = 0; c < colCount; ++c)
      {
        var theCol = new List<int>();
        for (int r = 0; r < rowCount; ++r)
          theCol.Add(array[r][c]);
        result.Add(theCol);
      }

      // now the diagonals
      var diag1 = new List<int>();
      var diag2 = new List<int>();
      for (int c = 0; c < colCount; ++c)
      {
        for (int r = 0; r < rowCount; ++r)
        {
          if (c == r)
            diag1.Add(array[r][c]);
          var r2 = rowCount - r - 1;
          if (c == r2)
            diag2.Add(array[r][c]);
        }
      }

      result.Add(diag1);
      result.Add(diag2);

      return result;
    }
  }

  public class Verifier
  {
    public bool Verify(List<List<int>> array)
    {
      if (!array.Any()) return false;

      var expected = array.First().Sum();

      return array.All(t => t.Sum() == expected);
    }
  }

	//facade
  public class MagicSquareGenerator
  {
	  private Generator generator = new Generator();
	  private Splitter splitter = new Splitter();
	  private Verifier verifier = new Verifier();
	  private List<List<int>> MakeSquare(int size)
	  {
		  var square = new List<List<int>>();
		  for (var i = 0; i<size;++i)
		  {
			  square.Add(generator.Generate(size));
		  }
		  return square;
	  }
	  
    public List<List<int>> Generate(int size)
    {
      	
		List<List<int>> splitted;
		do
		{
			var rows= MakeSquare(size);
			splitted= splitter.Split(rows);
		}
		while (!verifier.Verify(splitted));
		return splitted;
    }
  }
 	
	public class Program
	{
		public static void Main()
		{
			var Gen = new MagicSquareGenerator();
			var Square = Gen.Generate(2);
			var sums = Square.Select(x=> x.Sum()).ToList();
			Console.WriteLine($"Sums are: {string.Join(",",sums)}");
			
		}
	}
}