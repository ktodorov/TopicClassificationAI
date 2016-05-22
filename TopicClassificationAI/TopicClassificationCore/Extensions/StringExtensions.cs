using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Extensions
{
	public static class StringExtensions
	{
		public static string SeparateCamelCase(this string text)
		{
			var array = text.ToCharArray().ToList();

			var resultArray = new List<char>();

			foreach (var symbol in array)
			{
				if (char.IsUpper(symbol) && array.IndexOf(symbol) != 0)
				{
					resultArray.Add(' ');
					resultArray.Add(char.ToLower(symbol));
				}
				else
				{
					resultArray.Add(symbol);
				}
			}

			var result = string.Join("", resultArray.ToArray());

			return result;
		}
	}
}
