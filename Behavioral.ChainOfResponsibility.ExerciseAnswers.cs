using System;
using System.Collections.Generic;
using NUnit.Framework;


/*
this is from the lecture.
not something that i wrote.
Only used to comment and better understand the pattern.
*/

namespace DotNetDesignPatternDemos.Behavioral.ChainOfResponsibility
{
    namespace Coding.Exercise
    {

      //base class
        public abstract class Creature
        {
            protected Game game;
            protected readonly int baseAttack;
            protected readonly int baseDefense;

          //probably better to add the creature to the game right here?
            protected Creature(Game game, int baseAttack, int baseDefense)
            {
                this.game = game;
                this.baseAttack = baseAttack;
                this.baseDefense = baseDefense;
            }

            //should be an interface, no need for 'set'
            public virtual int Attack { get; set; }
            public virtual int Defense { get; set; }
            public abstract void Query(object source, StatQuery sq);
        }

        public class Goblin : Creature
        {

          //this is the big thing?
            public override void Query(object source, StatQuery sq)
            {
              // if the calling object, take the relevent member
                if (ReferenceEquals(source, this))
                {
                    switch (sq.Statistic)
                    {
                        case Statistic.Attack:
                            sq.Result += baseAttack;
                            break;
                        case Statistic.Defense:
                            sq.Result += baseDefense;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {

                  // if this is a different goblin, it adds to the defense stat!
                    if (sq.Statistic == Statistic.Defense)
                    {
                        sq.Result++;
                    }
                }
            }

            public override int Defense
            {
                get
                {
                    var q = new StatQuery { Statistic = Statistic.Defense };
                    foreach (var c in game.Creatures)
                        c.Query(this, q);
                    return q.Result;
                }
            }

            public override int Attack
            {
                get
                {
                    var q = new StatQuery { Statistic = Statistic.Attack };
                    foreach (var c in game.Creatures)
                        c.Query(this, q);
                    return q.Result;
                }
            }

            public Goblin(Game game) : base(game, 1, 1)
            {
            }

            protected Goblin(Game game, int baseAttack, int baseDefense) : base(game,
              baseAttack, baseDefense)
            {
            }
        }

        public class GoblinKing : Goblin
        {
            public GoblinKing(Game game) : base(game, 3, 3)
            {
            }

            public override void Query(object source, StatQuery sq)
            {
                // if this isn't the calling goblin and we  are doing an attack thing, add to attack.
                // otherwise? use basic behavior! this is remarbkly dumb.
                if (!ReferenceEquals(source, this) && sq.Statistic == Statistic.Attack)
                {
                    sq.Result++; // every goblin gets +1 attack
                }               
                else base.Query(source, sq);
            }
        }

        public enum Statistic
        {
            Attack,
            Defense
        }

        public class StatQuery
        {
            public Statistic Statistic;
            public int Result;
        }

        public class Game
        {
            public IList<Creature> Creatures = new List<Creature>();
        }
    }

    namespace Coding.Exercise.Tests
    {
        [TestFixture]
        public class TestSuite
        {
            [Test]
            public void ManyGoblinsTest()
            {
                var game = new Game();
                var goblin = new Goblin(game);
                game.Creatures.Add(goblin);

                Assert.That(goblin.Attack, Is.EqualTo(1));
                Assert.That(goblin.Defense, Is.EqualTo(1));

                var goblin2 = new Goblin(game);
                game.Creatures.Add(goblin2);

                Assert.That(goblin.Attack, Is.EqualTo(1));
                Assert.That(goblin.Defense, Is.EqualTo(2));

                var goblin3 = new GoblinKing(game);
                game.Creatures.Add(goblin3);

                Assert.That(goblin.Attack, Is.EqualTo(2));
                Assert.That(goblin.Defense, Is.EqualTo(3));
            }
        }
    }
}