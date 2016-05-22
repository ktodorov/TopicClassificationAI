using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicClassificationCore.Topics
{
	public class PoliticsTopic : BaseTopic
	{
		public override string DisplayName
		{
			get
			{
				return "Politics";
			}
		}

		public override bool MatchesArticle(string article)
		{
			return true;
		}

		public override void LearnArticle(string article)
		{
			//throw new NotImplementedException();
		}
	}
}
