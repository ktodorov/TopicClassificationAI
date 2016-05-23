using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
	public class WordOccurence : EntityBase
	{
		public int WordId { get; set; }

		public Word Word { get; set; }

		public int ArticleId { get; set; }

		public Article Article { get; set; }

		public int TimesOccured { get; set; }

		public override string ToString()
		{
			return $"{WordId} - {ArticleId}";
		}
	}
}
