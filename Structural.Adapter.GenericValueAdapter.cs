using System;
using System.Linq;

/*
this is from the lecture.
not something that i wrote.
Only used to comment and better understand the pattern.
*/
namespace DotNetDesignPatternDemos.Structural.Adapter
{
  // Vector2f, Vector3i
  //interface for something that generates integers
  public interface IInteger
  {
    int Value { get; }
  }

  // some concrete IInteger classes.
  public static class Dimensions
  {
    public class Two : IInteger
    {
      public int Value => 2;
    }

    public class Three : IInteger
    {
      public int Value => 3;
    }
  }

  // this is a generic type, curiously recursive template pattern
  // D must be an IIntger and be able to default construct.
  // TSelf is also itself, and default constructable.
  public class Vector<TSelf, T, D>
    where D : IInteger, new()
    where TSelf : Vector<TSelf, T, D>, new()
  {
    protected T[] data;

    public Vector()
    {
      data = new T[new D().Value];
    }

    public Vector(params T[] values)
    {
      var requiredSize = new D().Value;
      data = new T[requiredSize];

      //populate the data
      var providedSize = values.Length;

      for (int i = 0; i < Math.Min(requiredSize, providedSize); ++i)
        data[i] = values[i];
    }

    // factory method
    // any named class can create instances of this class
    public static TSelf Create(params T[] values)
    {
      var result = new TSelf();
      var requiredSize = new D().Value;
      result.data = new T[requiredSize];
      
      //populate the data
      var providedSize = values.Length;
      
      for (int i = 0; i < Math.Min(requiredSize, providedSize); ++i)
        result.data[i] = values[i];

      return result;
    }

    //index property
    public T this[int index]
    {
      get => data[index];
      set => data[index] = value;
    }

    // not used anymore.
    public T X
    {
      get => data[0];
      set => data[0] = value;
    }
  }

  public class VectorOfFloat<TSelf, D> 
    : Vector<TSelf, float, D>
    where D : IInteger, new() 
    where TSelf : Vector<TSelf, float, D>, new()
  {
  }

  public class VectorOfInt<D> : Vector<VectorOfInt<D>, int, D>
    where D : IInteger, new()
  {
    public VectorOfInt()
    {
    }

    public VectorOfInt(params int[] values) : base(values)
    {
    }

    //operator overloading, just for VectorOfInt<D> class.
    public static VectorOfInt<D> operator +
      (VectorOfInt<D> lhs, VectorOfInt<D> rhs)
    {
      var result = new VectorOfInt<D>();
      var dim = new D().Value;
      for (int i = 0; i < dim; i++)
      {
        result[i] = lhs[i] + rhs[i];
      }

      return result;
    }
  }

  public class Vector2i : VectorOfInt<Dimensions.Two>
  {
    public Vector2i()
    {
    }

    public Vector2i(params int[] values) : base(values)
    {
    }
  }

  public class Vector3f 
    : VectorOfFloat<Vector3f, Dimensions.Three>
  {
    public override string ToString()
    {
      return $"{string.Join(",", data)}";
    }
  }
  
  class Demo
  {
    public static void Main(string[] args)
    {
      var v = new Vector2i(1, 2);
      v[0] = 0;
      
      var vv = new Vector2i(3, 2);

      var result = v + vv;

      Vector3f u = Vector3f.Create(3.5f, 2.2f, 1);
      
    }
  }
}