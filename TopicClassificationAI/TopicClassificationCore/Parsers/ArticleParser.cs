using DataAccessLayer.Contexts;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Helpers;

namespace TopicClassificationCore.Parsers
{
	public class ArticleParser : BaseParser
	{
		public override async Task Parse(string text, IProgress<double> progress = null)
		{
			var article = new Article();
			article.Text = text;

			using (var context = new TopicClassificationContext())
			{
				context.Articles.Add(article);
				await context.SaveChangesAsync();

				var topics = await Storage.FindArticleTopic(context, article, progress);

				foreach (var topic in topics)
				{
					var articleTopic = new ArticleTopic();
					articleTopic.ArticleId = article.Id;
					articleTopic.Topic = (int)topic;
					context.ArticleTopics.Add(articleTopic);
				}

				await context.SaveChangesAsync();

				Topics.AddRange(topics);
			}
		}
	}
}
