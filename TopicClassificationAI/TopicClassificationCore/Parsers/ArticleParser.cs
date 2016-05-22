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
			var firstWord = text.Split(' ').FirstOrDefault();

			var topic = Storage.FindWordTopic(firstWord);

			Topics.Add(topic);

			//await Task.Delay(new TimeSpan(0, 0, 3));

			//var politicsTopic = new PoliticsTopic();
			//if (politicsTopic.MatchesArticle(text))
			//{
			//	Topics.Add(politicsTopic);
			//}

			//var sportsTopic = new SportsTopic();
			//if (sportsTopic.MatchesArticle(text))
			//{
			//	Topics.Add(sportsTopic);
			//}
		}
	}
}
