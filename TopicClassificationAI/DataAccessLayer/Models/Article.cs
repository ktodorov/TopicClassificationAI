using DataAccessLayer.Contexts;
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

		public override string ToString()
		{
			return $"{Text.Substring(0, 30)}";
		}

		public List<ArticleTopic> GetTopics(TopicClassificationContext context)
		{
			var topics = context.ArticleTopics.Where(at => at.ArticleId == this.Id).ToList();
			return topics;
		}
	}
}
