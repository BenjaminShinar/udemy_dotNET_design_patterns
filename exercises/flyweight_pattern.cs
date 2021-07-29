using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coding.Exercise
{
	/*
Flyweight
sentence takes a string that can be capitalized
   word token returns a thing that allows us to format a word
  	*/

    public class Sentence
    {
		private WordToken[] words;
      public Sentence(string plainText)
      {
        words = plainText.Split(' ').Select(w=> new WordToken{Text=w}).ToArray();
      }

      public WordToken this[int index]
      {
        get
        {
          return words[index];
        }
      }

      public override string ToString()
      {
        return string.Join(" ",words.Select(w=>w.Capitalize ? w.Text.ToUpper(): w.Text));
      }

      public class WordToken
      {
        public bool Capitalize;
		  public string Text {get;set;}
      }
    }
 	
	public class Program
	{
		public static void Main()
		{
			var sen = new Sentence("hello world");
			sen[1].Capitalize = true;
			Console.WriteLine(sen);
			
		}
	}
}