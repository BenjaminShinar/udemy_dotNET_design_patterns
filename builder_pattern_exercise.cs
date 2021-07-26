using System;
using System.Collections.Generic;
using System.Text;
				
namespace Coding.Exercise
{
    public class CodeBuilder
    {
        private string typeName;
        private IList<KeyValuePair<string,string>> properties = new List<KeyValuePair<string,string>>();
        // TODO
        public CodeBuilder(string name)
		{
			typeName=name;
		}
		public CodeBuilder AddField(string propertyName, string propertyType)
		{
			properties.Add(new KeyValuePair<string,string>(propertyName,propertyType));
			return this;
		}
		public override string ToString()
		{
			var sb= new StringBuilder("public class ").Append(typeName).AppendLine();
			sb.AppendLine("{");
			var format = "  public {0} {1};\n";
			foreach(var p in properties)
			{
				sb.AppendFormat(format,p.Value,p.Key);
			}
			
			sb.AppendLine("}");
			return sb.ToString();
		}
		
		public static void Main()
		{
			var cb = new CodeBuilder("Person").AddField("Name","string").AddField("Age","int");
			Console.WriteLine(cb);
		}
	}
}



