using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Helpers;
using TopicClassificationCore.Topics;

namespace TopicClassificationCore.Parsers
{
	public class ArticleParser : BaseParser
	{
		public override async Task Parse(string text)
		{
			var article = new Article();
			article.Text = text;

			using (var context = new TopicClassificationContext())
			{
				context.Articles.Add(article);
				await context.SaveChangesAsync();

				var topic = await Storage.FindArticleTopic(context, article);

				article.Topic = (int)topic;
				await context.SaveChangesAsync();

				Topics.Add(topic);
			}
		}
	}
}
