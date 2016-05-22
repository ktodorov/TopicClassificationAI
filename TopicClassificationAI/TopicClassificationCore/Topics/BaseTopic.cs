using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Topics
{
	public abstract class BaseTopic
	{
		public abstract string DisplayName
		{
			get;
		}

		public abstract bool MatchesArticle(string article);

		public abstract void LearnArticle(string article);
	}
}
