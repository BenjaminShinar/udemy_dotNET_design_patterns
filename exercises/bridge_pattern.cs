using System;

namespace Coding.Exercise
{
	/*
    Refactor the code to avoid the need of Creating Both VectorTriangle and RasterTriangle.
  	*/
	
	public interface IRenderer
	{
		string WhatToRenderAs {get;}
	}
	public class VectorRenderer : IRenderer
	{
		public string WhatToRenderAs => "lines";
	}
	
	public class RasterRenderer : IRenderer
	{
		public string WhatToRenderAs => "pixels";
	}
	
    public abstract class Shape
    {
		protected Shape(IRenderer r,string n) 
		{
			Renderer=r;
			Name=n;
		}
      public string Name { get; set; }
	  public IRenderer Renderer {get; set; }
		public override string ToString() => $"Drawing {Name} as {Renderer.WhatToRenderAs}";

    }

    public class Triangle : Shape
    {
      public Triangle(IRenderer r): base(r,"Triangle"){}
    }

    public class Square : Shape
    {
      public Square(IRenderer r) : base(r,"Square"){}
    }
 	
	public class Program
	{
		public static void Main()
		{
			var t1 = new Triangle(new VectorRenderer());
			var t2 = new Triangle(new RasterRenderer());
			Console.WriteLine(t1);
			Console.WriteLine(t2);
		}
	}
}