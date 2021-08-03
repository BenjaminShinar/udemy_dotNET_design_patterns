using System;
using System.Collections.Generic;

/*
	implement a meidator object.
	actors can 'say a number' and all the other actors increase the value by that value;

*/

namespace Coding.Exercise
{ 
    public class Participant
    {
      public int Value { get; set; }
		private Mediator mediator;
      public Participant(Mediator mediator)
      {
		this.mediator = mediator;
        mediator.Add(this);
      }

      public void Say(int n)
      {
        mediator.Echo(this, n);
      }
	
	public void Listen(Participant shouter, int n)
      {
        if (!ReferenceEquals(this,shouter))
		{
			Value +=n;
		}
      }
    }

    public class Mediator
    {
		
		private List<Participant> participants = new List<Participant>();
		public void Add(Participant p)
		{
			participants.Add(p);
		}
		
		public void Echo(Participant shouter, int value)
		{
			participants.ForEach(p=> p?.Listen(shouter,value));
		}
    }

	public class Program
	{
		public static void Main()
		{
			var med = new Mediator();
			var p1 = new Participant(med);
			var p2 = new Participant(med);
			
			p1.Say(3);
			Console.WriteLine($"p1 {p1.Value} p2 {p2.Value}");
			p2.Say(2);
			Console.WriteLine($"p1 {p1.Value} p2 {p2.Value}");
		}
	}
}