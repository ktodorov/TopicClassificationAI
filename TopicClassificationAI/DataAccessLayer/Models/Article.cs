using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
	public class Article : EntityBase
	{
		public string Text { get; set; }

		public int Topic { get; set; }

		public override string ToString()
		{
			return $"{Topic} - {Text.Substring(0, 20)}";
		}
	}
}
