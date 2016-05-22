using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopicClassificationCore.Helpers;

namespace TopicClassificationCore.Topics
{
	class SportsTopic : BaseTopic
	{
		public override string DisplayName
		{
			get
			{
				return "Sports";
			}
		}

		public override bool MatchesArticle(string article)
		{
			return true;
		}

		public override void LearnArticle(string article)
		{
			//var words = article.Split(' ');

			//foreach(var word in words)
			//{
			//	Storage.AddWordOccurance(word, )
			//}
		}
	}
}
