using System;
using System.Linq;

namespace Coding.Exercise
{
  public class Game
  {
	  
	  public static void Main(string[] args)
	  {
		  var game = new Game();
		  var r1 =new Rat(game);
		  Console.WriteLine($"attack {r1.Attack}");
		  
		  {
			using var r2 =new Rat(game);
		  	Console.WriteLine($"attack {r1.Attack} , {r2.Attack}");
		  }
		  Console.WriteLine($"attack {r1.Attack}");
	  }
	  public event EventHandler<EventArgs> MouseAdded; //event
	  public event EventHandler<EventArgs> MouseRemoved; //event
	  public event EventHandler<EventArgs> PumpUp; //event
	  public event EventHandler<EventArgs> RollCall; //event
	  public void AddMouse(Rat rat)
	  {
		  MouseAdded?.Invoke(rat,EventArgs.Empty);
	  }
	  public void RemoveMouse(Rat rat)
	  {
		  MouseRemoved.Invoke(rat,EventArgs.Empty);
	  }
	  public void MouseRollCall(Rat rat)
	  {
		   RollCall?.Invoke(rat,EventArgs.Empty);
	  }
	  
	  public void MouseCall()
	  {
		  PumpUp?.Invoke(null,EventArgs.Empty);
	  }
	  // todo
    // remember - no fields or properties!
  }

  public class Rat : IDisposable
  {
    public int Attack = 1;
    private Game game;
    public Rat(Game game)
    {
      this.game=game;
	  game.MouseAdded +=whenMouseAdded;
	  game.MouseRemoved +=whenMouseRemoved;
	  
	  game.PumpUp += whenMouseAdded;
	  game.MouseRollCall(this);
	  game.PumpUp -= whenMouseAdded; 
	  game.RollCall +=rollCallAction;
	  game.AddMouse(this);
    }


    public void Dispose()
    {
		game.RemoveMouse(this);
      game.MouseAdded -=whenMouseAdded;
	  game.MouseRemoved -=whenMouseRemoved;
	  game.RollCall -=rollCallAction;
	  
    }
	

private void rollCallAction(object sender, EventArgs eventArgs)
{
game.MouseCall();
}

	private  void whenMouseAdded(object sender, EventArgs eventArgs)
	{
		if(this != sender)
		{
			++Attack;
		}
	}
	
	private  void whenMouseRemoved(object sender, EventArgs eventArgs)
	{
		if(this != sender)
		{
		--Attack;
		}
	}
    
  }
}

