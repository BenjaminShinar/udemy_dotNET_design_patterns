using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

/*
	Template  Pattern
	implement the 'hit' override for permanent and temporary damage card games
	
*/

namespace Coding.Exercise
{ 
  public class Creature
  {
    public int Attack, Health;

    public Creature(int attack, int health)
    {
      Attack = attack;
      Health = health;
    }
  }

  public abstract class CardGame
  {
    public Creature[] Creatures;

    public CardGame(Creature[] creatures)
    {
      Creatures = creatures;
    }

    // returns -1 if no clear winner (both alive or both dead)
    public int Combat(int creature1, int creature2)
    {
      Creature first = Creatures[creature1];
      Creature second = Creatures[creature2];
      Hit(first, second);
      Hit(second, first);
      bool firstAlive = first.Health > 0;
      bool secondAlive = second.Health > 0;
      if (firstAlive == secondAlive) return -1;
      return firstAlive ? creature1 : creature2;
    }

    // attacker hits other creature
    protected abstract void Hit(Creature attacker, Creature other);
  }

  public class TemporaryCardDamageGame : CardGame
  {
  public TemporaryCardDamageGame(Creature[] creatures): base(creatures){}
    protected override void Hit(Creature attacker, Creature other)
	{
		if (attacker.Attack >= other.Health)
		{
			other.Health =0;
		}
	}
  }

  public class PermanentCardDamage : CardGame
  {
  	public PermanentCardDamage(Creature[] creatures): base(creatures){}
    protected override void Hit(Creature attacker, Creature other)
	{
		other.Health -= attacker.Attack;
	}
  }
	
	public class Program
	{
		public static void Main()
		{

			Creature[] crts = {new Creature(1,2),new Creature(1,3)};
			var tcd =  new TemporaryCardDamageGame(crts);
			Console.WriteLine($"tcd Winner {tcd.Combat(0,1)}");
			Console.WriteLine($"tcd Winner {tcd.Combat(0,1)}");
			Console.WriteLine($"tcd Winner {tcd.Combat(0,1)}");
			Console.WriteLine($"tcd Winner {tcd.Combat(0,1)}");
			

			
			
			Creature[] crts2 = {new Creature(1,2),new Creature(1,3)};
			var pcd =  new PermanentCardDamage(crts2);
			Console.WriteLine($"pcd Winner {pcd.Combat(0,1)}");
			Console.WriteLine($"pcd Winner {pcd.Combat(0,1)}");
			
						
			Creature[] crts3 = {new Creature(2,2),new Creature(2,2)};
			var tcd2 =  new TemporaryCardDamageGame(crts3);
			Console.WriteLine($"tcd2 Winner {tcd2.Combat(0,1)}");
			
		}
	}
}