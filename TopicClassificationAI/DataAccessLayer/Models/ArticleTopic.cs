using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
	public class ArticleTopic : EntityBase
	{
		public int ArticleId { get; set; }

		public Article Article { get; set; }

		public int Topic { get; set; }

		public override string ToString()
		{
			return $"{ArticleId} - {Topic}";
		}
	}
}
