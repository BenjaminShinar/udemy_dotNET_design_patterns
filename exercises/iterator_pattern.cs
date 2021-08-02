using System;
using System.Collections.Generic;
  namespace Coding.Exercise
  {
    public class Node<T>
    {
      public T Value;
      public Node<T> Left, Right;
      public Node<T> Parent;

      public Node(T value)
      {
        Value = value;
      }

      public Node(T value, Node<T> left, Node<T> right)
      {
        Value = value;
        Left = left;
        Right = right;

        left.Parent = right.Parent = this;
      }

	private  IEnumerable<Node<T>> TraversePreOrder(Node<T> current)
            {

               // this element
				yield return current;
				
                // left elements
                if (current.Left != null)
                {
                    foreach (var left in TraversePreOrder(current.Left))
                    {
                        yield return left;
                    }
                }
 
                // right elements
                if (current.Right != null)
                {
                    foreach (var right in TraversePreOrder(current.Right))
                    {
                        yield return right;
                    }
                }
            }
	public IEnumerable<T> PreOrder
      {
        get
        {
            foreach (var node in TraversePreOrder(this))
            {
                yield return node.Value;
            }
        }
	  }
    }

	public class Program
	{
		public static void Main()
		{
			var n2 = new Node<int>(8,new Node<int>(9),
									   new Node<int>(10));
			var root = new Node<int>(6,new Node<int>(7),n2);
			Console.WriteLine($"{string.Join(",",root.PreOrder)}");
		}
	}
}