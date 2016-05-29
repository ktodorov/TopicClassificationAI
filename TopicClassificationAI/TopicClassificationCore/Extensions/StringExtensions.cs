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

			var i = 0;
			foreach (var symbol in array)
			{
				if (char.IsUpper(symbol) && i != 0)
				{
					resultArray.Add(' ');
					resultArray.Add(char.ToLower(symbol));
				}
				else
				{
					resultArray.Add(symbol);
				}
				i++;
			}

			var result = string.Join("", resultArray.ToArray());

			return result;
		}

		public static string RemoveSpecialSymbols(this string text)
		{
			var filteredText = text.Replace(new char[] { ',', '.', '"', '*', '\r', '\n', '-', '_', '!', '?', '\"', '(', ')', '<', '>' }, " ");

			filteredText = filteredText.Replace(Environment.NewLine, " ");

			return filteredText;
		}

		public static string Replace(this string text, char[] separators, string newVal)
		{
			string[] temp;

			temp = text.Split(separators, StringSplitOptions.RemoveEmptyEntries);
			return String.Join(newVal, temp);
		}
	}
}
